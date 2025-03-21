using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a combination of all single races.
    /// </summary>
    public class CompetitionRaces : ObservableObject
    {
        public ObservableCollection<Race> Races { get; set; }

        public CompetitionRaces()
        {
            Races = new ObservableCollection<Race>();
        }

        public CompetitionRaces(List<Race> races)
        {
            Races = new ObservableCollection<Race>(races);
        }

        public CompetitionRaces(ObservableCollection<Race> races)
        {
            Races = races;
        }

    }
}
