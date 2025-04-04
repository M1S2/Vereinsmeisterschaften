using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vereinsmeisterschaften.Core.Models;
using Windows.Devices.PointOfService;

namespace Vereinsmeisterschaften.ViewModels
{
    public class DropAllowedHandlerParkingLot : DefaultDropHandler
    {
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
                dropInfo.DropHintText = "";
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropAllowed(dropInfo))
            {
                // Check if the default drag and drop handler can accept the data (if dragged data type matches the target list type)
                if (CanAcceptData(dropInfo))
                {
                    base.Drop(dropInfo);
                }
                else
                {
                    // Custom drop logic needed (e.g. Race is dropped on List of PersonStart)
                    DropRaceOnPersonStarts(dropInfo);
                }
            }
        }

        /// <summary>
        /// Custom drop logic inspired by <see cref="DefaultDropHandler.Drop(IDropInfo)"/>
        /// </summary>
        /// <param name="dropInfo">Object which contains several drop information.</param>
        private void DropRaceOnPersonStarts(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return;
            }

            int insertIndex = GetInsertIndex(dropInfo);
            IList destinationList = dropInfo.TargetCollection.TryGetList();
            List<object> data = ExtractData(dropInfo.Data).OfType<object>().ToList();

            if(data.FirstOrDefault().GetType() != typeof(Race))
            {
                return;
            }

            bool isSameCollection = false;
            IList sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
            if (sourceList != null)
            {
                isSameCollection = sourceList.IsSameObservableCollection(destinationList);
                if (!isSameCollection)
                {
                    foreach (object o in data)
                    {
                        int index = sourceList.IndexOf(o);
                        if (index != -1)
                        {
                            sourceList.RemoveAt(index);

                            // If source is destination too fix the insertion index
                            if (destinationList != null && ReferenceEquals(sourceList, destinationList) && index < insertIndex)
                            {
                                --insertIndex;
                            }
                        }
                    }
                }
            }

            if (destinationList != null)
            {
                List<object> objects2Insert = new List<object>();

                if(data.FirstOrDefault().GetType() == typeof(Race))
                {
                    List<PersonStart> persons = new List<PersonStart>();
                    data.Cast<Race>().ToList().ForEach(r => persons.AddRange(r.Starts));
                    data = persons.Cast<object>().ToList();
                }

                foreach (object o in data)
                {
                    object obj2Insert = o;
                    
                    objects2Insert.Add(obj2Insert);

                    if (isSameCollection)
                    {
                        int index = destinationList.IndexOf(o);
                        if (index != -1)
                        {
                            if (insertIndex > index)
                            {
                                insertIndex--;
                            }

                            Move(destinationList, index, insertIndex++);
                        }
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }

                    if (obj2Insert is IDragItemSource dragItemSource)
                    {
                        dragItemSource.ItemDropped(dropInfo);
                    }
                }

                SelectDroppedItems(dropInfo, objects2Insert);
            }
        }

        private bool dropAllowed(IDropInfo dropInfo)
        {
            Type dragItemType = dropInfo.DragInfo.SourceItem.GetType();
            return dragItemType == typeof(Race) || dragItemType == typeof(PersonStart);
        }

    }
}
