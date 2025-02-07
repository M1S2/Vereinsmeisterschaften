using System.Windows.Controls;

namespace Vereinsmeisterschaften.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);

    Page GetPage(string key);
}
