using System.ComponentModel;
using System.IO;
using System.Text;
using Newtonsoft.Json;

using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services;

public class FileService : IFileService
{
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

    public void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    public void SaveToCsv<T>(string filePath, List<T> dataList, CancellationToken cancellationToken, ProgressDelegate onProgress = null, FormatDataDelegate formatData = null, char delimiter = ';')
    {
        // https://stackoverflow.com/questions/25683161/fastest-way-to-convert-a-list-of-objects-to-csv-with-each-object-values-in-a-new
        onProgress?.Invoke(this, 0);
        List<string> lines = new List<string>();
        List<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().ToList();
        List<string> headers = props.Where(p => p.Attributes.OfType<FileServiceIgnoreAttribute>().Count() == 0).Select(p => p.Name).ToList();
        lines.Add(string.Join(delimiter.ToString(), headers));
        int processedElementsCnt = 0;
        foreach (T data in dataList)
        {
            string line = string.Empty;
            foreach (string header in headers)
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

        File.WriteAllLines(filePath, lines.ToArray());
        onProgress?.Invoke(this, 100);
    }

    public List<T> LoadFromCsv<T>(string filePath, CancellationToken cancellationToken, SetPropertyFromStringDelegate<T> setPropertyFromStringDelegate, ProgressDelegate onProgress = null, char delimiter = ';') where T : new()
    {
        List<T> dataList = new List<T>();
        if (!File.Exists(filePath)) { return dataList; }

        onProgress?.Invoke(this, 0);

        List<string> lines = File.ReadAllLines(filePath).ToList();
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
