using Avalonia.Interactivity;
using Avalonia.Threading;
using PropertyChanged;
using J113D.Avalonia.Utilities.Enum;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    [DoNotNotify]
    internal partial class WndXamlExportDialog : J113D.Avalonia.Controls.Window
    {
        private readonly XAMLExportFileHandler _xamlExportFileHandler;

        public WndXamlExportDialog()
        {
            _xamlExportFileHandler = new(this);

            InitializeComponent();

            ExportType.ItemsSource = EnumUtils.ToDescriptions<XAMLType>();
            ExportType.SelectedIndex = 0;
        }

        public void OnExport(object sender, RoutedEventArgs e)
        {
            _xamlExportFileHandler.Grouped = GroupedCheckbox.IsChecked == true;
            _xamlExportFileHandler.ExportType = (XAMLType)((EnumDescription)ExportType.SelectedItem!).Value;

            Dispatcher.UIThread.Post(async () =>
            {
                if(await _xamlExportFileHandler.Save(true))
                {
                    Close();
                }
            });
        }
    }
}