using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
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

        private static double GetEndOffset(Visual? from, Visual to)
        {
            if(from == null)
            {
                return 0;
            }

            Matrix matrix = from.TransformToVisual(to)!.Value;
            return matrix.M32 + (from.Bounds.Size.Height * matrix.M22);
        }

        public void ToggleDropArea()
        {
            NodeTreeViewItem? insertParent = GetDropParent(out NodeTreeViewItem? insertAfter);
            Border insertMarker = Tree!.InsertMarker!;
            Panel markerArea = insertMarker.GetLogicalParent<Panel>()!;

            double indent = ((insertParent?.Level ?? 0) * 16) + 10;
            double insertAfterEnd = GetEndOffset(insertAfter, markerArea);
            double insertParentEnd = GetEndOffset(insertParent?.ItemArea, markerArea);

            if(insertParent != null)
            {
                insertMarker.Margin = new(indent, insertParentEnd, 0, 0);

                if(insertAfter != null)
                {
                    insertMarker.Height = insertAfterEnd - insertParentEnd - 2;
                }
                else
                {
                    insertMarker.Height = 5;
                }
            }
            else if(insertAfter != null)
            {
                insertMarker.Margin = new(indent, insertAfterEnd - 5, 0, 0);
                insertMarker.Height = double.NaN;
            }
            else
            {
                insertMarker.Margin = new(0, 2, 0, 0);
                insertMarker.Height = double.NaN;
            }

            insertMarker.BorderBrush = insertParent?.ViewModel.PartOfSelectedBranch == true
                ? InsertMarkerBrushError
                : InsertMarkerBrushValid;
        }

        public NodeTreeViewItem? GetDropParent(out NodeTreeViewItem? after)
        {
            NodeTreeViewItem? dropParent = Item.FindLogicalAncestorOfType<NodeTreeViewItem>();
            after = null;

            switch(InsertRegionType)
            {
                case InsertRegionType.Above:
                    NodeTreeViewItem[] siblings = Item!.GetLogicalSiblings().OfType<NodeTreeViewItem>().ToArray();
                    int index = Array.IndexOf(siblings, Item);

                    if(index > 0)
                    {
                        after = siblings[index - 1];
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
                    dropParent = Item;
                    break;
                case InsertRegionType.Below:
                    after = Item;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return dropParent;
        }
    }
}
