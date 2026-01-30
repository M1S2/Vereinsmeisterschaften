using System.Collections;
using System.Windows;
using System.ComponentModel;
using GongSolutions.Wpf.DragDrop;
using System.Windows.Controls;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Drop handler that only allows dropping if the source and destination types are equal and if the e.g. DataGrid is not sorted.
    /// </summary>
    public class DropAllowedSameTypeAndNotSortedHandler : DefaultDropHandler
    {
        #region Singleton

        private static readonly DropAllowedSameTypeAndNotSortedHandler instance = new DropAllowedSameTypeAndNotSortedHandler();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static DropAllowedSameTypeAndNotSortedHandler()
        {
        }

        private DropAllowedSameTypeAndNotSortedHandler()
        {
        }

        /// <summary>
        /// Singleton instance for the <see cref="DropAllowedSameTypeAndNotSortedHandler"/>
        /// </summary>
        public static DropAllowedSameTypeAndNotSortedHandler Instance => instance;

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public override void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetHintAdorner = DropTargetAdorners.Hint;

            if (dropAllowed(dropInfo))
            {
                dropInfo.DropTargetHintState = DropHintState.Active;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                dropInfo.DropTargetHintState = DropHintState.Error;
                dropInfo.Effects = DragDropEffects.None;
                if(isVisualDataGridSorted(dropInfo))
                {
                    dropInfo.DropHintText = Properties.Resources.DragDropNotAllowedDataGridSortedString;
                }
                else
                {
                    dropInfo.DropHintText = Properties.Resources.DragDropDifferentTypesString;
                }
            }
        }

        /// <inheritdoc/>
        public override void Drop(IDropInfo dropInfo)
        {
            if (dropAllowed(dropInfo))
            {
                base.Drop(dropInfo);
            }
        }

        // Only return true if the drop target is a DataGrid and if the DataGrid is sorted.
        private bool isVisualDataGridSorted(IDropInfo dropInfo)
            => dropInfo.VisualTarget is DataGrid grid && grid.Items.SortDescriptions.Count > 0;

        // only allow drag and drop if the source and destination items have the same type
        private bool dropAllowed(IDropInfo dropInfo)
        {
            bool dropAllowed = false;
            IEnumerable sourceCollection = dropInfo.DragInfo.SourceCollection;
            IEnumerable targetCollection = dropInfo.TargetCollection;

            Type dragItemType = dropInfo.DragInfo.SourceItem?.GetType();
            Type dropItemType = dropInfo.TargetItem?.GetType();
            dropAllowed = (targetCollection == sourceCollection ||
                           targetCollection != sourceCollection && dragItemType != null && dropItemType != null && dragItemType.Equals(dropItemType));

            if (!CanAcceptData(dropInfo))
            {
                return false;
            }
            else
            {
                return dropAllowed && !isVisualDataGridSorted(dropInfo);
            }
        }

    }
}
