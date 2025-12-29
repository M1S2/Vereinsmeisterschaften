using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetGenderStarts.xaml
    /// </summary>
    public partial class AnalyticsWidgetGenderStarts : AnalyticsWidgetBase
    {
        private AnalyticsModuleGenderStarts _analyticsModule => AnalyticsModule as AnalyticsModuleGenderStarts;

        public AnalyticsWidgetGenderStarts(AnalyticsModuleGenderStarts analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Icon { get; } = "\uE7C1";
        public override double NormalAnalyticsWidgetWidth => ANALYTICS_WIDGET_WIDTH_NORMAL;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(GenderStartsSeries));
            base.Refresh();
        }

        public ISeries[] GenderStartsSeries => _analyticsModule == null ? null : new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new []{ _analyticsModule.MaleStartsPercentage },
                Name = $"{Core.Properties.EnumsCore.Genders_Male} ({_analyticsModule.MaleStartsCount})",
                Fill = COLORPAINT_MALE,
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 20,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => point.Coordinate.PrimaryValue == 0 ? "" : point.Coordinate.PrimaryValue.ToString("N1") + "%",
                Pushout = 3,
                HoverPushout = 10
            },
            new PieSeries<double>
            {
                Values = new[] { _analyticsModule.FemaleStartsPercentage },
                Name = $"{Core.Properties.EnumsCore.Genders_Female} ({_analyticsModule.FemaleStartsCount})",
                Fill = COLORPAINT_FEMALE,
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 20,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => point.Coordinate.PrimaryValue == 0 ? "" : point.Coordinate.PrimaryValue.ToString("N1") + "%",
                HoverPushout = 10
            }
        };
    }
}
