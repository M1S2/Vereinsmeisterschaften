using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetStartsCounters.xaml
    /// </summary>
    public partial class AnalyticsWidgetStartsCounters : AnalyticsUserControlBase
    {
        private AnalyticsModuleStartsCounters _analyticsModule => AnalyticsModule as AnalyticsModuleStartsCounters;

        public AnalyticsWidgetStartsCounters(AnalyticsModuleStartsCounters analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsWidgetStartsCountersTitle;
        public override string Icon { get; } = "\uE7C1";
        public override string Info => Properties.Tooltips.TooltipAnalyticsStartsCounters;
        public override double AnalyticsWidgetWidth => ANALYTICS_WIDGET_WIDTH_EXTRA_SMALL;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberOfStarts));
            OnPropertyChanged(nameof(NumberOfInactiveStarts));
        }

        public int NumberOfStarts => _analyticsModule?.NumberOfStarts ?? 0;
        public int NumberOfInactiveStarts => _analyticsModule?.NumberOfInactiveStarts ?? 0;
    }
}
