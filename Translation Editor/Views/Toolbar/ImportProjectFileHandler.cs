using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    internal class ImportProjectFileHandler : TextFileHandler
    {
        private readonly UcMenuBar _control;

        private MainViewModel ViewModel
            => (MainViewModel)_control.DataContext!;

        protected override Window Window
            => (Window)TopLevel.GetTopLevel(_control)!;


        protected override string FileTypeName
            => "Language project";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Project") {
                Patterns = ["*.json"]
            }
        ];

        protected override bool AskForResetConfirmation => false;

        protected override IFileChangeTracker? FileChangeTracker => null;

        public ImportProjectFileHandler(UcMenuBar control)
        {
            _control = control;
        }

        protected override string WriteText(Uri filePath)
        {
            throw new NotSupportedException();
        }

        protected override void ReadText(Uri filePath, string text)
        {
            ViewModel.ImportProjectValues(text);
        }

        protected override void InternalReset()
        {
            throw new NotSupportedException();
        }
    }
}
