using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
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
                                               c.Age + person.BirthYear == competitionYear &&
                                               c.SwimmingStyle == swimmingStyle).FirstOrDefault();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public async void CalculateRunOrder(ushort competitionYear)
        {
            List<PersonStart> starts = new List<PersonStart>();
            List<Competition> startCompetitions = new List<Competition>();
            foreach(Person person in _personService.GetPersons())
            {
                List<PersonStart> personStarts = person.Starts.Values.Cast<PersonStart>().Where(s => s != null).ToList();
                starts.AddRange(personStarts);
                startCompetitions.AddRange(personStarts.Select(s => GetCompetitionForPerson(person, s.Style, competitionYear)));
            }

            // Test some partitioning methods
            await ParallelPartitioningWithCallback.Main();


            //List<int> tmpStartsRunNumbers = Enumerable.Repeat(1, starts.Count).ToList();       // initialize the list with all ones
            //for (double hugeNumber = 0; hugeNumber < Math.Pow(starts.Count, starts.Count); hugeNumber++)
            //{
            //    for (int i = 0; i < starts.Count; i++)
            //    {

            //    }
            //}
        }
    }

    // ##########################################################################################

    #region Parallel Partitioning With Callback
    // Generiert durch Chat GPT
    class ParallelPartitioningWithCallback
    {
        public async static Task Main()
        {
            int n = 4; // Anzahl der Elemente (Test mit kleineren Werten, dann n=60)
            List<int> numbers = Enumerable.Range(1, n).ToList();

            // Callback-Methode zur Verarbeitung der Ergebnisse
            Action<List<List<int>>> onPartitionFound = partition =>
            {
                Trace.WriteLine("Partition gefunden: " + FormatPartition(partition));
            };

            // Parallele Berechnung starten
            int maxParallelTasks = Environment.ProcessorCount; // Begrenze parallele Tasks
            using (SemaphoreSlim semaphore = new SemaphoreSlim(maxParallelTasks))
            {
                await GeneratePartitionsParallel(numbers, new List<List<int>>(), onPartitionFound, semaphore);
            }

            Console.WriteLine("Alle Partitionen wurden berechnet!");
        }

        static async Task GeneratePartitionsParallel(List<int> remaining, List<List<int>> currentPartition, Action<List<List<int>>> callback, SemaphoreSlim semaphore)
        {
            if (remaining.Count == 0)
            {
                callback(new List<List<int>>(currentPartition)); // Partition speichern
                return;
            }

            List<Task> tasks = new List<Task>();

            // Erzeuge ALLE Kombinationen von 1 bis 3 Elementen aus "remaining"
            foreach (var group in GetAllGroups(remaining, 3))
            {
                List<int> newRemaining = remaining.Except(group).ToList();
                List<List<int>> newPartition = new List<List<int>>(currentPartition) { group };

                // SemaphoreSlim begrenzt parallele Tasks
                await semaphore.WaitAsync();
                var task = GeneratePartitionsParallel(newRemaining, newPartition, callback, semaphore)
                    .ContinueWith(t => semaphore.Release()); // Freigabe nach Abschluss

                tasks.Add(task);
            }

            await Task.WhenAll(tasks); // Warten, bis alle Tasks beendet sind
        }

        static List<List<int>> GetAllGroups(List<int> elements, int maxSize)
        {
            List<List<int>> groups = new List<List<int>>();

            // Erzeuge alle Gruppen von 1 bis maxSize
            for (int size = 1; size <= Math.Min(maxSize, elements.Count); size++)
            {
                groups.AddRange(GetCombinations(elements, size));
            }

            return groups;
        }

        static List<List<int>> GetCombinations(List<int> elements, int length)
        {
            if (length == 0) return new List<List<int>> { new List<int>() };
            if (elements.Count == 0) return new List<List<int>>();

            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < elements.Count; i++)
            {
                int current = elements[i];
                List<int> remaining = elements.Skip(i + 1).ToList();
                foreach (var subCombo in GetCombinations(remaining, length - 1))
                {
                    subCombo.Insert(0, current);
                    result.Add(subCombo);
                }
            }
            return result;
        }

        static string FormatPartition(List<List<int>> partition)
        {
            return "[" + string.Join(", ", partition.Select(group => $"[{string.Join(", ", group)}]")) + "]";
        }
    }

    #endregion

    // ##########################################################################################

    #region Simple Partitioning

    // Generiert durch Chat GPT
    class Partitioning
    {
        public static void Main()
        {
            int n = 8; // Anzahl der Elemente
            List<int> numbers = Enumerable.Range(1, n).ToList();

            List<List<List<int>>> partitions = GeneratePartitions(numbers, 3);

            // Gib die Ergebnisse aus
            foreach (var partition in partitions)
            {
                Console.WriteLine($"[{string.Join(", ", partition.Select(group => $"[{string.Join(", ", group)}]"))}]");
            }
        }

        static List<List<List<int>>> GeneratePartitions(List<int> elements, int maxGroupSize)
        {
            List<List<List<int>>> result = new List<List<List<int>>>();
            List<List<int>> currentPartition = new List<List<int>>();
            GeneratePartitionsRecursive(elements, maxGroupSize, currentPartition, result);
            return result;
        }

        static void GeneratePartitionsRecursive(List<int> remaining, int maxGroupSize, List<List<int>> currentPartition, List<List<List<int>>> result)
        {
            if (remaining.Count == 0)
            {
                result.Add(currentPartition.Select(group => new List<int>(group)).ToList());
                return;
            }

            // Erzeuge Gruppen von 1 bis maxGroupSize
            for (int size = 1; size <= Math.Min(maxGroupSize, remaining.Count); size++)
            {
                foreach (var group in GetCombinations(remaining, size))
                {
                    List<int> newRemaining = new List<int>(remaining);
                    foreach (var item in group)
                    {
                        newRemaining.Remove(item);
                    }

                    currentPartition.Add(group);
                    GeneratePartitionsRecursive(newRemaining, maxGroupSize, currentPartition, result);
                    currentPartition.RemoveAt(currentPartition.Count - 1);
                }
            }
        }

        static List<List<int>> GetCombinations(List<int> elements, int length)
        {
            if (length == 0) return new List<List<int>> { new List<int>() };
            if (elements.Count == 0) return new List<List<int>>();

            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < elements.Count; i++)
            {
                int current = elements[i];
                List<int> remaining = elements.Skip(i + 1).ToList();
                foreach (var subCombo in GetCombinations(remaining, length - 1))
                {
                    subCombo.Insert(0, current);
                    result.Add(subCombo);
                }
            }
            return result;
        }
    }

#endregion

    // ##########################################################################################

    #region DP Backtracking Partitioning

    // Generiert durch Chat GPT
    class DPBacktrackingPartitioning
    {
        public static void Main()
        {
            int n = 8; // Anzahl der Elemente
            string outputPath = "C:\\Users\\Markus\\Desktop\\partitions_dp.txt"; // Datei zum Speichern

            // Falls die Datei existiert, vorher löschen
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            List<int> numbers = Enumerable.Range(1, n).ToList();
            GeneratePartitions(numbers, new List<List<int>>(), outputPath);

            Console.WriteLine("Alle Partitionen wurden berechnet und gespeichert.");
        }

        static void GeneratePartitions(List<int> remaining, List<List<int>> currentPartition, string filePath)
        {
            if (remaining.Count == 0)
            {
                // Partition formatieren
                string partitionString = "[" + string.Join(", ", currentPartition.Select(group => $"[{string.Join(", ", group)}]")) + "]";

                // Partition in Datei schreiben
                File.AppendAllText(filePath, partitionString + Environment.NewLine);
                return;
            }

            // Erzeuge Gruppen von 1 bis 3 Elementen
            for (int size = 1; size <= Math.Min(3, remaining.Count); size++)
            {
                foreach (var group in GetCombinations(remaining, size))
                {
                    List<int> newRemaining = new List<int>(remaining);
                    foreach (var item in group)
                    {
                        newRemaining.Remove(item);
                    }

                    currentPartition.Add(group);
                    GeneratePartitions(newRemaining, currentPartition, filePath);
                    currentPartition.RemoveAt(currentPartition.Count - 1);
                }
            }
        }

        static List<List<int>> GetCombinations(List<int> elements, int length)
        {
            if (length == 0) return new List<List<int>> { new List<int>() };
            if (elements.Count == 0) return new List<List<int>>();

            List<List<int>> result = new List<List<int>>();
            for (int i = 0; i < elements.Count; i++)
            {
                int current = elements[i];
                List<int> remaining = elements.Skip(i + 1).ToList();
                foreach (var subCombo in GetCombinations(remaining, length - 1))
                {
                    subCombo.Insert(0, current);
                    result.Add(subCombo);
                }
            }
            return result;
        }
    }

#endregion


}
