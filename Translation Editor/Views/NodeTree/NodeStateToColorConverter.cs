using Avalonia.Data.Converters;
using Avalonia.Media;
using J113D.TranslationEditor.Data;
using System;
using System.Globalization;
using Colors = J113D.Avalonia.Theme.Accents.Colors;

namespace J113D.TranslationEditor.ProjectApp.Views.NodeTree
{
    internal class NodeStateToColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush _green = new(Colors.Green);
        private static readonly SolidColorBrush _yellow = new(Colors.Yellow);
        private static readonly SolidColorBrush _red = new(Colors.Red);

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is not NodeState state)
            {
                return Brushes.Transparent;
            }

            return state switch
            {
                NodeState.Outdated => _yellow,
                NodeState.Untranslated => _red,
                NodeState.Translated => _green,
                _ => Brushes.Transparent,
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
