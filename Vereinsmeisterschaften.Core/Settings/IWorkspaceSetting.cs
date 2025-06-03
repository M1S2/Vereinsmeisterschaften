using System.ComponentModel;

namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// Interface for a single workspace setting
    /// </summary>
    public interface IWorkspaceSetting : ICloneable, INotifyPropertyChanged
    {
        /// <summary>
        /// Key for this setting. Should be unique.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        object UntypedValue { get; set; }

        /// <summary>
        /// Default value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        object UntypedDefaultValue { get; }

        /// <summary>
        /// Snapshot value for the setting. This is the value at the time the <see cref="CreateSnapshot"/> method was called.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        object UntypedSnapshotValue { get; }

        /// <summary>
        /// Minimum value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        object UntypedMinValue { get; }

        /// <summary>
        /// Maximum value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        object UntypedMaxValue { get; }

        /// <summary>
        /// Set the <see cref="UntypedValue"/> to the <see cref="UntypedSnapshotValue"/>
        /// </summary>
        void Reset();

        /// <summary>
        /// Save the <see cref="UntypedValue"/> to the <see cref="UntypedSnapshotValue"/>
        /// </summary>
        void CreateSnapshot();

        /// <summary>
        /// Set the <see cref="UntypedValue"/> to the <see cref="UntypedDefaultValue"/>
        /// </summary>
        void SetToDefault();

        /// <summary>
        /// Indicating if the <see cref="UntypedValue"/> and <see cref="UntypedSnapshotValue"/> differ.
        /// </summary>
        bool HasChanged { get; }

        /// <summary>
        /// Indicating if the <see cref="UntypedValue"/> equals the <see cref="UntypedDefaultValue"/>.
        /// </summary>
        bool HasDefaultValue { get; }

        /// <summary>
        /// Type of this setting
        /// </summary>
        Type ValueType { get; }
    }
}
