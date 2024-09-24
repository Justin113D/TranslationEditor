using J113D.Avalonia.Utilities.IO;
using System;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal sealed class FormatFileAppendHandler : BaseFormatFileHandler
    {
        protected override IFileChangeTracker? FileChangeTracker => null;

        protected override bool AskForResetConfirmation => false;


        public FormatFileAppendHandler(UcMenuBar control) : base(control) { }


        protected override void InternalReset()
        {
            throw new NotSupportedException();
        }

        protected override void ReadText(Uri filePath, string text)
        {
            ViewModel.AppendFormat(text);
        }

        protected override string WriteText(Uri filePath)
        {
            throw new NotSupportedException();
        }

    }
}
