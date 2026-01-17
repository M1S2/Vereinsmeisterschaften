using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Settings
{
    /// <summary>
    /// Class holding all workspace settings
    /// </summary>
    public class WorkspaceSettings : ObservableObject, IEquatable<WorkspaceSettings>, ICloneable
    {
        #region Group / Setting keys

        public const string GROUP_GENERAL = "General";
        public const string SETTING_GENERAL_COMPETITIONYEAR = "CompetitionYear";
        public const string SETTING_GENERAL_COMPETITIONDATE = "CompetitionDate";
        public const string SETTING_GENERAL_COMPETITIONSEARCHMODE = "CompetitionSearchMode";
        public const string SETTING_GENERAL_TIMEINPUT_NUMBER_MILLISECOND_DIGITS = "TimeInputMillisecondDigits";
        public const string SETTING_GENERAL_SCORE_FRACTIONAL_DIGITS = "ScoreFractionalDigits";

        public const string GROUP_RACE_CALCULATION = "RaceCalculation";
        public const string SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES = "NumberOfSwimLanes";
        public const string SETTING_RACE_CALCULATION_NUM_RACE_VARIANTS_AFTER_CALCULATION = "NumberRacesVariantsAfterCalculation";
        public const string SETTING_RACE_CALCULATION_MAX_CALCULATION_LOOPS = "MaxRacesVariantCalculationLoops";
        public const string SETTING_RACE_CALCULATION_MIN_RACES_VARIANTS_SCORE = "MinRacesVariantsScore";
        public const string SETTING_RACE_CALCULATION_WEIGHT_SINGLE_STARTS = "WeightSingleStarts";
        public const string SETTING_RACE_CALCULATION_WEIGHT_SAME_STYLE_SEQUENCE = "WeightSameStyleSequence";
        public const string SETTING_RACE_CALCULATION_WEIGHT_PERSON_START_PAUSES = "WeightPersonStartPauses";
        public const string SETTING_RACE_CALCULATION_WEIGHT_STYLE_ORDER = "WeightStyleOrder";
        public const string SETTING_RACE_CALCULATION_WEIGHT_START_GENDERS = "WeightStartGenders";
        public const string SETTING_RACE_CALCULATION_WEIGHT_FRIENDSHIP = "WeightFriendship";
        public const string SETTING_RACE_CALCULATION_SHORT_PAUSE_THRESHOLD = "ShortPauseThreshold";
        public const string SETTING_RACE_CALCULATION_PRIORITY_STYLE_BREASTSTROKE = "PriorityStyleBreaststroke";
        public const string SETTING_RACE_CALCULATION_PRIORITY_STYLE_FREESTYLE = "PriorityStyleFreestyle";
        public const string SETTING_RACE_CALCULATION_PRIORITY_STYLE_BACKSTROKE = "PriorityStyleBackstroke";
        public const string SETTING_RACE_CALCULATION_PRIORITY_STYLE_BUTTERFLY = "PriorityStyleButterfly";
        public const string SETTING_RACE_CALCULATION_PRIORITY_STYLE_MEDLEY = "PriorityStyleMedley";
        public const string SETTING_RACE_CALCULATION_PRIORITY_STYLE_WATERFLEA = "PriorityStyleWaterflea";

        public const string GROUP_DOCUMENT_CREATION = "DocumentCreation";
        public const string SETTING_DOCUMENT_CREATION_PLACEHOLDER_MARKER = "PlaceholderMarker";
        public const string SETTING_DOCUMENT_CREATION_TEMPLATE_FILENAME_POSTFIX = "TemplateFilenamePostfix";
        public const string SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER = "DocumentOutputFolder";
        public const string SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH = "CertificateTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH = "OverviewlistTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_RACE_START_LIST_TEMPLATE_PATH = "RaceStartListTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_TIME_FORMS_TEMPLATE_PATH = "TimeFormsTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_RESULT_LIST_TEMPLATE_PATH = "ResultListTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_RESULT_LIST_DETAIL_TEMPLATE_PATH = "ResultListDetailTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_ANALYTICS_TEMPLATE_PATH = "AnalyticsTemplatePath";
        public const string SETTING_DOCUMENT_CREATION_FILE_TYPES = "DocumentFileTypes";

        //... Add new setting keys and group keys here ...

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Init Settings

        /// <summary>
        /// Folder name where the document templates are located per default (e.g. "Templates").
        /// </summary>
        public const string DEFAULT_TEMPLATE_FOLDER_NAME = "Templates";

        /// <summary>
        /// Make sure that all necessary groups and settings are available and create them is not existing
        /// </summary>
        private void initializeSettings()
        {
            // Initialize groups if not already initialized

            // +++++ Group General +++++
            WorkspaceSettingsGroup groupGeneral = GetGroup(GROUP_GENERAL, true);
            groupGeneral.MakeSureSettingExists<ushort>(SETTING_GENERAL_COMPETITIONYEAR, 0, 1900, 3000);
            groupGeneral.MakeSureSettingExists<DateTime>(SETTING_GENERAL_COMPETITIONDATE, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), new DateTime(3000, 12, 31));
            groupGeneral.MakeSureSettingExists<CompetitionSearchModes>(SETTING_GENERAL_COMPETITIONSEARCHMODE, CompetitionSearchModes.ExactOrNextLowerOnlyMaxAge);
            groupGeneral.MakeSureSettingExists<ushort>(SETTING_GENERAL_TIMEINPUT_NUMBER_MILLISECOND_DIGITS, 2, 1, 3);
            groupGeneral.MakeSureSettingExists<ushort>(SETTING_GENERAL_SCORE_FRACTIONAL_DIGITS, 1, 0, 5);

            // +++++ Group Race Calculation +++++
            WorkspaceSettingsGroup groupRaceCalculation = GetGroup(GROUP_RACE_CALCULATION, true);
            groupRaceCalculation.MakeSureSettingExists<ushort>(SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES, 3, 0, 10);
            groupRaceCalculation.MakeSureSettingExists<ushort>(SETTING_RACE_CALCULATION_NUM_RACE_VARIANTS_AFTER_CALCULATION, 100, 1, 1000);
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_MAX_CALCULATION_LOOPS, 1000000, 1, int.MaxValue);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_MIN_RACES_VARIANTS_SCORE, 90.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_WEIGHT_SINGLE_STARTS, 15.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_WEIGHT_SAME_STYLE_SEQUENCE, 5.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_WEIGHT_PERSON_START_PAUSES, 60.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_WEIGHT_STYLE_ORDER, 10.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_WEIGHT_START_GENDERS, 5.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<double>(SETTING_RACE_CALCULATION_WEIGHT_FRIENDSHIP, 5.0, 0.0, 100.0);
            groupRaceCalculation.MakeSureSettingExists<uint>(SETTING_RACE_CALCULATION_SHORT_PAUSE_THRESHOLD, 3u, 1u, 100u);
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_PRIORITY_STYLE_BREASTSTROKE, 1, 1, 6);          // The lower the number the higher the priority
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_PRIORITY_STYLE_FREESTYLE, 2, 1, 6);
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_PRIORITY_STYLE_BACKSTROKE, 3, 1, 6);
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_PRIORITY_STYLE_BUTTERFLY, 4, 1, 6);
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_PRIORITY_STYLE_MEDLEY, 5, 1, 6);
            groupRaceCalculation.MakeSureSettingExists<int>(SETTING_RACE_CALCULATION_PRIORITY_STYLE_WATERFLEA, 6, 1, 6);

            // +++++ Group Document Creation +++++
            WorkspaceSettingsGroup groupDocumentCreation = GetGroup(GROUP_DOCUMENT_CREATION, true);
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_PLACEHOLDER_MARKER, "%");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_TEMPLATE_FILENAME_POSTFIX, "_Template");
            groupDocumentCreation.MakeSureSettingExists<DocumentCreationFileTypes>(SETTING_DOCUMENT_CREATION_FILE_TYPES, DocumentCreationFileTypes.DOCX_AND_PDF);
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER, @"Dokumente");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\Urkunden_Template.docx");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\Gesamtliste_Template.docx");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_RACE_START_LIST_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\Startliste_Template.docx");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_TIME_FORMS_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\Zeitzettel_Template.docx");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_RESULT_LIST_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\Ergebnisliste_Template.docx");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_RESULT_LIST_DETAIL_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\ErgebnislisteDetail_Template.docx");
            groupDocumentCreation.MakeSureSettingExists<string>(SETTING_DOCUMENT_CREATION_ANALYTICS_TEMPLATE_PATH, @$"{DEFAULT_TEMPLATE_FOLDER_NAME}\Analyse_Template.docx");
            
            //... Add new settings and groups here ...

            CreateSnapshot();
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Properties

        /// <summary>
        /// List with <see cref="WorkspaceSettingsGroup"/> instances.
        /// </summary>
        public List<WorkspaceSettingsGroup> Groups { get; set; } = new List<WorkspaceSettingsGroup>();

        /// <summary>
        /// Indicating if any of the <see cref="Groups"/> has changed.
        /// </summary>
        public bool HasChanged => Groups.Any(g => g.HasChanged);

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Group management methods

        /// <summary>
        /// Add a new <see cref="WorkspaceSettingsGroup"/>
        /// </summary>
        /// <param name="group">Group to add</param>
        /// <exception cref="ArgumentException">If a group with the same key already exists, this exception is thrown</exception>
        public void AddGroup(WorkspaceSettingsGroup group)
        {
            if (Groups.Where(g => string.Compare(g.GroupKey, group.GroupKey, true) == 0).Any())
            {
                // A group with the same key already exists
                throw new ArgumentException($"A group with the key '{group.GroupKey}' already exists.", nameof(group));
            }
            else
            {
                Groups.Add(group);
                group.PropertyChanged += (sender, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(WorkspaceSettingsGroup.HasChanged): OnPropertyChanged(nameof(HasChanged)); break;
                        default: break;
                    }
                };
            }
        }

        /// <summary>
        /// Get the <see cref="WorkspaceSettingsGroup"/> with the requested key
        /// </summary>
        /// <param name="groupKey">Key of the group to find</param>
        /// <param name="createGroupIfNotExisting">If true, a new group with the key is created if it wasn't found; if false, no new group is created</param>
        /// <returns><see cref="WorkspaceSettingsGroup"/> or <see langword="null"/> if not found and create group parameter is false</returns>
        public WorkspaceSettingsGroup GetGroup(string groupKey, bool createGroupIfNotExisting = false)
        {
            WorkspaceSettingsGroup group = Groups.FirstOrDefault(g => string.Compare(g.GroupKey, groupKey, true) == 0);
            if (group == null && createGroupIfNotExisting)
            {
                group = new WorkspaceSettingsGroup(groupKey);
                AddGroup(group);
            }
            return group;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Setting Getter

        /// <summary>
        /// Get the <see cref="IWorkspaceSetting"/> by the requested keys
        /// </summary>
        /// <param name="groupKey">Key for the <see cref="WorkspaceSettingsGroup"/></param>
        /// <param name="settingKey">Key for the <see cref="IWorkspaceSetting"/></param>
        /// <returns><see cref="IWorkspaceSetting"/></returns>
        public IWorkspaceSetting GetSetting(string groupKey, string settingKey)
        {
            WorkspaceSettingsGroup group = GetGroup(groupKey);
            if (group != null)
            {
                IWorkspaceSetting setting = group.GetSetting(settingKey);
                return setting;
            }
            return null;
        }

        /// <summary>
        /// Get the <see cref="WorkspaceSetting{T}"/> by the requested keys
        /// </summary>
        /// <typeparam name="T">Type of the setting value</typeparam>
        /// <param name="groupKey">Key for the <see cref="WorkspaceSettingsGroup"/></param>
        /// <param name="settingKey">Key for the <see cref="WorkspaceSetting{T}"/></param>
        /// <returns><see cref="WorkspaceSetting{T}"/></returns>
        public WorkspaceSetting<T> GetSetting<T>(string groupKey, string settingKey)
        {
            WorkspaceSettingsGroup group = GetGroup(groupKey);
            if (group != null)
            {
                WorkspaceSetting<T> setting = group.GetSetting<T>(settingKey);
                return setting;
            }
            return null;
        }

        /// <summary>
        /// Get the <see cref="WorkspaceSetting{T}.Value"/> by the requested keys
        /// </summary>
        /// <typeparam name="T">Type of the setting value</typeparam>
        /// <param name="groupKey">Key for the <see cref="WorkspaceSettingsGroup"/></param>
        /// <param name="settingKey">Key for the <see cref="WorkspaceSetting{T}"/></param>
        /// <returns><see cref="WorkspaceSetting{T}.Value"/></returns>
        public T GetSettingValue<T>(string groupKey, string settingKey)
        {
            WorkspaceSetting<T> setting = GetSetting<T>(groupKey, settingKey);
            return setting == null ? default : setting.Value;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Reset / Snapshot

        /// <summary>
        /// Reset all <see cref="IWorkspaceSetting"/> instances in all <see cref="WorkspaceSettingsGroup"/> instances
        /// </summary>
        public void ResetAll()
        {
            foreach (WorkspaceSettingsGroup group in Groups)
            {
                foreach (IWorkspaceSetting setting in group.Settings)
                {
                    setting.Reset();
                }
            }
        }

        /// <summary>
        /// Create a snapshot of all <see cref="IWorkspaceSetting"/> instances in all <see cref="WorkspaceSettingsGroup"/> instances
        /// </summary>
        public void CreateSnapshot()
        {
            foreach (WorkspaceSettingsGroup group in Groups)
            {
                foreach (IWorkspaceSetting setting in group.Settings)
                {
                    setting.CreateSnapshot();
                }
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Constructors

        /// <summary>
        /// Parameterless Constructor
        /// Initialize all settings
        /// </summary>
        public WorkspaceSettings()
        {
            initializeSettings();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"><see cref="WorkspaceSettings"/> to copy</param>
        public WorkspaceSettings(WorkspaceSettings other)
        {
            Groups = new List<WorkspaceSettingsGroup>();
            if(other == null) { return; }
            foreach (WorkspaceSettingsGroup group in other?.Groups)
            {
                if (group is ICloneable cloneableGroup)
                {
                    AddGroup((WorkspaceSettingsGroup)cloneableGroup.Clone());
                }
                else
                {
                    AddGroup(group);    // This assumes the group is immutable or does not require cloning
                }
            }
            initializeSettings();
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Save / Load

        /// <summary>
        /// Save this instance to a file using the <see cref="IFileService"/>.
        /// The <see cref="SerializableWorkspaceSetting"/> and <see cref="SerializableWorkspaceSettingsGroup"/> classes are used to describe the file structure.
        /// </summary>
        /// <param name="fileService"><see cref="IFileService"/> used to save the file</param>
        /// <param name="filePath">Path where the file should be saved</param>
        public void Save(IFileService fileService, string filePath)
        {
            CreateSnapshot();
            List<SerializableWorkspaceSettingsGroup> serializableGroups = Groups.Select(group => new SerializableWorkspaceSettingsGroup
            {
                GroupKey = group.GroupKey,
                Settings = group.Settings.Select(s => new SerializableWorkspaceSetting
                {
                    Key = s.Key,
                    Value = s.UntypedValue
                }).ToList()
            }).ToList();

            fileService.Save(Path.GetDirectoryName(filePath), Path.GetFileName(filePath), serializableGroups);
        }

        /// <summary>
        /// Load the settings from a file using the <see cref="IFileService"/>
        /// The <see cref="SerializableWorkspaceSetting"/> and <see cref="SerializableWorkspaceSettingsGroup"/> classes are used to describe the file structure.
        /// A snapshot is created after loading using the <see cref="CreateSnapshot"/> method
        /// </summary>
        /// <param name="fileService"><see cref="IFileService"/> used to load the file</param>
        /// <param name="filePath">Path from where the file should be loaded</param>
        public void Load(IFileService fileService, string filePath)
        {
            List<SerializableWorkspaceSettingsGroup> serializableGroups = fileService.Read<List<SerializableWorkspaceSettingsGroup>>(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
            Groups.Clear();
            initializeSettings();
            if (serializableGroups != null)
            {
                foreach (SerializableWorkspaceSettingsGroup serializableGroup in serializableGroups)
                {
                    foreach (SerializableWorkspaceSetting serializableSetting in serializableGroup.Settings)
                    {
                        IWorkspaceSetting setting = GetSetting(serializableGroup.GroupKey, serializableSetting.Key);
                        if (setting != null)
                        {
                            object serializableValue = serializableSetting.Value;
                            if(serializableValue is string strValue && setting.ValueType.IsEnum)
                            {
                                serializableValue = Enum.Parse(setting.ValueType, strValue);
                            }
                            setting.UntypedValue = serializableValue;
                        }
                    }
                }
            }
            CreateSnapshot();
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region IEquatable, ICloneable

        public override bool Equals(object obj)
            => obj is WorkspaceSettings s && s.Groups.SequenceEqual(Groups);

        public bool Equals(WorkspaceSettings other)
            => Equals((object)other);

        public override int GetHashCode()
            => Groups.GetHashCode();

        public object Clone()
            => new WorkspaceSettings(this);

        #endregion
    }
}
