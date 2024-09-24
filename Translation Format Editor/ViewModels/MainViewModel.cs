using J113D.TranslationEditor.Data;
using J113D.TranslationEditor.Data.JSON;
using J113D.UndoRedo;
using System;
using System.Linq;
using System.Text.Json;

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

        public string? Copy()
        {
            if(Format.SelectedNodes.Count == 0)
            {
                return null;
            }

            Node[] nodes = Format.GetSelectedNodeRoots().Select(x => x.Node).ToArray();

            JsonSerializerOptions options = JsonConverterHelper.CreateOptions(false);
            string json = JsonSerializer.Serialize(nodes, options);

            SetMessage("Copied selected nodes to clipboard!", false);
            return json;
        }

        public void Paste(string? value)
        {
            if(string.IsNullOrEmpty(value))
            {
                SetMessage("Clipboard is empty!", true);
                return;
            }

            JsonSerializerOptions options = JsonConverterHelper.CreateOptions(false);
            Node[] nodes;
            try
            {
                nodes = JsonSerializer.Deserialize<Node[]>(value, options)!;
            }
            catch
            {
                SetMessage("Clipboard contents not formated as pastable nodes!", true);
                return;
            }

            int insertIndex = -1;
            ParentNode insertTarget = Format.RootNode.ParentNode;

            if(Format.LastSelectedNode?.Selected == true)
            {
                if(Format.LastSelectedNode is ParentNodeViewModel selectedParent)
                {
                    insertTarget = selectedParent.ParentNode;
                }
                else
                {
                    insertTarget = Format.LastSelectedNode.Parent!.ParentNode;
                    insertIndex = insertTarget.ChildNodes.IndexOf(Format.LastSelectedNode.Node) + 1;
                }
            }

            Format.GetNodeViewModel(insertTarget).Expanded = true;

            FormatTracker.BeginGroup("MainViewModel.Paste");

            foreach(Node node in nodes)
            {
                if(insertIndex >= 0)
                {
                    insertTarget.InsertChildNodeAt(node, insertIndex);
                    insertIndex++;
                }
                else
                {
                    insertTarget.AddChildNode(node);
                }
            }

            FormatTracker.EndGroup();

            SetMessage("Successfully pasted clipboard contents!", false);
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
