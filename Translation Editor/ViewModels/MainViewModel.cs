using J113D.TranslationEditor.Data;
using J113D.UndoRedo;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    internal sealed class MainViewModel : ViewModelBase
    {
        public ChangeTracker ProjectTracker { get; }

        public FormatViewModel? Format { get; private set; }


        public string? Message { get; private set; }

        public ToolbarMessageType MessageType { get; private set; }


        public MainViewModel()
        {
            ProjectTracker = new();
            ProjectTracker.UseTracker();
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
            if(ProjectTracker.Undo())
            {
                SetMessage("Performed Undo", false);
            }
        }

        public void Redo()
        {
            if(ProjectTracker.Redo())
            {
                SetMessage("Performed Redo", false);
            }
        }


        public void LoadFormat(string format)
        {
            try
            {
                Format headerNode = Data.Format.ReadFormatFromString(format);

                Format = new(headerNode);
                ProjectTracker.Reset();

                SetMessage("Loaded Format", false);
            }
            catch
            {
                SetMessage("Error loading Format", true);
                throw;
            }
        }


        public void ReadProject(string data)
        {
            if(Format == null)
            {
                return;
            }

            ProjectTracker.BeginGroup();

            try
            {
                Format.Format.ReadProjectFromString(data);
            }
            catch
            {
                ProjectTracker.EndGroup(discard: true);
                SetMessage("Failed to read Project", true);
                throw;
            }

            SetMessage("Read Project", false);
            ProjectTracker.EndGroup();
            Format.Refresh();
            ProjectTracker.Reset();
        }

        public string WriteProject()
        {
            if(Format == null)
            {
                return "";
            }

            try
            {
                string data = Format.Format.WriteProjectToString(true);
                SetMessage("Saved Project", false);
                return data;
            }
            catch
            {
                SetMessage("Failed to save Project", true);
                throw;
            }
        }

        public void NewProject()
        {
            if(Format == null)
            {
                return;
            }

            Format.Author = string.Empty;
            Format.Language = Format.DefaultLanguage;
            Format.Format.ResetAllStrings();
            Format.Refresh();
            ProjectTracker.Reset();
            SetMessage("Reset Project", false);
        }


        public void ImportProjectValues(string data)
        {
            if(Format == null)
            {
                return;
            }

            ProjectTracker.BeginGroup();

            try
            {
                Format.Format.ImportProjectValuesFromString(data);
            }
            catch
            {
                ProjectTracker.EndGroup(discard: true);
                SetMessage("Failed to read Project", true);
                throw;
            }

            SetMessage("Imported Project", false);
            Format.Refresh();
            ProjectTracker.EndGroup();
        }

        public string ExportLanguage()
        {
            Format!.Format.WriteExportToStrings(out _, out string values);
            SetMessage("Exported Language File", false);
            return values;
        }
    }
}
