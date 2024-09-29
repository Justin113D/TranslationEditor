using Avalonia;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    internal class LanguageExportFileHandler : BaseFileHandler
    {
        protected override bool AskForResetConfirmation => false;

        protected override string FileTypeName
            => "Language export";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Export") {
                Patterns = ["*.langkey"]
            }
        ];


        public bool ExportValuesFileToo { get; set; }


        public LanguageExportFileHandler(Visual visual) : base(visual, null) { }


        protected override void InternalSave(Uri filePath)
        {
            File.WriteAllText(filePath.LocalPath, ViewModel.ExportLanguageKeys());

            if(ExportValuesFileToo)
            {
                string valueFilePath = Path.ChangeExtension(filePath.LocalPath, ".lang");
                File.WriteAllText(valueFilePath, ViewModel.ExportLanguageValues());
            }
        }
    }
}
