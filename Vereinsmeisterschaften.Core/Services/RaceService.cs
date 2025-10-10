using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to manage <see cref="Race"/> and <see cref="RacesVariant"/> objects
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
        private IWorkspaceService _workspaceService;

        private int _nextVariantID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileService"><see cref="IFileService"/> object</param>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
        public RaceService(IFileService fileService, IPersonService personService, ICompetitionService competitionService)
        {
            _fileService = fileService;
            _personService = personService;
            _competitionService = competitionService;

            _nextVariantID = 1;
        }

        /// <summary>
        /// Save the reference to the <see cref="IWorkspaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> implementation</param>
        public void SetWorkspaceServiceObj(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all <see cref="RacesVariant"/> (including the loaded and calculated ones)
        /// </summary>
        public ObservableCollection<RacesVariant> AllRacesVariants { get; private set; } = new ObservableCollection<RacesVariant>();

        /// <summary>
        /// <see cref="RacesVariant"/> object that is persisted (saved/loaded to/from a file).
        /// </summary>
        public RacesVariant PersistedRacesVariant => AllRacesVariants.Where(r => r.IsPersistent).FirstOrDefault();

        /// <summary>
        /// <see cref="RacesVariant"/> object that is marked as best result at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private RacesVariant _persistedRacesVariantOnLoad { get; set; }

        /// <summary>
        /// Path to the race file
        /// </summary>
        public string PersistentPath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Calculate some <see cref="RacesVariant"/> objects for all person starts
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel this calculation</param>
        /// <param name="onProgress">Callback used to report progress of the calculation</param>
        /// <returns>All results if calculation was finished successfully; otherwise <see langword="null"/></returns>
        public async Task<ObservableCollection<RacesVariant>> CalculateRacesVariants(CancellationToken cancellationToken, ProgressDelegate onProgress = null)
        {
            // Collect all starts
            _competitionService.UpdateAllCompetitionsForPerson();
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

            ushort numSwimLanes = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES) ?? 0;
            ushort numberRequestedVariants = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUM_RACE_VARIANTS_AFTER_CALCULATION) ?? 0;
            int maxRaceVariantCalculationLoops = _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_MAX_CALCULATION_LOOPS) ?? 0;
            double minRacesVariantsScore = _workspaceService?.Settings?.GetSettingValue<double>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_MIN_RACES_VARIANTS_SCORE) ?? 0;
            int numberOfResultsToGenerate = Math.Max(0, numberRequestedVariants - AllRacesVariants.Count(r => r.KeepWhileRacesCalculation));
            
            RacesVariantsGenerator generator = new RacesVariantsGenerator(new Progress<double>(progress => onProgress?.Invoke(this, (float)progress, "")),
                                                                          numberOfResultsToGenerate,
                                                                          maxRaceVariantCalculationLoops,
                                                                          minRacesVariantsScore,
                                                                          numSwimLanes);
            List<RacesVariant> tmpRacesVariants = await generator.GenerateBestRacesAsync(groupedValuesStarts.Values.ToList(), cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                List<RacesVariant> racesToDelete = AllRacesVariants.Where(r => !r.KeepWhileRacesCalculation).ToList();
                racesToDelete.ForEach(r => AllRacesVariants.Remove(r));
                tmpRacesVariants.ForEach(AddRacesVariant);
                SortVariantsByScore();
                OnPropertyChanged(nameof(PersistedRacesVariant));
            }
            return AllRacesVariants;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load the best race as the only element to the <see cref="AllRacesVariants"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="AllRacesVariants"/> is cleared and the functions returns loading success.
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
                        uiContext.Send((d) => { ClearAllRacesVariants(); }, null);
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<Race> raceList = _fileService.LoadFromCsv<Race>(path, cancellationToken, setRacePropertyFromString, OnFileProgress, (header) =>
                        {
                            return PropertyNameLocalizedStringHelper.FindProperty(typeof(Race), header);
                        });
                        RacesVariant bestRacesVariant = new RacesVariant(raceList, _workspaceService);
                        bestRacesVariant.IsPersistent = true;
                        uiContext.Send((d) =>
                        {
                            ClearAllRacesVariants();
                            AddRacesVariant(bestRacesVariant);
                        }, null);
                    }

                    if (PersistedRacesVariant == null) { _persistedRacesVariantOnLoad = null; }
                    else { _persistedRacesVariantOnLoad = new RacesVariant(PersistedRacesVariant, true, true, _workspaceService); }

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
                case nameof(Race.Style): if (EnumCoreLocalizedStringHelper.TryParse(value, out SwimmingStyles parsedStyle)) { _lastParsedStyle = parsedStyle; } break;
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
        /// Save the <see cref="PersistedRacesVariant"/> to a file
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
                    if (PersistedRacesVariant == null)
                    {
                        if (File.Exists(path)) { File.Delete(path); }
                        _persistedRacesVariantOnLoad = null;
                    }
                    else
                    {
                        int maxNumberStarts = PersistedRacesVariant.Races.Count == 0 ? 0 : PersistedRacesVariant.Races.Select(r => r.Starts.Count).Max();
                        _fileService.SaveToCsv(path, PersistedRacesVariant.Races.ToList(), cancellationToken, OnFileProgress,
                        (data, parentObject, currentProperty) =>
                        {
                            if (data is IList<PersonStart> dataList)
                            {
                                return string.Join(";", dataList.Select(p => p.PersonObj?.FirstName + ", " + p.PersonObj?.Name));
                            }
                            else if (data is Enum dataEnum)
                            {
                                return EnumCoreLocalizedStringHelper.Convert(dataEnum);
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
                                    formatedHeader += $"{PropertyNameLocalizedStringHelper.Convert(typeof(Race), "Person")} {i};";
                                }
                                formatedHeader = formatedHeader.Trim(';');
                                return formatedHeader;
                            }
                            else
                            {
                                return PropertyNameLocalizedStringHelper.Convert(typeof(Race), header);
                            }
                        });

                        _persistedRacesVariantOnLoad = new RacesVariant(PersistedRacesVariant, true, true, _workspaceService);
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
        /// Add a new <see cref="RacesVariant"/> to the list <see cref="AllRacesVariants"/>
        /// </summary>
        /// <param name="racesVariant"><see cref="RacesVariant"/> to add</param>
        public void AddRacesVariant(RacesVariant racesVariant)
        {
            AllRacesVariants.Add(racesVariant);
            racesVariant.VariantID = _nextVariantID;
            _nextVariantID++;
            racesVariant.PropertyChanged += RacesVariant_PropertyChanged;
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Remove the given <see cref="RacesVariant"/> object from the list <see cref="AllRacesVariants"/>
        /// </summary>
        /// <param name="racesVariant"><see cref="RacesVariant"/> object to remove</param>
        public void RemoveRacesVariant(RacesVariant racesVariant)
        {
            if(racesVariant == null) { return; }
            racesVariant.PropertyChanged -= RacesVariant_PropertyChanged;
            AllRacesVariants.Remove(racesVariant);
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Remove all <see cref="RacesVariant"/> from the list <see cref="AllRacesVariants"/>
        /// </summary>
        public void ClearAllRacesVariants()
        {
            foreach(RacesVariant racesVariant in AllRacesVariants)
            {
                racesVariant.PropertyChanged -= RacesVariant_PropertyChanged;
                AllRacesVariants.Remove(racesVariant);
            }
            OnPropertyChanged(nameof(HasUnsavedChanges));
            _nextVariantID = 1;
        }

        /// <summary>
        /// Reset the <see cref="PersistedRacesVariant"> to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        public void ResetToLoadedState()
        {
            RacesVariantFullEqualityComparer fullEqualityComparer = new RacesVariantFullEqualityComparer();
            foreach (RacesVariant racesVariant in AllRacesVariants)
            {
                racesVariant.IsPersistent = fullEqualityComparer.Equals(racesVariant, _persistedRacesVariantOnLoad);
            }
            OnPropertyChanged(nameof(AllRacesVariants));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Sort the complete list <see cref="AllRacesVariants"/> descending by the <see cref="RacesVariant.Score"/>
        /// </summary>
        public void SortVariantsByScore()
        {
            AllRacesVariants = new ObservableCollection<RacesVariant>(AllRacesVariants.OrderByDescending(r => r.Score));
            OnPropertyChanged(nameof(AllRacesVariants));
        }

        /// <summary>
        /// Reassign all <see cref="RacesVariant.VariantID"/> so that the IDs start from 1 and have no gaps.
        /// </summary>
        /// <param name="oldVariantID">If not -1, the method returns the new variant ID after reordering that matches this old variant ID</param>
        /// <returns>New variant ID after reordering matching the oldVariantID; if oldVariantID == -1 this returns -1</returns>
        public int RecalculateVariantIDs(int oldVariantID = -1)
        {
            int newVariantID = -1;
            _nextVariantID = 1;
            foreach (RacesVariant racesVariant in AllRacesVariants)
            {
                if (racesVariant.VariantID == oldVariantID) { newVariantID = _nextVariantID; }
                racesVariant.VariantID = _nextVariantID;
                _nextVariantID++;

                racesVariant.UpdateNotAssignedStarts(_personService.GetAllPersonStarts());
            }
            OnPropertyChanged(nameof(AllRacesVariants));
            return newVariantID;
        }

        private void RacesVariant_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RacesVariant.Races))
            {
                (sender as RacesVariant)?.UpdateNotAssignedStarts(_personService.GetAllPersonStarts());
            }
            OnPropertyChanged(nameof(HasUnsavedChanges));
            OnPropertyChanged(nameof(AllRacesVariants));
            OnPropertyChanged(nameof(PersistedRacesVariant));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Remove non-existing <see cref="PersonStart"/> objects from all races in <see cref="AllRacesVariants"/>.
        /// Also delete empty <see cref="Race">.
        /// </summary>
        public void CleanupRacesVariants()
        {
            List<PersonStart> validPersonStarts = _personService.GetAllPersonStarts();

            foreach (RacesVariant racesVariant in AllRacesVariants)
            {
                foreach(Race race in racesVariant.Races)
                {
                    // Find all starts in this race that are no longer part of the valid PersonStarts and remove them from the Race
                    List<PersonStart> startsToDelete = race.Starts.Except(validPersonStarts).ToList();
                    startsToDelete.ForEach(s => race.Starts.Remove(s));
                }

                // Find all empty races and remove them from the variant
                List<Race> racesToDelete = racesVariant.Races.Where(r => r.Starts.Count == 0).ToList();
                racesToDelete.ForEach(r => racesVariant.Races.Remove(r));
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Check if the <see cref="PersistedRacesVariant"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get
            {
                if(PersistedRacesVariant != null && _persistedRacesVariantOnLoad != null)
                {
                    RacesVariantFullEqualityComparer fullEqualityComparer = new RacesVariantFullEqualityComparer();
                    return !fullEqualityComparer.Equals(PersistedRacesVariant, _persistedRacesVariantOnLoad);
                }
                else if(PersistedRacesVariant == null && _persistedRacesVariantOnLoad == null)
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
