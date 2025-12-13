using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsGenderStartsUserControl.xaml
    /// </summary>
    public partial class AnalyticsGenderStartsUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleGenderStarts _analyticsModule;

        public AnalyticsGenderStartsUserControl(AnalyticsModuleGenderStarts analyticsModule)
        {
            InitializeComponent();
            _analyticsModule = analyticsModule;
        }

        public override string Title => Properties.Resources.AnalyticsGenderStartsUserControlTitle;
        public override string Icon { get; } = "\uE7C1";
        public override double AnalyticsModuleWidth => 300;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(GenderStartsSeries));
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
