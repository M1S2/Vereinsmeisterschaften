using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Workspace setting group view model
    /// </summary>
    public class WorkspaceSettingsGroupViewModel : ObservableObject
    {
        /// <summary>
        /// <see cref="WorkspaceSettingsGroup"/> that is managed by this view model
        /// </summary>
        public WorkspaceSettingsGroup SettingsGroup { get; }

        /// <summary>
        /// Name of the group
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Command to set all setting values in this group back to the snapshot value
        /// </summary>
        public ICommand ResetCommand { get; }

        /// <summary>
        /// Command to set all setting values in this group back to the default value
        /// </summary>
        public ICommand SetToDefaultCommand { get; }

        /// <summary>
        /// True, if any setting value in this group is not the snapshot value
        /// </summary>
        public bool HasChanged => Settings?.Any(s => s.HasChanged) ?? false;

        /// <summary>
        /// True if all setting values in this group are the default value. Settings that do not support resetting to default are regarded as default.
        /// </summary>
        public bool HasDefaultValue => !Settings?.Any(s => s.SupportResetToDefault && !s.HasDefaultValue) ?? true;

        /// <summary>
        /// List with all <see cref="IWorkspaceSettingViewModel"/> instances belonging to this group
        /// </summary>
        public ObservableCollection<IWorkspaceSettingViewModel> Settings { get; }

        /// <summary>
        /// Constructor for the <see cref="WorkspaceSettingsGroupViewModel"/>
        /// </summary>
        /// <param name="settingsGroup"><see cref="WorkspaceSettingsGroup"/> that is managed by this view model</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="settings">List with all <see cref="IWorkspaceSettingViewModel"/> instances belonging to this group</param>
        public WorkspaceSettingsGroupViewModel(WorkspaceSettingsGroup settingsGroup, string groupName, IEnumerable<IWorkspaceSettingViewModel> settings)
        {
            SettingsGroup = settingsGroup;
            GroupName = groupName;
            Settings = new ObservableCollection<IWorkspaceSettingViewModel>(settings);
            ResetCommand = new RelayCommand(() => 
            {
                foreach (IWorkspaceSettingViewModel setting in Settings)
                {
                    setting.ResetCommand.Execute(null);
                }
                OnPropertyChanged(nameof(HasChanged));
                OnPropertyChanged(nameof(HasDefaultValue));
            });
            SetToDefaultCommand = new RelayCommand(() =>
            {
                foreach (IWorkspaceSettingViewModel setting in Settings)
                {
                    setting.SetToDefaultCommand.Execute(null);
                }
                OnPropertyChanged(nameof(HasChanged));
                OnPropertyChanged(nameof(HasDefaultValue));
            });

            foreach (var setting in Settings)
            {
                setting.PropertyChanged += (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(IWorkspaceSettingViewModel.HasChanged): OnPropertyChanged(nameof(HasChanged)); break;
                        case nameof(IWorkspaceSettingViewModel.HasDefaultValue): OnPropertyChanged(nameof(HasDefaultValue)); break;
                        default: break;
                    }
                };
            }
        }
    }
}
