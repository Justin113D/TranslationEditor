using Avalonia;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    internal sealed class ImportProjectFileHandler : BaseFileHandler
    {
        protected override bool AskForResetConfirmation => false;

        protected override string FileTypeName
            => "Language project";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Project") {
                Patterns = ["*.json"]
            }
        ];


        public ImportProjectFileHandler(Visual visual) : base(visual, null) { }


        protected override void InternalLoad(Uri filePath)
        {
            ViewModel.ImportProjectValues(File.ReadAllText(filePath.LocalPath));
        }
    }
}
