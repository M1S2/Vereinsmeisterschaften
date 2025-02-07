using System.Windows.Controls;

namespace Vereinsmeisterschaften.Contracts.Views;

public interface IShellWindow
{
    Frame GetNavigationFrame();

    void ShowWindow();

    void CloseWindow();
}
