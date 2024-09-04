using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using PropertyChanged;

namespace J113D.TranslationEditor.ProjectApp.Views.NodeTree
{
    [DoNotNotify]
    [PseudoClasses(":canexpand")]
    internal class NodeTreeViewItem : TreeViewItem
    {
        public static readonly StyledProperty<bool> CanExpandProperty =
            AvaloniaProperty.Register<NodeTreeViewItem, bool>(nameof(CanExpand));

        public bool CanExpand
        {
            get => GetValue(CanExpandProperty);
            set => SetValue(CanExpandProperty, value);
        }


        public NodeTreeViewItem() : base()
        {
            PseudoClasses.Set(":canexpand", CanExpand);
        }


        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == CanExpandProperty)
            {
                PseudoClasses.Set(":canexpand", CanExpand);
            }
        }
    }
}
