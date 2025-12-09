using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsPersonCountersUserControl.xaml
    /// </summary>
    public partial class AnalyticsPersonCountersUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModulePersonCounters _analyticsModule;

        public AnalyticsPersonCountersUserControl(AnalyticsModulePersonCounters analyticsModule)
        {
            InitializeComponent();
            _analyticsModule = analyticsModule;
        }

        public override string Title => Properties.Resources.AnalyticsPersonCountersUserControlTitle;
        public override string Icon { get; } = "\uE77B";
        public override double AnalyticsModuleWidth => this.Width;      // Size is determined by the control content
        public override double AnalyticsModuleHeight => this.Height;    // Size is determined by the control content

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberOfPeople));
            OnPropertyChanged(nameof(NumberOfInactivePeople));
        }

        public int NumberOfPeople => _analyticsModule?.NumberOfPeople ?? 0;
        public int NumberOfInactivePeople => _analyticsModule?.NumberOfInactivePeople ?? 0;
    }
}
