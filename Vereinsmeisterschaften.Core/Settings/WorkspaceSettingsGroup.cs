using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// A group of <see cref="WorkspaceSetting{T}"/> objects
    /// </summary>
    public class WorkspaceSettingsGroup : ObservableObject, IEquatable<WorkspaceSettingsGroup>, ICloneable
    {
        #region Properties

        private string _groupKey;
        /// <summary>
        /// Key for this group. Should be unique.
        /// </summary>
        public string GroupKey
        {
            get => _groupKey;
            set => SetProperty(ref _groupKey, value);
        }

        /// <summary>
        /// List with <see cref="IWorkspaceSetting"/> instances belonging to this group.
        /// </summary>
        public List<IWorkspaceSetting> Settings { get; set; } = new List<IWorkspaceSetting>();

        /// <summary>
        /// Indicating if any of the <see cref="Settings"/> has changed.
        /// </summary>
        public bool HasChanged => Settings.Any(s => s.HasChanged);

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Setting management methods

        /// <summary>
        /// Add a new <see cref="WorkspaceSetting{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of the setting value</typeparam>
        /// <param name="setting">Setting to add</param>
        public void AddSetting<T>(WorkspaceSetting<T> setting) => AddSetting(setting as IWorkspaceSetting);
        
        /// <summary>
        /// Add a new <see cref="IWorkspaceSetting"/>
        /// </summary>
        /// <param name="setting">Setting to add</param>
        /// <exception cref="ArgumentException">If a setting with the same key already exists, this exception is thrown</exception>
        public void AddSetting(IWorkspaceSetting setting)
        {
            if (Settings.Where(s => string.Compare(s.Key, setting.Key, true) == 0).Any())
            {
                // A setting with the same key already exists
                throw new ArgumentException($"A setting with the key '{setting.Key}' already exists.", nameof(setting));
            }
            else
            {
                Settings.Add(setting);
                setting.PropertyChanged += (sender, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(IWorkspaceSetting.HasChanged): OnPropertyChanged(nameof(HasChanged)); break;
                        default: break;
                    }
                };
            }
        }

        /// <summary>
        /// Get the setting with the requested key
        /// </summary>
        /// <typeparam name="T">Type of the setting value</typeparam>
        /// <param name="settingKey">Key of the setting to get</param>
        /// <returns><see cref="WorkspaceSetting{T}"/></returns>
        public WorkspaceSetting<T> GetSetting<T>(string settingKey) => (WorkspaceSetting<T>)GetSetting(settingKey);

        /// <summary>
        /// Get the setting with the requested key
        /// </summary>
        /// <param name="settingKey">Key of the setting to get</param>
        /// <returns><see cref="IWorkspaceSetting"/></returns>
        public IWorkspaceSetting GetSetting(string settingKey) => Settings.FirstOrDefault(s => string.Compare(s.Key, settingKey, true) == 0);

        /// <summary>
        /// Try to get the <see cref="WorkspaceSetting{T}"/> with the requested key and add a new setting with the default value if it wasn't found.
        /// </summary>
        /// <typeparam name="T">Type of the setting value</typeparam>
        /// <param name="settingKey">Key for the setting</param>
        /// <param name="defaultValue">Default value for the setting</param>
        /// <param name="minValue">Minimum value for the setting</param>
        /// <param name="maxValue">Maximum value for the setting</param>
        /// <returns><see cref="WorkspaceSetting{T}"/></returns>
        public WorkspaceSetting<T> MakeSureSettingExists<T>(string settingKey, T defaultValue, T minValue = default, T maxValue = default)
        {
            WorkspaceSetting<T> setting = GetSetting<T>(settingKey);
            if (setting == null)
            {
                setting = new WorkspaceSetting<T>(settingKey, defaultValue, minValue, maxValue);
                AddSetting(setting);
            }
            else
            {
                setting.DefaultValue = defaultValue;
            }
            return setting;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Reset

        /// <summary>
        /// Reset all <see cref="IWorkspaceSetting"/> in <see cref="Settings"/>
        /// </summary>
        public void Reset()
        {
            foreach (IWorkspaceSetting setting in Settings)
            {
                setting.Reset();
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Constructors

        /// <summary>
        /// Parameterless Constructor
        /// </summary>
        public WorkspaceSettingsGroup()
        {
        }

        /// <summary>
        /// Create a new instance and set the group key
        /// </summary>
        /// <param name="groupKey">Key for this group. Should be unique.</param>
        public WorkspaceSettingsGroup(string groupKey) => GroupKey = groupKey;

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"><see cref="WorkspaceSettingsGroup"/> to copy</param>
        public WorkspaceSettingsGroup(WorkspaceSettingsGroup other)
        {
            GroupKey = other.GroupKey;
            foreach(IWorkspaceSetting setting in other.Settings)
            {
                if (setting is ICloneable cloneableSetting)
                {
                    AddSetting((IWorkspaceSetting)cloneableSetting.Clone());
                }
                else
                {
                    AddSetting(setting);    // This assumes the value is immutable or does not require cloning
                }
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region IEquatable, ICloneable

        public override bool Equals(object obj)
            => obj is WorkspaceSettingsGroup g && g.Settings.SequenceEqual(Settings);

        public bool Equals(WorkspaceSettingsGroup other)
            => Equals((object)other);

        public override int GetHashCode()
            => Settings.GetHashCode();

        public object Clone()
            => new WorkspaceSettingsGroup(this);

        #endregion
    }
}
