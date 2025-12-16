using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using System.Xaml;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsPlacesAgeDistributionUserControl.xaml
    /// </summary>
    public partial class AnalyticsPlacesAgeDistributionUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModulePlacesAgeDistribution _analyticsModule => AnalyticsModule as AnalyticsModulePlacesAgeDistribution;

        public AnalyticsPlacesAgeDistributionUserControl(AnalyticsModulePlacesAgeDistribution analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsPlacesAgeDistributionUserControlTitle;
        public override string Icon => "\uE9F9";
        public override string Info => Properties.Tooltips.TooltipAnalyticsPlacesAgeDistribution;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(BirthYearsPerResultPlaceSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        public Dictionary<int, ushort> BirthYearsPerResultPlace => _analyticsModule?.BirthYearsPerResultPlace ?? new Dictionary<int, ushort>();

        public ISeries[] BirthYearsPerResultPlaceSeries => _analyticsModule == null ? null : new ISeries[]
        {
            new LineSeries<KeyValuePair<int, ushort>>
            {
                Values = BirthYearsPerResultPlace,
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
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_AXIS_TEXTSIZE_DEFAULT
            }
        ];

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_AXIS_TEXTSIZE_DEFAULT,
                LabelsDensity = 0
            }
        ];
    }
}
