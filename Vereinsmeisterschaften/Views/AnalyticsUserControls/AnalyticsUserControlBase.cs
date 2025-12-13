using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Vereinsmeisterschaften.Views.AnalyticsUserControls
{
    public abstract class AnalyticsUserControlBase : UserControl, IAnalyticsUserControl, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public abstract string Title { get; }
        /// <inheritdoc/>
        public virtual string Icon { get; } = "\uE9D2";
        /// <inheritdoc/>
        public virtual Geometry IconGeometry { get; } = null;
        /// <inheritdoc/>
        public virtual double AnalyticsModuleWidth { get; } = ANALYTICS_WIDTH_DEFAULT;
        /// <inheritdoc/>
        public virtual double AnalyticsModuleHeight { get; } = ANALYTICS_HEIGHT_DEFAULT;

        /// <inheritdoc/>
        public virtual void Refresh() { }
        
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Fixed colors

        public static readonly SolidColorPaint COLORPAINT_MALE = new SolidColorPaint(SKColor.Parse("2986cc"));
        public static readonly SolidColorPaint COLORPAINT_FEMALE = new SolidColorPaint(SKColor.Parse("c90076"));
        public static readonly SolidColorPaint COLORPAINT_SEPARATORS = new SolidColorPaint(SKColor.Parse("dcdcdc"));

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Fixed sizes

        public static readonly double ANALYTICS_WIDTH_DEFAULT = 400;
        public static readonly double ANALYTICS_HEIGHT_DEFAULT = 275;
        public static readonly double ANALYTICS_LEGEND_TEXTSIZE_DEFAULT = 14;
        public static readonly double ANALYTICS_AXIS_TEXTSIZE_DEFAULT = 16;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Theme colors

        /// <summary>
        /// <see cref="SolidColorPaint"/> that represents the MahApps.Brushes.Text brush.
        /// This can be used as color in charts
        /// </summary>
        public SolidColorPaint ColorPaintMahAppsText
        {
            get
            {
                Brush textBrush = (Brush)ThemeManager.Current.DetectTheme(Application.Current).Resources["MahApps.Brushes.Text"];
                string textBrushString = (string)new BrushConverter().ConvertTo(textBrush, typeof(string));
                return new SolidColorPaint(SKColor.Parse(textBrushString));
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor of the <see cref="AnalyticsUserControlBase"/>
        /// </summary>
        public AnalyticsUserControlBase()
        {
            ThemeManager.Current.ThemeChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(ColorPaintMahAppsText));
                Refresh();
            };
        }
    }
}
