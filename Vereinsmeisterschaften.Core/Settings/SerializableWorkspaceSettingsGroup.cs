namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// Class describing the properties of the <see cref="WorkspaceSettingsGroup"/> that should be saved to a file
    /// </summary>
    public class SerializableWorkspaceSettingsGroup
    {
        /// <summary>
        /// Key for this group. Should be unique.
        /// </summary>
        public string GroupKey { get; set; }

        /// <summary>
        /// List with <see cref="SerializableWorkspaceSetting"/> instances belonging to this group.
        /// </summary>
        public List<SerializableWorkspaceSetting> Settings { get; set; }
    }
}
