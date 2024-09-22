using Avalonia.Controls;
using PropertyChanged;

namespace J113D.TranslationEditor.FormatApp.Views.NodeTree
{
    [DoNotNotify]
    internal sealed class NodeTreeViewItem : TreeViewItem
    {
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
