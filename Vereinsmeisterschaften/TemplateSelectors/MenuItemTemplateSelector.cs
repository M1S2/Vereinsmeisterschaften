using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;

namespace Vereinsmeisterschaften.TemplateSelectors;

/// <summary>
/// DataTemplateSelector for HamburgerMenu items.
/// </summary>
public class MenuItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// DataTemplate for HamburgerMenuGlyphItem.
    /// </summary>
    public DataTemplate GlyphDataTemplate { get; set; }

    /// <summary>
    /// DataTemplate for HamburgerMenuImageItem.
    /// </summary>
    public DataTemplate ImageDataTemplate { get; set; }

    /// <summary>
    /// Selects the appropriate DataTemplate based on the type of the item.
    /// </summary>
    /// <param name="item">Item used to select</param>
    /// <param name="container">Container</param>
    /// <returns><see cref="DataTemplate"/></returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is HamburgerMenuGlyphItem)
        {
            return GlyphDataTemplate;
        }

        if (item is HamburgerMenuImageItem)
        {
            return ImageDataTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}
