using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// ViewModel for a group of friends.
    /// </summary>
    public partial class FriendGroupViewModel : ObservableObject
    {
        /// <summary>
        /// ID of the friend group.
        /// </summary>
        [ObservableProperty]
        private int _groupId;

        /// <summary>
        /// List of friends in the friend group.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Person> _friends = new ObservableCollection<Person>();

        /// <summary>
        /// List of available friends for the friend group.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Person> _availableFriends = new ObservableCollection<Person>();
    }
}
