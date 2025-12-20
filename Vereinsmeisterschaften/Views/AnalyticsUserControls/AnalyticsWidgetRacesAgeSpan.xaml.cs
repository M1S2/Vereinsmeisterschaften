using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetRacesAgeSpan.xaml
    /// </summary>
    public partial class AnalyticsWidgetRacesAgeSpan : AnalyticsUserControlBase
    {
        #region Class WidgetModelRaceAgePoint

        public class WidgetModelRaceAgePoint : ObservablePoint
        {
            private int _raceID;
            public int RaceID
            {
                get => _raceID;
                set { _raceID = value; OnPropertyChanged(); }
            }

            private ushort _birthYear;
            public ushort BirthYear
            {
                get => _birthYear;
                set { _birthYear = value; OnPropertyChanged(); }
            }
            
            private bool _isHighlighted = true;
            public bool IsHighlighted
            {
                get => _isHighlighted;
                set { _isHighlighted = value; OnPropertyChanged(); }
            }

            public WidgetModelRaceAgePoint(double x, int raceID, int numberRaces) : base(x, numberRaces - raceID)
            {
                RaceID = raceID;
            }
        }

        #endregion

        private AnalyticsModuleRacesAgeSpan _analyticsModule => AnalyticsModule as AnalyticsModuleRacesAgeSpan;

        public AnalyticsWidgetRacesAgeSpan(AnalyticsModuleRacesAgeSpan analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsWidgetRacesAgeSpanTitle;
        public override string Icon => "\uED55";
        public override string Info => Properties.Tooltips.TooltipAnalyticsRacesAgeSpan;

        public override void Refresh()
        {
            AgeSpanPoints = AgeSpansPerRaceReversed.SelectMany(model =>
                                                       model.BirthYears.Select(year =>
                                                           new WidgetModelRaceAgePoint(
                                                               year,
                                                               model.RaceID,
                                                               NumberRaces)))
                                                   .ToList();

            OnPropertyChanged(nameof(AgeSpansPerRaceSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            base.Refresh();
        }

        public List<AnalyticsModuleRacesAgeSpan.ModelRaceAgeSpan> AgeSpansPerRaceReversed => _analyticsModule?.AgeListsPerRace?.AsEnumerable().Reverse().ToList() ?? new List<AnalyticsModuleRacesAgeSpan.ModelRaceAgeSpan>();
        public int NumberRaces => AgeSpansPerRaceReversed.Count;
        public List<int> RaceIds => AgeSpansPerRaceReversed.Select(r => r.RaceID).ToList();

        /// <summary>
        /// List with points that are displayed in the scatter chart.
        /// Not only a getter, because the <see cref="WidgetModelRaceAgePoint.IsHighlighted"/> can't be edited otherwise.
        /// </summary>
        public List<WidgetModelRaceAgePoint> AgeSpanPoints { get; set; } = new List<WidgetModelRaceAgePoint>();

        public ISeries[] AgeSpansPerRaceSeries
        {
            get
            {
                if (_analyticsModule == null || AnalyticsAvailable == false) return null;

                List<ISeries> seriesList = new List<ISeries>();
                ISeries series = new ScatterSeries<WidgetModelRaceAgePoint>
                {
                    Values = AgeSpanPoints,
                    YToolTipLabelFormatter = point => $"{Properties.Resources.RaceIDString} {point.Model.RaceID}{Environment.NewLine}{point.Model.X}",
                    Stroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4),
                    Fill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(100)),
                    GeometrySize = 20
                }
                .OnPointMeasured(point =>
                {
                    if (point.Visual is null) return;

                    // modify alpha channel for transparency
                    SolidColorPaint normalPaintFill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(50));
                    SolidColorPaint fadedPaintFill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(10));
                    SolidColorPaint normalPaintStroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4);
                    SolidColorPaint fadedPaintStroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(30), 4);

                    point.Visual.Fill = point.Model.IsHighlighted ? normalPaintFill : fadedPaintFill;
                    point.Visual.Stroke = point.Model.IsHighlighted ? normalPaintStroke : fadedPaintStroke;
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

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.RaceIDString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                Labels = RaceIds.Select(id => id.ToString()).ToArray(),
                LabelsDensity = 0,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1
            }
        ];

        private int? _hoveredRaceID;

        public ICommand ChartHoveredPointsChangedCommand => new RelayCommand<HoverCommandArgs>(args =>
        {
            ChartPoint point = args?.NewPoints?.FirstOrDefault();

            if (point == null)
            {
                ClearHover();
                return;
            }

            WidgetModelRaceAgePoint model = (WidgetModelRaceAgePoint)point.Context.DataSource;
            int raceID = model.RaceID;

            if (_hoveredRaceID == raceID)
                return;

            _hoveredRaceID = raceID;

            UpdatePointHighlighting(raceID);

            args.Chart.CoreChart.Update(new ChartUpdateParams { IsAutomaticUpdate = false, Throttling = false });
        });

        private void UpdatePointHighlighting(int raceID)
        {
            AgeSpanPoints.ForEach(p => p.IsHighlighted = (p.RaceID == raceID));
        }

        private void ClearHover()
        {
            _hoveredRaceID = null;
            AgeSpanPoints.ForEach(p => p.IsHighlighted = true);
        }
    }
}
