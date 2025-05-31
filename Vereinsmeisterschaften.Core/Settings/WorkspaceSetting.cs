namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// Implementation for a single workspace setting
    /// </summary>
    /// <typeparam name="T">Type of the setting</typeparam>
    public class WorkspaceSetting<T> : IWorkspaceSetting, IEquatable<WorkspaceSetting<T>>
    {
        #region Properties

        /// <summary>
        /// Key for this setting. Should be unique.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value for the setting.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Value for the setting. This reflects the value of <see cref="Value"/>.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedValue
        {
            get => Value!;
            set => Value = (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Default value for the setting.
        /// </summary>
        public T DefaultValue { get; set; }

        /// <summary>
        /// Default value for the setting. This reflects the value of <see cref="DefaultValue"/>
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedDefaultValue => DefaultValue!;

        /// <summary>
        /// Snapshot value for the setting. This is the value at the time the <see cref="CreateSnapshot"/> method was called.
        /// </summary>
        public T SnapshotValue { get; private set; }

        /// <summary>
        /// Snapshot value for the setting. This reflects the value of <see cref="SnapshotValue"/>
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedSnapshotValue => SnapshotValue!;

        /// <summary>
        /// Type of this setting
        /// </summary>
        public Type ValueType => typeof(T);

        /// <summary>
        /// Indicating if the <see cref="Value"/> and <see cref="SnapshotValue"/> differ.
        /// </summary>
        public bool HasChanged => !EqualityComparer<T>.Default.Equals(Value, SnapshotValue);

        /// <summary>
        /// Indicating if the <see cref="Value"/> equals the <see cref="DefaultValue"/>.
        /// </summary>
        public bool HasDefaultValue => EqualityComparer<T>.Default.Equals(Value, DefaultValue);

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Constructors

        /// <summary>
        /// Parameterless Constructor
        /// </summary>
        public WorkspaceSetting()
        {
        }

        /// <summary>
        /// Create a new <see cref="WorkspaceSetting{T}"/>
        /// </summary>
        /// <param name="key">Key for this setting. This should be unique.</param>
        /// <param name="defaultValue">Default value</param>
        public WorkspaceSetting(string key, T defaultValue)
        {
            Key = key;
            Value = defaultValue;
            SnapshotValue = defaultValue;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"><see cref="WorkspaceSetting{T}"/> to copy</param>
        public WorkspaceSetting(WorkspaceSetting<T> other)
        {
            if (other == null) { return; }
            Key = other.Key;
            Value = (other.Value is ICloneable cloneableValue) ? (T)cloneableValue.Clone() : other.Value;
            DefaultValue = (other.DefaultValue is ICloneable cloneableDefaultValue) ? (T)cloneableDefaultValue.Clone() : other.DefaultValue;
            SnapshotValue = (other.SnapshotValue is ICloneable cloneableSnapshotValue) ? (T)cloneableSnapshotValue.Clone() : other.SnapshotValue;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Reset / Snapshot / Default

        /// <summary>
        /// Set the <see cref="Value"/> to the <see cref="SnapshotValue"/>
        /// </summary>
        public void Reset() => Value = SnapshotValue;

        /// <summary>
        /// Save the <see cref="Value"/> to the <see cref="SnapshotValue"/>
        /// </summary>
        public void CreateSnapshot() => SnapshotValue = Value;

        /// <summary>
        /// Set the <see cref="Value"/> to the <see cref="DefaultValue"/>
        /// </summary>
        public void SetToDefault() => Value = DefaultValue;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region IEquatable, ICloneable, ToString()

        public override bool Equals(object obj)
            => obj is WorkspaceSetting<T> s && s.Value.Equals(Value);

        public bool Equals(WorkspaceSetting<T> other)
            => Equals((object)other);

        public override int GetHashCode()
            => Value.GetHashCode();

        public object Clone()
            => new WorkspaceSetting<T>(this);

        public override string ToString()
            => $"{Key}: {Value} (Default: {DefaultValue}, Snapshot: {SnapshotValue})";

        #endregion

    }
}
