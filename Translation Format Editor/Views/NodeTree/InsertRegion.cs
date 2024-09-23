using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using J113D.TranslationEditor.FormatApp.ViewModels;
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
            ParentNodeViewModel insertTarget = GetInsertTarget(out NodeViewModel? insertAfter);

            NodeTreeViewItem? insertTargetContainer = insertTarget.Parent == null ? null : (NodeTreeViewItem)Tree!.ContainerFromItem(insertTarget)!;
            NodeTreeViewItem? insertAfterContainer = insertAfter == null ? null : (NodeTreeViewItem)Tree!.ContainerFromItem(insertAfter)!;

            Border insertMarker = Tree!.InsertMarker!;
            Panel markerArea = insertMarker.GetLogicalParent<Panel>()!;

            double indent = ((insertTargetContainer?.Level ?? 0) * 16) + 10;
            double insertAfterEnd = GetEndOffset(insertAfterContainer, markerArea);
            double insertParentEnd = GetEndOffset(insertTargetContainer?.ItemArea, markerArea);

            if(insertTargetContainer != null)
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
            else if(insertAfterContainer != null)
            {
                insertMarker.Margin = new(indent, insertAfterEnd - 5, 0, 0);
                insertMarker.Height = double.NaN;
            }
            else
            {
                insertMarker.Margin = new(indent, 2, 0, 0);
                insertMarker.Height = double.NaN;
            }

            insertMarker!.IsVisible = true;
            insertMarker.BorderBrush = insertTarget.PartOfSelectedBranch == true
                ? InsertMarkerBrushError
                : InsertMarkerBrushValid;
        }

        public ParentNodeViewModel GetInsertTarget(out NodeViewModel? after)
        {
            ParentNodeViewModel insertParent = Item!.ViewModel.Parent!;
            after = null;

            switch(InsertRegionType)
            {
                case InsertRegionType.Above:
                    int index = insertParent.ChildNodes!.IndexOf(Item.ViewModel);
                    if(index > 0)
                    {
                        after = insertParent.ChildNodes[index - 1];
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
                    insertParent = (ParentNodeViewModel)Item.ViewModel;
                    break;

                case InsertRegionType.Below:
                    after = Item.ViewModel;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return insertParent;
        }
    }
}
