using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Services;

/// <summary>
/// Service that handles file operations.
/// </summary>
public class FileService : IFileService
{
    /// <summary>
    /// Read the given file and deserialize it to the given type.
    /// </summary>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="folderPath">Path where the file is located</param>
    /// <param name="fileName">Filename with extension</param>
    /// <returns>Deserialized object</returns>
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

    /// <summary>
    /// Save the given object to a file. The file is overwritten if it already exists.
    /// </summary>
    /// <typeparam name="T">Type of the object to serialize</typeparam>
    /// <param name="folderPath">Path where the file should be saved</param>
    /// <param name="fileName">Filename with extension</param>
    /// <param name="content">Object to save</param>
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

    /// <summary>
    /// Delete the given file.
    /// </summary>
    /// <param name="folderPath">Path where the file is located</param>
    /// <param name="fileName">Filename with extension</param>
    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <summary>
    /// Save the given data list to a .csv file
    /// </summary>
    /// <typeparam name="T">Type of the data elements</typeparam>
    /// <param name="filePath">Save to this file</param>
    /// <param name="dataList">List with data to save</param>
    /// <param name="cancellationToken">Used to cancel the save process</param>
    /// <param name="onProgress"><see cref="ProgressDelegate"/> used to report save progress</param>
    /// <param name="formatData">Callback that can be used to format the data. Use <see langword="null"/> to use the default ToString method</param>
    /// <param name="formatDataHeader">Callback that can be used to format the data headers. Use <see langword="null"/> to use the default header</param>
    /// <param name="delimiter">Delimiter for the .csv file</param>
    public void SaveToCsv<T>(string filePath, List<T> dataList, CancellationToken cancellationToken, ProgressDelegate onProgress = null, FormatDataDelegate formatData = null, FormatDataHeaderDelegate formatDataHeader = null, char delimiter = ';')
    {
        // https://stackoverflow.com/questions/25683161/fastest-way-to-convert-a-list-of-objects-to-csv-with-each-object-values-in-a-new
        onProgress?.Invoke(this, 0);
        List<string> lines = new List<string>();

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
                object dataObj = typeof(T).GetProperty(header)?.GetValue(data, null);
                if(formatData != null)
                {
                    line += formatData(dataObj);
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

    /// <summary>
    /// Load the .csv file to a list of data
    /// </summary>
    /// <typeparam name="T">Type of the data elements</typeparam>
    /// <param name="filePath">Load from this file</param>
    /// <param name="cancellationToken">Used to cancel the load process</param>
    /// <param name="setPropertyFromStringDelegate">Delegate used to change the data element properties</param>
    /// <param name="onProgress"><see cref="ProgressDelegate"/> used to report load progress</param>
    /// <param name="delimiter">Delimiter for the .csv file</param>
    /// <returns>Loaded data list</returns>
    public List<T> LoadFromCsv<T>(string filePath, CancellationToken cancellationToken, SetPropertyFromStringDelegate<T> setPropertyFromStringDelegate, ProgressDelegate onProgress = null, char delimiter = ';') where T : new()
    {
        List<T> dataList = new List<T>();
        if (!File.Exists(filePath)) { return dataList; }

        onProgress?.Invoke(this, 0);

        List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();
        if (lines.Count >= 2)
        {
            List<string> headers = lines.First().Split(delimiter).ToList();
            lines.RemoveAt(0);

            int processedElementsCnt = 0;
            foreach (string line in lines)
            {
                List<string> lineParts = line.Split(delimiter).ToList();
                T data = new T();
                for (int i = 0; i < Math.Min(headers.Count, lineParts.Count); i++)
                {
                    setPropertyFromStringDelegate(data, headers[i], lineParts[i]);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                dataList.Add(data);

                onProgress?.Invoke(this, (processedElementsCnt++ / (float)lines.Count) * 100);
            }
        }
        onProgress?.Invoke(this, 100);
        return dataList;
    }

}
