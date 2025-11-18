using System.Windows;
using CommunityToolkit.Mvvm.Input;
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

        /// <summary>
        /// Message displayed on the dialog
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(DoubleProgressDialog), new PropertyMetadata(""));

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


        /// <summary>
        /// Command to raise the <see cref="OnCanceled"/> event via the Cancel button
        /// </summary>
        [ICommand]
        public void Cancel()
        {
            OnCanceled?.Invoke(this, null);
        }

        /// <summary>
        /// Event that is raised, when the user clicks the Cancel button
        /// </summary>
        public event EventHandler OnCanceled;

    }
}
