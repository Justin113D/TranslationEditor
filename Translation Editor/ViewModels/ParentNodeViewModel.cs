using J113D.TranslationEditor.Data;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    public class ParentNodeViewModel : NodeViewModel
    {
        private ParentNode ParentNode
            => (ParentNode)_node;

        private bool _expanded;

        /// <summary>
        /// Whether the node is expanded or collapsed
        /// </summary>
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

                if(_expanded && ChildNodes == null)
                {
                    ChildNodes = CreateNodeViewModels(_project, ParentNode);
                }
            }
        }

        /// <summary>
        /// Whether the node can be expanded at all
        /// </summary>
        public override bool CanExpand
            => ParentNode.ChildNodes.Count > 0;


        public ParentNodeViewModel(FormatViewModel project, ParentNode node)
            : base(project, node) { }


        public override void RefreshNodeValues()
        {
            if(ChildNodes == null)
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
            if(ChildNodes == null)
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
