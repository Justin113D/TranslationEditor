using Avalonia;
using Avalonia.Controls;
using PropertyChanged;

namespace J113D.TranslationEditor.ProjectApp.Views.NodeTree
{
    [DoNotNotify]
    internal class NodeTreeView : TreeView
    {

        public static readonly StyledProperty<double> NameWidthProperty =
            AvaloniaProperty.Register<NodeTreeView, double>(nameof(NameWidth));

        public static readonly StyledProperty<double> ContentWidthProperty =
            AvaloniaProperty.Register<NodeTreeView, double>(nameof(ContentWidth));

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
    }
}
