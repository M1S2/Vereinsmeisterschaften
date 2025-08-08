using System.Collections;
using System.IO;

using Microsoft.Extensions.Options;

using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Models;

namespace Vereinsmeisterschaften.Services;

/// <summary>
/// Service to persist and restore application data.
/// </summary>
public class PersistAndRestoreService : IPersistAndRestoreService
{
    private readonly IFileService _fileService;
    private readonly AppConfig _appConfig;
    private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    /// <summary>
    /// Constructor for the PersistAndRestoreService.
    /// </summary>
    /// <param name="fileService"><see cref="IFileService"/> object</param>
    /// <param name="appConfig"><see cref="AppConfig"/></param>
    public PersistAndRestoreService(IFileService fileService, IOptions<AppConfig> appConfig)
    {
        _fileService = fileService;
        _appConfig = appConfig.Value;
    }

    /// <inheritdoc/>
    public void PersistData()
    {
        if (App.Current.Properties != null)
        {
            var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            var fileName = _appConfig.AppPropertiesFileName;
            _fileService.Save(folderPath, fileName, App.Current.Properties);
        }
    }

    /// <inheritdoc/>
    public void RestoreData()
    {
        var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
        var fileName = _appConfig.AppPropertiesFileName;
        var properties = _fileService.Read<IDictionary>(folderPath, fileName);
        if (properties != null)
        {
            foreach (DictionaryEntry property in properties)
            {
                App.Current.Properties.Add(property.Key, property.Value);
            }
        }
    }
}
