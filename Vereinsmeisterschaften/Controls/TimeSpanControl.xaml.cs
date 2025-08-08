using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Interaktionslogik für TimeSpanControl.xaml
    /// This control is inspired by the HTML time input (https://www.w3schools.com/tags/tryit.asp?filename=tryhtml5_input_type_time)
    /// </summary>
    public partial class TimeSpanControl : UserControl
    {
        /// <summary>
        /// Enum with editable parts of the TimeSpan
        /// </summary>
        public enum TimeSpanControlEditModes
        {
            None,
            EditHours,
            EditMinutes,
            EditSeconds,
            EditMilliseconds
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor of the TimeSpanControl
        /// </summary>
        public TimeSpanControl()
        {
            InitializeComponent();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private bool _digitEntry_StartWithFirstDigit = true;        // If this flag is true, the next entered digit is used as 1er digit (rightmost digit)
        private byte _digitEntry_CountEnteredZeroes = 0;            // Number of entered zeroes. This is reset whenever the EditModes changes
        
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Dependency Properties

        /// <summary>
        /// Current edit mode. This determines the part of the TimeSpan that is currently edited.
        /// </summary>
        public TimeSpanControlEditModes EditMode
        {
            get { return (TimeSpanControlEditModes)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="EditMode"/> property.
        /// </summary>
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register(nameof(EditMode), typeof(TimeSpanControlEditModes), typeof(TimeSpanControl), new UIPropertyMetadata(TimeSpanControlEditModes.EditMinutes, new PropertyChangedCallback(OnEditModeChanged)));

        private static void OnEditModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanControl control = obj as TimeSpanControl;
            control._digitEntry_StartWithFirstDigit = true;
            control._digitEntry_CountEnteredZeroes = 0;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// <see cref="TimeSpan"/> value that is editable by this control
        /// </summary>
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(TimeSpan), typeof(TimeSpanControl), new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanControl control = obj as TimeSpanControl;
            control.Hours = ((TimeSpan)e.NewValue).Hours;
            control.Minutes = ((TimeSpan)e.NewValue).Minutes;
            control.Seconds = ((TimeSpan)e.NewValue).Seconds;
            control.Milliseconds = ((TimeSpan)e.NewValue).Milliseconds;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hours of the <see cref="Value"/> (<see cref="TimeSpan.Hours"/>)
        /// </summary>
        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="Hours"/> property.
        /// </summary>
        public static readonly DependencyProperty HoursProperty = DependencyProperty.Register(nameof(Hours), typeof(int), typeof(TimeSpanControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        /// <summary>
        /// Minutes of the <see cref="Value"/> (<see cref="TimeSpan.Minutes"/>)
        /// </summary>
        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="Minutes"/> property.
        /// </summary>
        public static readonly DependencyProperty MinutesProperty = DependencyProperty.Register(nameof(Minutes), typeof(int), typeof(TimeSpanControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        /// <summary>
        /// Seconds of the <see cref="Value"/> (<see cref="TimeSpan.Seconds"/>)
        /// </summary>
        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="Seconds"/> property.
        /// </summary>
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register(nameof(Seconds), typeof(int), typeof(TimeSpanControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        /// <summary>
        /// Milliseconds of the <see cref="Value"/> (<see cref="TimeSpan.Milliseconds"/>)
        /// </summary>
        public int Milliseconds
        {
            get { return (int)GetValue(MillisecondsProperty); }
            set { SetValue(MillisecondsProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="Milliseconds"/> property.
        /// </summary>
        public static readonly DependencyProperty MillisecondsProperty = DependencyProperty.Register(nameof(Milliseconds), typeof(int), typeof(TimeSpanControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanControl control = obj as TimeSpanControl;
            control.Value = new TimeSpan(0, control.Hours, control.Minutes, control.Seconds, control.Milliseconds);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// If true, the controls show the <see cref="Hours"/> field.
        /// </summary>
        public bool HoursVisible
        {
            get { return (bool)GetValue(HoursVisibleProperty); }
            set { SetValue(HoursVisibleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="HoursVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty HoursVisibleProperty = DependencyProperty.Register(nameof(HoursVisible), typeof(bool), typeof(TimeSpanControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnVisiblePartsChanged)));

        /// <summary>
        /// If true, the controls show the <see cref="Milliseconds"/> field.
        /// </summary>
        public bool MillisecondsVisible
        {
            get { return (bool)GetValue(MillisecondsVisibleProperty); }
            set { SetValue(MillisecondsVisibleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="MillisecondsVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty MillisecondsVisibleProperty = DependencyProperty.Register(nameof(MillisecondsVisible), typeof(bool), typeof(TimeSpanControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnVisiblePartsChanged)));

        private static void OnVisiblePartsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanControl control = obj as TimeSpanControl;
            if(!control.HoursVisible && control.EditMode == TimeSpanControlEditModes.EditHours)
            {
                control.EditMode = TimeSpanControlEditModes.EditMinutes;
            }
            else if (!control.MillisecondsVisible && control.EditMode == TimeSpanControlEditModes.EditMilliseconds)
            {
                control.EditMode = TimeSpanControlEditModes.EditSeconds;
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Digit Update Logic

        /// <summary>
        /// Integrate the entered digit into the TimeSpan value.
        /// If it is the first digit after an EditMode change, it is used at the rightmost digit.
        /// For each following digit, the value is multiplied by 10 (shifted one digit to the left) and the digit is added.
        /// If the EditMode is changed automatically when the next digit will produce a number that is too large.
        /// E.g. if Hours = 3 the Edit mode will automatically change to EditMinutes because all following digits would lead to hours > 23.
        /// </summary>
        /// <param name="digit">Digit to handle (0 .. 9)</param>
        private void handleDigitInput(byte digit)
        {
            if(digit > 9) { digit = 9; }

            int oldValueMultiplier = _digitEntry_StartWithFirstDigit ? 0 : 10;
            _digitEntry_StartWithFirstDigit = false;
            if(digit == 0) { _digitEntry_CountEnteredZeroes++; }
            switch (EditMode)
            {
                case TimeSpanControlEditModes.EditHours:
                    Hours = (Hours * oldValueMultiplier) + digit;
                    if (Hours > 2 || _digitEntry_CountEnteredZeroes >= (digit == 0 ? 2 : 1)) { EditMode = TimeSpanControlEditModes.EditMinutes; }
                    break;
                case TimeSpanControlEditModes.EditMinutes:
                    Minutes = (Minutes * oldValueMultiplier) + digit;
                    if (Minutes >= 6 || _digitEntry_CountEnteredZeroes >= (digit == 0 ? 2 : 1)) { EditMode = TimeSpanControlEditModes.EditSeconds; }
                    break;
                case TimeSpanControlEditModes.EditSeconds:
                    Seconds = (Seconds * oldValueMultiplier) + digit;
                    if(MillisecondsVisible)
                    {
                        if (Seconds >= 6 || _digitEntry_CountEnteredZeroes >= (digit == 0 ? 2 : 1)) { EditMode = TimeSpanControlEditModes.EditMilliseconds; }
                    }
                    else
                    {
                        if (Seconds * 10 >= 59 || _digitEntry_CountEnteredZeroes >= 2) { _digitEntry_StartWithFirstDigit = true; _digitEntry_CountEnteredZeroes = 0; }
                    }
                    break;
                case TimeSpanControlEditModes.EditMilliseconds:
                    Milliseconds = (Milliseconds * oldValueMultiplier) + digit;
                    if (Milliseconds * 10 >= 999 || _digitEntry_CountEnteredZeroes >= 3) { _digitEntry_StartWithFirstDigit = true; _digitEntry_CountEnteredZeroes = 0; }
                    break;
            }
        }

        /// <summary>
        /// Increase the digits depending on the current <see cref="EditMode"/>
        /// </summary>
        private void increaseDigit()
        {
            switch (EditMode)
            {
                case TimeSpanControlEditModes.EditHours:
                    if(Hours < 23) { Hours++; }
                    break;
                case TimeSpanControlEditModes.EditMinutes:
                    if(Minutes < 59) { Minutes++; }
                    break;
                case TimeSpanControlEditModes.EditSeconds:
                    if(Seconds < 59) { Seconds++; }
                    break;
                case TimeSpanControlEditModes.EditMilliseconds:
                    if(Milliseconds < 999) { Milliseconds++; }
                    break;
            }
        }

        /// <summary>
        /// Decrease the digits depending on the current <see cref="EditMode"/>
        /// </summary>
        private void decreaseDigit()
        {
            switch (EditMode)
            {
                case TimeSpanControlEditModes.EditHours:
                    if (Hours > 0) { Hours--; }
                    break;
                case TimeSpanControlEditModes.EditMinutes:
                    if (Minutes > 0) { Minutes--; }
                    break;
                case TimeSpanControlEditModes.EditSeconds:
                    if (Seconds > 0) { Seconds--; }
                    break;
                case TimeSpanControlEditModes.EditMilliseconds:
                    if (Milliseconds > 0) { Milliseconds--; }
                    break;
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Control Events

        private void txt_hours_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            EditMode = TimeSpanControlEditModes.EditHours;
            e.Handled = true;   // avoids focus loss by e.g. parent ListViewItem
        }

        private void txt_minutes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            EditMode = TimeSpanControlEditModes.EditMinutes;
            e.Handled = true;   // avoids focus loss by e.g. parent ListViewItem
        }

        private void txt_seconds_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            EditMode = TimeSpanControlEditModes.EditSeconds;
            e.Handled = true;   // avoids focus loss by e.g. parent ListViewItem
        }

        private void txt_milliseconds_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            EditMode = TimeSpanControlEditModes.EditMilliseconds;
            e.Handled = true;   // avoids focus loss by e.g. parent ListViewItem
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                // Return here to enable the default behavior of the Tab key
                return;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                string keyString = e.Key.ToString().Replace("NumPad", "").Replace("D", "");
                byte digit;
                if (byte.TryParse(keyString, out digit))
                {
                    handleDigitInput(digit);
                }
            }
            else if (e.Key == Key.Right)
            {
                TimeSpanControlEditModes highestMode = TimeSpanControlEditModes.EditMilliseconds;
                if(!MillisecondsVisible) { highestMode = TimeSpanControlEditModes.EditSeconds; }
                if (EditMode < highestMode)
                {
                    EditMode++;
                }
            }
            else if (e.Key == Key.Left)
            {
                TimeSpanControlEditModes lowestMode = TimeSpanControlEditModes.EditHours;
                if (!HoursVisible) { lowestMode = TimeSpanControlEditModes.EditMinutes; }
                if (EditMode > lowestMode)
                {
                    EditMode--;
                }
            }
            else if(e.Key == Key.Up)
            {
                increaseDigit();
            }
            else if (e.Key == Key.Down)
            {
                decreaseDigit();
            }
            e.Handled = true;
        }

        private void btn_Increase_Click(object sender, RoutedEventArgs e)
        {
            increaseDigit();
        }

        private void btn_Decrease_Click(object sender, RoutedEventArgs e)
        {
            decreaseDigit();
        }

        #endregion
    }
}
