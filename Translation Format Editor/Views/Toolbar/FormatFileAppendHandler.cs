using Avalonia;
using System;
using System.IO;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    internal sealed class FormatFileAppendHandler : BaseFormatFileHandler
    {
        protected override bool AskForResetConfirmation => false;


        public FormatFileAppendHandler(Visual visual) : base(visual, null) { }


        protected override void InternalLoad(Uri filePath)
        {
            ViewModel.AppendFormat(File.ReadAllText(filePath.LocalPath));
        }
    }
}
