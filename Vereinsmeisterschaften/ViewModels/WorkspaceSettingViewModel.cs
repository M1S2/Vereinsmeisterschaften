using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
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
        /// Icon for this setting. This should be e.g. "\uE787"
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// True, if the setting value is not the snapshot value
        /// </summary>
        public bool HasChanged => Setting?.HasChanged ?? false;

        /// <summary>
        /// True if the setting value is the default value
        /// </summary>
        public bool HasDefaultValue => Setting?.HasDefaultValue ?? true;

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
            set { Setting.Value = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasChanged)); OnPropertyChanged(nameof(HasDefaultValue)); }
        }

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
        /// <param name="icon">Icon for this setting. This should be e.g. "\uE787"</param>
        /// <param name="editorTemplate">Data template to assign a setting dependent editor view.</param>
        public WorkspaceSettingViewModel(WorkspaceSetting<T> setting, string label, string tooltip, string icon, DataTemplate editorTemplate)
        {
            Setting = setting;
            Label = label;
            Tooltip = tooltip;
            Icon = icon;
            ResetCommand = new RelayCommand(() =>
            {
                setting?.Reset();
                OnPropertyChanged(nameof(Value)); 
                OnPropertyChanged(nameof(HasChanged)); 
                OnPropertyChanged(nameof(HasDefaultValue)); 
            });
            SetToDefaultCommand = new RelayCommand(() => 
            { 
                setting?.SetToDefault(); 
                OnPropertyChanged(nameof(Value)); 
                OnPropertyChanged(nameof(HasDefaultValue)); 
                OnPropertyChanged(nameof(HasChanged)); 
            });
            EditorTemplate = editorTemplate;
        }
    }
}
