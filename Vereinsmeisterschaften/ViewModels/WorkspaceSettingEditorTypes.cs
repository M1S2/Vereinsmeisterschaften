
namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Enum with available editor types for workspace settings.
    /// </summary>
    public enum WorkspaceSettingEditorTypes
    {
        /// <summary>
        /// Display a numeric value editor.
        /// </summary>
        Numeric,

        /// <summary>
        /// Display a string value editor.
        /// </summary>
        String,

        /// <summary>
        /// Display a file path editor that is relative to the workspace.
        /// </summary>
        FileRelative,

        /// <summary>
        /// Display a file path editor that is absolute.
        /// </summary>
        FileAbsolute,

        /// <summary>
        /// Display a file path editor for a .docx file that is relative to the workspace.
        /// </summary>
        FileDocxRelative,

        /// <summary>
        /// Display a file path editor for a .docx file that is absolute.
        /// </summary>
        FileDocxAbsolute,

        /// <summary>
        /// Display a folder path editor that is relative to the workspace.
        /// </summary>
        FolderRelative,

        /// <summary>
        /// Display a folder path editor that is absolute.
        /// </summary>
        FolderAbsolute,

        /// <summary>
        /// Display a boolean value editor (e.g. checkbox).
        /// </summary>
        Boolean,

        /// <summary>
        /// Display an editor for an enumeration value.
        /// </summary>
        Enum
    }
}
