using System.Windows;
using System.Windows.Controls;

namespace Vereinsmeisterschaften.TemplateSelectors
{
    /// <summary>
    /// Template selector that returns the <see cref="DataTemplate"/> depending on the given <see cref="Enum"/> value
    /// </summary>
    public class EnumTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Dictionary with <see cref="KeyValuePair{Enum, DataTemplate}"/>
        /// </summary>
        public Dictionary<Enum, DataTemplate> Templates { get; } = new();

        /// <summary>
        /// If the given item is an <see cref="Enum"/> and the <see cref="Templates"/> have an entry for this <see cref="Enum"/>, return the corresponding <see cref="DataTemplate"/>
        /// </summary>
        /// <param name="item">Item used to get the <see cref="DataTemplate"/></param>
        /// <param name="container">Parent Container. Not used.</param>
        /// <returns><see cref="DataTemplate"/> from <see cref="Templates"/> if found. Otherwise the base method is called.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Enum filter && Templates.TryGetValue(filter, out var template))
                return template;

            // return empty DataTemplate, if item isn't found in Templates. Nothing is shown then (instead of the default .ToString()).
            return new DataTemplate();
        }
    }
}
