using Avalonia.Interactivity;
using PropertyChanged;

namespace J113D.TranslationEditor.ProjectApp.Views.Windows
{
    [DoNotNotify]
    internal partial class WndHelp : J113D.Avalonia.Controls.Window
    {
        public WndHelp()
        {
            InitializeComponent();
        }

        private void OnLinkClicked(object? sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new("https://github.com/Justin113D/TranslationEditor"));
        }
    }
}