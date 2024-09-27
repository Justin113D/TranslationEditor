using Avalonia;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    internal class CSVExportFileHandler : BaseFileHandler
    {
        protected override bool AskForResetConfirmation => false;

        protected override string FileTypeName
            => "CSV File";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("CSV File") {
                Patterns = ["*.csv"]
            }
        ];


        public bool IncludeFormatValues { get; set; }

        public string FilePaths { get; set; } = string.Empty;


        public CSVExportFileHandler(Visual visual) : base(visual, null)
        {
            FilePaths = string.Empty;
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
