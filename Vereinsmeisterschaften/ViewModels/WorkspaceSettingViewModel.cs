using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Workspace setting view model
    /// </summary>
    /// <typeparam name="T">Type of the setting</typeparam>
    public class WorkspaceSettingViewModel<T> : ObservableObject, IWorkspaceSettingViewModel
    {
        /// <summary>
        /// <see cref="WorkspaceSetting{T}"/> that is managed by this view model
        /// </summary>
        public WorkspaceSetting<T> Setting { get; }

        /// <summary>
        /// Label describing the setting
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Tooltip for this setting
        /// </summary>
        public string Tooltip { get; }

        /// <summary>
        /// Icon for this setting. This should be e.g. "\uE787". If this is <see langword="null"/>, <see cref="IconDrawingImage"/> is used.
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// Icon Geometry for this setting. This is used instead of <see cref="Icon"/> when <see cref="Icon"/> is <see langword="null"/>.
        /// </summary>
        public Geometry IconGeometry { get; }

        /// <summary>
        /// True, if the setting value is not the snapshot value
        /// </summary>
        public bool HasChanged => Setting?.HasChanged ?? false;

        /// <summary>
        /// True if the setting value is the default value
        /// </summary>
        public bool HasDefaultValue => Setting?.HasDefaultValue ?? true;

        private bool _supportResetToDefault;
        /// <summary>
        /// Support for resetting the setting value to the default value.
        /// </summary>
        public bool SupportResetToDefault
        {
            get => _supportResetToDefault;
            set => SetProperty(ref _supportResetToDefault, value);
        }

        /// <summary>
        /// Command to set the setting value back to the snapshot value
        /// </summary>
        public ICommand ResetCommand { get; }

        /// <summary>
        /// Command to set the setting value back to the default value
        /// </summary>
        public ICommand SetToDefaultCommand { get; }

        /// <summary>
        /// Setting value
        /// </summary>
        public T Value
        {
            get => Setting.Value;
            set
            {
                if (!Setting.Value.Equals(value))
                {
                    Setting.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasChanged));
                    OnPropertyChanged(nameof(HasDefaultValue));
                }
            }
        }

        /// <summary>
        /// Minimum value for the setting.
        /// </summary>
        public T MinValue => Setting.MinValue;

        /// <summary>
        /// Maximum value for the setting.
        /// </summary>
        public T MaxValue => Setting.MaxValue;

        /// <summary>
        /// Minimum value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedMinValue => Setting.UntypedMinValue!;

        /// <summary>
        /// Maximum value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedMaxValue => Setting.UntypedMaxValue!;

        /// <summary>
        /// Type of the setting value
        /// </summary>
        public Type ValueType => typeof(T);

        /// <summary>
        /// Value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        public object UntypedValue
        {
            get => Value!;
            set => Value = (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Data template to assign a setting dependent editor view.
        /// </summary>
        public DataTemplate EditorTemplate { get; }

        /// <summary>
        /// Constructor for the <see cref="WorkspaceSettingViewModel{T}"/>
        /// </summary>
        /// <param name="setting"><see cref="WorkspaceSetting{T}"/> that is managed by this view model</param>
        /// <param name="label">Label describing the setting</param>
        /// <param name="tooltip">Tooltip for this setting</param>
        /// <param name="icon">Icon for this setting. This should be e.g. "\uE787". If this is <see langword="null"/>, <see cref="IconDrawingImage"/> is used.</param>
        /// <param name="iconGeometry">Icon Geometry for this setting. This is used instead of <see cref="Icon"/> when <see cref="Icon"/> is <see langword="null"/>.</param>
        /// <param name="editorTemplate">Data template to assign a setting dependent editor view.</param>
        /// <param name="supportResetToDefault">Support for resetting the setting value to the default value.</param>
        public WorkspaceSettingViewModel(WorkspaceSetting<T> setting, string label, string tooltip, string icon, Geometry iconGeometry, DataTemplate editorTemplate, bool supportResetToDefault)
        {
            Setting = setting;
            Label = label;
            Tooltip = tooltip;
            Icon = icon;
            IconGeometry = iconGeometry;
            ResetCommand = new RelayCommand(() =>
            {
                setting?.Reset();
                OnPropertyChanged(nameof(Value)); 
                OnPropertyChanged(nameof(HasChanged)); 
                OnPropertyChanged(nameof(HasDefaultValue)); 
            });
            SetToDefaultCommand = new RelayCommand(() => 
            {
                if (SupportResetToDefault)
                {
                    setting?.SetToDefault();
                }
                OnPropertyChanged(nameof(Value)); 
                OnPropertyChanged(nameof(HasDefaultValue)); 
                OnPropertyChanged(nameof(HasChanged)); 
            });
            EditorTemplate = editorTemplate;
            SupportResetToDefault = supportResetToDefault;

            if (setting != null)
            {
                setting.PropertyChanged += (sender, e) =>
                {
                    switch(e.PropertyName)
                    {
                        case nameof(WorkspaceSetting<T>.Value): OnPropertyChanged(nameof(Value)); break;
                        case nameof(WorkspaceSetting<T>.HasChanged): OnPropertyChanged(nameof(HasChanged)); break;
                        case nameof(WorkspaceSetting<T>.HasDefaultValue): OnPropertyChanged(nameof(HasDefaultValue)); break;
                        default: break;
                    }
                };
            }
        }
    }
}
