using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
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
        /// Take the <see cref="Race"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(Race item, IWorkspaceService workspaceService)
        {
            ushort numSwimLanes = workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES) ?? 3;

            List<string> personBirthYears = item.Starts.Select(s => s.PersonObj?.BirthYear.ToString()).ToList();
            List<string> personCompetitionIDs = item.Starts.Select(s => s.CompetitionObj?.ID.ToString() ?? "?").ToList();
            List<string> personNames = item.Starts.Select(s => s.PersonObj?.FirstName + " " + s.PersonObj?.Name).ToList();
            int maxPersonsInRace = 0;
            maxPersonsInRace = Math.Max(maxPersonsInRace, personNames.Count);

            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            for (int i = 0; i < Math.Max(numSwimLanes, maxPersonsInRace); i++)
            {
                foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder + (i + 1), personNames.Count > i ? personNames[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder + (i + 1), personBirthYears.Count > i ? personBirthYears[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder + (i + 1), personCompetitionIDs.Count > i ? personCompetitionIDs[i] : ""); }
                foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder + (i + 1), item.Distance.ToString() + "m"); }
                foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder + (i + 1), EnumCoreToLocalizedString.Convert(item.Style)); }
            }
            foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder, string.Join(", ", personNames)); }
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
            Placeholders.PLACEHOLDER_KEY_BIRTH_YEAR,
            Placeholders.PLACEHOLDER_KEY_COMPETITION_ID,
            Placeholders.PLACEHOLDER_KEY_DISTANCE,
            Placeholders.PLACEHOLDER_KEY_SWIMMING_STYLE,
        };
    }
}
