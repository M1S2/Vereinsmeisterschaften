using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// service used to manage Race objects
    /// </summary>
    public class RaceService : ObservableObject, IRaceService
    {
        /// <summary>
        /// Number of variants in <see cref="AllCompetitionRaces"/> after calling <see cref="CalculateCompetitionRaces(ushort, CancellationToken, int, ProgressDelegate)"/>
        /// The number of variants to keep (marked with <see cref="CompetitionRaces.KeepWhileRacesCalculation"/>) is subtracted before calculating the remaining elements.
        /// </summary>
        public const int NUM_VARIANTS_AFTER_CALCULATION = 100;

        /// <summary>
        /// Event that is raised when the file operation progress changes
        /// </summary>
        public event ProgressDelegate OnFileProgress;

        /// <summary>
        /// Event that is raised when the file operation is finished.
        /// </summary>
        public event EventHandler OnFileFinished;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;
        private IPersonService _personService;
        private ICompetitionService _competitionService;

        private int _nextVariantID;

        /// <summary>
        /// Constructor
        /// </summary>
        public RaceService(IFileService fileService, IPersonService personService, ICompetitionService competitionService)
        {
            _fileService = fileService;
            _personService = personService;
            _competitionService = competitionService;

            _nextVariantID = 1;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all <see cref="CompetitionRaces"/> (including the loaded and calculated ones)
        /// </summary>
        public ObservableCollection<CompetitionRaces> AllCompetitionRaces { get; private set; } = new ObservableCollection<CompetitionRaces>();

        /// <summary>
        /// <see cref="CompetitionRaces"/> object that is persisted (saved/loaded to/from a file).
        /// </summary>
        public CompetitionRaces PersistedCompetitionRaces => AllCompetitionRaces.Where(r => r.IsPersistent).FirstOrDefault();

        /// <summary>
        /// <see cref="CompetitionRaces"/> object that is marked as best result at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private CompetitionRaces _bestCompetitionRacesOnLoad { get; set; }

        /// <summary>
        /// Path to the race file
        /// </summary>
        public string PersistentPath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Calculate some combination variants for all person starts
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel this calculation</param>
        /// <param name="numberAvailableSwimLanes">Number of available swimming lanes. This determines the maximum number of parallel starts</param>
        /// <param name="onProgress">Callback used to report progress of the calculation</param>
        /// <returns>All results if calculation was finished successfully; otherwise <see langword="null"/></returns>
        public async Task<ObservableCollection<CompetitionRaces>> CalculateCompetitionRaces(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null)
        {
            // Collect all starts
            _competitionService.UpdateAllCompetitionsForPersonStarts(competitionYear);
            List<PersonStart> starts = _personService.GetAllPersonStarts();

            // Create groups of competitions with same style and distance
            Dictionary<(SwimmingStyles, ushort), List<PersonStart>> groupedValuesStarts = new Dictionary<(SwimmingStyles, ushort), List<PersonStart>>();
            for (int i = 0; i < starts.Count; i++)
            {
                if (!starts[i].IsCompetitionObjAssigned) { continue; }

                (SwimmingStyles, ushort) key = (starts[i].CompetitionObj.SwimmingStyle, starts[i].CompetitionObj.Distance);
                if (!groupedValuesStarts.ContainsKey(key))
                {
                    groupedValuesStarts.Add(key, new List<PersonStart>());
                }
                else if (groupedValuesStarts[key] == null)
                {
                    groupedValuesStarts[key] = new List<PersonStart>();
                }
                groupedValuesStarts[key].Add(starts[i]);
            }

            List<CompetitionRaces> racesToDelete = AllCompetitionRaces.Where(r => !r.KeepWhileRacesCalculation).ToList();
            racesToDelete.ForEach(r => AllCompetitionRaces.Remove(r));
            int numberOfResultsToGenerate = Math.Max(0, NUM_VARIANTS_AFTER_CALCULATION - AllCompetitionRaces.Count(r => r.KeepWhileRacesCalculation));
            
            CompetitionRaceGenerator generator = new CompetitionRaceGenerator(new Progress<double>(progress => onProgress?.Invoke(this, (float)progress, "")), numberOfResultsToGenerate, 1000000, 20);
            List<CompetitionRaces> tmpCompetitionRaces = await generator.GenerateBestRacesAsync(groupedValuesStarts.Values.ToList(), cancellationToken);
            
            tmpCompetitionRaces.ForEach(AddCompetitionRaces);
            SortVariantsByScore();
            OnPropertyChanged(nameof(PersistedCompetitionRaces));
            return AllCompetitionRaces;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load the best race as the only element to the <see cref="AllCompetitionRaces"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="AllCompetitionRaces"/> is cleared and the functions returns loading success.
        /// </summary>
        /// <param name="path">Path from where to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> Load(string path, CancellationToken cancellationToken)
        {
            bool importingResult = false;
            Exception exception = null;

            SynchronizationContext uiContext = SynchronizationContext.Current;
            await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        OnFileProgress?.Invoke(this, 0);
                        uiContext.Send((d) => { ClearAllCompetitionRaces(); }, null);
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<Race> raceList = _fileService.LoadFromCsv<Race>(path, cancellationToken, setRacePropertyFromString, OnFileProgress);
                        CompetitionRaces bestCompetitionRaces = new CompetitionRaces(raceList);
                        bestCompetitionRaces.IsPersistent = true;
                        uiContext.Send((d) =>
                        {
                            ClearAllCompetitionRaces();
                            AddCompetitionRaces(bestCompetitionRaces);
                        }, null);
                    }

                    if (PersistedCompetitionRaces == null) { _bestCompetitionRacesOnLoad = null; }
                    else { _bestCompetitionRacesOnLoad = new CompetitionRaces(PersistedCompetitionRaces); }

                    PersistentPath = path;
                    importingResult = true;
                }
                catch (OperationCanceledException)
                {
                    importingResult = false;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return importingResult;
        }

        private int _lastParsedDistance = 0;
        private SwimmingStyles _lastParsedStyle = SwimmingStyles.Unknown;
        /// <summary>
        /// Set the requested property in the <see cref="Race"/> object by parsing the given string value
        /// </summary>
        /// <param name="dataObj"><see cref="Race"/> in which to set the property</param>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">String value that will be parsed and set to the property</param>
        private void setRacePropertyFromString(Race dataObj, string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            switch (propertyName)
            {
                case nameof(Race.Distance): _lastParsedDistance = int.Parse(value); break;
                case nameof(Race.Style): _lastParsedStyle = (SwimmingStyles)Enum.Parse(typeof(SwimmingStyles), value); break;
                default:
                    {
                        if (dataObj.Starts == null) { dataObj.Starts = new ObservableCollection<PersonStart>(); }
                        if(_lastParsedDistance != 0 && _lastParsedStyle != SwimmingStyles.Unknown)
                        {
                            string[] nameParts = value.Split(',');
                            string firstName = nameParts.Length > 0 ? nameParts[0].Trim() : "";
                            string name = nameParts.Length > 1 ? nameParts[1].Trim() : "";
                            PersonStart matchingStart = _personService.GetAllPersonStarts().Where(s =>  s.Style == _lastParsedStyle &&
                                                                                                        s.CompetitionObj?.Distance == _lastParsedDistance && 
                                                                                                        s.PersonObj?.FirstName == firstName &&
                                                                                                        s.PersonObj?.Name == name).FirstOrDefault();
                            if (matchingStart != null)
                            {
                                dataObj.Starts.Add(matchingStart);
                            }
                        }
                        break;
                    }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Save the <see cref="BestCompetitionRaces"/> to a file
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="path">Path to which to save</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> Save(CancellationToken cancellationToken, string path = "")
        {
            if (string.IsNullOrEmpty(path)) { path = PersistentPath; }

            bool saveResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    if (PersistedCompetitionRaces == null)
                    {
                        if (File.Exists(path)) { File.Delete(path); }
                        _bestCompetitionRacesOnLoad = null;
                    }
                    else
                    {
                        int maxNumberStarts = PersistedCompetitionRaces.Races.Count == 0 ? 0 : PersistedCompetitionRaces.Races.Select(r => r.Starts.Count).Max();
                        _fileService.SaveToCsv(path, PersistedCompetitionRaces.Races.ToList(), cancellationToken, OnFileProgress,
                        (data) =>
                        {
                            if (data is IList<PersonStart> dataList)
                            {
                                return string.Join(";", dataList.Select(p => p.PersonObj?.FirstName + ", " + p.PersonObj?.Name));
                            }
                            else
                            {
                                return data.ToString();
                            }
                        },
                        (header, headerType) =>
                        {
                            if (headerType == typeof(ObservableCollection<PersonStart>))
                            {
                                string formatedHeader = "";
                                for(int i = 1; i <= maxNumberStarts; i++)
                                {
                                    formatedHeader += $"Person {i};";
                                }
                                formatedHeader = formatedHeader.Trim(';');
                                return formatedHeader;
                            }
                            else
                            {
                                return header;
                            }
                        });

                        _bestCompetitionRacesOnLoad = new CompetitionRaces(PersistedCompetitionRaces);
                    }
                    saveResult = true;
                }
                catch (OperationCanceledException)
                {
                    saveResult = false;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return saveResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Add a new <see cref="CompetitionRaces"/> to the list <see cref="AllCompetitionRaces"/>
        /// </summary>
        /// <param name="competitionRaces"><see cref="CompetitionRaces"/> to add</param>
        public void AddCompetitionRaces(CompetitionRaces competitionRaces)
        {
            AllCompetitionRaces.Add(competitionRaces);
            competitionRaces.VariantID = _nextVariantID;
            _nextVariantID++;
            competitionRaces.PropertyChanged += competitionRaces_PropertyChanged;
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Remove the given <see cref="CompetitionRaces"/> object from the list <see cref="AllCompetitionRaces"/>
        /// </summary>
        /// <param name="competitionRaces"><see cref="CompetitionRaces"/> object to remove</param>
        public void RemoveCompetitionRaces(CompetitionRaces competitionRaces)
        {
            if(competitionRaces == null) { return; }
            competitionRaces.PropertyChanged -= competitionRaces_PropertyChanged;
            AllCompetitionRaces.Remove(competitionRaces);
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Remove all <see cref="CompetitionRaces"/> from the list <see cref="AllCompetitionRaces"/>
        /// </summary>
        public void ClearAllCompetitionRaces()
        {
            foreach(CompetitionRaces competitionRaces in AllCompetitionRaces)
            {
                competitionRaces.PropertyChanged -= competitionRaces_PropertyChanged;
                AllCompetitionRaces.Remove(competitionRaces);
            }
            OnPropertyChanged(nameof(HasUnsavedChanges));
            _nextVariantID = 1;
        }

        /// <summary>
        /// Sort the complete list <see cref="AllCompetitionRaces"/> descending by the <see cref="CompetitionRaces.Score"/>
        /// </summary>
        public void SortVariantsByScore()
        {
            AllCompetitionRaces = new ObservableCollection<CompetitionRaces>(AllCompetitionRaces.OrderByDescending(r => r.Score));
            OnPropertyChanged(nameof(AllCompetitionRaces));
        }

        /// <summary>
        /// Reassign all <see cref="CompetitionRaces.VariantID"/> so that the IDs start from 1 and have no gaps.
        /// </summary>
        /// <param name="oldVariantID">If not -1, the method returns the new variant ID after reordering that matches this old variant ID</param>
        /// <returns>New variant ID after reordering matching the oldVariantID; if oldVariantID == -1 this returns -1</returns>
        public int RecalculateVariantIDs(int oldVariantID = -1)
        {
            int newVariantID = -1;
            _nextVariantID = 1;
            foreach (CompetitionRaces competitionRaces in AllCompetitionRaces)
            {
                if (competitionRaces.VariantID == oldVariantID) { newVariantID = _nextVariantID; }
                competitionRaces.VariantID = _nextVariantID;
                _nextVariantID++;

                competitionRaces.UpdateNotAssignedStarts(_personService.GetAllPersonStarts());
            }
            OnPropertyChanged(nameof(AllCompetitionRaces));
            return newVariantID;
        }

        private void competitionRaces_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CompetitionRaces.Races))
            {
                (sender as CompetitionRaces)?.UpdateNotAssignedStarts(_personService.GetAllPersonStarts());
            }
            OnPropertyChanged(nameof(HasUnsavedChanges));
            OnPropertyChanged(nameof(AllCompetitionRaces));
            OnPropertyChanged(nameof(PersistedCompetitionRaces));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Remove non-existing <see cref="PersonStart"/> objects from all races in <see cref="BestCompetitionRaces"/> and <see cref="LastCalculatedCompetitionRaces"/>.
        /// Also delete empty <see cref="Race">.
        /// </summary>
        public void CleanupCompetitionRaces()
        {
            List<PersonStart> validPersonStarts = _personService.GetAllPersonStarts();

            foreach (CompetitionRaces competitionRaces in AllCompetitionRaces)
            {
                foreach(Race race in competitionRaces.Races)
                {
                    // Find all starts in this race that are no longer part of the valid PersonStarts and remove them from the Race
                    List<PersonStart> startsToDelete = race.Starts.Except(validPersonStarts).ToList();
                    startsToDelete.ForEach(s => race.Starts.Remove(s));
                }

                // Find all empty races and remove them from the CompetitionRaces
                List<Race> racesToDelete = competitionRaces.Races.Where(r => r.Starts.Count == 0).ToList();
                racesToDelete.ForEach(r => competitionRaces.Races.Remove(r));
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Check if the <see cref="BestCompetitionRaces"> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get
            {
                if(PersistedCompetitionRaces != null && _bestCompetitionRacesOnLoad != null)
                {
                    CompetitionRacesFullEqualityComparer fullEqualityComparer = new CompetitionRacesFullEqualityComparer();
                    return !fullEqualityComparer.Equals(PersistedCompetitionRaces, _bestCompetitionRacesOnLoad);
                }
                else if(PersistedCompetitionRaces == null && _bestCompetitionRacesOnLoad == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
