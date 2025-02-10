using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Models
{
    public class WorkspaceSettings : ObservableObject
    {
        private ushort _competitionYear = 0;
        /// <summary>
        /// Year in which the competition takes place
        /// </summary>
        public ushort CompetitionYear
        {
            get => _competitionYear;
            set => SetProperty(ref _competitionYear, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static void SetPropertyFromString(WorkspaceSettings dataObj, string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(CompetitionYear): dataObj.CompetitionYear = ushort.Parse(value); break;
                default: break;
            }
        }
    }
}
