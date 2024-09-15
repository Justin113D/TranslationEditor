using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;
using PropertyChanged;

namespace J113D.TranslationEditor.ProjectApp.Views.Help
{
    [DoNotNotify]
    internal class HelpRectangle : Border
    {
        private Layoutable? _currentTargetElement;

        public static readonly StyledProperty<Layoutable?> TargetElementProperty =
            AvaloniaProperty.Register<HelpRectangle, Layoutable?>(nameof(TargetElement));

        public static readonly StyledProperty<Thickness> HelpMarginProperty =
            AvaloniaProperty.Register<HelpRectangle, Thickness>(nameof(HelpMargin));

        public Layoutable? TargetElement
        {
            get => GetValue(TargetElementProperty);
            set => SetValue(TargetElementProperty, value);
        }

        public Thickness HelpMargin
        {
            get => GetValue(HelpMarginProperty);
            set => SetValue(HelpMarginProperty, value);
        }


        static HelpRectangle()
        {
            TargetElementProperty.Changed.AddClassHandler<HelpRectangle>(TargetElementChanged);
        }

        private static void TargetElementChanged(HelpRectangle sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (sender._currentTargetElement != null)
            {
                sender._currentTargetElement.LayoutUpdated -= sender.OnTargetLayoutUpdated;
            }

            sender._currentTargetElement = sender.TargetElement;

            if (sender._currentTargetElement != null)
            {
                sender._currentTargetElement.LayoutUpdated += sender.OnTargetLayoutUpdated;
            }
        }

        private void OnTargetLayoutUpdated(object? sender, System.EventArgs e)
        {
            Canvas? canvas = this.FindAncestorOfType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            TransformedBounds? bounds = _currentTargetElement!.GetTransformedBounds();
            TransformedBounds? canvasBounds = canvas.GetTransformedBounds();

            if (bounds == null || canvasBounds == null)
            {
                return;
            }

            Width = bounds.Value.Bounds.Width + HelpMargin.Left + HelpMargin.Right;
            Height = bounds.Value.Bounds.Height + HelpMargin.Top + HelpMargin.Bottom;
            Canvas.SetLeft(this, bounds.Value.Transform.M31 - canvasBounds.Value.Transform.M31 - HelpMargin.Left);
            Canvas.SetTop(this, bounds.Value.Transform.M32 - canvasBounds.Value.Transform.M32 - HelpMargin.Top);

        }

    }
}
