using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace J113D.TranslationEditor.ProjectApp.Views.NodeTree
{
    [DoNotNotify]
    internal class NodeTreeViewItem : TreeViewItem, ICustomKeyboardNavigation
    {
        private TextBox? _nodeValueTextBox;
        private CheckBox? _defaultValueCheckBox;

        protected override void LogicalChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            base.LogicalChildrenCollectionChanged(sender, e);
            if(DataContext is StringNodeViewModel)
            {
                IEnumerable<Control> descendants = this.GetLogicalDescendants().OfType<Control>();
                _nodeValueTextBox = (TextBox)descendants.First(x => x.Name == "NodeValueTextBox");
                _defaultValueCheckBox = (CheckBox)descendants.First(x => x.Name == "DefaultValueCheckBox");
            }
        }

        public (bool handled, IInputElement? next) GetNext(IInputElement element, NavigationDirection direction)
        {
            IInputElement? result = GetNextItem(element, direction);

            if(result != null)
            {
                return (true, result);
            }

            NodeTreeView tree = this.FindAncestorOfType<NodeTreeView>()!;
            return ((ICustomKeyboardNavigation)tree).GetNext(this, direction);
        }

        private IInputElement? GetNextItem(IInputElement element, NavigationDirection direction)
        {
            if(this != element && !this.IsVisualAncestorOf((Visual)element))
            {
                return null;
            }

            if(DataContext is StringNodeViewModel)
            {
                IInputElement? stringNodeResult = GetStringNodeControl(element, direction);
                if(stringNodeResult != null)
                {
                    return stringNodeResult;
                }
            }

            NodeTreeViewItem? resultItem;

            if(direction == NavigationDirection.Previous)
            {
                resultItem = GetPreviousSibling(this);
            }
            else if(ItemCount == 0)
            {
                resultItem = GetNextSibling(this);
            }
            else
            {
                if(!IsExpanded)
                {
                    IsExpanded = true;
                    UpdateLayout();
                }

                resultItem = (NodeTreeViewItem)ContainerFromIndex(0)!;
            }

            IInputElement? result = resultItem;

            if(resultItem?.DataContext is StringNodeViewModel)
            {
                result = direction == NavigationDirection.Next && resultItem._nodeValueTextBox!.IsEnabled 
                    ? resultItem._nodeValueTextBox 
                    : resultItem._defaultValueCheckBox;
            }

            return result;
        }

        private IInputElement? GetStringNodeControl(IInputElement element, NavigationDirection direction)
        {
            Visual visual = (Visual)element;

            if(visual == this)
            {
                if(direction == NavigationDirection.Next)
                {
                    return _nodeValueTextBox!.IsEnabled ? _nodeValueTextBox : _defaultValueCheckBox;
                }
            }
            else if(visual == _nodeValueTextBox || _nodeValueTextBox.IsVisualAncestorOf(visual))
            {
                if(direction == NavigationDirection.Next)
                {
                    return _defaultValueCheckBox;
                }
            }
            else if(visual == _defaultValueCheckBox || _defaultValueCheckBox.IsVisualAncestorOf(visual))
            {
                if(direction == NavigationDirection.Previous)
                {
                    return _nodeValueTextBox!.IsEnabled ? _nodeValueTextBox : null;
                }
            }
            
            
            return null;
        }

        private static NodeTreeViewItem? GetPreviousSibling(NodeTreeViewItem item)
        {
            NodeTreeViewItem[] siblings = item.GetLogicalSiblings().OfType<NodeTreeViewItem>().ToArray();
            int index = Array.IndexOf(siblings, item);

            if(index == 0)
            {
                return item.GetLogicalParent<NodeTreeViewItem>();
            }

            NodeTreeViewItem? result = siblings[index - 1];

            while(result.ItemCount > 0)
            {
                if(!result.IsExpanded)
                {
                    result.IsExpanded = true;
                    result.UpdateLayout();
                }

                result = (NodeTreeViewItem)result.ContainerFromIndex(result.ItemCount - 1)!;
            }

            return result;
        }

        private static NodeTreeViewItem? GetNextSibling(NodeTreeViewItem item)
        {
            NodeTreeViewItem[] siblings = item.GetLogicalSiblings().OfType<NodeTreeViewItem>().ToArray();
            int index = Array.IndexOf(siblings, item);

            if(index < siblings.Length - 1)
            {
                return siblings[index + 1];
            }

            NodeTreeViewItem? parent = item.GetLogicalParent<NodeTreeViewItem>();
            return parent == null ? null : GetNextSibling(parent);
        }
    }
}
