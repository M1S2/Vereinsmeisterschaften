using System.Collections.ObjectModel;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Class containing helper methods for collections
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Sort the <see cref="ObservableCollection{T}"/> without creating it new
        /// </summary>
        /// <typeparam name="T">Generic type of the collection</typeparam>
        /// <param name="collection">Collection to sort</param>
        /// <param name="comparison">Comparision used for sorting</param>
        /// <see href="https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-through-c-sharp"/>
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
    }
}
