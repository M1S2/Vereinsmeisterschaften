using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Models
{
    public class WorkspaceSettings : ObservableObject, IEquatable<WorkspaceSettings>, ICloneable
    {
        private ushort _competitionYear;
        /// <summary>
        /// Year in which the competition takes place
        /// </summary>
        public ushort CompetitionYear
        {
            get => _competitionYear;
            set => SetProperty(ref _competitionYear, value);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        public const ushort DEFAULT_NUMBER_OF_SWIM_LANES = 3;

        private ushort _numberOfSwimLanes;
        /// <summary>
        /// Number of available swim lanes. This is used during calculation of the new <see cref="RacesVariant"/> (determines the maximum number of parallel starts)
        /// </summary>
        public ushort NumberOfSwimLanes
        {
            get => _numberOfSwimLanes;
            set => SetProperty(ref _numberOfSwimLanes, value);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        public const int DEFAULT_NUMBER_RACESVARIANTS_AFTER_CALCULATION = 100;

        private ushort _numberRacesVariantsAfterCalculation;
        /// <summary>
        /// Number of variants in <see cref="IRaceService.AllRacesVariants"/> after calling <see cref="IRaceService.CalculateRacesVariants(CancellationToken, ProgressDelegate)"/>
        /// The number of variants to keep (marked with <see cref="IRacesVariant.KeepWhileRacesCalculation"/>) is subtracted before calculating the remaining elements.
        /// </summary>
        public ushort NumberRacesVariantsAfterCalculation
        {
            get => _numberRacesVariantsAfterCalculation;
            set => SetProperty(ref _numberRacesVariantsAfterCalculation, value);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        public const int DEFAULT_MAX_RACESVARIANTS_CALCULATION_LOOPS = 1000000;

        private int _maxRacesVariantCalculationLoops;
        /// <summary>
        /// Maximum number of iterations the race variant calculation loop will run in the worst case.
        /// If <see cref="NumberRacesVariantsAfterCalculation"/> are reached earlier, the loop will break.
        /// </summary>
        public int MaxRacesVariantCalculationLoops
        {
            get => _maxRacesVariantCalculationLoops;
            set => SetProperty(ref _maxRacesVariantCalculationLoops, value);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        public const double DEFAULT_MIN_RACESVARIANTS_SCORE = 90.0;

        private double _minRacesVariantsScore;
        /// <summary>
        /// Only <see cref="RacesVariant"/> with a score higher or equal this value (in percent) are kept during <see cref="IRaceService.CalculateRacesVariants(CancellationToken, ProgressDelegate)"/>.
        /// </summary>
        public double MinRacesVariantsScore
        {
            get => _minRacesVariantsScore;
            set => SetProperty(ref _minRacesVariantsScore, value);
        }
        
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public WorkspaceSettings()
        {
            CompetitionYear = 0;
            NumberOfSwimLanes = DEFAULT_NUMBER_OF_SWIM_LANES;
            NumberRacesVariantsAfterCalculation = DEFAULT_NUMBER_RACESVARIANTS_AFTER_CALCULATION;
            MaxRacesVariantCalculationLoops = DEFAULT_MAX_RACESVARIANTS_CALCULATION_LOOPS;
            MinRacesVariantsScore = DEFAULT_MIN_RACESVARIANTS_SCORE;
        }

        public WorkspaceSettings(WorkspaceSettings other)
        {
            if (other == null) { return; }
            CompetitionYear = other.CompetitionYear;
            NumberOfSwimLanes = other.NumberOfSwimLanes;
            NumberRacesVariantsAfterCalculation = other.NumberRacesVariantsAfterCalculation;
            MaxRacesVariantCalculationLoops = other.MaxRacesVariantCalculationLoops;
            MinRacesVariantsScore = other.MinRacesVariantsScore;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static void SetPropertyFromString(WorkspaceSettings dataObj, string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(CompetitionYear): dataObj.CompetitionYear = ushort.Parse(value); break;
                case nameof(NumberOfSwimLanes): dataObj.NumberOfSwimLanes = ushort.Parse(value); break;
                case nameof(NumberRacesVariantsAfterCalculation): dataObj.NumberRacesVariantsAfterCalculation = ushort.Parse(value); break;
                case nameof(MaxRacesVariantCalculationLoops): dataObj.MaxRacesVariantCalculationLoops = int.Parse(value); break;
                case nameof(MinRacesVariantsScore): dataObj.MinRacesVariantsScore = double.Parse(value); break;
                default: break;
            }
        }

        public override bool Equals(object obj)
            => obj is WorkspaceSettings s && (s.CompetitionYear, s.NumberOfSwimLanes, s.NumberRacesVariantsAfterCalculation, s.MaxRacesVariantCalculationLoops, s.MinRacesVariantsScore)
                                              .Equals((CompetitionYear, NumberOfSwimLanes, NumberRacesVariantsAfterCalculation, MaxRacesVariantCalculationLoops, MinRacesVariantsScore));

        public bool Equals(WorkspaceSettings other)
            => Equals((object)other);

        public override int GetHashCode()
            => (CompetitionYear, NumberOfSwimLanes, NumberRacesVariantsAfterCalculation, MaxRacesVariantCalculationLoops, MinRacesVariantsScore).GetHashCode();

        public object Clone()
            => new WorkspaceSettings(this);
    }
}
