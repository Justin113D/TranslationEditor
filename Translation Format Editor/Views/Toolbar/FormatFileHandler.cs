using Avalonia;
using J113D.Avalonia.Utilities.IO;
using System;
using System.IO;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal sealed class FormatFileHandler : BaseFormatFileHandler
    {
        public FormatFileHandler(Visual control, IFileChangeTracker tracker) : base(control, tracker) { }


        protected override void InternalReset()
        {
            ViewModel.NewFormat();
        }

        protected override void InternalLoad(Uri filePath)
        {
            ViewModel.OpenFormat(File.ReadAllText(filePath.LocalPath));
        }

        protected override void InternalSave(Uri filePath)
        {
            string text = ViewModel.SaveFormat(((App)Application.Current!).Settings.IndentJson);
            File.WriteAllText(filePath.LocalPath, text);
        }
    }
}
