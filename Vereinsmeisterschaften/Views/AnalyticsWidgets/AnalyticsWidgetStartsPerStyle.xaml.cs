using System.Windows;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetStartsPerStyle.xaml
    /// </summary>
    public partial class AnalyticsWidgetStartsPerStyle : AnalyticsWidgetBase
    {
        private AnalyticsModuleStartsPerStyle _analyticsModule => AnalyticsModule as AnalyticsModuleStartsPerStyle;

        public AnalyticsWidgetStartsPerStyle(AnalyticsModuleStartsPerStyle analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Static reference to a resource dictionary containing swimming style resources.
        /// </summary>
        private static ResourceDictionary swimmingStyleResourceDict = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Vereinsmeisterschaften;component/Styles/SwimmingStyleResources.xaml")
        };

        public override Geometry IconGeometry => (Geometry)swimmingStyleResourceDict["Geometry_Breaststroke"];

        public override void Refresh()
        {
            OnPropertyChanged(nameof(StartsPerStyleSeries));
            base.Refresh();
        }

        public ISeries[] StartsPerStyleSeries
        {
            get
            {
                if (_analyticsModule == null) return null;

                Dictionary<SwimmingStyles, double> percentageStartsPerStyle = _analyticsModule.PercentageStartsPerStyle;
                Dictionary<SwimmingStyles, int> numberStartsPerStyle = _analyticsModule.NumberStartsPerStyle;

                List<ISeries> seriesList = new List<ISeries>();
                foreach (SwimmingStyles style in percentageStartsPerStyle.Keys)
                {
                    var series = new PieSeries<double>
                    {
                        Values = new[] { percentageStartsPerStyle[style] },
                        Name = $"{EnumCoreLocalizedStringHelper.Convert(style)} ({numberStartsPerStyle[style]})",
                        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                        DataLabelsSize = 14,
                        DataLabelsPosition = PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => point.Coordinate.PrimaryValue == 0 ? "" : point.Coordinate.PrimaryValue.ToString("N1") + "%",
                        Pushout = 3,
                        HoverPushout = 10,
                        Fill = new SolidColorPaint(ColorPalletes.MaterialDesign500[(int)style % ColorPalletes.MaterialDesign500.Length].AsSKColor())
                    };
                    seriesList.Add(series);
                }

                return seriesList.ToArray();
            }
        }
    }
}
