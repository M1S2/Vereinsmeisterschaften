using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Interaktionslogik für NewWorkspaceSettingsDialog.xaml
    /// </summary>
    public partial class NewWorkspaceSettingsDialog : CustomDialog, INotifyDataErrorInfo
    {
        #region Constructor

        private readonly TaskCompletionSource<MessageDialogResult> _tcs;

        public NewWorkspaceSettingsDialog()
        {
            InitializeComponent();
            _tcs = new TaskCompletionSource<MessageDialogResult>();
            Title = Properties.Resources.NewWorkspaceString;
            Validate();
        }

        public NewWorkspaceSettingsDialog(MetroWindow parentWindow) : base(parentWindow)
        {
            InitializeComponent();
            _tcs = new TaskCompletionSource<MessageDialogResult>();
            Title = Properties.Resources.NewWorkspaceString;
            Validate();
        }

        #endregion

        // **********************************************************************************************************************************************

        #region Properties

        public string NewWorkspaceFolder
        {
            get { return (string)GetValue(NewWorkspaceFolderProperty); }
            set { SetValue(NewWorkspaceFolderProperty, value); }
        }
        public static readonly DependencyProperty NewWorkspaceFolderProperty = DependencyProperty.Register(nameof(NewWorkspaceFolder), typeof(string), typeof(NewWorkspaceSettingsDialog), new PropertyMetadata("", OnPropertyChanged));

        public bool CopyDefaultTemplates
        {
            get { return (bool)GetValue(CopyDefaultTemplatesProperty); }
            set { SetValue(CopyDefaultTemplatesProperty, value); }
        }
        public static readonly DependencyProperty CopyDefaultTemplatesProperty = DependencyProperty.Register(nameof(CopyDefaultTemplates), typeof(bool), typeof(NewWorkspaceSettingsDialog), new PropertyMetadata(true, OnPropertyChanged));

        public string PreviousWorkspaceFolder
        {
            get { return (string)GetValue(PreviousWorkspaceFolderProperty); }
            set { SetValue(PreviousWorkspaceFolderProperty, value); }
        }
        public static readonly DependencyProperty PreviousWorkspaceFolderProperty = DependencyProperty.Register(nameof(PreviousWorkspaceFolder), typeof(string), typeof(NewWorkspaceSettingsDialog), new PropertyMetadata("", OnPropertyChanged));

        public bool CopyCompetitions
        {
            get { return (bool)GetValue(CopyCompetitionsProperty); }
            set { SetValue(CopyCompetitionsProperty, value); }
        }
        public static readonly DependencyProperty CopyCompetitionsProperty = DependencyProperty.Register(nameof(CopyCompetitions), typeof(bool), typeof(NewWorkspaceSettingsDialog), new PropertyMetadata(false, OnPropertyChanged));

        public bool CopyCompetitionDistanceRules
        {
            get { return (bool)GetValue(CopyCompetitionDistanceRulesProperty); }
            set { SetValue(CopyCompetitionDistanceRulesProperty, value); }
        }
        public static readonly DependencyProperty CopyCompetitionDistanceRulesProperty =  DependencyProperty.Register(nameof(CopyCompetitionDistanceRules), typeof(bool), typeof(NewWorkspaceSettingsDialog), new PropertyMetadata(false, OnPropertyChanged));

        public bool CopyTemplates
        {
            get { return (bool)GetValue(CopyTemplatesProperty); }
            set { SetValue(CopyTemplatesProperty, value); }
        }
        public static readonly DependencyProperty CopyTemplatesProperty = DependencyProperty.Register(nameof(CopyTemplates), typeof(bool), typeof(NewWorkspaceSettingsDialog), new PropertyMetadata(false, OnPropertyChanged));


        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NewWorkspaceSettingsDialog dialog = d as NewWorkspaceSettingsDialog;
            if(dialog == null) { return; }
            ((RelayCommand)dialog.DialogResultAffirmativeCommand).NotifyCanExecuteChanged();

            dialog.Validate();
        }
        #endregion

        // **********************************************************************************************************************************************

        #region Dialog Handling

        /// <summary>
        /// Wait until the cancel or ok button is pressed.
        /// The dialog isn't closed here automatically. This must be done by using <see cref="IDialogCoordinator.HideMetroDialogAsync(object, BaseMetroDialog, MetroDialogSettings)"/>
        /// </summary>
        /// <returns><see cref="MessageDialogResult"/></returns>
        public Task<MessageDialogResult> WaitForDialogButtonPressAsync()
            => _tcs.Task;

        private ICommand _dialogResultNegativeCommand;
        /// <summary>
        /// Command called by the negative button (cancel)
        /// </summary>
        public ICommand DialogResultNegativeCommand => _dialogResultNegativeCommand ?? (_dialogResultNegativeCommand = new RelayCommand(() =>
        {
            _tcs.TrySetResult(MessageDialogResult.Negative);
        }));

        private ICommand _dialogResultAffirmativeCommand;

        /// <summary>
        /// Command called by the affirmative button (ok)
        /// </summary>
        public ICommand DialogResultAffirmativeCommand => _dialogResultAffirmativeCommand ?? (_dialogResultAffirmativeCommand = new RelayCommand(() =>
        {
            _tcs.TrySetResult(MessageDialogResult.Affirmative);
        }, () => !string.IsNullOrEmpty(NewWorkspaceFolder) &&
                 !(CopyTemplates && CopyDefaultTemplates)));

        #endregion

        // **********************************************************************************************************************************************

        #region Validation

        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != null && _errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            return Enumerable.Empty<string>();
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            _errors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void Validate()
        {
            ClearErrors(nameof(NewWorkspaceFolder));
            ClearErrors(nameof(CopyTemplates));
            ClearErrors(nameof(CopyDefaultTemplates));

            if (string.IsNullOrEmpty(NewWorkspaceFolder))
            {
                AddError(nameof(NewWorkspaceFolder), Properties.Resources.EmptyPathString);
            }

            if (CopyTemplates && CopyDefaultTemplates)
            {
                AddError(nameof(CopyTemplates), Properties.Resources.OnlyOneOptionCanBeActiveString);
                AddError(nameof(CopyDefaultTemplates), Properties.Resources.OnlyOneOptionCanBeActiveString);
            }
        }

        #endregion
    }
}
