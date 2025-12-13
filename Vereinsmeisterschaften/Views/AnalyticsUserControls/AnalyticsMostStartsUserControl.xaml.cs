using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsMostStartsUserControl.xaml
    /// </summary>
    public partial class AnalyticsMostStartsUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleMostStarts _analyticsModule;

        public AnalyticsMostStartsUserControl(AnalyticsModuleMostStarts analyticsModule)
        {
            InitializeComponent();
            _analyticsModule = analyticsModule;
        }

        public override string Title => Properties.Resources.AnalyticsMostStartsUserControlTitle;
        public override string Icon { get; } = "\uE77B";

        public override void Refresh()
        {
            OnPropertyChanged(nameof(StartsPerPersonSeries));
            OnPropertyChanged(nameof(ChartHeight));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        public Dictionary<Person, int> NumberStartsPerPersonReordered => _analyticsModule?.NumberStartsPerPerson?.OrderBy(s => s.Value)?.ToDictionary() ?? new Dictionary<Person, int>();

        public ISeries[] StartsPerPersonSeries
        {
            get
            {
                if (_analyticsModule == null) return null;

                List<ISeries> seriesList = new List<ISeries>();
                var series = new RowSeries<KeyValuePair<Person, int>>
                {
                    Values = NumberStartsPerPersonReordered,
                    Mapping = (model, index) =>
                    {
                        return new Coordinate(index, model.Value);
                    },
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 14,
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsTranslate = new(-1, 0),
                    MaxBarWidth = ChartMaxBarWidth
                }
                .OnPointMeasured(point =>
                {
                    // assign a different color to each point
                    if (point.Visual is null) return;
                    int colorIndex = point.Index;
                    point.Visual.Fill = new SolidColorPaint(ColorPalletes.MaterialDesign500[colorIndex % ColorPalletes.MaterialDesign500.Length].AsSKColor());
                });
                seriesList.Add(series);

                return seriesList.ToArray();
            }
        }

        /// <summary>
        /// Maximum width for the bars in the chart
        /// </summary>
        public double ChartMaxBarWidth => 35;

        /// <summary>
        /// Calculate the chart height manually to support scrolling of the RowSeries by the surrounding scroll viewer.
        /// The minimum height for the chart is limited by the size of the scroll viewer.
        /// </summary>
        public double ChartHeight => Math.Max(PART_scrollViewerChart.ActualHeight, NumberStartsPerPersonReordered.Keys.Count * ChartMaxBarWidth);

        public Axis[] XAxes =>
        [
            new Axis
            {
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_AXIS_TEXTSIZE_DEFAULT,
                Labeler = (value) =>
                {
                    // Only return labels for real values (no doubles with fractional part)
                    if(value == Math.Floor(value))
                    {
                        return value.ToString();
                    }
                    return "";
                }
            }
        ];

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                SeparatorsPaint = null,
                Labels = NumberStartsPerPersonReordered.Keys.Select(p => $"{p.FirstName}, {p.Name}").ToArray(),
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_AXIS_TEXTSIZE_DEFAULT,
                LabelsDensity = 0
            }
        ];
    }
}
