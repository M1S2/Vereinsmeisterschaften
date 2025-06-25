using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using static System.Formats.Asn1.AsnWriter;
using static Vereinsmeisterschaften.Core.Helpers.DocXPlaceholderHelper;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="Person"/>.
    /// </summary>
    public class DocumentPlaceholderResolverPerson : DocumentPlaceholderResolverBase<Person>
    {
        /// <summary>
        /// Take the <see cref="Person"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(Person item, IWorkspaceService workspaceService)
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder, item.FirstName + " " + item.Name); }
            foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder, item.BirthYear.ToString() ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder, EnumCoreToLocalizedString.Convert(item.HighestScoreStyle)); }
            foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder, item.HighestScoreCompetition?.Distance.ToString() + "m" ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder, item.HighestScoreCompetition?.ID.ToString() ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_Score) { textPlaceholder.Add(placeholder, item.HighestScore.ToString("F2")); }
            foreach (string placeholder in Placeholders.Placeholders_ResultListPlace) { textPlaceholder.Add(placeholder, item?.ResultListPlace == 0 ? "-" : item.ResultListPlace.ToString() ?? "?"); }
            return textPlaceholder;
        }
    }
}
