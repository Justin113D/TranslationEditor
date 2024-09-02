using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PropertyChanged;
using J113D.Avalonia.Theme;
using J113D.TranslationEditor.ProjectApp.Config;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using J113D.TranslationEditor.ProjectApp.Views.Windows;

namespace J113D.TranslationEditor.ProjectApp
{
    [DoNotNotify]
    public partial class App : Application
    {
        public SettingsViewModel Settings { get; }

        public App()
        {
            Settings = new();
            Settings.PropertyChanged += OnSettingChanged;
        }

        private void OnSettingChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(SettingsViewModel.FontSize):
                    Resources["AppFontSize"] = (double)Settings.FontSize;
                    break;
                case nameof(SettingsViewModel.Theme):
                    Settings.Theme.ApplyTheme(this);
                    break;
                case nameof(SettingsViewModel.UndoRedoLimit):
                    if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop 
                        && desktop.MainWindow?.DataContext is MainViewModel vm)
                    {
                        vm.ProjectTracker.ChangeLimit = Settings.UndoRedoLimit;
                    }

                    break;
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Settings.Load();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainViewModel vm = new();
                vm.ProjectTracker.ChangeLimit = Settings.UndoRedoLimit;

                desktop.MainWindow = new WndMain
                {
                    DataContext = vm,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}