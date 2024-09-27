using Avalonia;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.ProjectApp.Views.Export
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

        public CSVExportFileHandler(Visual visual) : base(visual, null) { }


        protected override void InternalSave(Uri filePath)
        {
            File.WriteAllText(filePath.AbsolutePath, ViewModel.ExportCSV());
        }
    }
}
