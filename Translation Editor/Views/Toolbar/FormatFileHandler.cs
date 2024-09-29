using Avalonia;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    internal sealed class FormatFileHandler : BaseFileHandler
    {
        protected override string FileTypeName
            => "Language format";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Format") {
                Patterns = ["*.json"]
            }
        ];


        public FormatFileHandler(Visual visual, IFileChangeTracker fileChangeTracker) : base(visual, fileChangeTracker) { }


        protected override void InternalLoad(Uri filePath)
        {
            ViewModel.LoadFormat(File.ReadAllText(filePath.LocalPath));
        }
    }
}
