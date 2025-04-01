using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels
{
    public class DropAllowedHandler : DefaultDropHandler
    {
        public int MaxItemsInTargetCollection { get; set; } = 3;

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
                return;
            }
        }

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
            ICollection sourceList = dropInfo.DragInfo.SourceCollection as ICollection;
            ICollection targetList = dropInfo.TargetCollection as ICollection;
            
            return CanAcceptData(dropInfo) &&
                   dropInfo.DragInfo.SourceItem is PersonStart dragItem &&
                   dropInfo.TargetItem is PersonStart dropItem &&
                   dragItem.Style == dropItem.Style &&
                   dragItem.CompetitionObj?.Distance == dropItem.CompetitionObj?.Distance &&
                   (targetList == sourceList ||
                   targetList != sourceList && targetList.Count + 1 <= MaxItemsInTargetCollection);
        }

    }
}
