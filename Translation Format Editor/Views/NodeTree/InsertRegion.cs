using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using PropertyChanged;
using System;
using System.Linq;

namespace J113D.TranslationEditor.FormatApp.Views.NodeTree
{
    internal enum InsertRegionType
    {
        Above,
        After,
        Inside,
        Below
    }

    [DoNotNotify]
    internal class InsertRegion : Border
    {
        public static SolidColorBrush InsertMarkerBrushValid = new(J113D.Avalonia.Theme.Accents.Colors.Green);
        public static SolidColorBrush InsertMarkerBrushError = new(J113D.Avalonia.Theme.Accents.Colors.Red);

        public static readonly StyledProperty<InsertRegionType> InsertRegionTypeProperty =
            AvaloniaProperty.Register<InsertRegion, InsertRegionType>(nameof(InsertRegionType));

        protected override Type StyleKeyOverride => typeof(Border);

        public NodeTreeViewItem? Item { get; private set; }
        public NodeTreeView? Tree { get; private set; }

        public InsertRegionType InsertRegionType
        {
            get => GetValue(InsertRegionTypeProperty);
            set => SetValue(InsertRegionTypeProperty, value);
        }

        public InsertRegion() : base()
        {
            IsHitTestVisible = false;
            Background = Brushes.Transparent;
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            Item = this.FindLogicalAncestorOfType<NodeTreeViewItem>();
            Tree = this.FindLogicalAncestorOfType<NodeTreeView>();
        }


        public void ToggleDropArea(bool show)
        {
            Border dropAreaMarker;
            NodeTreeViewItem? dropParent = Item.FindLogicalAncestorOfType<NodeTreeViewItem>();

            switch(InsertRegionType)
            {
                case InsertRegionType.Above:
                    NodeTreeViewItem[] siblings = Item!.GetLogicalSiblings().OfType<NodeTreeViewItem>().ToArray();
                    int index = Array.IndexOf(siblings, Item);

                    if(index > 0)
                    {
                        dropAreaMarker = siblings[index - 1].InsertBelowMarker!;

                    }
                    else if(dropParent != null)
                    {
                        dropAreaMarker = dropParent.InsertInsideMarker!;
                    }
                    else
                    {
                        dropAreaMarker = Tree!.InsertAtRootMarker!;
                        dropParent = null;
                    }

                    break;
                case InsertRegionType.After:
                    if(Item!.IsExpanded)
                    {
                        goto case InsertRegionType.Inside;
                    }
                    else
                    {
                        goto case InsertRegionType.Below;
                    }
                case InsertRegionType.Inside:
                    dropAreaMarker = Item!.InsertInsideMarker!;
                    dropParent = Item;
                    break;
                case InsertRegionType.Below:
                    dropAreaMarker = Item!.InsertBelowMarker!;
                    break;
                default:
                    return;
            }

            if(!show)
            {
                dropAreaMarker.Background = Brushes.Transparent;
            }
            else if(dropParent == null)
            {
                dropAreaMarker.Background = InsertMarkerBrushValid;
            }
            else
            {
                dropAreaMarker.Background = dropParent.ViewModel.PartOfSelectedBranch 
                    ? InsertMarkerBrushError 
                    : InsertMarkerBrushValid;
            }
        }
    }
}
