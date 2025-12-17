using System.Windows.Media;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    /// <summary>
    /// Interface used for any user control that displays analytics data
    /// </summary>
    public interface IAnalyticsUserControl
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
        /// Width for the analytics widget
        /// </summary>
        double AnalyticsWidgetWidth { get; }

        /// <summary>
        /// Height for the analytics widget
        /// </summary>
        double AnalyticsWidgetHeight { get; }

        /// <summary>
        /// Refresh the data displayed in the user control
        /// </summary>
        void Refresh();
    }
}
