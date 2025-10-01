using CommunityToolkit.Mvvm.ComponentModel;

namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// Implementation for a single workspace setting
    /// </summary>
    /// <typeparam name="T">Type of the setting</typeparam>
    public class WorkspaceSetting<T> : ObservableObject, IWorkspaceSetting, IEquatable<WorkspaceSetting<T>>
    {
        #region Properties

        private string _key;
        /// <summary>
        /// Key for this setting. Should be unique.
        /// </summary>
        public string Key 
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        private T _value;
        /// <summary>
        /// Value for the setting.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))     // returns true, if the value has changed
                {
                    OnPropertyChanged(nameof(UntypedValue));
                    OnPropertyChanged(nameof(HasChanged));
                    OnPropertyChanged(nameof(HasDefaultValue));
                }
            }
        }

        /// <summary>
        /// Value for the setting. This reflects the value of <see cref="Value"/>.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedValue
        {
            get => Value!;
            set => Value = (T)Convert.ChangeType(value, typeof(T));
        }

        private T _defaultValue;
        /// <summary>
        /// Default value for the setting.
        /// </summary>
        public T DefaultValue
        {
            get => _defaultValue;
            set
            {
                if (SetProperty(ref _defaultValue, value))      // returns true, if the value has changed
                {
                    OnPropertyChanged(nameof(HasDefaultValue));
                }
            }
        }

        /// <summary>
        /// Default value for the setting. This reflects the value of <see cref="DefaultValue"/>
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedDefaultValue => DefaultValue!;

        private T _snapshotValue;
        /// <summary>
        /// Snapshot value for the setting. This is the value at the time the <see cref="CreateSnapshot"/> method was called.
        /// </summary>
        public T SnapshotValue
        {
            get => _snapshotValue;
            private set
            {
                if (SetProperty(ref _snapshotValue, value))     // returns true, if the value has changed
                {
                    OnPropertyChanged(nameof(HasChanged));
                }
            }
        }

        /// <summary>
        /// Snapshot value for the setting. This reflects the value of <see cref="SnapshotValue"/>
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedSnapshotValue => SnapshotValue!;

        /// <summary>
        /// Minimum value for the setting.
        /// </summary>
        public T MinValue { get; }

        /// <summary>
        /// Maximum value for the setting.
        /// </summary>
        public T MaxValue { get; }

        /// <summary>
        /// Minimum value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedMinValue => MinValue!;

        /// <summary>
        /// Maximum value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedMaxValue => MaxValue!;

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
        /// <param name="minValue">Minimum value for the setting</param>
        /// <param name="maxValue">Maximum value for the setting</param>
        public WorkspaceSetting(string key, T defaultValue, T minValue = default, T maxValue = default)
        {
            Key = key;
            Value = defaultValue;
            SnapshotValue = defaultValue;
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
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
            MinValue = (other.MinValue is ICloneable cloneableMinValue) ? (T)cloneableMinValue.Clone() : other.MinValue;
            MaxValue = (other.MaxValue is ICloneable cloneableMaxValue) ? (T)cloneableMaxValue.Clone() : other.MaxValue;
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
