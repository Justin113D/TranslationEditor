using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    internal class CSVExportFileHandler : BaseFileHandler
    {
        protected override string FileTypeName
            => "CSV File";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("CSV File") {
                Patterns = ["*.csv"]
            }
        ];

        protected override Window Window { get; }

        private MainViewModel ViewModel
            => (MainViewModel)Window.DataContext!;

        protected override IFileChangeTracker? FileChangeTracker => null;


        public bool IncludeFormatValues { get; set; }

        public string FilePaths { get; set; }


        public CSVExportFileHandler(Window window) : base()
        {
            Window = window;
            FilePaths = string.Empty;
        }

        protected override void InternalLoad(Uri filePath)
        {
            throw new NotSupportedException();
        }

        protected override void InternalReset()
        {
            throw new NotSupportedException();
        }

        protected override void InternalSave(Uri filePath)
        {
            string[] fileContents = FilePaths
                .Split([Environment.NewLine, "\n\r", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries)
                .Select(File.ReadAllText)
                .ToArray();

            string csv = ViewModel.ExportCSV(IncludeFormatValues, fileContents);
            File.WriteAllText(filePath.AbsolutePath, csv);
        }
    }
}
