using J113D.TranslationEditor.Data;
using J113D.UndoRedo;
using J113D.UndoRedo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal sealed class FormatViewModel : ViewModelBase
    {
        private readonly Dictionary<Node, NodeViewModel> _internalNodeTable;

        public Format Format { get; }

        public string Author
        {
            get => Format.Author;
            set
            {
                BeginChangeGroup();
                Format.Author = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Author));
                EndChangeGroup();
            }
        }

        public string Language
        {
            get => Format.Language;
            set
            {
                BeginChangeGroup();
                Format.Language = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Language));
                EndChangeGroup();
            }
        }

        public string Name
        {
            get => Format.Name;
            set
            {
                BeginChangeGroup();
                Format.Name = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Name));
                EndChangeGroup();
            }
        }

        public string Version
        {
            get => Format.Version.ToString();
            set
            {
                Version newVersion = new(value);

                int mostRecentVersionIndex = Format.GetMostRecentUsedVersionIndex();

                if(mostRecentVersionIndex >= 0 && newVersion <= Format.Versions[mostRecentVersionIndex])
                {
                    throw new ArgumentException("New version must be greater most recent used version!");
                }

                BeginChangeGroup();
                Format.Version = newVersion;
                this.AddChangeGroupInvokePropertyChanged(nameof(Version));
                EndChangeGroup();
            }
        }

        public ParentNodeViewModel RootNode { get; }


        public FormatViewModel(Format data)
        {
            Format = data;
            _internalNodeTable = [];
            RootNode = new(this, Format.RootNode, true);
        }


        public NodeViewModel GetNodeViewModel(Node node)
        {
            if(!_internalNodeTable.TryGetValue(node, out NodeViewModel? result))
            {
                result = node switch
                {
                    ParentNode => new ParentNodeViewModel(this, (ParentNode)node, false),
                    StringNode => new StringNodeViewModel(this, (StringNode)node),
                    _ => throw new NotSupportedException(node.GetType().Name + " is not a valid node type"),
                };

                _internalNodeTable.Add(node, result);
            }

            return result;
        }

        public void CreateNodeViewModels(ParentNode parentNode, IList<NodeViewModel> nodeList)
        {
            BeginChangeGroup();
            nodeList.Clear();

            foreach(Node node in parentNode.ChildNodes)
            {
                nodeList.Add(GetNodeViewModel(node));
            }

            EndChangeGroup();
        }
    }
}
