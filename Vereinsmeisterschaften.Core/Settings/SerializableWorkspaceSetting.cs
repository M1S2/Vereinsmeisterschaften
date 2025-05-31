namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// Class describing the properties of the <see cref="WorkspaceSetting{T}"/> that should be saved to a file
    /// </summary>
    public class SerializableWorkspaceSetting
    {
        /// <summary>
        /// Key for this setting. Should be unique.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value for the setting.
        /// </summary>
        public object Value { get; set; }
    }
}
