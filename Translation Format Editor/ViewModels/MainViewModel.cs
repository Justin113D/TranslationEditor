using J113D.TranslationEditor.Data;
using J113D.UndoRedo;
using System;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public ChangeTracker FormatTracker { get; }

        public FormatViewModel Format { get; private set; }


        public string? Message { get; private set; }

        public ToolbarMessageType MessageType { get; private set; }


        public MainViewModel()
        {
            FormatTracker = new();
            FormatTracker.UseTracker();
            Format = new(new());
        }


        private void SetMessage(string message, bool warning)
        {
            if(MessageType != ToolbarMessageType.None)
            {
                MessageType = ToolbarMessageType.None;
                Message = null;
            }

            Message = message;
            MessageType = warning ? ToolbarMessageType.Error : ToolbarMessageType.Success;
        }

        public void Undo()
        {
            if(FormatTracker.Undo())
            {
                SetMessage("Performed Undo", false);
            }
        }

        public void Redo()
        {
            if(FormatTracker.Redo())
            {
                SetMessage("Performed Redo", false);
            }
        }


        public void NewFormat()
        {
            FormatTracker.Reset();
            Format = new(new());
            SetMessage("Created new Format", false);
        }

        public void OpenFormat(string format)
        {
            try
            {
                Format headerNode = Data.Format.ReadFormatFromString(format);

                Format = new(headerNode);
                FormatTracker.Reset();

                SetMessage("Loaded Format", false);
            }
            catch
            {
                SetMessage("Error loading Format", true);
                throw;
            }
        }

        public string SaveFormat(bool useIndentation)
        {
            string result = Format!.Format.WriteFormatToString(useIndentation);
            SetMessage("Saved Format", false);
            return result;
        }

        internal object SaveFormat(App? current)
        {
            throw new NotImplementedException();
        }
    }
}
