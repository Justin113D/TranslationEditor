using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using PropertyChanged;
using System;

namespace J113D.TranslationEditor.FormatApp.Views.NodeTree
{
    [DoNotNotify]
    [TemplatePart("PART_InsertMarker", typeof(Border), IsRequired = true)]
    internal sealed class NodeTreeView : TreeView
    {
        private NodeTreeViewItem? _movingItem;

        public static readonly StyledProperty<double> NameWidthProperty =
            AvaloniaProperty.Register<NodeTreeView, double>(nameof(NameWidth));

        public static readonly StyledProperty<double> ContentWidthProperty =
            AvaloniaProperty.Register<NodeTreeView, double>(nameof(ContentWidth));

        public static readonly DirectProperty<NodeTreeView, NodeTreeViewItem?> MovingItemProperty =
            AvaloniaProperty.RegisterDirect<NodeTreeView, NodeTreeViewItem?>(
                nameof(MovingItem), 
                o => o.MovingItem);

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

        public NodeTreeViewItem? MovingItem
        {
            get => _movingItem;
            set => SetAndRaise(MovingItemProperty, ref _movingItem, value);
        }

        public Border? InsertMarker { get; private set; }


        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            InsertMarker = e.NameScope.Get<Border>("PART_InsertMarker");
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new NodeTreeViewItem();
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<NodeTreeViewItem>(item, out recycleKey);
        }
    }
}
