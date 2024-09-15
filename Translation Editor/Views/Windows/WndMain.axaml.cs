using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PropertyChanged;
using System;

namespace J113D.TranslationEditor.ProjectApp.Views.Windows
{
    [DoNotNotify]
    public partial class WndMain : J113D.Avalonia.Controls.Window
    {
        private bool _ignoreResetConfirmation;

        public WndMain()
        {
            InitializeComponent();
        }

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            if(!_ignoreResetConfirmation && Toolbar.MenuBar.HasFileChanged)
            {
                e.Cancel = true;
                if(await Toolbar.MenuBar.CloseConfirmation())
                {
                    _ignoreResetConfirmation = true;
                    Close();
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Toolbar.MenuBar.HelpWindow?.Close();
            base.OnClosed(e);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            string filepath = ((App)Application.Current!).Settings.StartupFormatFile;
            if(!string.IsNullOrWhiteSpace(filepath))
            {
                Uri fileUri = new(filepath, UriKind.Absolute);
                Toolbar.MenuBar.LoadFormatDirectly(fileUri);
            }
        }

    }
}