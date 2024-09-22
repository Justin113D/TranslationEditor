﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using J113D.Avalonia.Theme.Accents;
using J113D.TranslationEditor.FormatApp.ViewModels;
using PropertyChanged;
using System.Linq;

namespace J113D.TranslationEditor.FormatApp.Views.NodeTree
{
    [DoNotNotify]
    [TemplatePart("PART_Grab", typeof(Border), IsRequired = true)]
    [TemplatePart("PART_InsertInsideMarker", typeof(Border), IsRequired = true)]
    [TemplatePart("PART_InsertBelowMarker", typeof(Border), IsRequired = true)]
    [PseudoClasses(":nodeselected")]
    internal sealed class NodeTreeViewItem : TreeViewItem
    {
        public static readonly StyledProperty<bool> InsertModeProperty =
            AvaloniaProperty.Register<NodeTreeViewItem, bool>(nameof(InsertMode));

        public static readonly StyledProperty<bool> NodeSelectedProperty =
            AvaloniaProperty.Register<NodeTreeViewItem, bool>(nameof(NodeSelected));

        private NodeTreeView? _tree;
        private Border? _grab;
        private InsertRegion? _currentDropZone;
        private Point? _dragStartPosition;

        public bool InsertMode
        {
            get => GetValue(InsertModeProperty);
            set => SetValue(InsertModeProperty, value);
        }

        public bool NodeSelected
        {
            get => GetValue(NodeSelectedProperty);
            set => SetValue(NodeSelectedProperty, value);
        }

        public Border? InsertInsideMarker { get; private set; }

        public Border? InsertBelowMarker { get; private set; }

        public NodeViewModel ViewModel => (NodeViewModel)DataContext!;

        static NodeTreeViewItem()
        {
            NodeSelectedProperty.Changed.AddClassHandler<NodeTreeViewItem>(NodeSelectedChanged);
        }

        private static void NodeSelectedChanged(NodeTreeViewItem sender, AvaloniaPropertyChangedEventArgs e)
        {
            if(sender.NodeSelected)
            {
                sender.PseudoClasses.Add(":nodeselected");
            }
            else
            {
                sender.PseudoClasses.Remove(":nodeselected");
            }
        }

        protected override void OnHeaderDoubleTapped(TappedEventArgs e) { }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            _tree = this.FindLogicalAncestorOfType<NodeTreeView>();
            Bind(InsertModeProperty, _tree!.GetObservable(NodeTreeView.MovingItemProperty, x => x != null));
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _grab = e.NameScope.Get<Border>("PART_Grab");

            InsertInsideMarker = e.NameScope.Get<Border>("PART_InsertInsideMarker");
            InsertBelowMarker = e.NameScope.Get<Border>("PART_InsertBelowMarker");

            _grab.PointerPressed += OnGrabPressed;
            _grab.PointerReleased += OnGrabReleased;
            _grab.PointerMoved += OnGrabMoved;
        }

        private void OnGrabMoved(object? sender, PointerEventArgs e)
        {
            Point position = e.GetPosition(_tree);

            if(_dragStartPosition != null)
            {
                double distance = Point.Distance(position, _dragStartPosition.Value);
                if(distance > 10)
                {
                    _tree!.MovingItem = this;
                    _dragStartPosition = null;

                    switch(e.KeyModifiers)
                    {
                        case KeyModifiers.Control:
                            if(!ViewModel.Selected)
                            {
                                ViewModel.SelectMulti();
                            }

                            break;
                        case KeyModifiers.Shift:
                            ViewModel.SelectSequence();
                            break;
                        default:
                            if(!ViewModel.Selected)
                            {
                                ViewModel.SelectSingle();
                            }

                            break;
                    }
                }
            }

            if(_tree!.MovingItem != this)
            {
                return;
            }

            InsertRegion? insertRegion = _tree.GetVisualsAt(position).OfType<InsertRegion>().FirstOrDefault();
            if(_currentDropZone == insertRegion)
            {
                return;
            }

            _currentDropZone?.ToggleDropArea(false);
            insertRegion?.ToggleDropArea(true);
            _currentDropZone = insertRegion;
        }

        private void OnGrabReleased(object? sender, PointerReleasedEventArgs e)
        {
            if(_dragStartPosition != null)
            {
                switch(e.KeyModifiers)
                {
                    case KeyModifiers.Control:
                        ViewModel.SelectMulti();
                        break;
                    case KeyModifiers.Shift:
                        ViewModel.SelectSequence();
                        break;
                    default:
                        ViewModel.SelectSingle();
                        break;
                }

                _dragStartPosition = null;
                return;
            }

            if(_tree!.MovingItem != this)
            {
                return;
            }

            _currentDropZone?.ToggleDropArea(false);
            _currentDropZone = null;
            _tree!.MovingItem = null;
        }

        private void OnGrabPressed(object? sender, PointerPressedEventArgs e)
        {
            _dragStartPosition = e.GetPosition(_tree);
        }
    }
}
