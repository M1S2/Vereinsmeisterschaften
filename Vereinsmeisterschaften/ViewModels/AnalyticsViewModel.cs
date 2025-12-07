using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Views.AnalyticsUserControls;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// ViewModel for the analytics page
    /// </summary>
    public class AnalyticsViewModel : ObservableObject, INavigationAware
    {        
        public IEnumerable<IAnalyticsUserControl> AnalyticsUserControls { get; set; }

        /// <summary>
        /// Constructor of the analytics view model
        /// </summary>
        public AnalyticsViewModel(IEnumerable<IAnalyticsUserControl> analyticsUserControls)
        {
            AnalyticsUserControls = analyticsUserControls;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void OnNavigatedFrom()
        {
        }

        /// <inheritdoc/>
        public void OnNavigatedTo(object parameter)
        {
            foreach (IAnalyticsUserControl analyticsUserControl in AnalyticsUserControls)
            {
                analyticsUserControl.Refresh();
            }
        }
    }
}
