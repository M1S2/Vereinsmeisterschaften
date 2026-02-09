using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views.AnalyticsWidgets
{
    /// <summary>
    /// Interaktionslogik für AnalyticsWidgetCompetitionTimes.xaml
    /// </summary>
    public partial class AnalyticsWidgetCompetitionTimes : AnalyticsWidgetBase
    {
        private AnalyticsModuleCompetitionTimes _analyticsModule => AnalyticsModule as AnalyticsModuleCompetitionTimes;

        public AnalyticsWidgetCompetitionTimes(AnalyticsModuleCompetitionTimes analyticsModule) : base(analyticsModule)
        {
            _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
            InitializeComponent();
        }

        public override string Icon => "\uE916";

        public override void Refresh()
        {
            OnPropertyChanged(nameof(CompetitionTimesPerAgeSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            base.Refresh();
        }

        private List<SwimmingStyles> _availableSwimmingStyles;
        /// <summary>
        /// List with all available <see cref="SwimmingStyles"/> without <see cref="SwimmingStyles.Unknown"/>
        /// </summary>
        public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

        private SwimmingStyles _selectedSwimmingStyle;
        /// <summary>
        /// Selected <see cref="SwimmingStyles"/> that is used to display the chart
        /// </summary>
        public SwimmingStyles SelectedSwimmingStyle
        {
            get => _selectedSwimmingStyle;
            set { _selectedSwimmingStyle = value; OnPropertyChanged(); OnPropertyChanged(nameof(CompetitionTimesPerAgeSeries)); }
        }

        private Genders _selectedGender;
        /// <summary>
        /// Selected <see cref="Genders"/> that is used to display the chart
        /// </summary>
        public Genders SelectedGender
        {
            get => _selectedGender;
            set { _selectedGender = value; OnPropertyChanged(); OnPropertyChanged(nameof(CompetitionTimesPerAgeSeries)); }
        }

        public ISeries[] CompetitionTimesPerAgeSeries
        {
            get
            {
                if (_analyticsModule == null || AnalyticsAvailable == false) return null;

                List<ISeries> seriesList = new List<ISeries>();

                List<AnalyticsModuleCompetitionTimes.ModelCompetitionTimes> competitionTimeModels = _analyticsModule.GetCompetitionTimesPerAge(SelectedGender, SelectedSwimmingStyle);

                var series = new LineSeries<AnalyticsModuleCompetitionTimes.ModelCompetitionTimes>
                {
                    Values = competitionTimeModels,
                    Mapping = (model, index) =>
                    {
                        return new Coordinate(model.Age, model.Time.TotalMilliseconds);
                    },
                    YToolTipLabelFormatter = point =>
                    {
                        string tooltipString = $"{Properties.Resources.AgeString}: {point.Model.Age}{Environment.NewLine}{Properties.Resources.TimeString}: {point.Model.Time.ToString(@"mm\:ss\.ff")}";
                        if(point.Model.IsTimeFromRudolphTable)
                        {
                            tooltipString += $"{Environment.NewLine}{Properties.Resources.ParsedFromRudolphTableString}";
                        }
                        if (point.Model.IsTimeInterpolatedFromRudolphTable)
                        {
                            tooltipString += $"{Environment.NewLine}{Properties.Resources.InterpolatedFromRudolphTableString}";
                        }
                        return tooltipString;
                    },
                    Stroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 4),
                    Fill = new SolidColorPaint(ColorPaintMahAppsAccent.Color.WithAlpha(0x33)),     // modify alpha channel for transparency
                    GeometryStroke = new SolidColorPaint(ColorPaintMahAppsAccent.Color, 2),
                    GeometrySize = 15
                }
                .OnPointMeasured(point =>
                {
                    // assign a color to each point depending on the model flags
                    if (point.Visual is null) return;

                    SolidColorBrush displayColor;
                    if (point.Model.IsTimeFromRudolphTable)
                    {
                        displayColor = Application.Current.Resources["BrushTimeFromRudolphTable"] as SolidColorBrush;
                        point.Visual.Fill = new SolidColorPaint(SKColor.Parse(displayColor.Color.ToString()));
                    }
                    else if(point.Model.IsTimeInterpolatedFromRudolphTable)
                    {
                        displayColor = Application.Current.Resources["BrushTimeInterpolatedFromRudolphTable"] as SolidColorBrush;
                        point.Visual.Fill = new SolidColorPaint(SKColor.Parse(displayColor.Color.ToString()));
                    }
                });
                seriesList.Add(series);
                return seriesList.ToArray();
            }
        }

        public Axis[] XAxes =>
        [
            new Axis
            {
                IsVisible = true,
                Name = Properties.Resources.AgeString,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                MinStep = 1
            }
        ];

        public Axis[] YAxes =>
        [
            new Axis
            {
                IsVisible = true,
                NamePaint = ColorPaintMahAppsText,
                NameTextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                SeparatorsPaint = COLORPAINT_SEPARATORS,
                LabelsPaint = ColorPaintMahAppsText,
                TextSize = ANALYTICS_WIDGET_AXIS_TEXTSIZE_DEFAULT,
                LabelsDensity = 0,
                Labeler = (value) =>
                {
                    return TimeSpan.FromMilliseconds(value).ToString(@"mm\:ss\.ff");
                }
            }
        ];
    }
}
