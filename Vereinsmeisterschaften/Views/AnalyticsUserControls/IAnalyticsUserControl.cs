using System.ComponentModel;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interface used for any user control that displays analytics data
    /// </summary>
    public interface IAnalyticsUserControl : INotifyPropertyChanged
    {
        /// <summary>
        /// Title used for the diagram of the analytics user control
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Icon for this analytics. This should be e.g. "\uE787". If this is <see langword="null"/>, <see cref="IconGeometry"/> is used.
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// Info text for this analytics
        /// </summary>
        string Info { get; }

        /// <summary>
        /// Icon Geometry for this analytics. This is used instead of <see cref="Icon"/> when <see cref="Icon"/> is <see langword="null"/>.
        /// </summary>
        Geometry IconGeometry { get; }

        /// <summary>
        /// Normal Width for the analytics widget (this is the default width regardless if the control is maximized)
        /// </summary>
        double NormalAnalyticsWidgetWidth { get; }

        /// <summary>
        /// Normal Height for the analytics widget (this is the default height regardless if the control is maximized)
        /// </summary>
        double NormalAnalyticsWidgetHeight { get; }

        /// <summary>
        /// Current Width for the analytics widget
        /// </summary>
        double CurrentAnalyticsWidgetWidth { get; }

        /// <summary>
        /// Current Height for the analytics widget
        /// </summary>
        double CurrentAnalyticsWidgetHeight { get; }

        /// <summary>
        /// True if the analytics user control is displayed maximized (only one at a time should have this flag set to true)
        /// </summary>
        bool IsMaximized { get; set; }

        /// <summary>
        /// Refresh the data displayed in the user control
        /// </summary>
        void Refresh();
    }
}
