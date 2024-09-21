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
    internal sealed class NodeTreeViewItem : TreeViewItem
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


        public IInputElement? GetInitialControl(NavigationDirection direction)
        {
            return direction == NavigationDirection.Next && _nodeValueTextBox!.IsEnabled
                    ? _nodeValueTextBox
                    : _defaultValueCheckBox;
        }

        public IInputElement? GetStringNodeControl(IInputElement element, NavigationDirection direction)
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


        public NodeTreeViewItem? GetPreviousSibling()
        {
            NodeTreeViewItem[] siblings = this.GetLogicalSiblings().OfType<NodeTreeViewItem>().ToArray();
            int index = Array.IndexOf(siblings, this);

            if(index == 0)
            {
                return this.GetLogicalParent<NodeTreeViewItem>();
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

        public NodeTreeViewItem? GetNextSibling()
        {
            NodeTreeViewItem[] siblings = this.GetLogicalSiblings().OfType<NodeTreeViewItem>().ToArray();
            int index = Array.IndexOf(siblings, this);

            if(index < siblings.Length - 1)
            {
                return siblings[index + 1];
            }

            NodeTreeViewItem? parent = this.GetLogicalParent<NodeTreeViewItem>();
            return parent?.GetNextSibling();
        }
    }
}
