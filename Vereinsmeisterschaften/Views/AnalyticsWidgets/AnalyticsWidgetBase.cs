using ControlzEx.Theming;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    public abstract class AnalyticsWidgetBase : UserControl, IAnalyticsWidget
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

        /// <summary>
        /// Title used for the diagram of the analytics user control.
        /// You can override this to assign a custom title. Or add a resource to the Resources.resx with the format: %ClassNameOfTheWidget%Title (e.g. AnalyticsWidgetAgeDistributionTitle)
        /// </summary>
        public virtual string Title
        {
            get
            {
                ResourceManager rm = new ResourceManager(typeof(Properties.Resources));
                return rm.GetString($"{this.GetType().Name}Title") ?? "?";
            }
        }

        /// <inheritdoc/>
        public virtual string Icon { get; } = "\uE9D2";

        /// <summary>
        /// Info text for this analytics.
        /// You can override this to assign a custom tooltip. Or add a resource to the Tooltips.resx with the format: Tooltip%ClassNameOfTheWidget% (e.g. TooltipAnalyticsWidgetAgeDistribution)
        /// </summary>
        public virtual string Info
        {
            get
            {
                ResourceManager rm = new ResourceManager(typeof(Properties.Tooltips));
                return rm.GetString($"Tooltip{this.GetType().Name}") ?? "?";
            }
        }

        /// <inheritdoc/>
        public virtual Geometry IconGeometry { get; } = null;
        
        /// <inheritdoc/>
        public virtual double NormalAnalyticsWidgetWidth { get; } = ANALYTICS_WIDGET_WIDTH_NORMAL;
        /// <inheritdoc/>
        public virtual double NormalAnalyticsWidgetHeight { get; } = ANALYTICS_WIDGET_HEIGHT_NORMAL;

        /// <inheritdoc/>
        public double CurrentAnalyticsWidgetWidth => IsMaximized ? double.NaN : NormalAnalyticsWidgetWidth;     // use NaN to fill up the complete space

        /// <inheritdoc/>
        public double CurrentAnalyticsWidgetHeight => IsMaximized ? double.NaN : NormalAnalyticsWidgetHeight;   // use NaN to fill up the complete space

        private bool _isMaximized = false;
        /// <inheritdoc/>
        public bool IsMaximized
        {
            get => _isMaximized;
            set
            {
                _isMaximized = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentAnalyticsWidgetWidth)); 
                OnPropertyChanged(nameof(CurrentAnalyticsWidgetHeight)); 
            }
        }

        /// <inheritdoc/>
        public virtual void Refresh()
        {
            OnPropertyChanged(nameof(AnalyticsAvailable));
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Analytics Module

        /// <summary>
        /// Analytics module used by this user control
        /// </summary>
        public IAnalyticsModule AnalyticsModule { get; }

        /// <summary>
        /// Indicating if the analytics module has data
        /// </summary>
        public bool AnalyticsAvailable => AnalyticsModule?.AnalyticsAvailable ?? false;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Fixed colors

        public static readonly SolidColorPaint COLORPAINT_MALE = new SolidColorPaint(SKColor.Parse("2986cc"));
        public static readonly SolidColorPaint COLORPAINT_FEMALE = new SolidColorPaint(SKColor.Parse("c90076"));
        public static readonly SolidColorPaint COLORPAINT_SEPARATORS = new SolidColorPaint(SKColor.Parse("dcdcdc"));

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Fixed sizes

        public static readonly double ANALYTICS_WIDGET_WIDTH_NORMAL = 400;
        public static readonly double ANALYTICS_WIDGET_WIDTH_SMALL = 200;
        public static readonly double ANALYTICS_WIDGET_HEIGHT_NORMAL = 275;
        public static readonly double ANALYTICS_WIDGET_LEGEND_TEXTSIZE_DEFAULT = 14;
        public static readonly double ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT = 16;

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

        /// <summary>
        /// <see cref="SolidColorPaint"/> that represents the MahApps.Brushes.Accent brush.
        /// This can be used as color in charts
        /// </summary>
        public SolidColorPaint ColorPaintMahAppsAccent
        {
            get
            {
                Brush accentBrush = (Brush)ThemeManager.Current.DetectTheme(Application.Current).Resources["MahApps.Brushes.Accent"];
                string accentBrushString = (string)new BrushConverter().ConvertTo(accentBrush, typeof(string));
                return new SolidColorPaint(SKColor.Parse(accentBrushString));
            }
        }
        /// <summary>
        /// <see cref="SolidColorPaint"/> that represents the MahApps.Brushes.ThemeBackground brush.
        /// This can be used as color in charts
        /// </summary>
        public SolidColorPaint ColorPaintMahAppsBackground
        {
            get
            {
                Brush backgroundBrush = (Brush)ThemeManager.Current.DetectTheme(Application.Current).Resources["MahApps.Brushes.ThemeBackground"];
                string backgroundBrushString = (string)new BrushConverter().ConvertTo(backgroundBrush, typeof(string));
                return new SolidColorPaint(SKColor.Parse(backgroundBrushString));
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor of the <see cref="AnalyticsWidgetBase"/>
        /// </summary>
        public AnalyticsWidgetBase(IAnalyticsModule analyticsModule)
        {
            AnalyticsModule = analyticsModule;
            ThemeManager.Current.ThemeChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(ColorPaintMahAppsText));
                OnPropertyChanged(nameof(ColorPaintMahAppsAccent));
                OnPropertyChanged(nameof(ColorPaintMahAppsBackground));
                Refresh();
            };
        }
    }
}
