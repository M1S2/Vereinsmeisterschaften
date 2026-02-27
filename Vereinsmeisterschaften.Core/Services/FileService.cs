using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Services;

/// <summary>
/// Service that handles file operations.
/// </summary>
public class FileService : IFileService
{
    /// <inheritdoc/>
    public T Read<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        return default;
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <inheritdoc/>
    public void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <inheritdoc/>
    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public const string CSV_METADATA_FORMAT = "#{0}={1}";
    public const string CSV_METADATA_REGEX = "#(.*)=(.*)";

    /// <inheritdoc/>
    public void SaveToCsv<T>(string filePath, List<T> dataList, CancellationToken cancellationToken, Dictionary<string, string> metadataDict = null, ProgressDelegate onProgress = null, FormatDataDelegate formatData = null, FormatDataHeaderDelegate formatDataHeader = null, char delimiter = ';')
    {
        // https://stackoverflow.com/questions/25683161/fastest-way-to-convert-a-list-of-objects-to-csv-with-each-object-values-in-a-new
        onProgress?.Invoke(this, 0);
        List<string> lines = new List<string>();

        if (metadataDict != null)
        {
            // Write all metadata key-value-pairs to the file header
            foreach (KeyValuePair<string, string> metadata in metadataDict)
            {
                lines.Add(string.Format(CSV_METADATA_FORMAT, metadata.Key, metadata.Value));
            }
            lines.Add("");
        }

        List<PropertyInfo> props = typeof(T).GetProperties().ToList();
        props = props.Where(p => p.GetCustomAttributes<FileServiceIgnoreAttribute>().Count() == 0).ToList();
        props = props.OrderBy(p => p.GetCustomAttribute<FileServiceOrderAttribute>()?.Order ?? 0).ToList();

        List<string> originalHeaders = props.Select(p => p.Name).ToList();
        List<string> formatedHeaders = props.Select(p => formatDataHeader == null ? p.Name : formatDataHeader(p.Name, p.PropertyType)).ToList();

        lines.Add(string.Join(delimiter.ToString(), formatedHeaders));
                
        int processedElementsCnt = 0;
        foreach (T data in dataList)
        {
            string line = string.Empty;
            foreach (string header in originalHeaders)
            {
                PropertyInfo propInfo = typeof(T).GetProperty(header);
                object dataObj = propInfo?.GetValue(data, null);
                if(formatData != null)
                {
                    line += formatData(dataObj, data, propInfo);
                }
                else
                {
                    line += dataObj;
                }
                line += delimiter;
            }
            line = line.Trim(delimiter);
            lines.Add(line);
            cancellationToken.ThrowIfCancellationRequested();

            // 100 % shouldn't be reported here. Better do this after saving
            if ((processedElementsCnt + 1) < dataList.Count)
            {
                onProgress?.Invoke(this, (processedElementsCnt++ / (float)dataList.Count) * 100);
            }
        }

        File.WriteAllLines(filePath, lines.ToArray(), Encoding.UTF8);
        onProgress?.Invoke(this, 100);
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <inheritdoc/>
    public List<T> LoadFromCsv<T>(string filePath, CancellationToken cancellationToken, SetPropertyFromStringDelegate<T> setPropertyFromStringDelegate, out Dictionary<string, string> metadataDict, ProgressDelegate onProgress = null, FindPropertyFromHeaderDelegate findPropertyFromHeader = null, char delimiter = ';') where T : new()
    {
        metadataDict = null;
        List<T> dataList = new List<T>();
        if (!File.Exists(filePath)) { return dataList; }

        onProgress?.Invoke(this, 0);

        List<string> allLines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();
        List<string> metadataLines = allLines.Where(l => Regex.Match(l, CSV_METADATA_REGEX).Success).ToList();
        List<string> dataLines = allLines.Except(metadataLines).Where(l => !string.IsNullOrEmpty(l)).ToList();

        if (metadataLines.Count > 0)
        {
            metadataDict = new Dictionary<string, string>();
            // loop all found metadata lines
            foreach (string metadataLine in metadataLines)
            {
                Match match = Regex.Match(metadataLine, CSV_METADATA_REGEX);
                if (match.Success && match.Groups.Count >= 3)
                {
                    string key = match.Groups[1].Value;
                    string value = match.Groups[2].Value;
                    metadataDict.Add(key, value);
                }
            }
        }

        if (dataLines.Count >= 2)
        {
            List<string> headers = dataLines.First().Split(delimiter).ToList();
            dataLines.RemoveAt(0);

            int processedElementsCnt = 0;
            foreach (string line in dataLines)
            {
                List<string> lineParts = line.Split(delimiter).ToList();
                T data = new T();
                for (int i = 0; i < Math.Min(headers.Count, lineParts.Count); i++)
                {
                    string header = findPropertyFromHeader == null ? headers[i] : findPropertyFromHeader(headers[i]);
                    setPropertyFromStringDelegate(data, header, lineParts[i]);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                dataList.Add(data);

                onProgress?.Invoke(this, (processedElementsCnt++ / (float)dataLines.Count) * 100);
            }
        }
        onProgress?.Invoke(this, 100);
        return dataList;
    }

}
