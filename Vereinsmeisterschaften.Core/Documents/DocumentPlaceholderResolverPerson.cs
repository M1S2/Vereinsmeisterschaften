using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="Person"/>.
    /// </summary>
    public class DocumentPlaceholderResolverPerson : DocumentPlaceholderResolverBase<Person>
    {
        /// <summary>
        /// Constructor for a document placeholder resolver.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        public DocumentPlaceholderResolverPerson(IWorkspaceService workspaceService) : base(workspaceService)
        {
        }

        /// <summary>
        /// Take the <see cref="Person"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(Person item)
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder, item.FirstName + " " + item.Name); }
            foreach (string placeholder in Placeholders.Placeholders_FirstName) { textPlaceholder.Add(placeholder, item.FirstName); }
            foreach (string placeholder in Placeholders.Placeholders_LastName) { textPlaceholder.Add(placeholder, item.Name); }
            foreach (string placeholder in Placeholders.Placeholders_Gender) { textPlaceholder.Add(placeholder, EnumCoreToLocalizedString.Convert(item.Gender)); }
            foreach (string placeholder in Placeholders.Placeholders_GenderSymbol) { textPlaceholder.Add(placeholder, item.Gender == Genders.Male ? "♂" : "♀"); }
            foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder, item.BirthYear.ToString() ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder, EnumCoreToLocalizedString.Convert(item.HighestScoreStyle)); }
            foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder, item.HighestScoreCompetition?.Distance.ToString() + "m" ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder, item.HighestScoreCompetition?.Id.ToString() ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_Score) { textPlaceholder.Add(placeholder, item.HighestScore.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_ResultListPlace) { textPlaceholder.Add(placeholder, item?.ResultListPlace == 0 ? "-" : item.ResultListPlace.ToString() ?? "?"); }
            foreach (string placeholder in Placeholders.Placeholders_BestStyle) { textPlaceholder.Add(placeholder, EnumCoreToLocalizedString.Convert(item?.HighestScoreStyle)); }

            string cellEmptyString = "-";
            foreach (string placeholder in Placeholders.Placeholders_ScoreBreaststroke) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Breaststroke)?.Score.ToString() ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreFreestyle) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Freestyle)?.Score.ToString() ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreBackstroke) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Backstroke)?.Score.ToString() ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreButterfly) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Butterfly)?.Score.ToString() ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreMedley) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Medley)?.Score.ToString() ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreWaterflea) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.WaterFlea)?.Score.ToString() ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeBreaststroke) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Breaststroke)?.Time.ToString(@"mm\:ss\.fff") ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeFreestyle) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Freestyle)?.Time.ToString(@"mm\:ss\.fff") ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeBackstroke) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Backstroke)?.Time.ToString(@"mm\:ss\.fff") ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeButterfly) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Butterfly)?.Time.ToString(@"mm\:ss\.fff") ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeMedley) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.Medley)?.Time.ToString(@"mm\:ss\.fff") ?? cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeWaterflea) { textPlaceholder.Add(placeholder, item?.GetStartByStyle(SwimmingStyles.WaterFlea)?.Time.ToString(@"mm\:ss\.fff") ?? cellEmptyString); }

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
            Placeholders.PLACEHOLDER_KEY_SWIMMING_STYLE,
            Placeholders.PLACEHOLDER_KEY_DISTANCE,
            Placeholders.PLACEHOLDER_KEY_COMPETITION_ID,
            Placeholders.PLACEHOLDER_KEY_SCORE,
            Placeholders.PLACEHOLDER_KEY_RESULT_LIST_PLACE,
            Placeholders.PLACEHOLDER_KEY_BEST_STYLE,
            Placeholders.PLACEHOLDER_KEY_SCOREBREASTSTROKE,
            Placeholders.PLACEHOLDER_KEY_SCOREFREESTYLE,
            Placeholders.PLACEHOLDER_KEY_SCOREBACKSTROKE,
            Placeholders.PLACEHOLDER_KEY_SCOREBUTTERFLY,
            Placeholders.PLACEHOLDER_KEY_SCOREMEDLEY,
            Placeholders.PLACEHOLDER_KEY_SCOREWATERFLEA,
            Placeholders.PLACEHOLDER_KEY_TIMEBREASTSTROKE,
            Placeholders.PLACEHOLDER_KEY_TIMEFREESTYLE,
            Placeholders.PLACEHOLDER_KEY_TIMEBACKSTROKE,
            Placeholders.PLACEHOLDER_KEY_TIMEBUTTERFLY,
            Placeholders.PLACEHOLDER_KEY_TIMEMEDLEY,
            Placeholders.PLACEHOLDER_KEY_TIMEWATERFLEA
        };

    }
}
