using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// service used to manage Race objects
    /// </summary>
    public class RaceService : ObservableObject, IRaceService
    {
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

        /// <summary>
        /// Constructor
        /// </summary>
        public RaceService(IFileService fileService, IPersonService personService, ICompetitionService competitionService)
        {
            LastCalculatedCompetitionRaces = null;
            _fileService = fileService;
            _personService = personService;
            _competitionService = competitionService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private List<CompetitionRaces> _lastCalculatedCompetitionRaces;
        /// <summary>
        /// List with the the <see cref="CompetitionRaces"/> of the last time <see cref="CalculateRunOrder(ushort, CancellationToken, int, ProgressDelegate)"/> was called
        /// </summary>
        public List<CompetitionRaces> LastCalculatedCompetitionRaces
        {
            get => _lastCalculatedCompetitionRaces;
            set { SetProperty(ref _lastCalculatedCompetitionRaces, value); OnPropertyChanged(nameof(HasUnsavedChanges)); }
        }

        private CompetitionRaces _bestCompetitionRaces;
        /// <summary>
        /// <see cref="CompetitionRaces"/> object that is marked as best result.
        /// </summary>
        public CompetitionRaces BestCompetitionRaces
        {
            get => _bestCompetitionRaces;
            set { SetProperty(ref _bestCompetitionRaces, value); OnPropertyChanged(nameof(HasUnsavedChanges)); }
        }

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
        public async Task<List<CompetitionRaces>> CalculateCompetitionRaces(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null)
        {
            // Collect all starts
            _competitionService.UpdateAllCompetitionsForPersonStarts(competitionYear);
            List<PersonStart> starts = _personService.GetAllPersonStarts();

            // Create groups of competitions with same style and distance
            Dictionary<(SwimmingStyles, ushort), List<PersonStart>> groupedValuesStarts = new Dictionary<(SwimmingStyles, ushort), List<PersonStart>>();
            for (int i = 0; i < starts.Count; i++)
            {
                if (starts[i].CompetitionObj == null) { continue; }

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

            int numberOfResultsToGenerate = 100;
            CompetitionRaceGenerator generator = new CompetitionRaceGenerator(new Progress<double>(progress => onProgress?.Invoke(this, (float)progress, "")), numberOfResultsToGenerate, 1000000, 20);
            LastCalculatedCompetitionRaces = await generator.GenerateBestRacesAsync(groupedValuesStarts.Values.ToList(), cancellationToken);
            BestCompetitionRaces = LastCalculatedCompetitionRaces.FirstOrDefault();
            return LastCalculatedCompetitionRaces?.Count == 0 ? null : LastCalculatedCompetitionRaces;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load the best race to the <see cref="BestCompetitionRaces"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="BestCompetitionRaces"/> is cleared and the functions returns loading success.
        /// </summary>
        /// <param name="path">Path from where to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> Load(string path, CancellationToken cancellationToken)
        {
            bool importingResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    LastCalculatedCompetitionRaces = null;
                    if (!File.Exists(path))
                    {
                        OnFileProgress?.Invoke(this, 0);
                        BestCompetitionRaces = null;
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<Race> raceList = _fileService.LoadFromCsv<Race>(path, cancellationToken, setRacePropertyFromString, OnFileProgress);
                        BestCompetitionRaces = new CompetitionRaces(raceList);
                    }

                    if (BestCompetitionRaces == null) { _bestCompetitionRacesOnLoad = null; }
                    else { _bestCompetitionRacesOnLoad = new CompetitionRaces(BestCompetitionRaces); }

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
                    if (BestCompetitionRaces == null)
                    {
                        _bestCompetitionRacesOnLoad = null;
                    }
                    else
                    {
                        int maxNumberStarts = BestCompetitionRaces.Races.Select(r => r.Starts.Count).Max();
                        _fileService.SaveToCsv(path, BestCompetitionRaces.Races.ToList(), cancellationToken, OnFileProgress,
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

                        _bestCompetitionRacesOnLoad = new CompetitionRaces(BestCompetitionRaces);
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
        /// Check if the <see cref="BestCompetitionRaces"> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get
            {
                if(BestCompetitionRaces != null && _bestCompetitionRacesOnLoad != null)
                {
                    return !BestCompetitionRaces.Equals(_bestCompetitionRacesOnLoad);
                }
                else if(BestCompetitionRaces == null && _bestCompetitionRacesOnLoad == null)
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
