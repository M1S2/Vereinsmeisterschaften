using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetAgeDistribution.xaml
    /// </summary>
    public partial class AnalyticsWidgetAgeDistribution : AnalyticsUserControlBase
    {
        private AnalyticsModuleAgeDistribution _analyticsModule => AnalyticsModule as AnalyticsModuleAgeDistribution;

        public AnalyticsWidgetAgeDistribution(AnalyticsModuleAgeDistribution analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsWidgetAgeDistributionTitle;
        public override string Icon => "\uED55";
        public override string Info => Properties.Tooltips.TooltipAnalyticsAgeDistribution;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberPersonsPerBirthYearSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        public Dictionary<UInt16, int> NumberPersonsPerBirthYear => _analyticsModule?.NumberPersonsPerBirthYear ?? new Dictionary<UInt16, int>();

        public ISeries[] NumberPersonsPerBirthYearSeries => _analyticsModule == null ? null : new ISeries[]
        {
            new LineSeries<KeyValuePair<UInt16, int>>
            {
                Values = NumberPersonsPerBirthYear,
                Mapping = (model, index) =>
                {
                    return new Coordinate(model.Key, model.Value);
                },
                YToolTipLabelFormatter = point => $"{point.Model.Key}: {point.Model.Value}"
            }
        };

        public Axis[] XAxes =>
        [
            new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.BirthYearString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT
            }
        ];

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.CountString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsDensity = 0,
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
    }
}
