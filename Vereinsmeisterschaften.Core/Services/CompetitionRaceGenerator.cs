using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Class to generate different variants of <see cref="CompetitionRaces"/> using the score calculation.
    /// </summary>
    public class CompetitionRaceGenerator
    {
        private readonly Random _random = new();
        private readonly double _minScoreThreshold;
        private readonly int _maxGroupSize;
        private readonly double _maxOneElementGroupPercentage;
        private readonly IProgress<double> _progress;
        private readonly int _requiredVariantsCount;
        private readonly int _maxIterations;

        /// <summary>
        /// Create a new instance of <see cref="CompetitionRaceGenerator"/>. This isn't running any calculation yet.
        /// </summary>
        /// <param name="progress">Progress reporting functionality</param>
        /// <param name="requiredVariantsCount">Number of required variants to generate. If this number is reached, the loop breaks.</param>
        /// <param name="maxIterations">Maximum number of iterations the loop will run in the worst case.</param>
        /// <param name="minScoreThreshold">Only <see cref="CompetitionRaces"/> with a score higher or equal this value are kept.</param>
        /// <param name="maxGroupSize">Maximum allowed number of elements per group</param>
        /// <param name="maxOneElementGroupPercentage">Maximum percentage of single-item groups allowed in the final result</param>
        public CompetitionRaceGenerator(IProgress<double> progress = null, int requiredVariantsCount = 100, int maxIterations = 100000, int minScoreThreshold = 90, int maxGroupSize = 3, double maxOneElementGroupPercentage = 0.15)
        {
            _progress = progress;
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
        /// <returns>List with different <see cref="CompetitionRaces"/> all with a score above <see cref="_minScoreThreshold"/></returns>
        public async Task<List<CompetitionRaces>> GenerateBestRacesAsync(List<List<PersonStart>> sets, CancellationToken cancellationToken = default)
        {
            ConcurrentBag<CompetitionRaces> bestRaces = new ConcurrentBag<CompetitionRaces>();
            int attempts = 0;
            int foundVariants = 0;

            await Task.Run(() =>
            {
                Parallel.For(0, _maxIterations, (i, state) =>
                {
                    if (cancellationToken.IsCancellationRequested || bestRaces.Count >= _requiredVariantsCount)
                    {
                        state.Stop();
                        return;
                    }

                    // Random grouping creation
                    CompetitionRaces candidate = CreateValidGrouping(sets);

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
                    double overallProgress = Math.Min(1.0, Math.Max(iterationProgress, solutionProgress));

                    _progress?.Report(overallProgress * 100);
                });
            }, cancellationToken);

            return bestRaces.OrderByDescending(r => r.Score).ToList();
        }

        /// <summary>
        /// Create a valid group by combining the elements within the sets randomly.
        /// </summary>
        /// <param name="sets">Sets which elements are combined</param>
        /// <returns>Randomly created <see cref="CompetitionRaces"/></returns>
        private CompetitionRaces CreateValidGrouping(List<List<PersonStart>> sets)
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

            var competitionRaces = new CompetitionRaces();
            foreach (var group in groups)
            {
                competitionRaces.Races.Add(new Race(group));
            }

            return competitionRaces;
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
