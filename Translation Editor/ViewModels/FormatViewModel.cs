using J113D.TranslationEditor.Data;
using J113D.UndoRedo;
using System;
using System.Collections.ObjectModel;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    public class FormatViewModel : ViewModelBase
    {
        public Format Format { get; }

        public int UntranslatedNodes { get; private set; }

        public int OutdatedNodes { get; private set; }

        public int TranslatedNodes { get; private set; }


        public string DefaultLanguage { get; }

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
            => Format.Name;

        public string Version
            => Format.Version.ToString();

        public ReadOnlyCollection<NodeViewModel> Nodes { get; }


        public FormatViewModel(Format data)
        {
            Format = data;
            DefaultLanguage = data.Language;
            Nodes = NodeViewModel.CreateNodeViewModels(this, Format.RootNode);
            CountNodes(false);
        }


        private void CountNodes(bool track)
        {
            int translatedNodes = 0, outdatedNodes = 0, untranslatedNodes = 0;

            foreach(StringNode sNode in Format.StringNodes.Values)
            {
                switch(sNode.State)
                {
                    case NodeState.Translated:
                        translatedNodes++;
                        break;
                    case NodeState.Outdated:
                        outdatedNodes++;
                        break;
                    case NodeState.Untranslated:
                        untranslatedNodes++;
                        break;
                }
            }

            if(track)
            {
                TrackPropertyChange(this, nameof(TranslatedNodes), translatedNodes);
                TrackPropertyChange(this, nameof(OutdatedNodes), outdatedNodes);
                TrackPropertyChange(this, nameof(UntranslatedNodes), untranslatedNodes);
            }
            else
            {
                TranslatedNodes = translatedNodes;
                OutdatedNodes = outdatedNodes;
                UntranslatedNodes = untranslatedNodes;
            }
        }


        public void IncreaseNodeCounter(NodeState state)
        {
            NodeCounterChange(state, 1);
        }

        public void DecreaseNodeCounter(NodeState state)
        {
            NodeCounterChange(state, -1);
        }

        private void NodeCounterChange(NodeState state, int difference)
        {
            string propertyName;
            int newVal;

            switch(state)
            {
                case NodeState.Translated:
                    propertyName = nameof(TranslatedNodes);
                    newVal = TranslatedNodes + difference;
                    break;
                case NodeState.Outdated:
                    propertyName = nameof(OutdatedNodes);
                    newVal = OutdatedNodes + difference;
                    break;
                case NodeState.Untranslated:
                    propertyName = nameof(UntranslatedNodes);
                    newVal = UntranslatedNodes + difference;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            TrackPropertyChange(this, propertyName, newVal);
        }


        public void Refresh()
        {
            BeginChangeGroup();

            foreach(NodeViewModel node in Nodes)
            {
                node.RefreshNodeValues();
            }

            CountNodes(true);
            this.AddChangeGroupInvokePropertyChanged(nameof(Author));
            this.AddChangeGroupInvokePropertyChanged(nameof(Language));
            this.AddChangeGroupInvokePropertyChanged(nameof(Name));
            this.AddChangeGroupInvokePropertyChanged(nameof(Version));

            EndChangeGroup();
        }

        public void ExpandAll()
        {
            foreach(NodeViewModel node in Nodes)
            {
                if(node is ParentNodeViewModel parent)
                {
                    parent.ExpandAll();
                }
            }
        }

        public void CollapseAll()
        {
            foreach(NodeViewModel node in Nodes)
            {
                if(node is ParentNodeViewModel parent)
                {
                    parent.CollapseAll();
                }
            }
        }
    }
}
