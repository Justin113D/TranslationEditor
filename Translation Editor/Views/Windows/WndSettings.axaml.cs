using Avalonia;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.Interactivity;
using PropertyChanged;
using J113D.TranslationEditor.ProjectApp.Config;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.ProjectApp.Views.Windows
{
    [DoNotNotify]
    public partial class WndSettings : J113D.Avalonia.Controls.Window
    {
        public WndSettings()
        {
            DataContext = ((App)Application.Current!).Settings;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            ((App)Application.Current!).Settings.Save();
            base.OnClosed(e);
        }

        private void OnFileButtonClicked(object? sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new()
                {
                    Title = $"Select startup format file",
                    AllowMultiple = false,
                    FileTypeFilter = [ new("Language Format") { Patterns = ["*.json"] }]
                });

                if(files == null || files.Count == 0)
                {
                    return;
                }

                ((SettingsViewModel)DataContext!).StartupFormatFile = files[0].Path.AbsolutePath;
            });
        }

        private void OnCloseClicked(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
