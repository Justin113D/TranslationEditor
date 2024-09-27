using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    internal class LanguageExportFileHandler : BaseFileHandler
    {
        protected override string FileTypeName
            => "Language export";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Export") {
                Patterns = ["*.langkey"]
            }
        ];

        protected override Window Window { get; }

        private MainViewModel ViewModel
            => (MainViewModel)Window.DataContext!;

        protected override IFileChangeTracker? FileChangeTracker => null;


        public LanguageExportFileHandler(Window window) : base()
        {
            Window = window;
        }

        public bool ExportValuesFileToo { get; set; }

        protected override void InternalLoad(Uri filePath)
        {
            throw new NotSupportedException();
        }

        protected override void InternalReset()
        {
            throw new NotSupportedException();
        }

        protected override void InternalSave(Uri filePath)
        {
            File.WriteAllText(filePath.AbsolutePath, ViewModel.ExportLanguageKeys());

            if(ExportValuesFileToo)
            {
                string valueFilePath = Path.ChangeExtension(filePath.AbsolutePath, ".lang");
                File.WriteAllText(valueFilePath, ViewModel.ExportLanguageValues());
            }
        }
    }
}
