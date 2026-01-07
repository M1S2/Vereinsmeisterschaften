using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetRacesAges.xaml
    /// </summary>
    public partial class AnalyticsWidgetRacesAges : AnalyticsWidgetBase
    {
        #region Class WidgetModelRaceAgePoint

        /// <summary>
        /// Class that displays one point in the AnalyticsWidgetRacesAgeSpan
        /// </summary>
        public class WidgetModelRaceAgePoint : WeightedPoint
        {
            private int _raceID;
            /// <summary>
            /// ID of the race for the displayed point
            /// </summary>
            public int RaceID
            {
                get => _raceID;
                set { _raceID = value; OnPropertyChanged(); }
            }

            private ushort _birthYear;
            /// <summary>
            /// Birth year for the displayed point
            /// </summary>
            public ushort BirthYear
            {
                get => _birthYear;
                set { _birthYear = value; OnPropertyChanged(); }
            }

            private bool _isDisplayedFaded = false;
            /// <summary>
            /// Flag that indicates, if the point is displayed faded
            /// </summary>
            public bool IsDisplayedFaded
            {
                get => _isDisplayedFaded;
                set { _isDisplayedFaded = value; OnPropertyChanged(); }
            }

            /// <summary>
            /// Constructor for one <see cref="WidgetModelRaceAgePoint"/>
            /// </summary>
            /// <param name="birthYear">The birth year that is used as X-Coordinate</param>
            /// <param name="numberOccurences">Counter how often the birth year occured in this race.</param>
            /// <param name="raceID">ID for the race. The Y-Coordinate is calculated by (numberRaces - raceID)</param>
            /// <param name="numberRaces">Total number of races. This is used to calculate the Y-Coordinate.</param>
            public WidgetModelRaceAgePoint(ushort birthYear, int numberOccurences, int raceID, int numberRaces) : base(birthYear, numberRaces - raceID, numberOccurences)
            {
                RaceID = raceID;
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private AnalyticsModuleRacesAges _analyticsModule => AnalyticsModule as AnalyticsModuleRacesAges;

        public AnalyticsWidgetRacesAges(AnalyticsModuleRacesAges analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
            PART_scrollViewerChart.SizeChanged += (sender, e) => OnPropertyChanged(nameof(ChartHeight));
            Refresh();
        }

        public override string Icon => "\uED55";

        public override void Refresh()
        {
            AgeSpanPoints = AgesPerRaceReversed.SelectMany(model =>
                                                       model.BirthYears.GroupBy(year => year)
                                                                       .Select(group =>
                                                                               new WidgetModelRaceAgePoint(
                                                                                   group.Key,       // year
                                                                                   group.Count(),
                                                                                   model.RaceID,
                                                                                   NumberRaces)))
                                                   .ToList();

            _yAxis = new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.RaceString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1,
                Labels = RaceIds.Select(id => $"#{id}").ToArray(),
                LabelsDensity = 0,
                LabelsPaint = ColorPaintMahAppsText,
                SeparatorsPaint = IsPointHovered ? ColorPaintMahAppsBackground : COLORPAINT_SEPARATORS,
                CrosshairPaint = ColorPaintMahAppsText,
                CrosshairSnapEnabled = true
            };

            OnPropertyChanged(nameof(AgesPerRaceSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            base.Refresh();
        }

        /// <summary>
        /// Get the <see cref="AnalyticsModuleRacesAges.AgeListsPerRace"/> in reversed order.
        /// </summary>
        public List<AnalyticsModuleRacesAges.ModelRaceAges> AgesPerRaceReversed => _analyticsModule?.AgeListsPerRace?.AsEnumerable().Reverse().ToList() ?? new List<AnalyticsModuleRacesAges.ModelRaceAges>();

        /// <summary>
        /// Number of available races
        /// </summary>
        public int NumberRaces => AgesPerRaceReversed.Count;

        /// <summary>
        /// Get a list of race IDs
        /// </summary>
        public List<int> RaceIds => AgesPerRaceReversed.Select(r => r.RaceID).ToList();

        /// <summary>
        /// List with points that are displayed in the scatter chart.
        /// Not only a getter, because the <see cref="WidgetModelRaceAgePoint.IsHighlighted"/> can't be edited otherwise.
        /// </summary>
        public List<WidgetModelRaceAgePoint> AgeSpanPoints { get; set; } = new List<WidgetModelRaceAgePoint>();

        /// <summary>
        /// Series that is displayed in the chart
        /// </summary>
        public ISeries[] AgesPerRaceSeries
        {
            get
            {
                if (_analyticsModule == null || AnalyticsAvailable == false) return null;

                List<ISeries> seriesList = new List<ISeries>();
                ISeries series = new ScatterSeries<WidgetModelRaceAgePoint>
                {
                    Values = AgeSpanPoints,
                    YToolTipLabelFormatter = point => $"{Properties.Resources.RaceString} #{point.Model.RaceID}" +
                                                      Environment.NewLine +
                                                      $"{point.Model.X}" + (point.Model.Weight > 1 ? $" ({point.Model.Weight}x)" : ""),
                    Stroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4),
                    Fill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(100)),
                    MinGeometrySize = 15,
                    GeometrySize = 22
                }
                .OnPointMeasured(point =>
                {
                    if (point.Visual is null) return;

                    // modify alpha channel for transparency
                    SolidColorPaint normalPaintFill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(100));
                    SolidColorPaint fadedPaintFill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(20));
                    SolidColorPaint normalPaintStroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4);
                    SolidColorPaint fadedPaintStroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(30), 4);

                    point.Visual.Fill = point.Model.IsDisplayedFaded ? fadedPaintFill : normalPaintFill;
                    point.Visual.Stroke = point.Model.IsDisplayedFaded ? fadedPaintStroke : normalPaintStroke;
                });
                seriesList.Add(series);
                return seriesList.ToArray();
            }
        }

        /// <summary>
        /// Maximum width for the rows in the chart
        /// </summary>
        public double ChartMaxRowWidth => 30;

        /// <summary>
        /// Calculate the chart height manually to support scrolling of the ScatterSeries by the surrounding scroll viewer.
        /// The minimum height for the chart is limited by the size of the scroll viewer.
        /// </summary>
        public double ChartHeight => Math.Max(PART_scrollViewerChart.ActualHeight, NumberRaces * ChartMaxRowWidth);

        /// <summary>
        /// X-Axes array.
        /// </summary>
        public Axis[] XAxes =>
        [
            new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.BirthYearString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1
            }
        ];

        // Private field used here for y axis because it contains properties that change dynamically during mouse hovering
        private Axis _yAxis;
        /// <summary>
        /// Y-Axes array. The Y-Axis is created in the <see cref="Refresh"/> method.
        /// </summary>
        public Axis[] YAxes => [_yAxis];

        private bool _isPointHovered;
        /// <summary>
        /// True, if any point in the chart is hovered.
        /// </summary>
        public bool IsPointHovered
        {
            get => _isPointHovered;
            set
            {
                _isPointHovered = value;
                OnPropertyChanged();

                _yAxis?.SeparatorsPaint = IsPointHovered ? ColorPaintMahAppsBackground : COLORPAINT_SEPARATORS;
                OnPropertyChanged(nameof(YAxes));
            }
        }

        /// <summary>
        /// Called, when a new point in the chart is hovered or when no point is hovered anymore.
        /// </summary>
        public ICommand ChartHoveredPointsChangedCommand => new RelayCommand<HoverCommandArgs>(args =>
        {
            bool hasPoint = args?.NewPoints != null && args.NewPoints.Any();
            IsPointHovered = hasPoint;

            if (!hasPoint)
            {
                ClearPointFading();
            }
            else
            {
                ChartPoint point = args?.NewPoints?.FirstOrDefault();
                WidgetModelRaceAgePoint model = (WidgetModelRaceAgePoint)point.Context.DataSource;

                UpdatePointFading(model.RaceID);

                args.Chart.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
            }
        });

        private void UpdatePointFading(int hoveredRaceID)
            => AgeSpanPoints.ForEach(p => p.IsDisplayedFaded = (p.RaceID != hoveredRaceID));
        private void ClearPointFading()
            => AgeSpanPoints.ForEach(p => p.IsDisplayedFaded = false);
    }
}
