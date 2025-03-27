using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using static Vereinsmeisterschaften.Core.Services.CompetitionService;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of Competition objects
    /// </summary>
    public class CompetitionService : ICompetitionService
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

        /// <summary>
        /// Path to the competition file
        /// </summary>
        public string PersistentPath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all competitions
        /// </summary>
        private List<Competition> _competitionList { get; set; }

        /// <summary>
        /// List with all competitions at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private List<Competition> _competitionListOnLoad { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;
        private IPersonService _personService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CompetitionService(IFileService fileService, IPersonService personService)
        {
            _competitionList = new List<Competition>();
            _fileService = fileService;
            _personService = personService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of Competitions to the <see cref="_competitionList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_competitionList"/> is cleared and the functions returns loading success.
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
                    if (!File.Exists(path))
                    {
                        OnFileProgress?.Invoke(this, 0);
                        _competitionList.Clear();
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        _competitionList = _fileService.LoadFromCsv<Competition>(path, cancellationToken, Competition.SetPropertyFromString, OnFileProgress);
                    }

                    _competitionListOnLoad = _competitionList.ConvertAll(c => new Competition(c));

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

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Save the list of Competitions to a file
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
                    _fileService.SaveToCsv(path, _competitionList, cancellationToken, OnFileProgress);

                    _competitionListOnLoad = _competitionList.ConvertAll(c => new Competition(c));
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
        /// Return all available Competitions
        /// </summary>
        /// <returns>List of <see cref="Competition"/> objects</returns>
        public List<Competition> GetCompetitions() => _competitionList;

        /// <summary>
        /// Clear all Competitions
        /// </summary>
        public void ClearAll()
        {
            if (_competitionList == null) { _competitionList = new List<Competition>(); }
            _competitionList.Clear();
        }

        /// <summary>
        /// Add a new <see cref="Competition"/> to the list of Competitions.
        /// </summary>
        /// <param name="person"><see cref="Competition"/> to add</param>
        public void AddCompetition(Competition competition)
        {
            if (_competitionList == null) { _competitionList = new List<Competition>(); }
            _competitionList.Add(competition);
        }

        /// <summary>
        /// Return the number of <see cref="Competition"/>
        /// </summary>
        /// <returns>Number of <see cref="Competition"/></returns>
        public int CompetitionCount => _competitionList?.Count ?? 0;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Check if the list of <see cref="Competition"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => (_competitionList != null && _competitionListOnLoad != null) ? !_competitionList.SequenceEqual(_competitionListOnLoad) : false;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Return the competition that matches the person and swimming style
        /// </summary>
        /// <param name="person"><see cref="Person"/> used to search the <see cref="Competition"/></param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/> that must match the <see cref="Competition"/></param>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        public Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle, ushort competitionYear)
        {
            if (person.Starts[swimmingStyle] == null)
            {
                return null;
            }
            return _competitionList.Where(c => c.Gender == person.Gender &&
                                               c.SwimmingStyle == swimmingStyle &&
                                               (c.SwimmingStyle == SwimmingStyles.WaterFlea ? true : c.Age + person.BirthYear == competitionYear)).FirstOrDefault();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects for the given <see cref="Person"/> with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="person"><see cref="Person"/> to update</param>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        public void UpdateAllCompetitionsForPersonStarts(Person person, ushort competitionYear)
        {
            foreach(PersonStart personStart in _personService.GetAllPersonStartsForPerson(person))
            {
                Competition competition = GetCompetitionForPerson(person, personStart.Style, competitionYear);
                if (competition != null)
                {
                    personStart.CompetitionObj = competition;
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        public void UpdateAllCompetitionsForPersonStarts(ushort competitionYear)
        {
            foreach (Person person in _personService.GetPersons())
            {
                UpdateAllCompetitionsForPersonStarts(person, competitionYear);
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with the the <see cref="CompetitionRaces"/> of the last time <see cref="CalculateRunOrder(ushort, CancellationToken, int, ProgressDelegate)"/> was called
        /// </summary>
        public List<CompetitionRaces> LastCalculatedCompetitionRaces { get; set; }

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
            UpdateAllCompetitionsForPersonStarts(competitionYear);
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
                else if(groupedValuesStarts[key] == null)
                {
                    groupedValuesStarts[key] = new List<PersonStart>();
                }
                groupedValuesStarts[key].Add(starts[i]);
            }

            int numberOfResultsToGenerate = 100;
            CompetitionRaceGenerator generator = new CompetitionRaceGenerator(new Progress<double>(progress => onProgress?.Invoke(this, (float)progress, "")), numberOfResultsToGenerate, 1000000, 90);
            LastCalculatedCompetitionRaces = await generator.GenerateBestRacesAsync(groupedValuesStarts.Values.ToList(), cancellationToken);

#if false
            // Create groups of competitions with same style and distance
            Dictionary<(SwimmingStyles, ushort), List<int>> groupedValues = new Dictionary<(SwimmingStyles, ushort), List<int>>();
            for (int i = 0; i < starts.Count; i++)
            {
                if (starts[i].CompetitionObj == null) { continue; }

                (SwimmingStyles, ushort) key = (starts[i].CompetitionObj.SwimmingStyle, starts[i].CompetitionObj.Distance);
                if (!groupedValuesStarts.ContainsKey(key))
                {
                    groupedValues.Add(key, new List<int>());
                }
                else if (groupedValuesStarts[key] == null)
                {
                    groupedValues[key] = new List<int>();
                }
                groupedValues[key].Add(i);
            }

            int numberOfResultsToGenerate = 100;
            EvolutionaryGroupGenerator<int> generator2 = new EvolutionaryGroupGenerator<int>(groupedValues.Values.ToList(), numberOfResultsToGenerate, 300, numberAvailableSwimLanes, 0.25,
                progress => onProgress?.Invoke(this, (float)progress, ""));
            List<List<List<int>>> results = await generator2.GenerateAsync(cancellationToken);

            LastCalculatedCompetitionRaces = new List<CompetitionRaces>();
            foreach (List<List<int>> combination in results)
            {
                Trace.WriteLine(string.Join(" | ", combination.Select(g => $"[{string.Join(", ", g)}]")));

                CompetitionRaces competitionRaces = new CompetitionRaces();
                foreach(List<int> group in combination)
                {
                    Race race = new Race(group.Select(index => starts[index]).ToList());
                    competitionRaces.Races.Add(race);
                }
                LastCalculatedCompetitionRaces.Add(competitionRaces);
            }

            LastCalculatedCompetitionRaces = LastCalculatedCompetitionRaces.OrderByDescending(compRace => compRace.CalculateScore()).ToList();
#endif

            return LastCalculatedCompetitionRaces?.Count == 0 ? null : LastCalculatedCompetitionRaces;
        }
    }
}
