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
        public override double AnalyticsModuleWidth => 400;
        public override double AnalyticsModuleHeight => 250;

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
                CustomSeparators = Enumerable.Range(0, NumberPersonsPerBirthYear.Values.Count == 0 ? 0 : NumberPersonsPerBirthYear.Values.Max() + 1).Select(i => (double)i),
                LabelsPaint = ColorPaintMahAppsText,
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
