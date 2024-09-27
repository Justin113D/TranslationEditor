using Avalonia;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.ProjectApp.Views.Export
{
    internal sealed class LanguageExportFileHandler : BaseFileHandler
    {
        protected override bool AskForResetConfirmation => false;

        protected override string FileTypeName
            => "Language export";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Export") {
                Patterns = ["*.lang"]
            }
        ];


        public LanguageExportFileHandler(Visual visual) : base(visual, null) { }


        protected override void InternalSave(Uri filePath)
        {
            File.WriteAllText(filePath.AbsolutePath, ViewModel.ExportLanguage());
        }
    }
}
