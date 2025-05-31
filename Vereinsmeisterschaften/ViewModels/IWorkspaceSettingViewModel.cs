using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Interface for the workspace setting view model
    /// </summary>
    public interface IWorkspaceSettingViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Label describing the setting
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Tooltip for this setting
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Icon for this setting. This should be e.g. "\uE787"
        /// </summary>
        string Icon { get; }

        /// <summary>
        /// True, if the setting value is not the snapshot value
        /// </summary>
        bool HasChanged { get; }

        /// <summary>
        /// True if the setting value is the default value
        /// </summary>
        bool HasDefaultValue { get; }

        /// <summary>
        /// Command to set the setting value back to the snapshot value
        /// </summary>
        ICommand ResetCommand { get; }

        /// <summary>
        /// Command to set the setting value back to the default value
        /// </summary>
        ICommand SetToDefaultCommand { get; }

        /// <summary>
        /// Type of the setting value
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Value for the setting.
        /// This is type independent by using the <see cref="object"/> type.
        /// </summary>
        object UntypedValue { get; set; }

        /// <summary>
        /// Data template to assign a setting dependent editor view.
        /// </summary>
        DataTemplate EditorTemplate { get; }
    }
}
