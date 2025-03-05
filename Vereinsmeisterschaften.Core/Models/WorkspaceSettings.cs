using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Models
{
    public class WorkspaceSettings : ObservableObject, IEquatable<WorkspaceSettings>
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

        public WorkspaceSettings()
        {
        }

        public WorkspaceSettings(WorkspaceSettings other)
        {
            CompetitionYear = other?.CompetitionYear ?? 0;
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

        public override bool Equals(object obj)
        {
            WorkspaceSettings other = obj as WorkspaceSettings;
            if (other == null) return false;

            return CompetitionYear.Equals(other.CompetitionYear);
        }

        public bool Equals(WorkspaceSettings other)
        {
            return Equals((object)other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
