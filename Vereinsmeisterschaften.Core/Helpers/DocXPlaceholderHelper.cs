using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Documents;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Vereinsmeisterschaften.Core.Helpers
{
    public static class DocXPlaceholderHelper
    {
        /// <summary>
        /// Placeholder that can be used in a table to insert the row number.
        /// </summary>
        public const string TableRowIDPlaceholder = "ID";

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Text Placeholders

        /// <summary>
        /// Class to hold text placeholders and their values.
        /// </summary>
        public class TextPlaceholders
        {
            public Dictionary<string, string> Placeholders = new Dictionary<string, string>();

            /// <summary>
            /// Add a placeholder to the dictionary. If the placeholder already exists, it will be updated with the new value.
            /// </summary>
            /// <param name="placeholder">Placeholder string without the <see cref="PlaceholderMarker"/></param>
            /// <param name="value">Value for this placeholder</param>
            public void Add(string placeholder, string value)
            {
                if (Placeholders.ContainsKey(placeholder))
                {
                    Placeholders[placeholder] = value;
                }
                else
                {
                    Placeholders.Add(placeholder, value);
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Replace text placeholders in a DocX template file with values from the <see cref="TextPlaceholders"/> class.
        /// </summary>
        /// <param name="templateFile">Path to a template file containing placeholders</param>
        /// <param name="outputFile">Path where the file with replaced placeholders should be saved to</param>
        /// <param name="placeholders">Placeholder, value pairs</param>
        /// <param name="placeholderMarker">String marker for placeholders in the template files. The placeholder must be enclosed by this marker.</param>
        /// <param name="keepUnknownPlaceholders">Don't change placeholders from unknown values; if false, placeholder is replaced by <see cref="string.Empty"/></param>
        /// <returns>True, if placeholders were replaced; otherwise false</returns>
        public static bool ReplaceTextPlaceholders(string templateFile, string outputFile, TextPlaceholders placeholders, string placeholderMarker, bool keepUnknownPlaceholders = true)
        {
            bool result;
            using (DocX templateDocument = DocX.Load(templateFile))
            {
                // Replace all placeholders in the document
                result = templateDocument.ReplaceText(new FunctionReplaceTextOptions()
                {
                    FindPattern = placeholderMarker + "(.*?)" + placeholderMarker,
                    RegExOptions = RegexOptions.IgnoreCase,
                    RegexMatchHandler = (placeholderWithoutMarkers) =>
                    {
                        if (placeholders.Placeholders.ContainsKey(placeholderWithoutMarkers))
                        {
                            return placeholders.Placeholders[placeholderWithoutMarkers];
                        }
                        return keepUnknownPlaceholders ? (placeholderMarker + placeholderWithoutMarkers + placeholderMarker) : string.Empty;
                    }
                });
                // Save the document with the replaced placeholders as new file
                templateDocument.SaveAs(outputFile);
            }
            return result;
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Table Placeholders

        /// <summary>
        /// Class to hold table placeholders and their list of values.
        /// </summary>
        public class TablePlaceholders
        {
            public Dictionary<string, List<string>> Placeholders = new Dictionary<string, List<string>>();

            /// <summary>
            /// Add a placeholder to the dictionary. If the placeholder already exists, it will be updated with the new values.
            /// </summary>
            /// <param name="placeholder">Placeholder string without the <see cref="PlaceholderMarker"/></param>
            /// <param name="value">Value for this placeholder</param>
            public void Add(string placeholder, List<string> value)
            {
                if (Placeholders.ContainsKey(placeholder))
                {
                    Placeholders[placeholder] = value;
                }
                else
                {
                    Placeholders.Add(placeholder, value);
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Replace table placeholders in a DocX template file with values from the <see cref="TablePlaceholders"/> class.
        /// All tables in the document that contain at least one placeholder from <see cref="TablePlaceholders"/> will be processed.
        /// The first row that contains a placeholder will be used as a pattern for the new rows.
        /// </summary>
        /// <param name="templateFile">Path to a template file containing placeholders</param>
        /// <param name="outputFile">Path where the file with replaced placeholders should be saved to</param>
        /// <param name="placeholders">Placeholder, value pairs</param>
        /// <param name="placeholderMarker">String marker for placeholders in the template files. The placeholder must be enclosed by this marker.</param>
        /// <returns>True, if placeholders were replaced; otherwise false</returns>
        public static bool ReplaceTablePlaceholders(string templateFile, string outputFile, TablePlaceholders placeholders, string placeholderMarker)
        {
            bool result = false;
            using (DocX templateDocument = DocX.Load(templateFile))
            {
                foreach (Table templateTable in templateDocument.Tables)
                {
                    if (templateTable == null) continue;

                    // Try to find at least one placeholder in this table
                    Row rowPattern = null;
                    foreach (Row row in templateTable.Rows)
                    {
                        foreach (string placeholder in placeholders.Placeholders.Keys)
                        {
                            if (row.FindUniqueByPattern(placeholderMarker + placeholder + placeholderMarker, RegexOptions.IgnoreCase).Count > 0)
                            {
                                // Placeholder found in this table
                                rowPattern = row;
                                break;
                            }
                        }
                        if (rowPattern != null)
                        {
                            break;
                        }
                    }
                    if (rowPattern == null)
                    {
                        // No placeholder found in this table
                        continue;
                    }

                    int numberOfRows = placeholders.Placeholders.Values.Select(v => v.Count).Max();
                    for (int i = 0; i < numberOfRows; i++)
                    {
                        // Insert new row at the end of this table
                        Row newRow = templateTable.InsertRow(rowPattern, templateTable.RowCount - 1, true);

                        // Replace all placeholders in the new row
                        result |= newRow.ReplaceText(new FunctionReplaceTextOptions()
                        {
                            FindPattern = placeholderMarker + "(.*?)" + placeholderMarker,
                            RegExOptions = RegexOptions.IgnoreCase,
                            RegexMatchHandler = (placeholderWithoutMarkers) =>
                            {
                                if (placeholders.Placeholders.ContainsKey(placeholderWithoutMarkers))
                                {
                                    List<string> values = placeholders.Placeholders[placeholderWithoutMarkers];
                                    return (i < values.Count) ? values[i] : string.Empty;
                                }
                                else if (placeholderWithoutMarkers == TableRowIDPlaceholder)
                                {
                                    return (i + 1).ToString();
                                }
                                return placeholderMarker + placeholderWithoutMarkers + placeholderMarker;
                            }
                        });
                    }

                    // Remove the pattern row.
                    rowPattern.Remove();
                }

                // Save the document with the replaced placeholders as new file
                templateDocument.SaveAs(outputFile);

                return result;
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Conversion Methods

        /// <summary>
        /// Convert a list of <see cref="TextPlaceholders"/> to a <see cref="TablePlaceholders"/>
        /// </summary>
        /// <param name="textPlaceholderList">List of <see cref="TextPlaceholders"/></param>
        /// <returns><see cref="TablePlaceholders"/></returns>
        public static TablePlaceholders ConvertTextToTablePlaceholders(IEnumerable<TextPlaceholders> textPlaceholderList)
        {
            TablePlaceholders tablePlaceholders = new TablePlaceholders();

            textPlaceholderList.SelectMany(dict => dict.Placeholders)
                .GroupBy(kvp => kvp.Key)
                .ToList()
                .ForEach(group => tablePlaceholders.Add(group.Key, group.Select(kvp => kvp.Value).ToList()));

            return tablePlaceholders;
        }

        #endregion
    }
}
