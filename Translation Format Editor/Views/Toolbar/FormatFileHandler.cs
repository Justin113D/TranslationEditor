﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal sealed class FormatFileHandler : TextFileHandler
    {
        private readonly UcMenuBar _control;

        private MainViewModel ViewModel
            => (MainViewModel)_control.DataContext!;

        protected override Window Window
            => (Window)TopLevel.GetTopLevel(_control)!;


        protected override string FileTypeName
            => "Language format";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } =
        [
            new("Language Format") {
                Patterns = ["*.json"]
            }
        ];

        protected override IFileChangeTracker? FileChangeTracker => _control;


        public FormatFileHandler(UcMenuBar control)
        {
            _control = control;
        }


        protected override void InternalReset()
        {
            ViewModel.NewFormat();
        }

        protected override void ReadText(Uri filePath, string text)
        {
            ViewModel.OpenFormat(text);
        }

        protected override string WriteText(Uri filePath)
        {
            return ViewModel.SaveFormat(((App)Application.Current!).Settings.IndentJson);
        }

    }
}