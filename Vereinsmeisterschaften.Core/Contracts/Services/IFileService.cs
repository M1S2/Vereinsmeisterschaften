using System.Runtime.CompilerServices;

namespace Vereinsmeisterschaften.Core.Contracts.Services;

/// <summary>
/// Interface for a service that handles file operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Read the given file and deserialize it to the given type.
    /// </summary>
    /// <typeparam name="T">Type to deserialize to</typeparam>
    /// <param name="folderPath">Path where the file is located</param>
    /// <param name="fileName">Filename with extension</param>
    /// <returns>Deserialized object</returns>
    T Read<T>(string folderPath, string fileName);

    /// <summary>
    /// Save the given object to a file. The file is overwritten if it already exists.
    /// </summary>
    /// <typeparam name="T">Type of the object to serialize</typeparam>
    /// <param name="folderPath">Path where the file should be saved</param>
    /// <param name="fileName">Filename with extension</param>
    /// <param name="content">Object to save</param>
    void Save<T>(string folderPath, string fileName, T content);

    /// <summary>
    /// Delete the given file.
    /// </summary>
    /// <param name="folderPath">Path where the file is located</param>
    /// <param name="fileName">Filename with extension</param>
    void Delete(string folderPath, string fileName);

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
    void SaveToCsv<T>(string filePath, List<T> dataList, CancellationToken cancellationToken, ProgressDelegate onProgress = null, FormatDataDelegate formatData = null, FormatDataHeaderDelegate formatDataHeader = null, char delimiter = ';');

    /// <summary>
    /// Load the .csv file to a list of data
    /// </summary>
    /// <typeparam name="T">Type of the data elements</typeparam>
    /// <param name="filePath">Load from this file</param>
    /// <param name="cancellationToken">Used to cancel the load process</param>
    /// <param name="setPropertyFromStringDelegate">Delegate used to change the data element properties</param>
    /// <param name="onProgress"><see cref="ProgressDelegate"/> used to report load progress</param>
    /// <param name="findPropertyFromHeader">Callback that can be used to get the property name from the data headers. Use <see langword="null"/> to use the header name as property name</param>
    /// <param name="delimiter">Delimiter for the .csv file</param>
    /// <returns>Loaded data list</returns>
    List<T> LoadFromCsv<T>(string filePath, CancellationToken cancellationToken, SetPropertyFromStringDelegate<T> setPropertyFromStringDelegate, ProgressDelegate onProgress = null, FindPropertyFromHeaderDelegate findPropertyFromHeader = null, char delimiter = ';') where T : new();
}

/// <summary>
/// Use this attribute to decorate elements that should not be saved to a file.
/// This has only effect for the <see cref="IFileService.SaveToCsv{T}()"/> and <see cref="IFileService.LoadFromCsv{T}()"/> methods."/>
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FileServiceIgnoreAttribute : Attribute
{
}

/// <summary>
/// Use this attribute to order some elements to their position in the file.
/// </summary>
/// <see href="https://stackoverflow.com/questions/9062235/get-properties-in-order-of-declaration-using-reflection"/>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FileServiceOrderAttribute : Attribute
{
    /// <summary>
    /// Order of the property in the file.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Constructor of the <see cref="FileServiceOrderAttribute"/>.
    /// </summary>
    /// <param name="order">Order of the property in the file.</param>
    public FileServiceOrderAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }
}

