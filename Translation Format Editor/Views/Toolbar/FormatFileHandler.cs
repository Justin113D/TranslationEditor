﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using J113D.TranslationEditor.FormatApp.ViewModels;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal sealed class FormatFileHandler : BaseFormatFileHandler
    {
        public FormatFileHandler(UcMenuBar control) : base(control) { }


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
