using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a combination of all single races.
    /// </summary>
    public class CompetitionRaces : ObservableObject, IEquatable<CompetitionRaces>, ICloneable
    {
        /// <summary>
        /// List with races
        /// </summary>
        public ObservableCollection<Race> Races { get; set; }

        public CompetitionRaces()
        {
            Races = new ObservableCollection<Race>();
            Races.CollectionChanged += (s, e) => { CalculateScore(); updateRaceStartsCollectionChangedEvent(); };
        }

        public CompetitionRaces(List<Race> races)
        {
            Races = new ObservableCollection<Race>(races);
            Races.CollectionChanged += (s, e) => { CalculateScore(); updateRaceStartsCollectionChangedEvent(); };
            CalculateScore();
            updateRaceStartsCollectionChangedEvent();
        }

        public CompetitionRaces(ObservableCollection<Race> races)
        {
            Races = races;
            Races.CollectionChanged += (s, e) => { CalculateScore(); updateRaceStartsCollectionChangedEvent(); };
            CalculateScore();
            updateRaceStartsCollectionChangedEvent();
        }

        public CompetitionRaces(CompetitionRaces other) : this()
        {
            if(other == null) { return; }
            // Create a deep copy of the list
            Races = new ObservableCollection<Race>(other.Races.Select(item => (Race)item.Clone()));
            Races.CollectionChanged += (s, e) => { CalculateScore(); updateRaceStartsCollectionChangedEvent(); };
            CalculateScore();
            updateRaceStartsCollectionChangedEvent();
        }

        private void updateRaceStartsCollectionChangedEvent()
        {
            OnPropertyChanged(nameof(Races));
            foreach (Race race in Races)
            {
                race.Starts.CollectionChanged -= collectionChangedEventHandler;
                race.Starts.CollectionChanged += collectionChangedEventHandler;
            }
        }

        private void collectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateScore();
            OnPropertyChanged(nameof(Races));
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Overall score. This combines all single score using weights.
        /// </summary>
        public double Score
        {
            get
            {
                double score = 0;
                score += ScoreSingleStarts * 0.15;          // 15% Weight
                score += ScoreSameStyleSequence * 0.05;     //  5% Weight
                score += ScorePersonStartPauses * 0.70;     // 70% Weight
                score += ScoreStyleOrder * 0.10;            // 10% Weight

                return LimitValue(score, 0, 100);
            }
        }

        private double _scoreSingleStarts;
        /// <summary>
        /// Score regarding single starts
        /// </summary>
        public double ScoreSingleStarts
        {
            get => _scoreSingleStarts;
            set => SetProperty(ref _scoreSingleStarts, value);
        }

        private double _scoreSameStyleSequence;
        /// <summary>
        /// Score regarding same styles in sequence
        /// </summary>
        public double ScoreSameStyleSequence
        {
            get => _scoreSameStyleSequence;
            set => SetProperty(ref _scoreSameStyleSequence, value);
        }

        private double _scorePersonStartPauses;
        /// <summary>
        /// Score regarding pauses between person starts
        /// </summary>
        public double ScorePersonStartPauses
        {
            get => _scorePersonStartPauses;
            set => SetProperty(ref _scorePersonStartPauses, value);
        }

        private double _scoreStyleOrder;
        /// <summary>
        /// Score regarding the preferred order of the styles
        /// </summary>
        public double ScoreStyleOrder
        {
            get => _scoreStyleOrder;
            set => SetProperty(ref _scoreStyleOrder, value);
        }

        /// <summary>
        /// Recalculate all scores
        /// </summary>
        /// <returns>Overall score</returns>
        public double CalculateScore()
        {
            if (Races.Count == 0)
            {
                ScoreSingleStarts = 0;
                ScoreSameStyleSequence = 0;
                ScorePersonStartPauses = 0;
                ScoreStyleOrder = 0;
            }
            else
            {
                ScoreSingleStarts = EvaluateSingleStarts();
                ScoreSameStyleSequence = EvaluateSameStyleSequence();
                ScorePersonStartPauses = EvaluatePersonStartPauses();
                ScoreStyleOrder = EvaluateStyleOrder();
            }
            OnPropertyChanged(nameof(Score));
            return Score;
        }

        /// <summary>
        /// Score for single starts:
        /// The less races with only one start the better
        /// </summary>
        /// <returns>Score for single starts</returns>
        private double EvaluateSingleStarts()
        {
            if (Races.Count == 0) return 0;

            int singleStarts = Races.Count(r => r.Starts.Count == 1);
            double ratio = (double)singleStarts / Races.Count;

            // The less single starts, the better
            return 100 * (1 - ratio);
        }

        /// <summary>
        /// Score for same styles in sequence:
        /// The more consecutive races with the same style the better
        /// </summary>
        /// <returns>Score for same styles in sequence</returns>
        private double EvaluateSameStyleSequence()
        {
            if (Races.Count <= 1) return 100;   // Only one race = no evaluation needed

            int matchingPairs = 0;
            for (int i = 1; i < Races.Count; i++)
            {
                if (Races[i].Style == Races[i - 1].Style)
                    matchingPairs++;
            }

            return 100 * ((double)matchingPairs / (Races.Count - 1));
        }

        /// <summary>
        /// Score for person pauses:
        /// The more pause between the starts of a person the better
        /// This returns 0 if there are any <see cref="PersonStart"/> of the same person that have a pause of less than 3 races
        /// </summary>
        /// <returns>Score for person pauses</returns>
        private double EvaluatePersonStartPauses()
        {
            double penalty = 0;
            double maxPenalty = 0;  // This is calculated dynamically

            Dictionary<Person, int> lastRaceIndex = new();
            Dictionary<Person, double> individualPenalties = new();

            int severelyAffectedPersons = 0;    // Number of persons with hard penalty

            for (int i = 0; i < Races.Count; i++)
            {
                foreach (var personStart in Races[i].Starts)
                {
                    Person person = personStart.PersonObj;

                    if (lastRaceIndex.TryGetValue(person, out int lastIndex))
                    {
                        int distance = i - lastIndex;

                        double personPenalty = distance switch
                        {
                            1 => 100,   // Extremly high penalty for directly consecutive starts
                            2 => 100,   // Extremly high penalty for only one race pause
                            3 => 50,    // Noticeable penalty for two races pause
                            4 => 30,   // Noticeable penalty for three races pause
                            _ => Math.Max(0, 10 - distance) // Minimal penalty for 3+ races pause
                        };

                        // if the distance is below 3 races, this is regarded as severly affected
                        if (distance < 3)
                        {
                            severelyAffectedPersons++;
                        }

                        // Save the highest penalty for a person
                        if (!individualPenalties.ContainsKey(person) || individualPenalties[person] < personPenalty)
                        {
                            individualPenalties[person] = personPenalty;

                            maxPenalty += 100;  // Add the highest possible penalty
                        }
                    }

                    lastRaceIndex[person] = i;
                }
            }

            // Return the worst score if there are severely affected persons
            if (severelyAffectedPersons > 0)
            {
                return 0;
            }
            else
            {
                // Sum up the hardest penalties per person
                penalty = individualPenalties.Values.Sum();
                double score = 100 - (penalty / maxPenalty * 100);
                return LimitValue(score, 0, 100);
            }
        }

        private readonly Dictionary<SwimmingStyles, int> STYLE_PRIORITY = new()
        {
            { SwimmingStyles.Breaststroke, 1 },     // lower numbers should start earlier
            { SwimmingStyles.Freestyle, 2 },
            { SwimmingStyles.Backstroke, 3 },
            { SwimmingStyles.Butterfly, 4 },
            { SwimmingStyles.Medley, 5 },
            { SwimmingStyles.WaterFlea, 6 }
        };

        /// <summary>
        /// Score for style order:
        /// There is a preferred order for the styles (<see cref="STYLE_PRIORITY"/>). The more the races follow this order, the better.
        /// </summary>
        /// <returns>Score for style order</returns>
        private double EvaluateStyleOrder()
        {
            if (Races.Count == 0) return 100;

            double penalty = 0;
            foreach (var (race, index) in Races.Select((r, i) => (r, i)))
            {
                if (STYLE_PRIORITY.TryGetValue(race.Style, out int priority))
                {
                    int expectedMinIndex = Races.Count * priority / STYLE_PRIORITY.Count;
                    if (index < expectedMinIndex)
                    {
                        penalty += (expectedMinIndex - index);
                    }
                }
            }

            double maxPenalty = Races.Count * STYLE_PRIORITY.Count;
            return 100 - LimitValue(100 * (penalty / maxPenalty), 0, 100);
        }

        /// <summary>
        /// Limit the value to [min, max] (both inclusive)
        /// </summary>
        /// <param name="value">Value to limit</param>
        /// <param name="min">Minimum value (inclusive)</param>
        /// <param name="max">Maximum value (inclusive)</param>
        /// <returns>Limited value</returns>
        private static double LimitValue(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two <see cref="CompetitionRaces"/> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="CompetitionRaces"/> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="CompetitionRaces"/></returns>
        public override bool Equals(object obj)
            => obj is CompetitionRaces r && r.Score == Score && r.Races.SequenceEqual(Races);

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(CompetitionRaces other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => (Races, Score).GetHashCode();

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new CompetitionRaces(this);
    }
}
