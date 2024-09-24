using J113D.TranslationEditor.Data;
using J113D.UndoRedo.Collections;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal sealed class ParentNodeViewModel : NodeViewModel
    {
        private bool _expanded;
        private TrackList<NodeViewModel>? _childNodes;

        public ParentNode ParentNode
            => (ParentNode)Node;

        public ReadOnlyObservableCollection<NodeViewModel> ChildNodes { get; private set; }

        public override bool Expanded
        {
            get => _expanded;
            set
            {
                if(_expanded == value)
                {
                    return;
                }

                _expanded = value;

                if(_expanded && _childNodes == null)
                {
                    _format.CreateNodeViewModels(ParentNode, out _childNodes, out ReadOnlyObservableCollection<NodeViewModel> observableNodes);
                    ChildNodes = observableNodes;
                }
            }
        }


        public ParentNodeViewModel(FormatViewModel format, ParentNode node, bool expanded) : base(format, node)
        {
            if(ParentNode.ChildNodes.Count > 0 && !expanded)
            {
                ChildNodes = new([this]);
            }
            else
            {
                _format.CreateNodeViewModels(ParentNode, out _childNodes, out ReadOnlyObservableCollection<NodeViewModel>? observableNodes);
                ChildNodes = observableNodes;
            }

            node.ChildrenChanged += OnChildrenChanged;
        }


        [SuppressPropertyChangedWarnings]
        private void OnChildrenChanged(ParentNode source, Data.Events.NodeChildrenChangedEventArgs args)
        {
            if(_childNodes == null)
            {
                throw new InvalidOperationException("Node has to have been extended at least once!");
            }

            BeginChangeGroup("ParentNodeViewModel.OnChildrenChanged");

            bool canExpandBefore = ChildNodes!.Count > 0;

            if(args.FromIndex > -1)
            {
                _childNodes!.RemoveAt(args.FromIndex);
            }

            if(args.ToIndex > -1)
            {
                NodeViewModel vmNode = _format.GetNodeViewModel(args.Child);
                _childNodes!.Insert(args.ToIndex, vmNode);
            }

            bool canExpandAfter = ChildNodes!.Count > 0;

            TrackCallbackChange(
                canExpandAfter ? ExpandUpward : CollapseAndExpandParentUpward,
                canExpandBefore ? ExpandUpward : CollapseAndExpandParentUpward
            );

            EndChangeGroup();
        }


        private void AddNewNode(Node node)
        {
            if(ChildNodes == null)
            {
                _format.CreateNodeViewModels(ParentNode, out _childNodes, out ReadOnlyObservableCollection<NodeViewModel>? observableNodes);
                ChildNodes = observableNodes;
            }
            else if(_childNodes == null)
            {
                Expanded = true;
            }

            ParentNode.AddChildNode(node);
        }

        public void AddNewStringNode()
        {
            AddNewNode(new StringNode("string", ""));
        }

        public void AddNewParentNode()
        {
            AddNewNode(new ParentNode("Category"));
        }


        public void CollapseAndExpandParentUpward()
        {
            Expanded = false;
            Parent?.ExpandUpward();
        }

        public void ExpandUpward()
        {
            Expanded = true;
            if(ParentNode.Parent != null)
            {
                ((ParentNodeViewModel)_format.GetNodeViewModel(ParentNode.Parent)).ExpandUpward();
            }
        }

        public void ExpandAll()
        {
            Expanded = true;
            foreach(NodeViewModel node in ChildNodes!)
            {
                if(node is ParentNodeViewModel parent)
                {
                    parent.ExpandAll();
                }
            }
        }

        public void CollapseAll()
        {
            if(ChildNodes == null || ChildNodes[0] == this)
            {
                return;
            }

            Expanded = false;
            foreach(NodeViewModel node in ChildNodes)
            {
                if(node is ParentNodeViewModel parent)
                {
                    parent.CollapseAll();
                }
            }
        }
    }
}
