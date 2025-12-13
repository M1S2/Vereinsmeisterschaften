using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsAgeDistributionUserControl.xaml
    /// </summary>
    public partial class AnalyticsAgeDistributionUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleAgeDistribution _analyticsModule;

        public AnalyticsAgeDistributionUserControl(AnalyticsModuleAgeDistribution analyticsModule)
        {
            InitializeComponent();
            _analyticsModule = analyticsModule;
        }

        public override string Title => Properties.Resources.AnalyticsAgeDistributionUserControlTitle;
        public override string Icon => "\uED55";

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberPersonsPerBirthYearSeries));
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

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_AXIS_TEXTSIZE_DEFAULT,
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
