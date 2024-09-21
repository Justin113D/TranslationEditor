using J113D.TranslationEditor.Data;
using System.Collections.ObjectModel;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    internal sealed class ParentNodeViewModel : NodeViewModel
    {
        private bool _expanded;

        private ParentNode ParentNode
            => (ParentNode)_node;

        public ReadOnlyCollection<NodeViewModel>? ChildNodes { get; protected set; }

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
                    ChildNodes = CreateNodeViewModels(_project, ParentNode);
                }
            }
        }


        public ParentNodeViewModel(FormatViewModel project, ParentNode node)
            : base(project, node)
        {
            if(ParentNode.ChildNodes.Count > 0)
            {
                ChildNodes = new([this]);
            }
        }


        public override void RefreshNodeValues()
        {
            if(ChildNodes == null || ChildNodes[0] == this)
            {
                return;
            }

            foreach(NodeViewModel node in ChildNodes)
            {
                node.RefreshNodeValues();
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
