using System.Drawing;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
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
            base.Refresh();
        }

        public List<AnalyticsModulePlacesAgeDistribution.ModelPlacesAgeDistribution> BirthYearsPerResultPlace => _analyticsModule?.BirthYearsPerResultPlace ?? new List<AnalyticsModulePlacesAgeDistribution.ModelPlacesAgeDistribution>();

        static float regressionLineStrokeThickness = 4;
        static float[] regressionLineStrokeDashArray = new float[] { 3 * regressionLineStrokeThickness, 2 * regressionLineStrokeThickness };
        static DashEffect regressionLineStrokeEffect = new DashEffect(regressionLineStrokeDashArray);

        public ISeries[] BirthYearsPerResultPlaceSeries => _analyticsModule == null ? null : new ISeries[]
        {
            new LineSeries<AnalyticsModulePlacesAgeDistribution.ModelPlacesAgeDistribution>
            {
                Values = BirthYearsPerResultPlace,
                Mapping = (model, index) =>
                {
                    return new Coordinate(model.ResultPlace, model.BirthYear);
                },
                YToolTipLabelFormatter = point => $"{point.Model.ResultPlace}: {point.Model.BirthYear}{Environment.NewLine}{point.Model.PersonObj.FirstName}, {point.Model.PersonObj.Name}",
                Stroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4),
                GeometryStroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4),
                Fill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(0x33))     // modify alpha channel for transparency
            },
            // Linear Regression Line
            new LineSeries<PointF>
            {
                Values = _analyticsModule.LinearRegressionLinePoints,
                Mapping = (model, index) =>
                {
                    return new Coordinate(model.X, model.Y);
                },
                YToolTipLabelFormatter = point => point.Model.Y.ToString("F2"),
                Stroke = new SolidColorPaint(ColorPaintMahAppsText.Color)
                {
                    StrokeCap = SkiaSharp.SKStrokeCap.Round,
                    StrokeThickness = regressionLineStrokeThickness,
                    PathEffect = regressionLineStrokeEffect
                },
                GeometrySize = 0,
                Fill = null
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
                MinLimit = 0.5
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
