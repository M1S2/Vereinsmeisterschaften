using System.IO;
using System.Windows;
using System.Windows.Controls;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Interaktionslogik für FileFolderPathControl.xaml
    /// </summary>
    public partial class FileFolderPathControl : UserControl
    {
        /// <summary>
        /// Available selection modes for the file/folder path control.
        /// </summary>
        public enum FileFolderSelectionModes
        {
            Files,
            Folders
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor of the <see cref="FileFolderPathControl"/>
        /// </summary>
        public FileFolderPathControl()
        {
            InitializeComponent();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Current selection mode for the file/folder path control.
        /// </summary>
        public FileFolderSelectionModes FileFolderSelectionMode
        {
            get { return (FileFolderSelectionModes)GetValue(FileFolderSelectionModeProperty); }
            set { SetValue(FileFolderSelectionModeProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="FileFolderSelectionMode"/>.
        /// </summary>
        public static readonly DependencyProperty FileFolderSelectionModeProperty = DependencyProperty.Register(nameof(FileFolderSelectionMode), typeof(FileFolderSelectionModes), typeof(FileFolderPathControl), new PropertyMetadata(FileFolderSelectionModes.Files));

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Current file or folder path (can be relative or absolute).
        /// </summary>
        public string FileFolderPath
        {
            get { return (string)GetValue(FileFolderPathProperty); }
            set { SetValue(FileFolderPathProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="FileFolderPath"/>.
        /// </summary>
        public static readonly DependencyProperty FileFolderPathProperty = DependencyProperty.Register(nameof(FileFolderPath), typeof(string), typeof(FileFolderPathControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFileFolderPathChanged));

        private static void OnFileFolderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileFolderPathControl control = d as FileFolderPathControl;
            control.calculateResolvedFileFolderPath();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Root folder used for relative paths.
        /// </summary>
        public string RootFolderForRelativePaths
        {
            get { return (string)GetValue(RootFolderForRelativePathsProperty); }
            set { SetValue(RootFolderForRelativePathsProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="RootFolderForRelativePaths"/>.
        /// </summary>
        public static readonly DependencyProperty RootFolderForRelativePathsProperty = DependencyProperty.Register(nameof(RootFolderForRelativePaths), typeof(string), typeof(FileFolderPathControl), new PropertyMetadata("", OnRootFolderForRelativePathsChanged));

        private static void OnRootFolderForRelativePathsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileFolderPathControl control = d as FileFolderPathControl;
            if (!string.IsNullOrEmpty(e.NewValue as string))
            {
                control.FileFolderPath = FilePathHelper.MakePathRelative(control.ResolvedFileFolderPath, e.NewValue as string);
            }
            else
            {
                // If the root folder is empty, we just leave the FileFolderPath as is
            }
            control.calculateResolvedFileFolderPath();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Resolved file or folder path (absolute path).
        /// This property is automatically calculated based on the <see cref="FileFolderPath"/> and <see cref="RootFolderForRelativePaths"/>.
        /// </summary>
        public string ResolvedFileFolderPath
        {
            get { return (string)GetValue(ResolvedFileFolderPathProperty); }
            set { SetValue(ResolvedFileFolderPathProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="ResolvedFileFolderPath"/>.
        /// </summary>
        public static readonly DependencyProperty ResolvedFileFolderPathProperty = DependencyProperty.Register(nameof(ResolvedFileFolderPath), typeof(string), typeof(FileFolderPathControl), new PropertyMetadata(""));

        private void calculateResolvedFileFolderPath()
        {
            if (!string.IsNullOrEmpty(RootFolderForRelativePaths) && !FilePathHelper.IsPathFullyQualified(FileFolderPath))
            {
                // Construct absolute path
                ResolvedFileFolderPath = Path.GetFullPath(Path.Combine(RootFolderForRelativePaths, FileFolderPath));
            }
            else if(string.IsNullOrEmpty(RootFolderForRelativePaths) && !FilePathHelper.IsPathFullyQualified(FileFolderPath))
            {
                // Use relative path as is
                ResolvedFileFolderPath = FileFolderPath;
            }
            else
            {
                // Use absolute path
                ResolvedFileFolderPath = FileFolderPath;
            }

            if (FileFolderSelectionMode == FileFolderSelectionModes.Folders && !FilePathHelper.IsPathDirectory(ResolvedFileFolderPath))
            {
                ResolvedFileFolderPath = Path.GetDirectoryName(ResolvedFileFolderPath);
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Filter for the OpenFileDialog.
        /// </summary>
        public string OpenFileDialogFilter
        {
            get { return (string)GetValue(OpenFileDialogFilterProperty); }
            set { SetValue(OpenFileDialogFilterProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="OpenFileDialogFilter"/>.
        /// </summary>
        public static readonly DependencyProperty OpenFileDialogFilterProperty = DependencyProperty.Register(nameof(OpenFileDialogFilter), typeof(string), typeof(FileFolderPathControl), new PropertyMetadata("All files (*.*)|*.*"));

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Indicating whether the <see cref="ResolvedFileFolderPath"/> should be shown.
        /// Only available when the <see cref="RootFolderForRelativePaths"/> is set.
        /// </summary>
        public bool ShouldShowFullPath
        {
            get { return (bool)GetValue(ShouldShowFullPathProperty); }
            set { SetValue(ShouldShowFullPathProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="ShouldShowFullPath"/>.
        /// </summary>
        public static readonly DependencyProperty ShouldShowFullPathProperty = DependencyProperty.Register(nameof(ShouldShowFullPath), typeof(bool), typeof(FileFolderPathControl), new PropertyMetadata(false));

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            string tmpFileFolderPath = string.Empty;
            switch (FileFolderSelectionMode)
            {
                case FileFolderSelectionModes.Files:
                    {
                        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                        openFileDialog.InitialDirectory = Path.GetDirectoryName(ResolvedFileFolderPath);
                        openFileDialog.FileName = FileFolderPath;
                        openFileDialog.Filter = OpenFileDialogFilter;
                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            tmpFileFolderPath = openFileDialog.FileName;
                        }
                        break;
                    }
                case FileFolderSelectionModes.Folders:
                    {
                        System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                        folderBrowserDialog.InitialDirectory = ResolvedFileFolderPath;
                        folderBrowserDialog.SelectedPath = FileFolderPath;
                        if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            tmpFileFolderPath = folderBrowserDialog.SelectedPath;
                        }
                        break;
                    }
                default: break;
            }

            if (!string.IsNullOrEmpty(tmpFileFolderPath))
            {
                if (!string.IsNullOrEmpty(RootFolderForRelativePaths))
                {
                    FileFolderPath = FilePathHelper.MakePathRelative(tmpFileFolderPath, RootFolderForRelativePaths);
                }
                else
                {
                    FileFolderPath = tmpFileFolderPath;
                }
            }
        }

        private void btn_showFullPath_Click(object sender, RoutedEventArgs e)
        {
            ShouldShowFullPath = !ShouldShowFullPath;
        }
    }
}
