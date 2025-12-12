using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Themes;
using SkiaSharp;
using System.Windows;
using System.Windows.Media;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsStartsPerStyleUserControl.xaml
    /// </summary>
    public partial class AnalyticsStartsPerStyleUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleStartsPerStyle _analyticsModule;

        public AnalyticsStartsPerStyleUserControl(AnalyticsModuleStartsPerStyle analyticsModule)
        {
            InitializeComponent();
            _analyticsModule = analyticsModule;
        }

        /// <summary>
        /// Static reference to a resource dictionary containing swimming style resources.
        /// </summary>
        private static ResourceDictionary swimmingStyleResourceDict = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Vereinsmeisterschaften;component/Styles/SwimmingStyleResources.xaml")
        };

        public override string Title => Properties.Resources.AnalyticsStartsPerStyleUserControlTitle;
        public override Geometry IconGeometry => (Geometry)swimmingStyleResourceDict["Geometry_Breaststroke"];
        public override double AnalyticsModuleWidth => 400;
        public override double AnalyticsModuleHeight => 250;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(StartsPerStyleSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        public Dictionary<SwimmingStyles, int> NumberStartsPerStyleReordered => _analyticsModule?.NumberStartsPerStyle?.OrderBy(s => s.Value)?.ToDictionary() ?? new Dictionary<SwimmingStyles, int>();

        public ISeries[] StartsPerStyleSeries
        {
            get
            {
                if (_analyticsModule == null) return null;

                List<ISeries> seriesList = new List<ISeries>();
                var series = new RowSeries<KeyValuePair<SwimmingStyles, int>>
                {
                    Values = NumberStartsPerStyleReordered,
                    Mapping = (model, index) => 
                    { 
                        return new Coordinate(index, model.Value);
                    },
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 14,
                    DataLabelsPosition = DataLabelsPosition.End,
                    DataLabelsTranslate = new(-1, 0)
                }
                .OnPointMeasured(point =>
                {
                    // assign a different color to each point
                    if (point.Visual is null) return;
                    int colorIndex = (int)NumberStartsPerStyleReordered.Keys.ToList()[(int)point.Index];
                    point.Visual.Fill = new SolidColorPaint(ColorPalletes.MaterialDesign500[colorIndex % ColorPalletes.MaterialDesign500.Length].AsSKColor());
                });
                seriesList.Add(series);

                return seriesList.ToArray();
            }
        }

        public Axis[] XAxes =>
        [
            new Axis
            {
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
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

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                SeparatorsPaint = null,
                Labels = NumberStartsPerStyleReordered.Keys.Select(s => EnumCoreLocalizedStringHelper.Convert(s)).ToArray(),
                LabelsPaint = ColorPaintMahAppsText,
                LabelsDensity = 0
            }
        ];
    }
}
