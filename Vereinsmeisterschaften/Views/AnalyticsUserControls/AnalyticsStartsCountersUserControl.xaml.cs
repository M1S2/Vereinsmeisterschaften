using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsStartsCountersUserControl.xaml
    /// </summary>
    public partial class AnalyticsStartsCountersUserControl : AnalyticsUserControlBase
    {
        private AnalyticsModuleStartsCounters _analyticsModule => AnalyticsModule as AnalyticsModuleStartsCounters;

        public AnalyticsStartsCountersUserControl(AnalyticsModuleStartsCounters analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsStartsCountersUserControlTitle;
        public override string Icon { get; } = "\uE7C1";
        public override string Info => Properties.Tooltips.TooltipAnalyticsStartsCounters;
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
