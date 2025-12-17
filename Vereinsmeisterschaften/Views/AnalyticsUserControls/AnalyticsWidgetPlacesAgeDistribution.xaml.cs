using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using System.Xaml;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetPlacesAgeDistribution.xaml
    /// </summary>
    public partial class AnalyticsWidgetPlacesAgeDistribution : AnalyticsUserControlBase
    {
        private AnalyticsModulePlacesAgeDistribution _analyticsModule => AnalyticsModule as AnalyticsModulePlacesAgeDistribution;

        public AnalyticsWidgetPlacesAgeDistribution(AnalyticsModulePlacesAgeDistribution analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsWidgetPlacesAgeDistributionTitle;
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
                Name = Properties.Resources.ResultPlaceString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1,
                ForceStepToMin = true
            }
        ];

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.BirthYearString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsDensity = 0,
                MinStep = 1
            }
        ];
    }
}
