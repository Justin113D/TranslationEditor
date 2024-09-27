using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using PropertyChanged;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    [DoNotNotify]
    internal partial class WndCSVExportDialog : J113D.Avalonia.Controls.Window
    {
        private readonly CSVExportFileHandler _csvExportFileHandler;

        public WndCSVExportDialog()
        {
            _csvExportFileHandler = new(this);
            InitializeComponent();
        }

        public void OnExport(object sender, RoutedEventArgs e)
        {
            _csvExportFileHandler.IncludeFormatValues = ExportFormatValues.IsChecked == true;
            _csvExportFileHandler.FilePaths = ProjectFilePaths.Text ?? string.Empty;

            Dispatcher.UIThread.Post(async () =>
            {
                if(await _csvExportFileHandler.Save(true))
                {
                    Close();
                }
            });
        }

        private void OnAddProjectFiles(object? sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new()
                {
                    Title = $"Select language projects to add",
                    AllowMultiple = true,
                    FileTypeFilter = [new FilePickerFileType("Language Project") { Patterns = ["*.json"] }]
                });

                string filepaths = string.Empty;
                foreach(IStorageFile file in files)
                {
                    filepaths += file.Path.AbsolutePath;
                    filepaths += Environment.NewLine;
                }

                if(!string.IsNullOrEmpty(filepaths))
                {
                    ProjectFilePaths.Text = filepaths + ProjectFilePaths.Text ?? string.Empty;
                }
            });
        }
    }
}