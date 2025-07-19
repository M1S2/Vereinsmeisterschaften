using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="PersonStart"/>.
    /// </summary>
    public class DocumentPlaceholderResolverPersonStart : DocumentPlaceholderResolverBase<PersonStart>
    {
        /// <summary>
        /// Take the <see cref="PersonStart"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(PersonStart item, IWorkspaceService workspaceService)
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder, item.PersonObj?.FirstName + " " + item.PersonObj?.Name); }
            foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder, item.PersonObj?.BirthYear.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder, EnumCoreToLocalizedString.Convert(item.Style)); }
            foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder, item.CompetitionObj?.Distance.ToString() + "m"); }
            foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder, item.CompetitionObj?.ID.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_Score) { textPlaceholder.Add(placeholder, item.Score.ToString("F2")); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public override List<string> SupportedPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_NAME,
            Placeholders.PLACEHOLDER_KEY_BIRTH_YEAR,
            Placeholders.PLACEHOLDER_KEY_SWIMMING_STYLE,
            Placeholders.PLACEHOLDER_KEY_DISTANCE,
            Placeholders.PLACEHOLDER_KEY_COMPETITION_ID,
            Placeholders.PLACEHOLDER_KEY_SCORE
        };
    }
}
