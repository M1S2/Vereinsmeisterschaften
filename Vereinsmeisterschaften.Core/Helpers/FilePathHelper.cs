using System.Diagnostics;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Class containing helper methods for file and folder paths.
    /// </summary>
    public static class FilePathHelper
    {
        /// <summary>
        /// Calculate a path that is relative to the given root folder.
        /// If the fullPath is already relative, the path isn't changed.
        /// </summary>
        /// <param name="fullPath">Full path to convert to a relative one</param>
        /// <param name="rootFolder">Root folder that is used as base for the relative path</param>
        /// <returns>Relative path</returns>
        public static string MakePathRelative(string fullPath, string rootFolder)
        {
            if(string.IsNullOrEmpty(fullPath))
            {
                return "."; // Return current directory if fullPath is empty
            }
            if(!IsPathFullyQualified(fullPath))
            {
                return fullPath;
            }
            // Remove any relative paths from these strings
            fullPath = Path.GetFullPath(fullPath);
            rootFolder = Path.GetFullPath(rootFolder);
            return Path.GetRelativePath(rootFolder, fullPath);
        }

        /// <summary>
        /// Make a relative path absolute by combining it with the given root folder.
        /// If the relative path is already absolute, it will be returned unchanged.
        /// </summary>
        /// <param name="relativePath">Relative part of the path</param>
        /// <param name="rootFolder">Root folder for relative path</param>
        /// <returns>Absolute path</returns>
        public static string MakePathAbsolute(string relativePath, string rootFolder)
        {
            if(IsPathFullyQualified(relativePath))
            {
                return relativePath; // already absolute
            }
            else
            {
                return Path.GetFullPath(Path.Combine(rootFolder, relativePath));
            }
        }

        /// <summary>
        /// Checks if the given path is fully qualified.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if fully qualified; otherwise false</returns>
        /// <see href="https://stackoverflow.com/questions/5565029/check-if-full-path-given"/>
        public static bool IsPathFullyQualified(string path)
        {
            var root = Path.GetPathRoot(path);
            return root != null && (root.StartsWith(@"\\") || root.EndsWith(@"\") && root != @"\");
        }

        /// <summary>
        /// Checks if the given path is a directory.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the path is a directory; otherwise false</returns>
        /// <exception cref="ArgumentNullException">When path is <see langword="null"/></exception>
        /// <see href="https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory"/>
        public static bool IsPathDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            path = path.Trim();

            if (Directory.Exists(path))
                return true;

            if (File.Exists(path))
                return false;

            // neither file nor directory exists. guess intention

            // if has trailing slash then it's a directory
            if (new[] { "\\", "/" }.Any(x => path.EndsWith(x)))
                return true; // ends with slash

            // if has extension then its a file; directory otherwise
            return string.IsNullOrWhiteSpace(Path.GetExtension(path));
        }

        /// <summary>
        /// Open the file with the default application.
        /// </summary>
        /// <param name="path">Path to the file to open</param>
        /// <see cref="https://stackoverflow.com/questions/11365984/c-sharp-open-file-with-default-application-and-parameters"/>
        public static void OpenWithDefaultProgram(string path)
        {
            using (Process fileopener = new Process())
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
                fileopener.Start();
            }
        }
    }
}
