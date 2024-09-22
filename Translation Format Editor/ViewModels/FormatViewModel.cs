using J113D.TranslationEditor.Data;
using J113D.UndoRedo;
using J113D.UndoRedo.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                BeginChangeGroup("FormatViewModel.Author");
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
                BeginChangeGroup("FormatViewModel.Language");
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
                BeginChangeGroup("FormatViewModel.Name");
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

                BeginChangeGroup("FormatViewModel.Version");
                Format.Version = newVersion;
                this.AddChangeGroupInvokePropertyChanged(nameof(Version));
                EndChangeGroup();
            }
        }

        public ParentNodeViewModel RootNode { get; }

        public HashSet<NodeViewModel> SelectedNodes { get; }

        public HashSet<NodeViewModel> SequenceSelectedNodes { get; }

        public NodeViewModel? LastSelectedNode { get; set; }

        public FormatViewModel(Format data)
        {
            Format = data;
            _internalNodeTable = [];
            SelectedNodes = [];
            SequenceSelectedNodes = [];
            RootNode = new(this, Format.RootNode, true);
            _internalNodeTable.Add(Format.RootNode, RootNode);
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

        public void CreateNodeViewModels(ParentNode parentNode, out TrackList<NodeViewModel> trackList, out ReadOnlyObservableCollection<NodeViewModel> observableList)
        {
            ObservableCollection<NodeViewModel> nodes = new(parentNode.ChildNodes.Select(GetNodeViewModel));
            trackList = new(nodes);
            observableList = new(nodes);
        }
    }
}
