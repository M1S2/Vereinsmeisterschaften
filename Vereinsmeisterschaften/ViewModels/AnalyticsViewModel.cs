using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Views.AnalyticsWidgets;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// ViewModel for the analytics page
    /// </summary>
    public partial class AnalyticsViewModel : ObservableObject, INavigationAware
    {        
        /// <summary>
        /// List of available <see cref="IAnalyticsWidget"/> objects
        /// </summary>
        public IEnumerable<IAnalyticsWidget> AvailableAnalyticsWidgets { get; }

        /// <summary>
        /// List of normal sized (not maximized) <see cref="IAnalyticsWidget"/> objects.
        /// When <see cref="IsMaximizedAnalyticsWidgetAvailable"/> is <see langword="true"/> the corresponding user control isn't part of this list anymore.
        /// </summary>
        public IEnumerable<IAnalyticsWidget> NormalSizedAnalyticsWidgets => AvailableAnalyticsWidgets.Where(c => !c.IsMaximized);

        /// <summary>
        /// Return the first <see cref="IAnalyticsWidget"/> with the <see cref="IAnalyticsWidget.IsMaximized"/> flag set.
        /// </summary>
        public IAnalyticsWidget MaximizedAnalyticsWidget => AvailableAnalyticsWidgets.Where(c => c.IsMaximized).FirstOrDefault();

        /// <summary>
        /// True, when the <see cref="MaximizedAnalyticsUserControl"/> isn't <see langword="null"/>
        /// </summary>
        public bool IsMaximizedAnalyticsWidgetAvailable => MaximizedAnalyticsWidget != null;

        /// <summary>
        /// Constructor of the analytics view model
        /// </summary>
        /// <param name="availableAnalyticsWidgets">list with all available <see cref="IAnalyticsWidget"/> instances</param>
        public AnalyticsViewModel(IEnumerable<IAnalyticsWidget> availableAnalyticsWidgets)
        {
            AvailableAnalyticsWidgets = availableAnalyticsWidgets;
            foreach (IAnalyticsWidget widget in AvailableAnalyticsWidgets)
            {
                widget.IsMaximized = false;
                widget.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(IAnalyticsWidget.IsMaximized))
                    {
                        OnPropertyChanged(nameof(NormalSizedAnalyticsWidgets));
                        OnPropertyChanged(nameof(IsMaximizedAnalyticsWidgetAvailable));
                        OnPropertyChanged(nameof(MaximizedAnalyticsWidget));
                    }
                };
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        [ICommand]
        public void RestoreLayout()
        {
            foreach (IAnalyticsWidget analyticsWidget in AvailableAnalyticsWidgets)
            {
                analyticsWidget.IsMaximized = false;
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
            foreach (IAnalyticsWidget analyticsWidget in AvailableAnalyticsWidgets)
            {
                analyticsWidget.Refresh();
            }
        }
    }
}
