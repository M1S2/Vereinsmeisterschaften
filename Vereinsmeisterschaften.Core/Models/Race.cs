using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a single race in the complete competition race.
    /// </summary>
    public class Race : ObservableObject
    {
        public ObservableCollection<PersonStart> Starts { get; set; }
        public ObservableCollection<Competition> Competitions { get; set; }

        public SwimmingStyles Style => Starts?.FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;
        public int Distance => Competitions?.FirstOrDefault()?.Distance ?? 0;

        public Race()
        {
            Starts = new ObservableCollection<PersonStart>();
            Competitions = new ObservableCollection<Competition>();
        }

        public Race(List<PersonStart> starts, List<Competition> competitions)
        {
            Starts = new ObservableCollection<PersonStart>(starts);
            Competitions = new ObservableCollection<Competition>(competitions);
        }

        public Race(ObservableCollection<PersonStart> starts, ObservableCollection<Competition> competitions)
        {
            Starts = starts;
            Competitions = competitions;
        }
    }
}
