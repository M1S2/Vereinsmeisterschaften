using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

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
        public string CompetitionFilePath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all competitions
        /// </summary>
        private List<Competition> _competitionList { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CompetitionService(IFileService fileService)
        {
            _competitionList = new List<Competition>();
            _fileService = fileService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of Competitions to the <see cref="_competitionList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_competitionList"/> is cleared and the functions returns loading success.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> LoadFromFile(CancellationToken cancellationToken)
        {
            bool importingResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(CompetitionFilePath))
                    {
                        OnFileProgress?.Invoke(this, 0);
                        _competitionList.Clear();
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        _competitionList = _fileService.LoadFromCsv<Competition>(CompetitionFilePath, cancellationToken, Competition.SetPropertyFromString, OnFileProgress);
                    }
                    
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
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> SaveToFile(CancellationToken cancellationToken)
        {
            bool saveResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    _fileService.SaveToCsv(CompetitionFilePath, _competitionList, cancellationToken, OnFileProgress);
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
        /// Return the competition that matches the person and swimming style
        /// </summary>
        /// <param name="person"><see cref="Person"/> used to search the <see cref="Competition"/></param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/> that must match the <see cref="Competition"/></param>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        public Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle, ushort competitionYear)
        {
            if (!person.Starts.ContainsKey(swimmingStyle))
            {
                return null;
            }
            return _competitionList.Where(c => c.Gender == person.Gender &&
                                               c.Age + person.BirthYear == competitionYear &&
                                               c.SwimmingStyle == swimmingStyle).FirstOrDefault();
        }
    }
}
