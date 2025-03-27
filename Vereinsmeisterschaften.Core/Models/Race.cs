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

        public SwimmingStyles Style => Starts?.FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;
        public int Distance => Starts?.FirstOrDefault()?.CompetitionObj?.Distance ?? 0;

        public Race()
        {
            Starts = new ObservableCollection<PersonStart>();
        }

        public Race(List<PersonStart> starts)
        {
            Starts = starts == null ? null : new ObservableCollection<PersonStart>(starts);
        }

        public Race(ObservableCollection<PersonStart> starts)
        {
            Starts = starts;
        }
    }
}
