using J113D.TranslationEditor.Data;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal abstract class NodeViewModel : ViewModelBase
    {
        protected readonly Node _node;
        protected readonly FormatViewModel _format;

        public string Name
        {
            get => _node.Name;
            set
            {
                if(_node.Name == value)
                {
                    return;
                }

                BeginChangeGroup("NodeViewModel.Name");

                _node.Name = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Name));

                EndChangeGroup();
            }
        }

        public string? Description
        {
            get => _node.Description;
            set
            {
                if(_node.Description == value)
                {
                    return;
                }

                BeginChangeGroup("NodeViewModel.Description");

                _node.Description = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(Description));

                EndChangeGroup();
            }
        }

        public virtual bool Expanded { get; set; }

        public bool Selected { get; private set; }

        public bool PartOfSelectedBranch
        {
            get
            {
                if(Selected)
                {
                    return true;
                }

                if(_node.Parent == null)
                {
                    return false;
                }

                return _format.GetNodeViewModel(_node.Parent).PartOfSelectedBranch;
            }
        }

        protected NodeViewModel(FormatViewModel format, Node node)
        {
            _node = node;
            _format = format;
        }


        public void Remove()
        {
            _node.Parent!.RemoveChildNode(_node);
        }

        public void SelectSingle()
        {
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

            _ = Node.GetCommonAncestor(_node, _format.LastSelectedNode._node, out bool thisIsAboveSelected)!;

            Node current = thisIsAboveSelected ? _node : _format.LastSelectedNode._node;
            Node target = thisIsAboveSelected ? _format.LastSelectedNode._node : _node;

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
    }
}
