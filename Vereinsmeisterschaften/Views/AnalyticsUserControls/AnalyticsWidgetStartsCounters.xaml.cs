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
        public override double NormalAnalyticsWidgetWidth => ANALYTICS_WIDGET_WIDTH_SMALL;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberOfStarts));
            OnPropertyChanged(nameof(NumberOfInactiveStarts));
            OnPropertyChanged(nameof(NumberOfStartsWithMissingCompetition));
            OnPropertyChanged(nameof(NumberOfValidStarts));
            base.Refresh();
        }

        public int NumberOfStarts => _analyticsModule?.NumberOfStarts ?? 0;
        public int NumberOfInactiveStarts => _analyticsModule?.NumberOfInactiveStarts ?? 0;
        public int NumberOfStartsWithMissingCompetition => _analyticsModule?.NumberOfStartsWithMissingCompetition ?? 0;
        public int NumberOfValidStarts => _analyticsModule?.NumberOfValidStarts ?? 0;
    }
}
