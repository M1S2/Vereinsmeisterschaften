
using System.Windows.Media;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// View configuration for a single workspace setting
    /// </summary>
    public class WorkspaceSettingViewConfig
    {
        /// <summary>
        /// Label describing the setting
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Tooltip with more information about the setting
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Icon for this setting. This should be e.g. "\uE787". If this is <see langword="null"/>, <see cref="IconDrawingImage"/> is used.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Icon drawing for this setting. This is used instead of <see cref="Icon"/> when <see cref="Icon"/> is <see langword="null"/>.
        /// </summary>
        public DrawingImage IconDrawingImage { get; set; }

        /// <summary>
        /// Type of editor to use for this setting
        /// </summary>
        public WorkspaceSettingEditorTypes Editor { get; set; }

        /// <summary>
        /// Support for resetting the setting value to the default value.
        /// </summary>
        public bool SupportResetToDefault { get; set; } = true;
    }
}
