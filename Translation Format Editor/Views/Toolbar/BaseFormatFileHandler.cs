using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal abstract class BaseFormatFileHandler : TextFileHandler
    {
        private readonly UcMenuBar _control;

        protected MainViewModel ViewModel
            => (MainViewModel)_control.DataContext!;

        protected override Window Window
            => (Window)TopLevel.GetTopLevel(_control)!;

        protected override IFileChangeTracker? FileChangeTracker => _control;

        protected override string FileTypeName
            => "Language format";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Format") {
                Patterns = ["*.json"]
            }
        ];

        public BaseFormatFileHandler(UcMenuBar control)
        {
            _control = control;
        }

    }
}
