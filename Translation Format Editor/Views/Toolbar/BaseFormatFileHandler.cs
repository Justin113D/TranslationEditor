using Avalonia;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal abstract class BaseFormatFileHandler : BaseFileHandler
    {
        protected override string FileTypeName
            => "Language format";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Format") {
                Patterns = ["*.json"]
            }
        ];


        protected BaseFormatFileHandler(Visual visual, IFileChangeTracker? fileChangeTracker) 
            : base(visual, fileChangeTracker) { }
    }
}
