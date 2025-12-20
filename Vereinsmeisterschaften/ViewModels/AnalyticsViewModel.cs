using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Views.AnalyticsUserControls;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// ViewModel for the analytics page
    /// </summary>
    public partial class AnalyticsViewModel : ObservableObject, INavigationAware
    {        
        /// <summary>
        /// List of available <see cref="IAnalyticsUserControl"/> objects
        /// </summary>
        public IEnumerable<IAnalyticsUserControl> AvailableAnalyticsUserControls { get; }

        /// <summary>
        /// List of normal sized (not maximized) <see cref="IAnalyticsUserControl"/> objects.
        /// When <see cref="IsMaximizedAnalyticsUserControlAvailable"/> is <see langword="true"/> the corresponding user control isn't part of this list anymore.
        /// </summary>
        public IEnumerable<IAnalyticsUserControl> NormalSizedAnalyticsUserControls => AvailableAnalyticsUserControls.Where(c => !c.IsMaximized);

        /// <summary>
        /// Return the first <see cref="IAnalyticsUserControl"/> with the <see cref="IAnalyticsUserControl.IsMaximized"/> flag set.
        /// </summary>
        public IAnalyticsUserControl MaximizedAnalyticsUserControl => AvailableAnalyticsUserControls.Where(c => c.IsMaximized).FirstOrDefault();

        /// <summary>
        /// True, when the <see cref="MaximizedAnalyticsUserControl"/> isn't <see langword="null"/>
        /// </summary>
        public bool IsMaximizedAnalyticsUserControlAvailable => MaximizedAnalyticsUserControl != null;

        /// <summary>
        /// Constructor of the analytics view model
        /// </summary>
        public AnalyticsViewModel(IEnumerable<IAnalyticsUserControl> availableAnalyticsUserControls)
        {
            AvailableAnalyticsUserControls = availableAnalyticsUserControls;
            foreach (IAnalyticsUserControl userControl in AvailableAnalyticsUserControls)
            {
                userControl.IsMaximized = false;
                userControl.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(IAnalyticsUserControl.IsMaximized))
                    {
                        OnPropertyChanged(nameof(NormalSizedAnalyticsUserControls));
                        OnPropertyChanged(nameof(IsMaximizedAnalyticsUserControlAvailable));
                        OnPropertyChanged(nameof(MaximizedAnalyticsUserControl));
                    }
                };
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [ICommand]
        public void RestoreLayout()
        {
            foreach (IAnalyticsUserControl analyticsUserControl in AvailableAnalyticsUserControls)
            {
                analyticsUserControl.IsMaximized = false;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void OnNavigatedFrom()
        {
        }

        /// <inheritdoc/>
        public void OnNavigatedTo(object parameter)
        {
            foreach (IAnalyticsUserControl analyticsUserControl in AvailableAnalyticsUserControls)
            {
                analyticsUserControl.Refresh();
            }
        }
    }
}
