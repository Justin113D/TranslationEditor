using Avalonia.Controls;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    internal abstract class BaseExportFileHandler : TextFileHandler
    {
        protected override Window Window { get; }

        protected MainViewModel ViewModel
            => (MainViewModel)Window.DataContext!;

        protected override IFileChangeTracker? FileChangeTracker => null;

        protected override bool AskForResetConfirmation => false;


        public BaseExportFileHandler(Window window) 
        {
            Window = window;
        }


        protected override void InternalReset()
        {
            throw new NotSupportedException();
        }

        protected override void ReadText(Uri filePath, string text)
        {
            throw new NotSupportedException();
        }
    }
}
