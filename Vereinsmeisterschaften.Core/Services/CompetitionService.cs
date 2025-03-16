using System;
using System.Collections.Concurrent;
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

        public async Task<bool> CalculateRunOrder(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null)
        {
            List<PersonStart> starts = new List<PersonStart>();
            List<Competition> startCompetitions = new List<Competition>();
            foreach(Person person in _personService.GetPersons())
            {
                List<PersonStart> personStarts = person.Starts.Values.Cast<PersonStart>().Where(s => s != null).ToList();
                starts.AddRange(personStarts);
                startCompetitions.AddRange(personStarts.Select(s => GetCompetitionForPerson(person, s.Style, competitionYear)));

                cancellationToken.ThrowIfCancellationRequested();
            }

            // ***** Do Monte Carlo Partitioning *****
            List<int> startIndices = Enumerable.Range(0, starts.Count).ToList();
            List<List<List<int>>> sampledPartitions = new List<List<List<int>>>();
            
            MonteCarloPartitioning<int>.OnProgressUpdate = progress => onProgress?.Invoke(this, progress, "Monte Carlo");
            MonteCarloPartitioning<int>.OnValidPartitionFound = partition => sampledPartitions.Add(partition);
            MonteCarloPartitioning<int>.CheckPartitionValid = partition =>
            {
                // Count the number of groups with only one element
                // Check if it is less than 25% of the total number of groups --> valid partition
                int oneElementGroups = partition.Count(group => group.Count == 1);
                bool isPartitionValid = oneElementGroups <= partition.Count * 0.25;

                if (isPartitionValid)
                {
                    // Loop each run and check if all starts of this run use the same style and distance
                    foreach (List<int> group in partition)
                    {
                        List<SwimmingStyles?> runStyles = group.Select(startIndex => startCompetitions[startIndex]?.SwimmingStyle).ToList();
                        List<ushort?> runDistances = group.Select(startIndex => startCompetitions[startIndex]?.Distance).ToList();

                        // The run is invalid if the runStyles list or runDistances list contain more than one distinct element (different styles or distances in one run)
                        if (runStyles.Distinct().Count() > 1 || runDistances.Distinct().Count() > 1)
                        {
                            isPartitionValid = false;
                            break;
                        }
                    }
                }
                return isPartitionValid;
            };
            
            await MonteCarloPartitioning<int>.CalculatePartitions(startIndices, cancellationToken, numberAvailableSwimLanes, 5, 10000000);
            MonteCarloPartitioning<int>.AnalyzeAndReportResults(sampledPartitions);
            return sampledPartitions.Count > 0;
        }
    }

    // ##########################################################################################

    #region Monte Carlo Partitioning

    public class MonteCarloPartitioning<T>
    {
        private static Random _random = new Random();

        /// <summary>
        /// Action that is called when the progress of the Monte Carlo simulation changes
        /// </summary>
        public static Action<float> OnProgressUpdate;

        /// <summary>
        /// Action that is called when a new valid partition is found
        /// </summary>
        public static Action<List<List<T>>> OnValidPartitionFound;

        /// <summary>
        /// Function that is called to check if a partition is valid
        /// </summary>
        public static Func<List<List<T>>, bool> CheckPartitionValid;

        /// <summary>
        /// Calculate partitions of a list of numbers using a Monte Carlo approach (random generation of partitions).
        /// This method breaks after maxSimulationRuns loops or when minNumberPartitions are found.
        /// </summary>
        /// <param name="numbers">List with all possible numbers</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="maxElementsPerGroup">Maximum number of elements for each group</param>
        /// <param name="minNumberPartitions">Minimum number of partitions. If this number of valid partitions is reached, the calculation breaks.</param>
        /// <param name="maxSimulationRuns">Maximum number of loops for the Monte Carlo simulation</param>
        /// <returns></returns>
        public static async Task CalculatePartitions(List<T> numbers, CancellationToken cancellationToken, int maxElementsPerGroup = 3, int minNumberPartitions = 5, int maxSimulationRuns = 10000000)
        {
            ConcurrentDictionary<string, bool> uniquePartitions = new ConcurrentDictionary<string, bool>();
            int maxParallelTasks = Environment.ProcessorCount;
            int totalSimulationRuns = 0;

            using (SemaphoreSlim semaphore = new SemaphoreSlim(maxParallelTasks))
            {
                List<Task> tasks = new List<Task>();

                while (uniquePartitions.Count < minNumberPartitions && totalSimulationRuns < maxSimulationRuns)
                {
                    for (int i = 0; i < maxParallelTasks && totalSimulationRuns < maxSimulationRuns; i++)
                    {
                        await semaphore.WaitAsync(cancellationToken);
                        int currentRun = totalSimulationRuns++;
                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                List<List<T>> partition = GenerateRandomPartition(numbers, maxElementsPerGroup);

                                // only if the partition is new and valid
                                if (CheckPartitionValid?.Invoke(partition) ?? true)
                                {
                                    // Generate and store partition key to avoid duplicates
                                    string partitionKey = GetPartitionKey(partition);
                                    if (uniquePartitions.TryAdd(partitionKey, true))
                                    {
                                        OnValidPartitionFound?.Invoke(partition);
                                    }
                                }

                                // Report progress
                                float progress = (currentRun / (float)maxSimulationRuns) * 100.0f;
                                OnProgressUpdate?.Invoke(progress);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }, cancellationToken));
                    }

                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
                OnProgressUpdate?.Invoke(100.0f);
            }

            if (uniquePartitions.Count < minNumberPartitions)
            {
                Trace.WriteLine($"Warning: Only {uniquePartitions.Count} unique partitions found after {totalSimulationRuns} simulation runs.");
            }
        }

        /// <summary>
        /// Generates random partitions with a maximum group size
        /// </summary>
        /// <param name="elements">List with all elements</param>
        /// <param name="maxElementsPerGroup">Maximum number of elements for each group</param>
        /// <returns>List of Groups</returns>
        private static List<List<T>> GenerateRandomPartition(List<T> elements, int maxElementsPerGroup)
        {
            List<List<T>> partition = new List<List<T>>();
            List<T> remainingElements = new List<T>(elements);

            while (remainingElements.Count > 0)
            {
                // Weighted group size that prefers larger groups
                int groupSize = GetWeightedGroupSize(remainingElements.Count, maxElementsPerGroup);
                groupSize = Math.Min(groupSize, remainingElements.Count); // Avoid that the group gets too large

                List<T> group = new List<T>();
                for (int i = 0; i < groupSize; i++)
                {
                    int index = _random.Next(remainingElements.Count);
                    group.Add(remainingElements[index]);
                    remainingElements.RemoveAt(index);
                }
                partition.Add(group);
            }
            return partition;
        }

        /// <summary>
        /// Get weighted group size that prefers larger groups
        /// </summary>
        /// <param name="remainingElementsCount">Number of remaining elements</param>
        /// <param name="maxElementsPerGroup">Maximum number of elements for each group</param>
        /// <returns>Weighted group size</returns>
        private static int GetWeightedGroupSize(int remainingElementsCount, int maxElementsPerGroup)
        {
            if (remainingElementsCount <= maxElementsPerGroup)
            {
                return _random.Next(1, maxElementsPerGroup + 1);
            }
            else
            {
                int rand = _random.Next(1, 101);
                if (rand <= 70)                 // 70% probability for the largest group
                    return maxElementsPerGroup; 
                else if (rand <= 90) // 20% probability for a group with one element less
                    return maxElementsPerGroup - 1;
                else // 10% probability for a group with only one element
                    return 1;
            }
        }

        /// <summary>
        /// Method to generate a "key" for the partition that ignores the order
        /// </summary>
        /// <param name="partition">Partition for which a key is generated</param>
        /// <returns>Key that can be used for e.g. a <see cref="HashSet{T}"/></returns>
        private static string GetPartitionKey(List<List<T>> partition)
        {
            List<string> sortedPartition = partition.Select(group => string.Join(",", group.OrderBy(x => x)))
                                                    .OrderBy(group => group) // Sort groups so that the order does not matter
                                                    .ToList();

            return string.Join(" | ", sortedPartition);
        }

        public static void AnalyzeAndReportResults(List<List<List<T>>> sampledPartitions)
        {
            int count3Elements = 0;
            int count2Elements = 0;
            int count1Elements = 0;

            foreach (var partition in sampledPartitions)
            {
                foreach (var group in partition)
                {
                    if (group.Count == 3) count3Elements++;
                    else if (group.Count == 2) count2Elements++;
                    else if (group.Count == 1) count1Elements++;
                }
            }

            Trace.WriteLine($"Groups with 3 elements: {count3Elements}");
            Trace.WriteLine($"Groups with 2 elements: {count2Elements}");
            Trace.WriteLine($"Groups with 1 element: {count1Elements}");
            Trace.WriteLine($"Number sampled Partitions: {sampledPartitions.Count}");
            foreach (var partition in sampledPartitions.Take(10)) // Show the first 10 partitions
            {
                Trace.WriteLine($"Partition: {string.Join(" | ", partition.Select(group => $"[{string.Join(", ", group)}]"))}");
            }
        }
    }

    #endregion

    // ##########################################################################################

    #region Parallel Partitioning With Callback
    // Generiert durch Chat GPT
    class ParallelPartitioningWithCallback
    {
        public static async Task Main()
        {
            int n = 8; // Beispiel n=8
            List<int> numbers = Enumerable.Range(1, n).ToList();

            completedPartitions = 0;
            // Callbacks
            Action<List<List<int>>> onPartitionFound = partition =>
            {
                Interlocked.Increment(ref completedPartitions);
                //Trace.WriteLine($"Partition gefunden ({completedPartitions}): " + FormatPartition(partition));
            };

            Action<double> onProgressUpdate = progress =>
            {
                //Trace.WriteLine($"Fortschritt: {progress:F2}%");
            };

            // Ermittlung der Gesamtzahl der Partitionen
            long estimatedPartitions = CalculateTotalPartitions(n);

            int maxParallelTasks = Environment.ProcessorCount;
            using (var semaphore = new SemaphoreSlim(maxParallelTasks))
            {
                await GeneratePartitionsParallel(numbers, new List<List<int>>(), onPartitionFound, onProgressUpdate, semaphore, estimatedPartitions);
            }

            Trace.WriteLine($"Alle Partitionen ({completedPartitions}) wurden berechnet!");
        }

        static long completedPartitions = 0; // Sicher für Interlocked.Increment

        static long CalculateTotalPartitions(int n)
        {
            // Diese Schätzung basiert auf der Anzahl der Gruppen und ist eine grobe Abschätzung.
            // Sie kann für größere n weiter optimiert werden.
            long totalPartitions = 0;

            // Berechne alle Partitionen mit Gruppengrößen 1, 2 und 3.
            // Zum Beispiel: Partitionen für n=8 umfassen alle Kombinationen von 1, 2, 3 Elementen aus 8.
            // Wir müssen den Algorithmus dafür finden oder eine empirische Schätzung aus den Berechnungen ziehen.

            // Beispielhafte Berechnung (einfacher Ansatz, hier nur zur Illustration):
            // Diese Funktion könnte auf Basis deiner Problemdomäne genauer angepasst werden.
            for (int i = 1; i <= n; i++)
            {
                totalPartitions += Factorial(n) / (Factorial(i) * Factorial(n - i));
            }

            return totalPartitions;
        }

        static long Factorial(int number)
        {
            long result = 1;
            for (int i = 1; i <= number; i++)
            {
                result *= i;
            }
            return result;
        }

        static async Task GeneratePartitionsParallel(List<int> remaining, List<List<int>> currentPartition, Action<List<List<int>>> onPartitionFound, Action<double> onProgressUpdate, SemaphoreSlim semaphore, long estimatedPartitions)
        {
            if (remaining.Count == 0)
            {
                onPartitionFound(new List<List<int>>(currentPartition));

                // Berechnung des Fortschritts
                double progress = Math.Min((double)completedPartitions / estimatedPartitions * 100, 99.9);
                onProgressUpdate(progress);

                return;
            }

            List<Task> tasks = new List<Task>();

            foreach (var group in GetPriorityGroups(remaining))
            {
                List<int> newRemaining = remaining.Except(group).ToList();
                List<List<int>> newPartition = new List<List<int>>(currentPartition) { group };

                if (semaphore.CurrentCount > 0)
                {
                    await semaphore.WaitAsync();
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await GeneratePartitionsSequential(newRemaining, newPartition, onPartitionFound, onProgressUpdate, estimatedPartitions);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
                    tasks.Add(task);
                }
                else
                {
                    await GeneratePartitionsSequential(newRemaining, newPartition, onPartitionFound, onProgressUpdate, estimatedPartitions);
                }
            }

            await Task.WhenAll(tasks);
        }

        static async Task GeneratePartitionsSequential(List<int> remaining, List<List<int>> currentPartition, Action<List<List<int>>> onPartitionFound, Action<double> onProgressUpdate, long estimatedPartitions)
        {
            if (remaining.Count == 0)
            {
                onPartitionFound(new List<List<int>>(currentPartition));

                // Fortschritt berechnen
                double progress = Math.Min((double)completedPartitions / estimatedPartitions * 100, 99.9);
                onProgressUpdate(progress);

                return;
            }

            foreach (var group in GetPriorityGroups(remaining))
            {
                List<int> newRemaining = remaining.Except(group).ToList();
                List<List<int>> newPartition = new List<List<int>>(currentPartition) { group };

                await GeneratePartitionsSequential(newRemaining, newPartition, onPartitionFound, onProgressUpdate, estimatedPartitions);
            }
        }

        static List<List<int>> GetPriorityGroups(List<int> elements)
        {
            List<List<int>> groups = new List<List<int>>();

            // **Zuerst Gruppen mit 3 Elementen**
            if (elements.Count >= 3)
            {
                groups.AddRange(GetCombinations(elements, 3));
            }
            // **Falls nicht genug übrig sind, versuche Gruppen mit 2**
            if (elements.Count >= 2)
            {
                groups.AddRange(GetCombinations(elements, 2));
            }
            // **Nur als letzte Option einzelne Elemente**
            if (elements.Count > 0)
            {
                groups.AddRange(GetCombinations(elements, 1));
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
