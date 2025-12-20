using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetPersonCounters.xaml
    /// </summary>
    public partial class AnalyticsWidgetPersonCounters : AnalyticsUserControlBase
    {
        private AnalyticsModulePersonCounters _analyticsModule => AnalyticsModule as AnalyticsModulePersonCounters;

        public AnalyticsWidgetPersonCounters(AnalyticsModulePersonCounters analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Title => Properties.Resources.AnalyticsWidgetPersonCountersTitle;
        public override string Icon { get; } = "\uE77B";
        public override string Info => Properties.Tooltips.TooltipAnalyticsPersonCounters;
        public override double NormalAnalyticsWidgetWidth => ANALYTICS_WIDGET_WIDTH_SMALL;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberOfPeople));
            OnPropertyChanged(nameof(NumberOfActivePeople));
            OnPropertyChanged(nameof(NumberOfInactivePeople));
        }

        public int NumberOfPeople => _analyticsModule?.NumberOfPeople ?? 0;
        public int NumberOfActivePeople => _analyticsModule?.NumberOfActivePeople ?? 0;
        public int NumberOfInactivePeople => _analyticsModule?.NumberOfInactivePeople ?? 0;
    }
}
