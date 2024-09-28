using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace J113D.TranslationEditor.FormatApp.Views.Help
{
    internal sealed class FontSizeMultiplicatorConverter : IValueConverter
    {
        public double Factor { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            switch(value)
            {
                case TemplatedControl:
                    return ((TemplatedControl)value).FontSize * Factor;
                case TextBlock:
                    return ((TextBlock)value).FontSize * Factor;
                default:
                    return 0;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
