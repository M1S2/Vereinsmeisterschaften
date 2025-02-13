using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    public class PersonStart : ObservableObject
    {
        private SwimmingStyles _style;
        public SwimmingStyles Style
        {
            get => _style;
            set => SetProperty(ref _style, value);
        }

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private double _score;
        public double Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }
    }
}
