using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetGenderPersons.xaml
    /// </summary>
    public partial class AnalyticsWidgetGenderPersons : AnalyticsUserControlBase
    {
        private AnalyticsModuleGenderPersons _analyticsModule => AnalyticsModule as AnalyticsModuleGenderPersons;

        public AnalyticsWidgetGenderPersons(AnalyticsModuleGenderPersons analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsWidgetGenderPersonsTitle;
        public override string Icon { get; } = "\uE77B";
        public override string Info => Properties.Tooltips.TooltipAnalyticsGenderPersons;
        public override double NormalAnalyticsWidgetWidth => ANALYTICS_WIDGET_WIDTH_NORMAL;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(GenderPersonsSeries));
            base.Refresh();
        }

        public ISeries[] GenderPersonsSeries => _analyticsModule == null ? null : new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new []{ _analyticsModule.MalePersonPercentage },
                Name = $"{Core.Properties.EnumsCore.Genders_Male} ({_analyticsModule.MalePersonCount})",
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
                Values = new[] { _analyticsModule.FemalePersonPercentage },
                Name = $"{Core.Properties.EnumsCore.Genders_Female} ({_analyticsModule.FemalePersonCount})",
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
