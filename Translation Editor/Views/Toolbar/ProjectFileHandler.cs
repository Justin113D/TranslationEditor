using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    internal sealed class ProjectFileHandler : TextFileHandler
    {
        private readonly UcMenuBar _menuBar;

        private MainViewModel ViewModel
            => (MainViewModel)_menuBar.DataContext!;

        protected override Window Window
            => (Window)TopLevel.GetTopLevel(_menuBar)!;


        protected override string FileTypeName 
            => "Language project";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } = 
        [
            new("Language Project") {
                Patterns = ["*.json"]
            }
        ];

        protected override IFileChangeTracker? FileChangeTracker => _menuBar;

        public ProjectFileHandler(UcMenuBar menuBar)
        {
            _menuBar = menuBar;
        }


        protected override void InternalReset()
        {
            ViewModel.NewProject();
        }

        protected override void ReadText(Uri filePath, string text)
        {
            ViewModel.ReadProject(text);
        }

        protected override string WriteText(Uri filePath)
        {
            return ViewModel.WriteProject();
        }

    }
}
