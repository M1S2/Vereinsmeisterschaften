using System.Windows;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetDistancesBetweenStarts.xaml
    /// </summary>
    public partial class AnalyticsWidgetDistancesBetweenStarts : AnalyticsUserControlBase
    {
        private AnalyticsModuleDistancesBetweenStarts _analyticsModule => AnalyticsModule as AnalyticsModuleDistancesBetweenStarts;
        private IWorkspaceService _workspaceService;

        public AnalyticsWidgetDistancesBetweenStarts(AnalyticsModuleDistancesBetweenStarts analyticsModule, IWorkspaceService workspaceService) : base(analyticsModule)
        {
            InitializeComponent();
            _workspaceService = workspaceService;
        }

        public override string Title => Properties.Resources.AnalyticsWidgetDistancesBetweenStartsTitle;
        public override string Icon { get; } = "\uE769";
        public override string Info => Properties.Tooltips.TooltipAnalyticsStartDistances;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(StartDistancesPerPersonSeries));
            OnPropertyChanged(nameof(ChartHeight));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            OnPropertyChanged(nameof(AnalyticsAvailable));
        }

        public Dictionary<Person, List<int>> DistancesBetweenStartsPerPersonReversed => _analyticsModule?.DistancesBetweenStartsPerPerson?.Reverse()?.ToDictionary() ?? new Dictionary<Person, List<int>>();

        public ISeries[] StartDistancesPerPersonSeries
        {
            get
            {
                if (_analyticsModule == null || AnalyticsAvailable == false) return null;

                uint shortPausesThreshold = _workspaceService?.Settings?.GetSettingValue<uint>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_SHORT_PAUSE_THRESHOLD) ?? 3;

                int maxLength = DistancesBetweenStartsPerPersonReversed.Values.Max(list => list.Count);
                List<List<int?>> normalizedLists = DistancesBetweenStartsPerPersonReversed.Values.Select(list =>
                {
                    List<int?> padded = list.Select(i => (int?)i).ToList();
                    while (padded.Count < maxLength) { padded.Add(null); }
                    return padded;
                }).ToList();

                List<ISeries> seriesList = new List<ISeries>();
                for (int i = 0; i < maxLength; i++)
                {
                    var series = new StackedRowSeries<int?>
                    {
                        StackGroup = 0,
                        Name = $"Value {i}",
                        Values = normalizedLists.Select(list => list[i]).ToList(),
                        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                        DataLabelsSize = 14,
                        DataLabelsPosition = DataLabelsPosition.Middle,
                        MaxBarWidth = ChartMaxBarWidth,
                        Stroke = ColorPaintMahAppsText
                    }
                    .OnPointMeasured(point =>
                    {
                        // assign a color to each point depending on the start distance
                        if (point.Visual is null) return;
                        SolidColorBrush displayColor;
                        if(point.Model.Value < shortPausesThreshold)
                        {
                            displayColor = Application.Current.Resources["BrushError"] as SolidColorBrush;
                        }
                        else
                        {
                            displayColor = Application.Current.Resources["BrushOk"] as SolidColorBrush;
                        }
                        point.Visual.Fill = new SolidColorPaint(SKColor.Parse(displayColor.Color.ToString()));
                    });
                    (series as StackedRowSeries<int?>).Stroke.StrokeThickness = 2;
                    seriesList.Add(series);
                }

                return seriesList.ToArray();
            }
        }

        /// <summary>
        /// Maximum width for the bars in the chart
        /// </summary>
        public double ChartMaxBarWidth => 45;

        /// <summary>
        /// Calculate the chart height manually to support scrolling of the RowSeries by the surrounding scroll viewer.
        /// The minimum height for the chart is limited by the size of the scroll viewer.
        /// </summary>
        public double ChartHeight => Math.Max(PART_scrollViewerChart.ActualHeight, DistancesBetweenStartsPerPersonReversed.Keys.Count * ChartMaxBarWidth);

        public Axis[] XAxes =>
        [
            new Axis
            {
                Name = Properties.Resources.DistanceBetweenStartsString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinLimit = 0,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
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
                SeparatorsPaint = null,
                Labels = DistancesBetweenStartsPerPersonReversed.Keys.Select(p => $"{p.FirstName}, {p.Name}").ToArray(),
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1,
                ForceStepToMin = true                
            }
        ];
    }
}
