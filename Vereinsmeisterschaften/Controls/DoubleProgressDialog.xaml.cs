using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Interaktionslogik für DoubleProgressDialog.xaml
    /// </summary>
    public partial class DoubleProgressDialog : CustomDialog
    {
        public DoubleProgressDialog()
        {
            InitializeComponent();
        }

        public DoubleProgressDialog(MetroWindow parentWindow) : base(parentWindow)
        {
            InitializeComponent();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Message

        /// <summary>
        /// Message displayed on the dialog
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(DoubleProgressDialog), new PropertyMetadata(""));

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Progress 1

        /// <summary>
        /// Progress in % (0..100) for the progress bar 1
        /// </summary>
        public double Progress1
        {
            get { return (double)GetValue(Progress1Property); }
            set { SetValue(Progress1Property, value); }
        }
        public static readonly DependencyProperty Progress1Property = DependencyProperty.Register(nameof(Progress1), typeof(double), typeof(DoubleProgressDialog), new PropertyMetadata(0.0));

        /// <summary>
        /// Text describing the progress bar 1
        /// </summary>
        public string ProgressDescription1
        {
            get { return (string)GetValue(ProgressDescription1Property); }
            set { SetValue(ProgressDescription1Property, value); }
        }
        public static readonly DependencyProperty ProgressDescription1Property = DependencyProperty.Register(nameof(ProgressDescription1), typeof(string), typeof(DoubleProgressDialog), new PropertyMetadata(""));

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Progress 2

        /// <summary>
        /// Progress in % (0..100) for the progress bar 2
        /// </summary>
        public double Progress2
        {
            get { return (double)GetValue(Progress2Property); }
            set { SetValue(Progress2Property, value); }
        }
        public static readonly DependencyProperty Progress2Property = DependencyProperty.Register(nameof(Progress2), typeof(double), typeof(DoubleProgressDialog), new PropertyMetadata(0.0));

        /// <summary>
        /// Text describing the progress bar 2
        /// </summary>
        public string ProgressDescription2
        {
            get { return (string)GetValue(ProgressDescription2Property); }
            set { SetValue(ProgressDescription2Property, value); }
        }
        public static readonly DependencyProperty ProgressDescription2Property = DependencyProperty.Register(nameof(ProgressDescription2), typeof(string), typeof(DoubleProgressDialog), new PropertyMetadata(""));

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Progress Format

        /// <summary>
        /// Number of decimal places used to display the progress
        /// </summary>
        public int ProgressNumberDecimals
        {
            get { return (int)GetValue(ProgressNumberDecimalsProperty); }
            set { SetValue(ProgressNumberDecimalsProperty, value); }
        }
        public static readonly DependencyProperty ProgressNumberDecimalsProperty = DependencyProperty.Register(nameof(ProgressNumberDecimals), typeof(int), typeof(DoubleProgressDialog), new PropertyMetadata(1, OnProgressNumberDecimalsChanged));

        private static void OnProgressNumberDecimalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleProgressDialog dialog = d as DoubleProgressDialog;
            if (dialog != null)
            {
                dialog.ProgressFormatString = $"{{0:F{dialog.ProgressNumberDecimals}}}%";
            }
        }

        /// <summary>
        /// Format string used to display the progress
        /// </summary>
        public string ProgressFormatString
        {
            get { return (string)GetValue(ProgressFormatStringProperty); }
            set { SetValue(ProgressFormatStringProperty, value); }
        }
        public static readonly DependencyProperty ProgressFormatStringProperty = DependencyProperty.Register(nameof(ProgressFormatString), typeof(string), typeof(DoubleProgressDialog), new PropertyMetadata("{0:F1}%"));

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Elapsed Time Measurement

        private DateTime elapsedTimeStart;
        private DispatcherTimer elapsedTimeTimer = new DispatcherTimer();

        /// <summary>
        /// Elapsed time displayed in the dialog. The time is only displayed and updated when the <see cref="IsElapsedTimeEnabled"/> flag is set.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return (TimeSpan)GetValue(ElapsedTimePropertyKey.DependencyProperty); }
            protected set { SetValue(ElapsedTimePropertyKey, value); }
        }
        private static readonly DependencyPropertyKey ElapsedTimePropertyKey = DependencyProperty.RegisterReadOnly(nameof(ElapsedTime), typeof(TimeSpan), typeof(DoubleProgressDialog), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));

        /// <summary>
        /// The elapsed time is only displayed when this flag is on.
        /// Toggle the flag to <see langword="false"/> and <see langword="true"/> again to restart the elapsed time.
        /// </summary>
        public bool IsElapsedTimeEnabled
        {
            get { return (bool)GetValue(IsElapsedTimeEnabledProperty); }
            set { SetValue(IsElapsedTimeEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsElapsedTimeEnabledProperty = DependencyProperty.Register(nameof(IsElapsedTimeEnabled), typeof(bool), typeof(DoubleProgressDialog), new PropertyMetadata(false, OnIsElapsedTimeEnabledChanged));

        private static void OnIsElapsedTimeEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleProgressDialog dialog = (DoubleProgressDialog)d;
            if(e.NewValue is bool enabled && enabled)
            {
                dialog.elapsedTimeStart = DateTime.Now;
                dialog.elapsedTimeTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                dialog.elapsedTimeTimer.Tick += (sender, e) => dialog.ElapsedTime = DateTime.Now - dialog.elapsedTimeStart;
                dialog.elapsedTimeTimer.Start();
            }
            else
            {
                dialog.elapsedTimeTimer.Stop();
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Detail Section

        /// <summary>
        /// List with entries for the details section of the dialog.
        /// It is possible to use any data type here.
        /// The correct <see cref="DataTemplate"/> targeting the type of the list elements must be available as app-wide resource (e.g. in App.xaml).
        /// </summary>
        public ObservableCollection<object> DetailSectionEntries
        {
            get { return (ObservableCollection<object>)GetValue(DetailSectionEntriesProperty); }
            set { SetValue(DetailSectionEntriesProperty, value); }
        }
        public static readonly DependencyProperty DetailSectionEntriesProperty = DependencyProperty.Register(nameof(DetailSectionEntries), typeof(ObservableCollection<object>), typeof(DoubleProgressDialog), new PropertyMetadata(new ObservableCollection<object>()));

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Cancel

        /// <summary>
        /// Command to raise the <see cref="OnCanceled"/> event via the Cancel button
        /// </summary>
        /// <param name="dismissData">Depending on which button this command is used, the calculated data should be dismissed or not.</param>
        [ICommand]
        public void Cancel(bool dismissData = false)
        {
            OnCanceled?.Invoke(this, dismissData);
        }

        /// <summary>
        /// Delegate used for the <see cref="OnCanceled"/> event
        /// </summary>
        /// <param name="senderDialog">Dialog that is sending the cancel event</param>
        /// <param name="dismissData">Flag that is indicating if the calculated data (while the dialog is open) should be dismissed or not.</param>
        public delegate void CancelDelegate(object? senderDialog, bool dismissData);

        /// <summary>
        /// Event that is raised, when the user clicks one of the Cancel buttons
        /// </summary>
        public event CancelDelegate OnCanceled;

        #endregion
    }
}
