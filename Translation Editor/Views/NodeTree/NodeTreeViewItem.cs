﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
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
    [TemplatePart("PART_LayoutRoot", typeof(Border))]
    internal sealed class NodeTreeViewItem : TreeViewItem
    {
        private NodeTreeView? _tree;
        private Border? _layoutRoot;
        private TextBox? _nodeValueTextBox;
        private CheckBox? _defaultValueCheckBox;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _layoutRoot = e.NameScope.Get<Border>("PART_LayoutRoot");
        }

        protected override void LogicalChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            base.LogicalChildrenCollectionChanged(sender, e);

            _tree = this.FindAncestorOfType<NodeTreeView>();

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

        protected override void OnHeaderDoubleTapped(TappedEventArgs e)
        {
            return;
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            if(e.NavigationMethod == NavigationMethod.Tab)
            {
                ScrollViewer scrollViewer = _tree!.ScrollViewer!;

                Matrix matrix = this.TransformToVisual(scrollViewer)!.Value;
                double offset = matrix.M32 + scrollViewer.Offset.Y - (scrollViewer.Bounds.Height * 0.5) + (_layoutRoot!.Bounds.Height * 0.5);
                scrollViewer.Offset = new(0, offset);
                e.Handled = true;
            }

            base.OnGotFocus(e);
        }
    }
}   
