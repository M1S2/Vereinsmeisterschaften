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

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetPodiumWinners.xaml
    /// </summary>
    public partial class AnalyticsWidgetPodiumWinners : AnalyticsWidgetBase
    {
        private AnalyticsModulePodiumWinners _analyticsModule => AnalyticsModule as AnalyticsModulePodiumWinners;

        public AnalyticsWidgetPodiumWinners(AnalyticsModulePodiumWinners analyticsModule) : base(analyticsModule)
        {
            _availablePodiumPlaces = Enum.GetValues(typeof(ResultPodiumsPlaces)).Cast<ResultPodiumsPlaces>().ToList();
            InitializeComponent();
            PART_scrollViewerChart.SizeChanged += (sender, e) => OnPropertyChanged(nameof(ChartHeight));
        }

        public override string Icon => "\uE81C";

        public override void Refresh()
        {
            OnPropertyChanged(nameof(PodiumPlacesPerPersonSeries));
            OnPropertyChanged(nameof(ChartHeight));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            base.Refresh();
        }

        private List<ResultPodiumsPlaces> _availablePodiumPlaces;
        /// <summary>
        /// List with all available <see cref="ResultPodiumsPlaces"/>
        /// </summary>
        public List<ResultPodiumsPlaces> AvailablePodiumPlaces => _availablePodiumPlaces;

        private ResultPodiumsPlaces _selectedPodiumPlace;
        /// <summary>
        /// Selected <see cref="ResultPodiumsPlaces"/> that is used to display the chart
        /// </summary>
        public ResultPodiumsPlaces SelectedPodiumPlace
        {
            get => _selectedPodiumPlace;
            set { _selectedPodiumPlace = value; OnPropertyChanged(); Refresh(); }
        }

        private Dictionary<WinnerName, int> numberPodiumPlacesPerPerson;

        public ISeries[] PodiumPlacesPerPersonSeries
        {
            get
            {
                if (_analyticsModule == null || AnalyticsAvailable == false) return null;

                numberPodiumPlacesPerPerson = _analyticsModule?.GetPreviousWinnersCountPerPerson(SelectedPodiumPlace).OrderBy(d => d.Value).ToDictionary(d => d.Key, d => d.Value); // order ascending to the the displayed bars in the correct order

                List<ISeries> seriesList = new List<ISeries>();
                var series = new RowSeries<KeyValuePair<WinnerName, int>>
                {
                    Values = numberPodiumPlacesPerPerson,
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
        public double ChartMaxBarWidth => 45;

        /// <summary>
        /// Calculate the chart height manually to support scrolling of the RowSeries by the surrounding scroll viewer.
        /// The minimum height for the chart is limited by the size of the scroll viewer.
        /// </summary>
        public double ChartHeight => Math.Max(PART_scrollViewerChart.ActualHeight, (numberPodiumPlacesPerPerson?.Keys.Count ?? 0) * ChartMaxBarWidth);

        public Axis[] XAxes =>
        [
            new Axis
            {
                Name = Properties.Resources.CountString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinLimit = 0,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1,
                ForceStepToMin = true,
                Position = AxisPosition.End
            }
        ];

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                SeparatorsPaint = null,
                Labels = numberPodiumPlacesPerPerson?.Keys.Select(w => $"{w.FirstName}, {w.Name}").ToArray(),
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1,
                ForceStepToMin = true
            }
        ];
    }
}
