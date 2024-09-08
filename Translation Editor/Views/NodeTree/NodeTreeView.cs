using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using PropertyChanged;
using System;
using System.Linq;

namespace J113D.TranslationEditor.ProjectApp.Views.NodeTree
{
    [DoNotNotify]
    internal class NodeTreeView : TreeView, ICustomKeyboardNavigation
    {

        public static readonly StyledProperty<double> NameWidthProperty =
            AvaloniaProperty.Register<NodeTreeView, double>(nameof(NameWidth));

        public static readonly StyledProperty<double> ContentWidthProperty =
            AvaloniaProperty.Register<NodeTreeView, double>(nameof(ContentWidth));

        protected override Type StyleKeyOverride => typeof(TreeView);

        public double NameWidth
        {
            get => GetValue(NameWidthProperty);
            set => SetValue(NameWidthProperty, value);
        }

        public double ContentWidth
        {
            get => GetValue(ContentWidthProperty);
            set => SetValue(ContentWidthProperty, value);
        }


        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new NodeTreeViewItem();
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<NodeTreeViewItem>(item, out recycleKey);
        }


        public (bool handled, IInputElement? next) GetNext(IInputElement element, NavigationDirection direction)
        {
            if(direction is not NavigationDirection.Next and not NavigationDirection.Previous)
            {
                return (false, null);
            }

            NodeTreeViewItem? item = ((Visual)element).FindAncestorOfType<NodeTreeViewItem>(true);
            IInputElement? result;

            if(item == null || !this.IsLogicalAncestorOf(item))
            {
                item = SelectedItem != null ?
                    (NodeTreeViewItem)TreeContainerFromItem(SelectedItem)! :
                    (NodeTreeViewItem)ContainerFromIndex(0)!;

                result = item.DataContext is StringNodeViewModel 
                    ? item.GetInitialControl(direction) 
                    : item;
            }
            else
            {
                result = GetNextItem(item, element, direction);
            }

            return (true, result);
        }

        private IInputElement? GetNextItem(NodeTreeViewItem item, IInputElement element, NavigationDirection direction)
        {
            if(item.DataContext is StringNodeViewModel)
            {
                IInputElement? stringNodeResult = item.GetStringNodeControl(element, direction);
                if(stringNodeResult != null)
                {
                    return stringNodeResult;
                }
            }

            NodeTreeViewItem? resultItem;

            if(direction == NavigationDirection.Previous)
            {
                resultItem = item.GetPreviousSibling();
            }
            else if(item.ItemCount == 0)
            {
                resultItem = item.GetNextSibling();
            }
            else
            {
                if(!item.IsExpanded)
                {
                    item.IsExpanded = true;
                    item.UpdateLayout();
                }

                resultItem = (NodeTreeViewItem)item.ContainerFromIndex(0)!;
            }

            IInputElement? result = resultItem;

            if(resultItem?.DataContext is StringNodeViewModel)
            {
                result = resultItem.GetInitialControl(direction);
            }

            return result;
        }
    }
}
