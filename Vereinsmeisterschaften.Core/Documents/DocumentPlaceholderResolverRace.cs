using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;
using static Vereinsmeisterschaften.Core.Helpers.DocXPlaceholderHelper;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="Race"/>.
    /// </summary>
    public class DocumentPlaceholderResolverRace : DocumentPlaceholderResolverBase<Race>
    {
        /// <summary>
        /// Constructor for a document placeholder resolver.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        public DocumentPlaceholderResolverRace(IWorkspaceService workspaceService) : base(workspaceService)
        {
        }

        /// <summary>
        /// Take the <see cref="Race"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(Race item)
        {
            ushort numSwimLanes = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES) ?? 3;

            List<string> personBirthYears = item.Starts.Select(s => s.PersonObj?.BirthYear.ToString()).ToList();
            List<string> personCompetitionIDs = item.Starts.Select(s => s.CompetitionObj?.ID.ToString() ?? "?").ToList();
            List<string> personCompleteNames = item.Starts.Select(s => s.PersonObj?.FirstName + " " + s.PersonObj?.Name).ToList();
            List<string> personFirstNames = item.Starts.Select(s => s.PersonObj?.FirstName).ToList();
            List<string> personLastNames = item.Starts.Select(s => s.PersonObj?.Name).ToList();
            List<string> personGenders = item.Starts.Select(s => EnumCoreToLocalizedString.Convert(s.PersonObj?.Gender)).ToList();
            List<string> personGenderSymbols = item.Starts.Select(s => s.PersonObj?.Gender == Genders.Male ? "♂" : "♀").ToList();

            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            for (int i = 0; i < numSwimLanes; i++)
            {
                foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder + (i + 1), personCompleteNames.Count > i ? personCompleteNames[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_FirstName) { textPlaceholder.Add(placeholder + (i + 1), personFirstNames.Count > i ? personFirstNames[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_LastName) { textPlaceholder.Add(placeholder + (i + 1), personLastNames.Count > i ? personLastNames[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_Gender) { textPlaceholder.Add(placeholder + (i + 1), personGenders.Count > i ? personGenders[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_GenderSymbol) { textPlaceholder.Add(placeholder + (i + 1), personGenderSymbols.Count > i ? personGenderSymbols[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder + (i + 1), personBirthYears.Count > i ? personBirthYears[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder + (i + 1), personCompetitionIDs.Count > i ? personCompetitionIDs[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder + (i + 1), item.Distance.ToString() + "m"); }
                foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder + (i + 1), EnumCoreToLocalizedString.Convert(item.Style)); }
            }
            foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder, string.Join(", ", personCompleteNames)); }
            foreach (string placeholder in Placeholders.Placeholders_FirstName) { textPlaceholder.Add(placeholder, string.Join(", ", personFirstNames)); }
            foreach (string placeholder in Placeholders.Placeholders_LastName) { textPlaceholder.Add(placeholder, string.Join(", ", personLastNames)); }
            foreach (string placeholder in Placeholders.Placeholders_Gender) { textPlaceholder.Add(placeholder, string.Join(", ", personGenders)); }
            foreach (string placeholder in Placeholders.Placeholders_GenderSymbol) { textPlaceholder.Add(placeholder, string.Join(", ", personGenderSymbols)); }
            foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder, string.Join(", ", personBirthYears)); }
            foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder, string.Join(", ", personCompetitionIDs)); }
            foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder, item.Distance.ToString() + "m"); }
            foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder, EnumCoreToLocalizedString.Convert(item.Style)); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public override List<string> SupportedPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_NAME,
            Placeholders.PLACEHOLDER_KEY_FIRSTNAME,
            Placeholders.PLACEHOLDER_KEY_LASTNAME,
            Placeholders.PLACEHOLDER_KEY_GENDER,
            Placeholders.PLACEHOLDER_KEY_GENDER_SYMBOL,
            Placeholders.PLACEHOLDER_KEY_BIRTH_YEAR,
            Placeholders.PLACEHOLDER_KEY_COMPETITION_ID,
            Placeholders.PLACEHOLDER_KEY_DISTANCE,
            Placeholders.PLACEHOLDER_KEY_SWIMMING_STYLE,
        };

        /// <inheritdoc/>
        public override List<int> PostfixNumbersSupported
        {
            get
            {
                ushort numSwimLanes = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES) ?? 3;
                return Enumerable.Repeat((int)numSwimLanes, SupportedPlaceholderKeys.Count).ToList();
            }
        }

    }
}
