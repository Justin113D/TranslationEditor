using J113D.Common;
using J113D.Avalonia.Theme;
using System;
using System.IO;
using System.Text.Json;

namespace J113D.TranslationEditor.ProjectApp.Config
{
    internal sealed class Settings : BaseSettings
    {
        public J113DThemeVariant Theme
        {
            get => (J113DThemeVariant)this[nameof(Theme)];
            set => this[nameof(Theme)] = value;
        }

        public int FontSize
        {
            get => (int)this[nameof(FontSize)];
            set => this[nameof(FontSize)] = int.Clamp(value, 10, 47);
        }

        public int UndoRedoLimit
        {
            get => (int)this[nameof(UndoRedoLimit)];
            set => this[nameof(UndoRedoLimit)] = int.Clamp(value, 1, 1000);
        }

        public string StartupFormatFile
        {
            get => (string)this[nameof(StartupFormatFile)];
            set => this[nameof(StartupFormatFile)] = value;
        }

        public Settings() : base() { }

        public override void Reset()
        {
            Theme = J113DThemeVariant.Dark;
            FontSize = 14;
            StartupFormatFile = string.Empty;
            UndoRedoLimit = 100;
        }

        protected override object ConvertValue(string name, JsonElement value)
        {
            return name switch
            {
                nameof(Theme) => Enum.Parse<J113DThemeVariant>(value.GetString()!),
                nameof(FontSize) => value.GetInt32(),
                nameof(UndoRedoLimit) => value.GetInt32(),
                nameof(StartupFormatFile) => value.GetString()!,
                _ => throw new InvalidDataException(),
            };
        }

    }
}
