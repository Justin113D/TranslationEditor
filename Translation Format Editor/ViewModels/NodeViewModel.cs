using J113D.TranslationEditor.Data;
using System;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal abstract class NodeViewModel : ViewModelBase
    {
        protected readonly FormatViewModel _format;

        #region private fields
#pragma warning disable IDE0044

        private string _realDescription;

#pragma warning restore IDE0044
        #endregion

        public Node Node { get; }

        public string Name
        {
            get => Node.Name;
            set
            {
                if(Node.Name == value)
                {
                    return;
                }

                BeginChangeGroup("NodeViewModel.Name");

                Node.Name = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Name));

                EndChangeGroup();
            }
        }

        public string Description
        {
            get => _realDescription;
            set
            {
                if(_realDescription == value)
                {
                    return;
                }

                BeginChangeGroup("NodeViewModel.Description");

                TrackFieldChange(this, nameof(_realDescription), value);
                Node.Description = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Description));

                EndChangeGroup();
            }
        }

        public virtual bool Expanded { get; set; }

        public bool Selected { get; private set; }

        public bool PartOfSelectedBranch 
            => Selected || Parent?.PartOfSelectedBranch == true;

        public ParentNodeViewModel? Parent
            => Node.Parent == null ? null : (ParentNodeViewModel)_format.GetNodeViewModel(Node.Parent);


        protected NodeViewModel(FormatViewModel format, Node node)
        {
            Node = node;
            _format = format;
            _realDescription = node.Description ?? string.Empty;
        }


        public virtual void Remove()
        {
            Node.Parent!.RemoveChildNode(Node);
            UnselectedResursive();
            _format.SequenceSelectedNodes.Clear();
        }

        public virtual void UnselectedResursive()
        {
            if(Selected)
            {
                Selected = false;
                _format.SelectedNodes.Remove(this);

                if(_format.LastSelectedNode == this)
                {
                    _format.LastSelectedNode = null;
                }
            }
        }

        public void SelectSingle()
        {
            if(Selected && _format.SelectedNodes.Count == 1)
            {
                Selected = false;
                _format.SelectedNodes.Remove(this);
                return;
            }

            foreach(NodeViewModel node in _format.SelectedNodes)
            {
                node.Selected = false;
            }

            _format.SelectedNodes.Clear();

            _format.SelectedNodes.Add(this);
            Selected = true;

            _format.LastSelectedNode = this;
            _format.SequenceSelectedNodes.Clear();
        }

        public void SelectMulti()
        {
            Selected = !Selected;

            if(Selected)
            {
                _format.SelectedNodes.Add(this);

                _format.LastSelectedNode = this;
                _format.SequenceSelectedNodes.Clear();
            }
            else
            {
                _format.SelectedNodes.Remove(this);
            }
        }

        public void SelectSequence()
        {
            if(_format.LastSelectedNode?.Selected != true)
            {
                SelectSingle();
                return;
            }
            
            foreach(NodeViewModel nodeViewModel in _format.SequenceSelectedNodes)
            {
                _format.SelectedNodes.Remove(nodeViewModel);
                nodeViewModel.Selected = false;
            }

            _format.SequenceSelectedNodes.Clear();

            if(_format.LastSelectedNode == this)
            {
                return;
            }

            _ = Data.Node.GetCommonAncestor(Node, _format.LastSelectedNode.Node, out bool thisIsAboveSelected)!;

            Node current = thisIsAboveSelected ? Node : _format.LastSelectedNode.Node;
            Node target = thisIsAboveSelected ? _format.LastSelectedNode.Node : Node;

            while(true)
            {
                NodeViewModel nodeViewModel = _format.GetNodeViewModel(current);

                if(!nodeViewModel.Selected)
                {
                    _format.SelectedNodes.Add(nodeViewModel);
                    _format.SequenceSelectedNodes.Add(nodeViewModel);
                    nodeViewModel.Selected = true;
                }

                if(current == target)
                {
                    break;
                }

                if(nodeViewModel is ParentNodeViewModel parentViewModel && parentViewModel.Expanded)
                {
                    current = ((ParentNode)current).ChildNodes[0];
                    continue;
                }

                ParentNode currentParent = current.Parent!;
                int siblingIndex = currentParent.ChildNodes.IndexOf(current);

                while(siblingIndex == currentParent.ChildNodes.Count - 1)
                {
                    siblingIndex = currentParent.Parent!.ChildNodes.IndexOf(currentParent);
                    currentParent = currentParent.Parent;
                }

                current = currentParent.ChildNodes[siblingIndex + 1];
            }

        }


        public void MoveToParent(ParentNodeViewModel parent, int index)
        {
            if(parent.ChildNodes == null || parent.ChildNodes[0] == parent)
            {
                throw new InvalidOperationException();
            }

            if(index < parent.ChildNodes.Count - 1 && parent.ChildNodes[index] == this)
            {
                return;
            }

            BeginChangeGroup("NodeViewModel.MoveToParent");

            if(Node.Parent == parent.Node)
            {
                Node.Parent.MoveChildNode(Node.Parent.ChildNodes.IndexOf(Node), index);
            }
            else
            {
                ((ParentNode)parent.Node).InsertChildNodeAt(Node, index);
            }

            EndChangeGroup();
        }
    }
}
