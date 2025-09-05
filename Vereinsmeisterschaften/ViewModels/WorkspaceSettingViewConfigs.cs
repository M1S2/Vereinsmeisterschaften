using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Class holding the configuration for workspace settings views.
    /// </summary>
    public static class WorkspaceSettingViewConfigs
    {
        /// <summary>
        /// Dictionary mapping group keys to their labels for display in the UI.
        /// </summary>
        public static Dictionary<string, string> GroupKeyLabelsDict = new Dictionary<string, string>()
        {
            { WorkspaceSettings.GROUP_GENERAL, Resources.SettingsGeneralString },
            { WorkspaceSettings.GROUP_RACE_CALCULATION, Resources.SettingsRacesVariantsCalculationString },
            { WorkspaceSettings.GROUP_DOCUMENT_CREATION, Resources.SettingsDocumentCreationString }
        };

        /// <summary>
        /// Dictionary mapping setting keys to their configuration for display in the UI.
        /// </summary>
        public static Dictionary<string, WorkspaceSettingViewConfig> SettingKeyConfigDict = new Dictionary<string, WorkspaceSettingViewConfig>()
        {
            {
                WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR,
                new WorkspaceSettingViewConfig() { Label=Resources.CompetitionYearString, Tooltip = Tooltips.TooltipCompetitionYear, Icon = "\uE787", Editor = WorkspaceSettingEditorTypes.Numeric, SupportResetToDefault = false }
            },
            // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES,
                new WorkspaceSettingViewConfig() { Label=Resources.NumberOfSwimLanesString, Tooltip = Tooltips.TooltipNumberOfSwimLanes, Icon = "\uE9E9", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_NUM_RACE_VARIANTS_AFTER_CALCULATION,
                new WorkspaceSettingViewConfig() { Label=Resources.NumberRacesVariantsAfterCalculationString, Tooltip = Tooltips.TooltipNumberRacesVariantsAfterCalculation, Icon = "\uE7C1", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_MAX_CALCULATION_LOOPS,
                new WorkspaceSettingViewConfig() { Label=Resources.MaxRacesVariantCalculationLoopsString, Tooltip = Tooltips.TooltipMaxRacesVariantCalculationLoops, Icon = "\uE895", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_MIN_RACES_VARIANTS_SCORE,
                new WorkspaceSettingViewConfig() { Label=Resources.MinimumRacesVariantsScoreString, Tooltip = Tooltips.TooltipMinRacesVariantsScore, Icon = "\uEDE1", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_SINGLE_STARTS,
                new WorkspaceSettingViewConfig() { Label=Resources.WeightSingleStartsString, Tooltip = Tooltips.TooltipWeightSingleStarts, Icon = "\uE8CB", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_SAME_STYLE_SEQUENCE,
                new WorkspaceSettingViewConfig() { Label=Resources.WeightSameStyleSequenceString, Tooltip = Tooltips.TooltipWeightSameStyleSequence, Icon = "\uE8CB", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_PERSON_START_PAUSES,
                new WorkspaceSettingViewConfig() { Label=Resources.WeightPersonStartPausesString, Tooltip = Tooltips.TooltipWeightPersonStartPauses, Icon = "\uE8CB", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_STYLE_ORDER,
                new WorkspaceSettingViewConfig() { Label=Resources.WeightStyleOrderString, Tooltip = Tooltips.TooltipWeightStyleOrder, Icon = "\uE8CB", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_WEIGHT_START_GENDERS,
                new WorkspaceSettingViewConfig() { Label=Resources.WeightStartGendersString, Tooltip = Tooltips.TooltipWeightStartGenders, Icon = "\uE8CB", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_BREASTSTROKE,
                new WorkspaceSettingViewConfig() { Label=Resources.PriorityStyleBreaststrokeString, Tooltip = Tooltips.TooltipPriorityStyleBreaststroke, Icon = "\uE8FD", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_FREESTYLE,
                new WorkspaceSettingViewConfig() { Label=Resources.PriorityStyleFreestyleString, Tooltip = Tooltips.TooltipPriorityStyleFreestyle, Icon = "\uE8FD", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_BACKSTROKE,
                new WorkspaceSettingViewConfig() { Label=Resources.PriorityStyleBackstrokeString, Tooltip = Tooltips.TooltipPriorityStyleBackstroke, Icon = "\uE8FD", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_BUTTERFLY,
                new WorkspaceSettingViewConfig() { Label=Resources.PriorityStyleButterflyString, Tooltip = Tooltips.TooltipPriorityStyleButterfly, Icon = "\uE8FD", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_MEDLEY,
                new WorkspaceSettingViewConfig() { Label=Resources.PriorityStyleMedleyString, Tooltip = Tooltips.TooltipPriorityStyleMedley, Icon = "\uE8FD", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            {
                WorkspaceSettings.SETTING_RACE_CALCULATION_PRIORITY_STYLE_WATERFLEA,
                new WorkspaceSettingViewConfig() { Label=Resources.PriorityStyleWaterfleaString, Tooltip = Tooltips.TooltipPriorityStyleWaterflea, Icon = "\uE8FD", Editor = WorkspaceSettingEditorTypes.Numeric }
            },
            // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_PLACEHOLDER_MARKER,
                new WorkspaceSettingViewConfig() { Label=Resources.PlaceholderMarkerString, Tooltip = Tooltips.TooltipPlaceholderMarker, Icon = "\uE94C", Editor = WorkspaceSettingEditorTypes.String }
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_TEMPLATE_FILENAME_POSTFIX,
                new WorkspaceSettingViewConfig() { Label=Resources.TemplateFilenamePostfixString, Tooltip = Tooltips.TooltipTemplateFilenamePostfix, Icon = "\uE75D", Editor = WorkspaceSettingEditorTypes.String }
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_FILE_TYPES,
                new WorkspaceSettingViewConfig() { Label=Resources.DocumentFileTypesString, Tooltip = Tooltips.TooltipDocumentFileTypes, Icon = "\uEA90", Editor = WorkspaceSettingEditorTypes.Enum }
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER,
                new WorkspaceSettingViewConfig() { Label=Resources.DocumentOutputFolderString, Tooltip = Tooltips.TooltipDocumentOutputFolder, Icon = "\uED25", Editor = WorkspaceSettingEditorTypes.FolderRelative, SupportResetToDefault = false }
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.CertificateTemplatePathString, Tooltip = Tooltips.TooltipTemplatePathCertificate, Icon = "\uE8A5", Editor = WorkspaceSettingEditorTypes.FileDocxRelative, SupportResetToDefault = false }
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.OverviewListTemplatePathString, Tooltip = Tooltips.TooltipTemplatePathOverviewList, Icon = "\uE9D5", Editor = WorkspaceSettingEditorTypes.FileDocxRelative, SupportResetToDefault = false}
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_RACE_START_LIST_TEMPLATE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.RaceStartListTemplatePathString, Tooltip = Tooltips.TooltipTemplatePathRaceStartList, Icon = "\uE7C1", Editor = WorkspaceSettingEditorTypes.FileDocxRelative, SupportResetToDefault = false}
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_TIME_FORMS_TEMPLATE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.TimeFormsTemplatePathString, Tooltip = Tooltips.TooltipTemplatePathTimeForms, Icon = "\uE916", Editor = WorkspaceSettingEditorTypes.FileDocxRelative, SupportResetToDefault = false}
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_RESULT_LIST_TEMPLATE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.ResultListTemplatePathString, Tooltip = Tooltips.TooltipTemplatePathResultList, Icon = "\uE9F9", Editor = WorkspaceSettingEditorTypes.FileDocxRelative, SupportResetToDefault = false}
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_RESULT_LIST_DETAIL_TEMPLATE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.ResultListDetailTemplatePathString, Tooltip = Tooltips.TooltipTemplatePathResultListDetail, Icon = "\uE7B3", Editor = WorkspaceSettingEditorTypes.FileDocxRelative, SupportResetToDefault = false}
            },
            {
                WorkspaceSettings.SETTING_DOCUMENT_CREATION_LIBRE_OFFICE_PATH,
                new WorkspaceSettingViewConfig() { Label=Resources.LibreOfficePathString, Tooltip = Tooltips.TooltipLibreOfficePath, Icon = "\uE756", Editor = WorkspaceSettingEditorTypes.FileAbsolute }
            }
        };

    }
}
