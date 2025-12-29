using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetPersonCounters.xaml
    /// </summary>
    public partial class AnalyticsWidgetPersonCounters : AnalyticsWidgetBase
    {
        private AnalyticsModulePersonCounters _analyticsModule => AnalyticsModule as AnalyticsModulePersonCounters;

        public AnalyticsWidgetPersonCounters(AnalyticsModulePersonCounters analyticsModule) : base(analyticsModule)
        {
            InitializeComponent();
        }

        public override string Icon { get; } = "\uE77B";
        public override double NormalAnalyticsWidgetWidth => ANALYTICS_WIDGET_WIDTH_SMALL;

        public override void Refresh()
        {
            OnPropertyChanged(nameof(NumberOfPeople));
            OnPropertyChanged(nameof(NumberOfActivePeople));
            OnPropertyChanged(nameof(NumberOfInactivePeople));
            base.Refresh();
        }

        public int NumberOfPeople => _analyticsModule?.NumberOfPeople ?? 0;
        public int NumberOfActivePeople => _analyticsModule?.NumberOfActivePeople ?? 0;
        public int NumberOfInactivePeople => _analyticsModule?.NumberOfInactivePeople ?? 0;
    }
}
