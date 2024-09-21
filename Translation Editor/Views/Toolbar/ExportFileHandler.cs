using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    internal sealed class ExportFileHandler : TextFileHandler
    {
        private readonly UcMenuBar _control;

        private MainViewModel ViewModel
            => (MainViewModel)_control.DataContext!;

        protected override Window Window
            => (Window)TopLevel.GetTopLevel(_control)!;


        protected override string FileTypeName 
            => "Language export";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Export") {
                Patterns = ["*.lang"]
            }
        ];

        protected override IFileChangeTracker? FileChangeTracker => null;

        public ExportFileHandler(UcMenuBar control)
        {
            _control = control;
        }


        protected override void InternalReset()
        {
            throw new NotSupportedException();
        }

        protected override void ReadText(Uri filePath, string text)
        {
            throw new NotSupportedException();
        }

        protected override string WriteText(Uri filePath)
        {
            return ViewModel.ExportLanguage();
        }

    }
}
