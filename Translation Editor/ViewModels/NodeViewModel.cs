﻿using J113D.TranslationEditor.Data.Events;
using J113D.TranslationEditor.Data;
using System.Collections.ObjectModel;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    public abstract class NodeViewModel : ViewModelBase
    {
        protected readonly Node _node;
        protected readonly FormatViewModel _project;

        public string Name
            => _node.Name;

        public string? Description
            => _node.Description;

        public NodeState State
            => _node.State;

        public ReadOnlyCollection<NodeViewModel>? ChildNodes { get; protected set; }

        public virtual bool CanExpand
            => false;

        public virtual bool Expanded { get; set; }


        protected NodeViewModel(FormatViewModel project, Node node)
        {
            _node = node;
            _project = project;

            _node.NodeStateChanged += OnStateChanged;
        }

        ~NodeViewModel()
        {
            _node.NodeStateChanged -= OnStateChanged;
        }


        protected virtual void OnStateChanged(Node node, NodeStateChangedEventArgs args)
        {
            this.AddChangeGroupInvokePropertyChanged(nameof(State));
        }

        public abstract void RefreshNodeValues();


        public static ReadOnlyCollection<NodeViewModel> CreateNodeViewModels(FormatViewModel project, ParentNode parentNode)
        {
            NodeViewModel[] result = new NodeViewModel[parentNode.ChildNodes.Count];

            for(int i = 0; i < result.Length; i++)
            {
                Node node = parentNode.ChildNodes[i];

                if(node is ParentNode p)
                {
                    result[i] = new ParentNodeViewModel(project, p);
                }
                else if(node is StringNode s)
                {
                    result[i] = new StringNodeViewModel(project, s);
                }
            }

            return new(result);
        }
    }
}
