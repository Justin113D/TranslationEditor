using Avalonia.Controls;
using Avalonia.Input;
using PropertyChanged;

namespace J113D.TranslationEditor.FormatApp.Views.NodeTree
{
    [DoNotNotify]
    internal sealed class NodeTreeViewItem : TreeViewItem
    {
        protected override void OnHeaderDoubleTapped(TappedEventArgs e)
        {
            return;
        }
    }
}
