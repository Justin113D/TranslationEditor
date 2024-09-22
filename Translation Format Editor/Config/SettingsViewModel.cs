using J113D.Avalonia.Theme;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace J113D.TranslationEditor.FormatApp.Config
{
    internal sealed class SettingsViewModel : ViewModelBase
    {
        private string _fontSizeText;
        private string _undoRedoLimitText;
        private readonly Settings _settings;

        public ObservableCollection<J113DThemeVariant> Themes { get; }

        public J113DThemeVariant Theme
        {
            get => _settings.Theme;
            set =>_settings.Theme = value;
        }

        public string FontSizeText
        {
            get => _fontSizeText;
            set
            {
                if (!int.TryParse(value, out int fontSize))
                {
                    throw new ArgumentException("Text has to be a number!");
                }

                if (fontSize < 10)
                {
                    throw new ArgumentException("Font size too tiny! Needs to be at least 10");
                }
                else if (fontSize >= 48)
                {
                    throw new ArgumentException("Font size too large! Needs to be below 48");
                }

                _settings.FontSize = fontSize;
                _fontSizeText = fontSize.ToString();
                InvokePropertyChanged(nameof(FontSize));
            }
        }

        public int FontSize
        {
            get => _settings.FontSize;
            set
            {
                _settings.FontSize = value;
                _fontSizeText = value.ToString();
                InvokePropertyChanged(nameof(FontSizeText));
            }
        }

        public string UndoRedoLimitText
        {
            get => _undoRedoLimitText;
            set
            {
                if(!int.TryParse(value, out int undoRedoLimit))
                {
                    throw new ArgumentException("Text has to be a number!");
                }

                if(undoRedoLimit < 1)
                {
                    throw new ArgumentException("Undo/Redo Limit too low! Needs to be at least 1");
                }
                else if(undoRedoLimit > 1000)
                {
                    throw new ArgumentException("Undo/Redo limit too large! Maximum possible value is 1000");
                }

                _settings.UndoRedoLimit = undoRedoLimit;
                _undoRedoLimitText = undoRedoLimit.ToString();
                InvokePropertyChanged(nameof(UndoRedoLimit));
            }
        }

        public int UndoRedoLimit
        {
            get => _settings.UndoRedoLimit;
            set
            {
                _settings.UndoRedoLimit = value;
                _undoRedoLimitText = value.ToString();
                InvokePropertyChanged(nameof(UndoRedoLimitText));
            }
        }

        public bool IndentJson
        {
            get => _settings.IndentJson;
            set => _settings.IndentJson = value;
        }

        public SettingsViewModel()
        {
            _settings = new();
            Themes = new(Enum.GetValues<J113DThemeVariant>());

            _settings.Reset();
            _fontSizeText = _settings.FontSize.ToString();
            _undoRedoLimitText = _settings.UndoRedoLimit.ToString();
        }

        public void Load()
        {
            _settings.Load();
            _fontSizeText = _settings.FontSize.ToString();
            _undoRedoLimitText = _settings.UndoRedoLimit.ToString();
            InvokePropertyChanged(nameof(Theme));
            InvokePropertyChanged(nameof(FontSizeText));
            InvokePropertyChanged(nameof(FontSize));
            InvokePropertyChanged(nameof(UndoRedoLimitText));
            InvokePropertyChanged(nameof(UndoRedoLimit));
            InvokePropertyChanged(nameof(IndentJson));
        }

        public void Save()
        {
            _settings.Save();
        }
    }
}
