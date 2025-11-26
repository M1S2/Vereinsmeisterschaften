using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="PersonStart"/>.
    /// </summary>
    public class DocumentPlaceholderResolverPersonStart : DocumentPlaceholderResolverBase<PersonStart>
    {
        /// <summary>
        /// Constructor for a document placeholder resolver.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        public DocumentPlaceholderResolverPersonStart(IWorkspaceService workspaceService) : base(workspaceService)
        {
        }

        /// <summary>
        /// Take the <see cref="PersonStart"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(PersonStart item)
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_Name) { textPlaceholder.Add(placeholder, item.PersonObj?.FirstName + " " + item.PersonObj?.Name); }
            foreach (string placeholder in Placeholders.Placeholders_FirstName) { textPlaceholder.Add(placeholder, item.PersonObj?.FirstName); }
            foreach (string placeholder in Placeholders.Placeholders_LastName) { textPlaceholder.Add(placeholder, item.PersonObj?.Name); }
            foreach (string placeholder in Placeholders.Placeholders_Gender) { textPlaceholder.Add(placeholder, EnumCoreLocalizedStringHelper.Convert(item.PersonObj?.Gender)); }
            foreach (string placeholder in Placeholders.Placeholders_GenderSymbol) { textPlaceholder.Add(placeholder, item.PersonObj?.Gender == Genders.Male ? "♂" : "♀"); }
            foreach (string placeholder in Placeholders.Placeholders_BirthYear) { textPlaceholder.Add(placeholder, item.PersonObj?.BirthYear.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_SwimmingStyle) { textPlaceholder.Add(placeholder, EnumCoreLocalizedStringHelper.Convert(item.Style)); }
            foreach (string placeholder in Placeholders.Placeholders_Distance) { textPlaceholder.Add(placeholder, item.CompetitionObj?.Distance.ToString() + "m"); }
            foreach (string placeholder in Placeholders.Placeholders_CompetitionID) { textPlaceholder.Add(placeholder, item.CompetitionObj?.Id.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_Score) { textPlaceholder.Add(placeholder, item.Score.ToString()); }

            string cellEmptyString = "-";
            ushort numberMillisecondDigits = _workspaceService.Settings.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_TIMEINPUT_NUMBER_MILLISECOND_DIGITS);
            string timeFormat = @"mm\:ss" + (numberMillisecondDigits == 0 ? "" : @"\.") + new string('f', numberMillisecondDigits);
            foreach (string placeholder in Placeholders.Placeholders_ScoreBreaststroke) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Breaststroke ? item?.Score.ToString() : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreFreestyle) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Freestyle ? item?.Score.ToString() : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreBackstroke) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Backstroke ? item?.Score.ToString() : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreButterfly) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Butterfly ? item?.Score.ToString() : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreMedley) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Medley ? item?.Score.ToString() : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_ScoreWaterflea) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.WaterFlea ? item?.Score.ToString() : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeBreaststroke) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Breaststroke ? item?.Time.ToString(timeFormat) : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeFreestyle) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Freestyle ? item?.Time.ToString(timeFormat) : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeBackstroke) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Backstroke ? item?.Time.ToString(timeFormat) : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeButterfly) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Butterfly ? item?.Time.ToString(timeFormat) : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeMedley) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.Medley ? item?.Time.ToString(timeFormat) : cellEmptyString); }
            foreach (string placeholder in Placeholders.Placeholders_TimeWaterflea) { textPlaceholder.Add(placeholder, item?.Style == SwimmingStyles.WaterFlea ? item?.Time.ToString(timeFormat) : cellEmptyString); }

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
