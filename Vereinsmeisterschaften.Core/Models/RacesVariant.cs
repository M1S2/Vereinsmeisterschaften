using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a combination variant of all single races.
    /// </summary>
    public class RacesVariant : ObservableObject, IEquatable<RacesVariant>, ICloneable
    {
        /// <summary>
        /// List with races
        /// </summary>
        public ObservableCollection<Race> Races { get; set; }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private int _variantID;
        /// <summary>
        /// Number for this <see cref="RacesVariant"/>
        /// </summary>
        [FileServiceIgnore]
        public int VariantID
        {
            get => _variantID;
            set => SetProperty(ref _variantID, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private List<PersonStart> _notAssignedStarts;
        /// <summary>
        /// List with not assigned <see cref="PersonStart"> objects (not part of any <see cref="Race"/> in <see cref="Races"/>)
        /// </summary>
        public List<PersonStart> NotAssignedStarts
        {
            get => _notAssignedStarts;
            set => SetProperty(ref _notAssignedStarts, value);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Update the list of not assigned <see cref="PersonStart"/> objects
        /// </summary>
        /// <param name="allStarts">Complete list with all <see cref="PersonStart"/> objects</param>
        public void UpdateNotAssignedStarts(List<PersonStart> allStarts)
        {
            List<PersonStart> raceStarts = GetAllStarts();
            if (allStarts == null)
            {
                NotAssignedStarts = new List<PersonStart>();
            }
            else if (raceStarts == null)
            {
                NotAssignedStarts = allStarts;
            }
            else
            {
                NotAssignedStarts = allStarts?.Except(raceStarts)?.ToList();
            }
            OnPropertyChanged(nameof(IsValid_AllRacesValid));
            OnPropertyChanged(nameof(IsValid_AllStartsAssigned));
            OnPropertyChanged(nameof(IsValid));
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// True when all <see cref="Races"/> are valid
        /// </summary>
        [FileServiceIgnore]
        public bool IsValid_AllRacesValid => Races?.All(r => r.IsValid) ?? true;

        /// <summary>
        /// True when there are no empty unassigned races
        /// </summary>
        [FileServiceIgnore]
        public bool IsValid_AllStartsAssigned => NotAssignedStarts?.Count == 0;

        /// <summary>
        /// This <see cref="RacesVariant"/> is consideres valid when:
        /// - All <see cref="Races"/> are valid
        /// - There are no empty unassigned races
        /// </summary>
        public bool IsValid => IsValid_AllRacesValid && IsValid_AllStartsAssigned;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private bool _isPersistent;
        /// <summary>
        /// If true, this <see cref="RacesVariant"/> should be persisted (to a file).
        /// </summary>
        [FileServiceIgnore]
        public bool IsPersistent
        {
            get => _isPersistent;
            set { SetProperty(ref _isPersistent, value); OnPropertyChanged(nameof(KeepWhileRacesCalculation)); }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        private bool _keepWhileRacesCalculation;
        /// <summary>
        /// Keep this <see cref="RacesVariant"/> while calculating new variants
        /// </summary>
        [FileServiceIgnore]
        public bool KeepWhileRacesCalculation
        {
            get => _keepWhileRacesCalculation || IsPersistent;
            set => SetProperty(ref _keepWhileRacesCalculation, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor for a new <see cref="RacesVariant"/> (create an empty <see cref="Race"/> collection).
        /// </summary>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> used to get the weights used to calculate the scores.</param>
        public RacesVariant(IWorkspaceService workspaceService = null)
        {
            Races = new ObservableCollection<Race>();
            Races.CollectionChanged += Races_CollectionChanged;
            _workspaceService = workspaceService;
            if (_workspaceService != null) { _workspaceService.PropertyChanged += workspaceServicePropertyChangedEvent; }
        }

        /// <summary>
        /// Constructor for a new <see cref="RacesVariant"/> (copy an <see cref="Race"/> collection).
        /// </summary>
        /// <param name="races">List of <see cref="Race"/> to copy</param>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> used to get the weights used to calculate the scores.</param>
        public RacesVariant(List<Race> races, IWorkspaceService workspaceService = null)
        {
            Races = new ObservableCollection<Race>(races);
            Races.CollectionChanged += Races_CollectionChanged;
            _workspaceService = workspaceService;
            if (_workspaceService != null) { _workspaceService.PropertyChanged += workspaceServicePropertyChangedEvent; }

            Races_CollectionChanged(Races, null);
        }

        /// <summary>
        /// Constructor for a new <see cref="RacesVariant"/> (copy an <see cref="Race"/> collection).
        /// </summary>
        /// <param name="races">Observable collection of <see cref="Race"/> to copy</param>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> used to get the weights used to calculate the scores.</param>
        public RacesVariant(ObservableCollection<Race> races, IWorkspaceService workspaceService = null)
        {
            Races = races;
            Races.CollectionChanged += Races_CollectionChanged;
            _workspaceService = workspaceService;
            if (_workspaceService != null) { _workspaceService.PropertyChanged += workspaceServicePropertyChangedEvent; }

            Races_CollectionChanged(Races, null);
        }

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <param name="other"><see cref="RacesVariant"/> object to clone</param>
        /// <param name="deepClone">If true, the <see cref="Races"/> are also cloned. Otherwise the same <see cref="Races"/> references are used.</param>
        /// <param name="deepCloneRaces">If true, the elements of the <see cref="Races"/> are also cloned. Otherwise the same race references are used.</param>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> used to get the weights used to calculate the scores. If this is <see langword="null"/> the value from the cloned object is taken</param>
        public RacesVariant(RacesVariant other, bool deepClone = true, bool deepCloneRaces = true, IWorkspaceService workspaceService = null) : this()
        {
            if(other == null) { return; }
            if (deepClone)
            {
                // Create a deep copy of the list
                Races = new ObservableCollection<Race>(other.Races.Select(item => new Race(item, deepCloneRaces)));
            }
            else
            {
                // Create a new list but keep the references to the <see cref="Race"/> objects
                Races = new ObservableCollection<Race>(other.Races);
            }

            Races.CollectionChanged += Races_CollectionChanged;
            _workspaceService = workspaceService ?? other._workspaceService;
            if (_workspaceService != null) { _workspaceService.PropertyChanged += workspaceServicePropertyChangedEvent; }

            Races_CollectionChanged(Races, null);
        }

        /// <summary>
        /// Destructor of the <see cref="RacesVariant"/> class. Unsubscribe from events.
        /// </summary>
        ~RacesVariant()
        {
            if (_workspaceService != null) { _workspaceService.PropertyChanged -= workspaceServicePropertyChangedEvent; }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        private IWorkspaceService _workspaceService;

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        private void workspaceServicePropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IWorkspaceService.Settings))
            {
                CalculateScore();
            }
        }

        private void updateRaceStartsCollectionChangedEvent()
        {
            OnPropertyChanged(nameof(Races));
            foreach (Race race in Races)
            {
                race.Starts.CollectionChanged -= raceStartsCollectionChangedEventHandler;
                race.Starts.CollectionChanged += raceStartsCollectionChangedEventHandler;
            }
        }

        private void raceStartsCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateScore();
            OnPropertyChanged(nameof(Races));
            OnPropertyChanged(nameof(IsValid_AllRacesValid));
            OnPropertyChanged(nameof(IsValid_AllStartsAssigned));
            OnPropertyChanged(nameof(IsValid));
        }

        private void Races_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            updateRaceIDs();
            CalculateScore();
            updateRaceStartsCollectionChangedEvent();
            OnPropertyChanged(nameof(IsValid_AllRacesValid));
            OnPropertyChanged(nameof(IsValid_AllStartsAssigned));
            OnPropertyChanged(nameof(IsValid));
        }

        private void updateRaceIDs()
        {
            int id = 1;
            foreach(Race race in Races)
            {
                race.RaceID = id;
                id++;
            }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Get a list with all <see cref="PersonStart"> objects of this <see cref="RacesVariant"/>
        /// </summary>
        /// <returns>List with all <see cref="PersonStart"/> objects of this <see cref="RacesVariant"/></returns>
        public List<PersonStart> GetAllStarts()
        {
            return Races.SelectMany(r => r.Starts).ToList();
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Overall score. This combines all single score using weights.
        /// </summary>
        public double Score
        {
            get
            {
                double weightSingleStarts = _workspaceService?.Settings?.GetSettingValue<double>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_SINGLE_STARTS) ?? 5;
                double weightSameStyleSequence = _workspaceService?.Settings?.GetSettingValue<double>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_SAME_STYLE_SEQUENCE) ?? 5;
                double weightPersonStartPauses = _workspaceService?.Settings?.GetSettingValue<double>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_PERSON_START_PAUSES) ?? 65;
                double weightStyleOrder = _workspaceService?.Settings?.GetSettingValue<double>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_STYLE_ORDER) ?? 10;
                double weightStartGenders = _workspaceService?.Settings?.GetSettingValue<double>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_START_GENDERS) ?? 5;
                double sumWeights = weightSingleStarts + weightSameStyleSequence + weightPersonStartPauses + weightStyleOrder + weightStartGenders;

                double score = 0;
                score += ScoreSingleStarts * (weightSingleStarts / sumWeights);
                score += ScoreSameStyleSequence * (weightSameStyleSequence / sumWeights);
                score += ScorePersonStartPauses * (weightPersonStartPauses / sumWeights);
                score += ScoreStyleOrder * (weightStyleOrder / sumWeights);
                score += ScoreStartGenders * (weightStartGenders / sumWeights);

                return LimitValue(score, 0, 100);
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

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

        private double _scoreStartGenders;
        /// <summary>
        /// Score regarding the homogenity of genders in the starts
        /// </summary>
        public double ScoreStartGenders
        {
            get => _scoreStartGenders;
            set => SetProperty(ref _scoreStartGenders, value);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

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
                ScoreStartGenders = 0;
            }
            else
            {
                ScoreSingleStarts = EvaluateSingleStarts();
                ScoreSameStyleSequence = EvaluateSameStyleSequence();
                ScorePersonStartPauses = EvaluatePersonStartPauses();
                ScoreStyleOrder = EvaluateStyleOrder();
                ScoreStartGenders = EvaluateStartGenders();
            }
            OnPropertyChanged(nameof(Score));
            return Score;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

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

        // ----------------------------------------------------------------------------------------------------------------------------------------------

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

        // ----------------------------------------------------------------------------------------------------------------------------------------------

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

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Score for style order:
        /// There is a preferred order for the styles (can be defined via the workspace settings). The more the races follow this order, the better.
        /// </summary>
        /// <returns>Score for style order</returns>
        private double EvaluateStyleOrder()
        {
            if (Races.Count == 0) return 100;

            // lower numbers should start earlier
            Dictionary<SwimmingStyles, int> StylePriorities = new()
            {
                { SwimmingStyles.Breaststroke, _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_BREASTSTROKE) ?? 1 },
                { SwimmingStyles.Freestyle, _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_FREESTYLE) ?? 2 },
                { SwimmingStyles.Backstroke, _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_BACKSTROKE) ?? 3 },
                { SwimmingStyles.Butterfly, _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_BUTTERFLY) ?? 4 },
                { SwimmingStyles.Medley, _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_MEDLEY) ?? 5 },
                { SwimmingStyles.WaterFlea, _workspaceService?.Settings?.GetSettingValue<int>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_WATERFLEA) ?? 6 }
            };

        double penalty = 0;
            foreach (var (race, index) in Races.Select((r, i) => (r, i)))
            {
                if (StylePriorities.TryGetValue(race.Style, out int priority))
                {
                    int expectedMinIndex = Races.Count * priority / StylePriorities.Count;
                    if (index < expectedMinIndex)
                    {
                        penalty += (expectedMinIndex - index);
                    }
                }
            }

            double maxPenalty = Races.Count * StylePriorities.Count;
            return 100 - LimitValue(100 * (penalty / maxPenalty), 0, 100);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Score for start genders:
        /// Homogenous genders in starts are better than heterogeneous ones.
        /// </summary>
        /// <returns>Score for start genders</returns>
        private double EvaluateStartGenders()
        {
            if (Races.Count == 0) return 100.0;

            int totalRaces = Races.Count;
            int homogeneousCount = 0;

            foreach (Race race in Races)
            {
                List<Genders> genders = race.Starts.Select(s => s.PersonObj.Gender).Distinct().ToList();
                if (genders.Count == 1)
                {
                    homogeneousCount++;
                }
            }

            double score = 100.0 * homogeneousCount / totalRaces;
            return LimitValue(score, 0, 100);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

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
        /// Compare if two <see cref="RacesVariant"/> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="RacesVariant"/> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="RacesVariant"/></returns>
        public override bool Equals(object obj)
            => obj is RacesVariant r && r.VariantID == VariantID;

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(RacesVariant other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => VariantID.GetHashCode();

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new RacesVariant(this, true, true);
    }
}
