using J113D.TranslationEditor.Data;
using J113D.UndoRedo.Collections;
using PropertyChanged;
using System.Collections.ObjectModel;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal sealed class ParentNodeViewModel : NodeViewModel
    {
        private bool _expanded;
        private readonly TrackList<NodeViewModel> _childNodes;
        private readonly ReadOnlyObservableCollection<NodeViewModel> _observableChildNodes;

        private ParentNode ParentNode
            => (ParentNode)_node;

        public ReadOnlyObservableCollection<NodeViewModel>? ChildNodes { get; private set; }

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

                if(_expanded && ChildNodes != null && ChildNodes[0] == this)
                {
                    _format.CreateNodeViewModels(ParentNode, _childNodes);
                }
            }
        }


        public ParentNodeViewModel(FormatViewModel format, ParentNode node, bool expanded) : base(format, node)
        {
            ObservableCollection<NodeViewModel> internalChildren = [];
            
            if(expanded && node.ChildNodes.Count > 0)
            {
                format.CreateNodeViewModels(node, internalChildren);
            }
            else
            {
                internalChildren.Add(this);
            }

            _childNodes = new(internalChildren);
            _observableChildNodes = new(internalChildren);

            if(ParentNode.ChildNodes.Count > 0)
            {
                ChildNodes = _observableChildNodes;
            }

            node.ChildrenChanged += OnChildrenChanged;
        }


        [SuppressPropertyChangedWarnings]
        private void OnChildrenChanged(ParentNode source, Data.Events.NodeChildrenChangedEventArgs args)
        {
            BeginChangeGroup();

            // If it is null, then the only change that could have occured is the insertion of a node
            bool canExpandBefore = ChildNodes != null;
            if(!canExpandBefore)
            {
                TrackPropertyChange(this, nameof(ChildNodes), _observableChildNodes);
            }

            if(ParentNode.ChildNodes.Count > 0 && ChildNodes![0] == this)
            {
                Expanded = true;
            }
            else
            {
                if(args.FromIndex > -1)
                {
                    _childNodes!.RemoveAt(args.FromIndex);
                }

                if(args.ToIndex > -1)
                {
                    NodeViewModel vmNode = _format.GetNodeViewModel(ParentNode.ChildNodes[args.ToIndex]);
                    _childNodes!.Insert(args.ToIndex, vmNode);
                }

                if(_childNodes!.Count == 0)
                {
                    TrackPropertyChange(this, nameof(ChildNodes), null);
                }
            }

            EndChangeGroup();
        }


        public void AddNewStringNode()
        {
            ParentNode.AddChildNode(new StringNode("String", ""));
            Expanded = true;
        }

        public void AddNewParentNode()
        {
            ParentNode.AddChildNode(new ParentNode("Category"));
            Expanded = true;
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
