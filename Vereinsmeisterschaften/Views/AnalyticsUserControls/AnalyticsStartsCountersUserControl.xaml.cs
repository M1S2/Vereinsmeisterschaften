using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsStartsCountersUserControl.xaml
    /// </summary>
    public partial class AnalyticsStartsCountersUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleStartsCounters _analyticsModule;

        public AnalyticsStartsCountersUserControl(AnalyticsModuleStartsCounters analyticsModule)
        {
            InitializeComponent();
            _analyticsModule = analyticsModule;
        }

        public override string Title => Properties.Resources.AnalyticsStartsCountersUserControlTitle;
        public override string Icon { get; } = "\uE7C1";
        public override double AnalyticsModuleWidth => this.Width;      // Size is determined by the control content
        public override double AnalyticsModuleHeight => this.Height;    // Size is determined by the control content

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberOfStarts));
            OnPropertyChanged(nameof(NumberOfInactiveStarts));
        }

        public int NumberOfStarts => _analyticsModule?.NumberOfStarts ?? 0;
        public int NumberOfInactiveStarts => _analyticsModule?.NumberOfInactiveStarts ?? 0;
    }
}
