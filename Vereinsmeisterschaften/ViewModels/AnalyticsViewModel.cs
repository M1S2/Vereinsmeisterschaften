using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// ViewModel for the analytics page
    /// </summary>
    public class AnalyticsViewModel : ObservableObject, INavigationAware
    {
        private IAnalyticsService _analyticsService;

        /// <summary>
        /// Constructor of the analytics view model
        /// </summary>
        /// <param name="anaylticsService"><see cref="IAnalyticsService"/> object</param>
        public AnalyticsViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Gender analytics

        public ISeries[] GenderSeries => new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new []{ _analyticsService.MalePersonPercentage },
                Name = Core.Properties.EnumsCore.Genders_Male,
                Fill = new SolidColorPaint(SKColor.Parse("2986cc")),
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => point.Coordinate.PrimaryValue.ToString("N1") + "%" + Environment.NewLine + Core.Properties.EnumsCore.Genders_Male
            },
            new PieSeries<double>
            {
                Values = new[] { _analyticsService.FemalePersonPercentage },
                Name = Core.Properties.EnumsCore.Genders_Female,
                Fill = new SolidColorPaint(SKColor.Parse("c90076")),
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => point.Coordinate.PrimaryValue.ToString("N1") + "%" + Environment.NewLine + Core.Properties.EnumsCore.Genders_Female
            }
        };

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void OnNavigatedFrom()
        {
        }

        /// <inheritdoc/>
        public void OnNavigatedTo(object parameter)
        {
        }
    }
}
