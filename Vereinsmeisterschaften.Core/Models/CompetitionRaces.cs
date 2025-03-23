using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a combination of all single races.
    /// </summary>
    public class CompetitionRaces : ObservableObject
    {
        public ObservableCollection<Race> Races { get; set; }

        public CompetitionRaces()
        {
            Races = new ObservableCollection<Race>();
        }

        public CompetitionRaces(List<Race> races)
        {
            Races = new ObservableCollection<Race>(races);
        }

        public CompetitionRaces(ObservableCollection<Race> races)
        {
            Races = races;
        }

        private double _score;
        public double Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        private double _scoreSingleStarts;
        public double ScoreSingleStarts
        {
            get => _scoreSingleStarts;
            set => SetProperty(ref _scoreSingleStarts, value);
        }

        private double _scoreSameStyleSequence;
        public double ScoreSameStyleSequence
        {
            get => _scoreSameStyleSequence;
            set => SetProperty(ref _scoreSameStyleSequence, value);
        }

        private double _scorePersonGaps;
        public double ScorePersonGaps
        {
            get => _scorePersonGaps;
            set => SetProperty(ref _scorePersonGaps, value);
        }

        private double _scoreStyleOrder;
        public double ScoreStyleOrder
        {
            get => _scoreStyleOrder;
            set => SetProperty(ref _scoreStyleOrder, value);
        }

        public double CalculateScore()
        {
            if (Races.Count == 0)
                return 0;

            ScoreSingleStarts = EvaluateSingleStarts();
            ScoreSameStyleSequence = EvaluateSameStyleSequence();
            ScorePersonGaps = EvaluatePersonGaps();
            ScoreStyleOrder = EvaluateStyleOrder();

            double score = 0;
            score += ScoreSingleStarts * 0.15;         // 15% Weight
            score += ScoreSameStyleSequence * 0.15;    // 15% Weight
            score += ScorePersonGaps * 0.55;           // 55% Weight
            score += ScoreStyleOrder * 0.15;           // 15% Weight

            Score = ClampValue(score, 0, 100);
            return Score;
        }

        private double EvaluateSingleStarts()
        {
            if (Races.Count == 0) return 0;

            int singleStarts = Races.Count(r => r.Starts.Count == 1);
            double ratio = (double)singleStarts / Races.Count;

            // The less single starts, the better
            return 100 * (1 - ratio);
        }

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

        private double EvaluatePersonGaps()
        {
            double penalty = 0;
            double maxPenalty = Races.Count * 200;

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
                            1 => 450,   // Extremely high penalty for directly consecutive starts
                            2 => 400,   // Very high penalty for only one race pause
                            3 => 350,   // Noticeable penalty for two races pause
                            _ => Math.Max(0, 10 - distance) // Minimal penalty for 3+ races pause
                        };

                        if (distance <= 2)
                        {
                            severelyAffectedPersons++;
                        }

                        // Save the highest penalty for a person
                        if (!individualPenalties.ContainsKey(person) || individualPenalties[person] < personPenalty)
                        {
                            individualPenalties[person] = personPenalty;
                        }
                    }

                    lastRaceIndex[person] = i;
                }
            }

            // Sum up the hardest penalties per person
            penalty = individualPenalties.Values.Sum();

            double score = 100 - (penalty / maxPenalty * 100);
            score -= severelyAffectedPersons * 5;       // subtract 5% for each severely affected person

            return ClampValue(score, 0, 100);
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
            return 100 - ClampValue(100 * (penalty / maxPenalty), 0, 100);
        }

        private static double ClampValue(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
