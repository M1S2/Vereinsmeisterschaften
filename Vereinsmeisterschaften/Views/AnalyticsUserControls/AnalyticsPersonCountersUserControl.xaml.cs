using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsPersonCountersUserControl.xaml
    /// </summary>
    public partial class AnalyticsPersonCountersUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModulePersonCounters _analyticsModule => AnalyticsModule as AnalyticsModulePersonCounters;

        public AnalyticsPersonCountersUserControl(AnalyticsModulePersonCounters analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsPersonCountersUserControlTitle;
        public override string Icon { get; } = "\uE77B";
        public override string Info => Properties.Tooltips.TooltipAnalyticsPersonCounters;
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
