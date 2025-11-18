using System.Collections.Concurrent;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Class to generate different variants of <see cref="RacesVariant"/> using the score calculation.
    /// This was created with the help of ChatGPT.
    /// </summary>
    public class RacesVariantsGenerator
    {
        private readonly Random _random = new();
        private readonly double _minScoreThreshold;
        private readonly int _maxGroupSize;
        private readonly double _maxOneElementGroupPercentage;
        private readonly IProgress<double> _progressIteration;
        private readonly IProgress<double> _progressSolution;
        private readonly int _requiredVariantsCount;
        private readonly int _maxIterations;

        /// <summary>
        /// Create a new instance of <see cref="RacesVariantsGenerator"/>. This isn't running any calculation yet.
        /// </summary>
        /// <param name="progressIteration">Progress reporting functionality for the iteration progress</param>
        /// <param name="progressSolution">Progress reporting functionality for the solution progress</param>
        /// <param name="requiredVariantsCount">Number of required variants to generate. If this number is reached, the loop breaks.</param>
        /// <param name="maxIterations">Maximum number of iterations the loop will run in the worst case.</param>
        /// <param name="minScoreThreshold">Only <see cref="RacesVariant"/> with a score higher or equal this value are kept.</param>
        /// <param name="maxGroupSize">Maximum allowed number of elements per group</param>
        /// <param name="maxOneElementGroupPercentage">Maximum percentage of single-item groups allowed in the final result</param>
        public RacesVariantsGenerator(IProgress<double> progressIteration = null, IProgress<double> progressSolution = null, int requiredVariantsCount = 100, int maxIterations = 100000, double minScoreThreshold = 90, int maxGroupSize = 3, double maxOneElementGroupPercentage = 0.15)
        {
            _progressIteration = progressIteration;
            _progressSolution = progressSolution;
            _maxGroupSize = maxGroupSize;
            _minScoreThreshold = minScoreThreshold;
            _maxOneElementGroupPercentage = maxOneElementGroupPercentage;
            _requiredVariantsCount = requiredVariantsCount;
            _maxIterations = maxIterations;
        }

        /// <summary>
        /// Calculate the variants asynchronously.
        /// </summary>
        /// <param name="sets">Sets with elements to combine. The elements are not mixed between sets.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List with different <see cref="RacesVariant"/> all with a score above <see cref="_minScoreThreshold"/></returns>
        public async Task<List<RacesVariant>> GenerateBestRacesAsync(List<List<PersonStart>> sets, CancellationToken cancellationToken = default)
        {
            ConcurrentBag<RacesVariant> bestRaces = new ConcurrentBag<RacesVariant>();
            int attempts = 0;
            int foundVariants = 0;

            await Task.Run(() =>
            {
                Parallel.For(0, _maxIterations, new ParallelOptions() { MaxDegreeOfParallelism = (int)(0.25 * Environment.ProcessorCount) }, (i, state) =>
                {
                    if (cancellationToken.IsCancellationRequested || bestRaces.Count >= _requiredVariantsCount)
                    {
                        state.Stop();
                        return;
                    }

                    // Random grouping creation
                    RacesVariant candidate = CreateValidGrouping(sets);

                    // Calculate score and keep if above threshold
                    double score = candidate.CalculateScore();
                    if (score >= _minScoreThreshold)
                    {
                        bestRaces.Add(candidate);
                        Interlocked.Increment(ref foundVariants);
                    }

                    Interlocked.Increment(ref attempts);

                    // Calculate progress as a mixture of iteration progress and solution progress
                    double iterationProgress = (double)attempts / _maxIterations;
                    double solutionProgress = (double)foundVariants / _requiredVariantsCount;
                    //double overallProgress = Math.Min(1.0, Math.Max(iterationProgress, solutionProgress));

                    _progressIteration?.Report(iterationProgress * 100);
                    _progressSolution?.Report(solutionProgress * 100);
                });
            }, cancellationToken);

            return bestRaces.OrderByDescending(r => r.Score).Take(_requiredVariantsCount).ToList();
        }

        /// <summary>
        /// Create a valid group by combining the elements within the sets randomly.
        /// </summary>
        /// <param name="sets">Sets which elements are combined</param>
        /// <returns>Randomly created <see cref="RacesVariant"/></returns>
        private RacesVariant CreateValidGrouping(List<List<PersonStart>> sets)
        {
            List<List<PersonStart>> groups = new List<List<PersonStart>>();
            List<List<PersonStart>> shuffledSets = sets.OrderBy(_ => _random.Next()).ToList(); // Shuffle sets for more randomness
            int oneElementGroupsCount = 0;

            foreach (List<PersonStart> set in shuffledSets)
            {
                HashSet<PersonStart> remainingElements = new HashSet<PersonStart>(set.OrderBy(_ => _random.Next())); // Shuffle each set

                while (remainingElements.Count > 0)
                {
                    int maxSize = Math.Min(_maxGroupSize, remainingElements.Count);
                    int minSize = Math.Max(2, (int)(maxSize * 0.5)); // Avoid too small groups

                    int groupSize = _random.Next(minSize, maxSize + 1);
                    List<PersonStart> selectedGroup = remainingElements.Take(groupSize).ToList();

                    if (selectedGroup.Count == 1)
                    {
                        oneElementGroupsCount++;
                    }

                    groups.Add(selectedGroup);
                    foreach (PersonStart item in selectedGroup)
                        remainingElements.Remove(item);
                }
            }

            // If too many single element groups were created → Adjust instead of restart
            if ((double)oneElementGroupsCount / groups.Count > _maxOneElementGroupPercentage)
            {
                groups = MergeSmallGroups(groups, sets);
            }

            RacesVariant variant = new RacesVariant();
            foreach (var group in groups)
            {
                variant.Races.Add(new Race(group));
            }

            return variant;
        }

        /// <summary>
        /// Merge small groups to reduce the number of single element groups
        /// </summary>
        /// <param name="groups">Groups to merge</param>
        /// <param name="sets">Sets which elements are combined</param>
        /// <returns>Merged groups</returns>
        private List<List<PersonStart>> MergeSmallGroups(List<List<PersonStart>> groups, List<List<PersonStart>> sets)
        {
            List<List<PersonStart>> result = new();
            HashSet<int> mergedIndices = new();

            for (int i = 0; i < groups.Count; i++)
            {
                if (mergedIndices.Contains(i))
                    continue;

                var groupA = groups[i];

                // Nur kleine Gruppen betrachten
                if (groupA.Count >= 1)
                {
                    result.Add(groupA);
                    continue;
                }

                bool merged = false;

                for (int j = i + 1; j < groups.Count; j++)
                {
                    if (mergedIndices.Contains(j))
                        continue;

                    var groupB = groups[j];

                    if (groupA.Count + groupB.Count <= _maxGroupSize &&
                        AreFromSameSet(groupA.Concat(groupB).ToList(), sets))
                    {
                        result.Add(groupA.Concat(groupB).ToList());
                        mergedIndices.Add(i);
                        mergedIndices.Add(j);
                        merged = true;
                        break;
                    }
                }

                if (!merged)
                {
                    result.Add(groupA);
                }
            }

            return result;
        }

        private bool AreFromSameSet(List<PersonStart> starts, List<List<PersonStart>> sets)
        {
            foreach (var set in sets)
            {
                if (starts.All(p => set.Contains(p)))
                    return true;
            }
            return false;
        }
    }
}
