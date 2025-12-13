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

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsStartDistancesUserControl.xaml
    /// </summary>
    public partial class AnalyticsStartDistancesUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleStartDistances _analyticsModule => AnalyticsModule as AnalyticsModuleStartDistances;

        public AnalyticsStartDistancesUserControl(AnalyticsModuleStartDistances analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsStartsPerDistanceUserControlTitle;
        public override string Icon { get; } = "\uECC6";
        public override string Info => Properties.Tooltips.TooltipAnalyticsStartsPerDistance;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(StartsPerDistanceSeries));
        }

        public ISeries[] StartsPerDistanceSeries
        {
            get
            {
                if (_analyticsModule == null) return null;

                Dictionary<ushort, double> percentageStartsPerDistance = _analyticsModule.PercentageStartsPerDistance;
                Dictionary<ushort, int> numberStartsPerDistance = _analyticsModule.NumberStartsPerDistance;

                List<ISeries> seriesList = new List<ISeries>();
                foreach (ushort distance in percentageStartsPerDistance.Keys)
                {
                    var series = new PieSeries<double>
                    {
                        Values = new[] { percentageStartsPerDistance[distance] },
                        Name = $"{distance}m ({numberStartsPerDistance[distance]})",
                        DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                        DataLabelsSize = 14,
                        DataLabelsPosition = PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => point.Coordinate.PrimaryValue == 0 ? "" : point.Coordinate.PrimaryValue.ToString("N1") + "%",
                        Pushout = 3,
                        HoverPushout = 10,
                        Fill = new SolidColorPaint(ColorPalletes.MaterialDesign500[(int)percentageStartsPerDistance.Keys.ToList().IndexOf(distance) % ColorPalletes.MaterialDesign500.Length].AsSKColor())
                    };
                    seriesList.Add(series);
                }

                return seriesList.ToArray();
            }
        }
    }
}
