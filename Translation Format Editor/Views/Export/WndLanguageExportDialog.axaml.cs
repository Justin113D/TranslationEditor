using Avalonia.Interactivity;
using Avalonia.Threading;
using PropertyChanged;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    [DoNotNotify]
    internal partial class WndLanguageExportDialog : J113D.Avalonia.Controls.Window
    {
        private readonly LanguageExportFileHandler _languageExportFileHandler;

        public WndLanguageExportDialog()
        {
            _languageExportFileHandler = new(this);
            InitializeComponent();
        }

        public void OnExport(object sender, RoutedEventArgs e)
        {
            _languageExportFileHandler.ExportValuesFileToo = ExportValueFile.IsChecked == true;

            Dispatcher.UIThread.Post(async () =>
            {
                if(await _languageExportFileHandler.Save(true))
                {
                    Close();
                }
            });
        }
    }
}