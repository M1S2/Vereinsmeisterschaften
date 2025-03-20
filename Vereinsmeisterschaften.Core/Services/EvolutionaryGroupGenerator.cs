using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Generator for creating groups of elements that must not be mixed with other groups.
    /// This class was generated with the help of ChatGPT.
    /// </summary>
    /// <typeparam name="T">Type of the elements</typeparam>
    public class EvolutionaryGroupGenerator<T>
    {
        private readonly List<List<T>> _sets;
        private readonly int _populationSize;
        private readonly int _generations;
        private readonly int _maxGroupSize;
        private readonly double _maxOneElementGroupPercentage;
        private readonly Action<double> _progressCallback;
        private readonly Random _random = new Random();

        /// <summary>
        /// Generate the <see cref="EvolutionaryGroupGenerator{T}"/> object. This is not running any calculations.
        /// </summary>
        /// <param name="sets">List of sets, where each set contains elements that must not be mixed with other sets.</param>
        /// <param name="populationSize">Number of different groupings generated per generation. Higher values improve diversity but increase computation time.</param>
        /// <param name="generations">
        ///     The 'generations' parameter controls the number of evolution cycles.
        ///     - More generations improve grouping quality by reducing single-item groups and ensuring all elements are used.
        ///     - Fewer generations run faster but may produce suboptimal results.
        ///     - High values increase computation time but lead to better distribution of elements.
        /// </param>
        /// <param name="maxGroupSize">Maximum allowed number of elements per group</param>
        /// <param name="maxSingleGroupPercentage">Maximum percentage of single-item groups allowed in the final result</param>
        /// <param name="progressCallback">Callback to report the progess of the calculation</param>
        public EvolutionaryGroupGenerator(List<List<T>> sets, int populationSize, int generations, int maxGroupSize, double maxSingleGroupPercentage, Action<double> progressCallback)
        {
            _sets = sets;
            _populationSize = populationSize;
            _generations = generations;
            _maxGroupSize = maxGroupSize;
            _maxOneElementGroupPercentage = maxSingleGroupPercentage;
            _progressCallback = progressCallback;
        }

        /// <summary>
        /// Generate the groupings asynchronously. This method is cancellable.
        /// </summary>
        /// <param name="token">CancellationToken used to cancel this method if needed</param>
        /// <returns>List with possible shuffled sets</returns>
        public async Task<List<List<List<T>>>> GenerateAsync(CancellationToken token)
        {
            var bestGroupings = new ConcurrentBag<List<List<T>>>();

            var population = new List<List<List<T>>>();
            for (int i = 0; i < _populationSize; i++)
            {
                token.ThrowIfCancellationRequested();
                population.Add(CreateValidGrouping());
            }

            for (int gen = 0; gen < _generations; gen++)
            {
                token.ThrowIfCancellationRequested();

                var evolvedPopulation = await Task.WhenAll(population.Select(async individual =>
                {
                    token.ThrowIfCancellationRequested();
                    return await EvolveAsync(individual, token);
                })).ConfigureAwait(false);

                population = evolvedPopulation.ToList();

                _progressCallback?.Invoke((double)gen / _generations * 100);
            }

            foreach (var grouping in population.Distinct(new GroupingComparer<T>()))
            {
                bestGroupings.Add(grouping);
            }

            // Return with random order
            return bestGroupings.OrderBy(_ => _random.Next()).ToList();
        }

        private List<List<T>> CreateValidGrouping()
        {
            var groups = new List<List<T>>();
            int oneElementGroupsCount = 0;

            foreach (var set in _sets)
            {
                var remainingElements = new HashSet<T>(set);

                // If a set has less than or equal to maxGroupSize elements, keep it as a group together
                if (remainingElements.Count <= _maxGroupSize)
                {
                    groups.Add(remainingElements.ToList());
                    continue;
                }

                // Otherwise, divide into groups
                while (remainingElements.Count > 0)
                {
                    int groupSize = _random.Next(2, Math.Min(_maxGroupSize + 1, remainingElements.Count + 1));
                    var selectedGroup = remainingElements.OrderBy(_ => _random.Next()).Take(groupSize).ToList();

                    if (groupSize == 1)
                    {
                        oneElementGroupsCount++;
                    }

                    groups.Add(selectedGroup);
                    foreach (var item in selectedGroup)
                        remainingElements.Remove(item);
                }
            }

            // If more than _maxSingleGroupPercentage of the groups are single-item groups, discard the solution and regenerate
            if ((double)oneElementGroupsCount / groups.Count > _maxOneElementGroupPercentage)
            {
                // Recursion until we have a valid grouping
                return CreateValidGrouping();
            }

            // Shuffle the groups to avoid bias
            return groups.OrderBy(_ => _random.Next()).ToList();
        }

        private async Task<List<List<T>>> EvolveAsync(List<List<T>> individual, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                return _random.NextDouble() < 0.4 ? Mutate(individual) : individual;
            }, token).ConfigureAwait(false);
        }

        private List<List<T>> Mutate(List<List<T>> groups)
        {
            var newGroups = groups.Select(g => new List<T>(g)).ToList();

            if (newGroups.Count < 2) return newGroups;

            int g1 = _random.Next(newGroups.Count);
            int g2 = _random.Next(newGroups.Count);

            if (g1 == g2 || GetSetIndex(newGroups[g1][0]) != GetSetIndex(newGroups[g2][0]))
                return newGroups;

            var swapIdx1 = _random.Next(newGroups[g1].Count);
            var swapIdx2 = _random.Next(newGroups[g2].Count);

            var temp = newGroups[g1][swapIdx1];
            newGroups[g1][swapIdx1] = newGroups[g2][swapIdx2];
            newGroups[g2][swapIdx2] = temp;

            // Shuffle the groups to avoid bias
            return newGroups.OrderBy(_ => _random.Next()).ToList();
        }

        private int GetSetIndex(T element)
        {
            for (int i = 0; i < _sets.Count; i++)
            {
                if (_sets[i].Contains(element))
                    return i;
            }
            return -1;
        }
    }

    /// <summary>
    /// Compares two groupings of elements (a list of lists of elements) for equality.
    /// </summary>
    /// <typeparam name="T">The type of elements in the groups.</typeparam>
    /// <remarks>
    /// This class compares the groupings by ensuring that the groups contain the same elements, regardless of the order of elements within each group.
    /// The order of the groups themselves is also ignored.
    /// </remarks>
    public class GroupingComparer<T> : IEqualityComparer<List<List<T>>>
    {
        private readonly GroupComparer<T> _groupComparer = new GroupComparer<T>();

        public bool Equals(List<List<T>> x, List<List<T>> y)
        {
            if (x == null || y == null) return false;
            if (x.Count != y.Count) return false;

            var xSets = new HashSet<HashSet<T>>(x.Select(g => new HashSet<T>(g)), HashSet<T>.CreateSetComparer());
            var ySets = new HashSet<HashSet<T>>(y.Select(g => new HashSet<T>(g)), HashSet<T>.CreateSetComparer());

            return xSets.SetEquals(ySets);
        }

        public int GetHashCode(List<List<T>> obj)
        {
            if (obj == null) return 0;
            return obj.Select(group => _groupComparer.GetHashCode(group)).Aggregate(0, (a, b) => a ^ b);
        }
    }

    /// <summary>
    /// Compares two lists of elements (of type <typeparamref name="T"/>) for equality, regardless of the order of elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the group.</typeparam>
    /// <remarks>
    /// This class uses HashSets to ignore the order of elements in the lists and provides an efficient equality check.
    /// </remarks>
    public class GroupComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T> x, List<T> y)
        {
            if (x == null || y == null) return false;
            return x.Count == y.Count && new HashSet<T>(x).SetEquals(y);
        }

        public int GetHashCode(List<T> obj)
        {
            if (obj == null) return 0;
            return obj.Select(item => item.GetHashCode()).Aggregate(0, (a, b) => a ^ b);
        }
    }
}
