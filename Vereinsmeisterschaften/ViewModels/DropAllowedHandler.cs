using System.Collections;
using System.Windows;
using Vereinsmeisterschaften.Core.Models;
using GongSolutions.Wpf.DragDrop;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Drop handler for the race page.
    /// </summary>
    public class DropAllowedHandler : DefaultDropHandler
    {
        /// <summary>
        /// Maximum items that are allowed in the target collection.
        /// If this number of items is reached, no dropping will be allowed anymore.
        /// </summary>
        public int MaxItemsInTargetCollection { get; set; } = 3;

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
                dropInfo.DropHintText = Properties.Resources.PersonStartDragDropErrorString;
                dropInfo.Effects = DragDropEffects.None;
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

        // only allow drag and drop if the source and destination items have the same swimming style and distance and limit the number of items in the target collection to MaxItemsInTargetCollection
        private bool dropAllowed(IDropInfo dropInfo)
        {
            ICollection sourceCollection = dropInfo.DragInfo.SourceCollection as ICollection;
            ICollection targetCollection = dropInfo.TargetCollection as ICollection;

            PersonStart dragItem = dropInfo.DragInfo.SourceItem as PersonStart;
            PersonStart dropItem = dropInfo.TargetItem as PersonStart;

            bool dropAllowed = (targetCollection == sourceCollection ||
                                targetCollection != sourceCollection && targetCollection.Count + 1 <= MaxItemsInTargetCollection);
            if (!CanAcceptData(dropInfo))
            {
                return false;
            }
            else if (dragItem != null && dropItem != null)
            {
                return dropAllowed && 
                       dragItem.Style == dropItem.Style &&
                       dragItem.CompetitionObj?.Distance == dropItem.CompetitionObj?.Distance;
            }
            else if (dragItem != null && dropItem == null && targetCollection.Count == 0)
            {
                return dropAllowed;
            }
            else if (dragItem != null && dropItem == null && targetCollection.Count > 0)
            {
                PersonStart firstStart = (targetCollection as IList)?.Cast<PersonStart>().FirstOrDefault();

                return dropAllowed &&
                       dragItem.Style == firstStart?.Style &&
                       dragItem.CompetitionObj?.Distance == firstStart?.CompetitionObj?.Distance;
            }
            return false;
        }

    }
}
