using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Interaktionslogik für FileFolderPathControl.xaml
    /// </summary>
    public partial class FileFolderPathControl : UserControl
    {
        public enum FileFolderSelectionModes
        {
            Files,
            Folders
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public FileFolderPathControl()
        {
            InitializeComponent();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public FileFolderSelectionModes FileFolderSelectionMode
        {
            get { return (FileFolderSelectionModes)GetValue(FileFolderSelectionModeProperty); }
            set { SetValue(FileFolderSelectionModeProperty, value); }
        }

        public static readonly DependencyProperty FileFolderSelectionModeProperty = DependencyProperty.Register(nameof(FileFolderSelectionMode), typeof(FileFolderSelectionModes), typeof(FileFolderPathControl), new PropertyMetadata(FileFolderSelectionModes.Files));

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public string FileFolderPath
        {
            get { return (string)GetValue(FileFolderPathProperty); }
            set { SetValue(FileFolderPathProperty, value); }
        }

        public static readonly DependencyProperty FileFolderPathProperty = DependencyProperty.Register(nameof(FileFolderPath), typeof(string), typeof(FileFolderPathControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFileFolderPathChanged));

        private static void OnFileFolderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileFolderPathControl control = d as FileFolderPathControl;
            control.calculateResolvedFileFolderPath();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public string RootFolderForRelativePaths
        {
            get { return (string)GetValue(RootFolderForRelativePathsProperty); }
            set { SetValue(RootFolderForRelativePathsProperty, value); }
        }

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

        public string ResolvedFileFolderPath
        {
            get { return (string)GetValue(ResolvedFileFolderPathProperty); }
            set { SetValue(ResolvedFileFolderPathProperty, value); }
        }

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

        public string OpenFileDialogFilter
        {
            get { return (string)GetValue(OpenFileDialogFilterProperty); }
            set { SetValue(OpenFileDialogFilterProperty, value); }
        }
        public static readonly DependencyProperty OpenFileDialogFilterProperty = DependencyProperty.Register(nameof(OpenFileDialogFilter), typeof(string), typeof(FileFolderPathControl), new PropertyMetadata("All files (*.*)|*.*"));

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

    }
}
