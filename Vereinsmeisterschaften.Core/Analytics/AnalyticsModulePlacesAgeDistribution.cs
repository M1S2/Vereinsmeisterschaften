using System.Drawing;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age distribution over the result places
    /// </summary>
    public class AnalyticsModulePlacesAgeDistribution : IAnalyticsModule
    {
        #region Model
        public class ModelPlacesAgeDistribution
        {
            public int ResultPlace { get; set; }
            public ushort BirthYear { get; set; }
            public Person PersonObj { get; set; }
        }
        #endregion
        
        private IScoreService _scoreService;
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModulePlacesAgeDistribution"/>
        /// </summary>
        /// <param name="scoreService"><see cref="IScoreService"/> object</param>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModulePlacesAgeDistribution(IScoreService scoreService, IPersonService personService)
        {
            _scoreService = scoreService;
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// List with <see cref="ModelPlacesAgeDistribution"/> ordered by the result place.
        /// </summary>
        public List<ModelPlacesAgeDistribution> BirthYearsPerResultPlace => _scoreService.GetPersonsSortedByScore(ResultTypes.Overall, true)
                                                                                         .Select((person, index) => new ModelPlacesAgeDistribution() { ResultPlace = index + 1, BirthYear = person.BirthYear, PersonObj = person }).ToList();

        /// <summary>
        /// List with the start and end points of a linear regression line through the BirthYearsPerResultPlace points.
        /// Point.X is the result place, Point.Y is the birth year.
        /// </summary>
        public List<PointF> LinearRegressionLinePoints
        {
            get
            {
                if(BirthYearsPerResultPlace == null || BirthYearsPerResultPlace.Count == 0) { return new  List<PointF>(); }

                List<ModelPlacesAgeDistribution> birthYearsPerResultPlace = BirthYearsPerResultPlace;
                List<float> xs = birthYearsPerResultPlace.Select(p => (float)p.ResultPlace).ToList();
                List<float> ys = birthYearsPerResultPlace.Select(p => (float)p.BirthYear).ToList();

                float xAvg = xs.Average();
                float yAvg = ys.Average();

                float numerator = 0;
                float denominator = 0;

                for (int i = 0; i < xs.Count; i++)
                {
                    numerator += (xs[i] - xAvg) * (ys[i] - yAvg);
                    denominator += (xs[i] - xAvg) * (xs[i] - xAvg);
                }

                float a = numerator / denominator;
                float b = yAvg - a * xAvg;

                int minPlace = birthYearsPerResultPlace.Min(p => p.ResultPlace);
                int maxPlace = birthYearsPerResultPlace.Max(p => p.ResultPlace);

                return new List<PointF>()
                {
                    new PointF(minPlace, a * minPlace + b),
                    new PointF(maxPlace, a * maxPlace + b)
                };
            }
        }

    }
}
