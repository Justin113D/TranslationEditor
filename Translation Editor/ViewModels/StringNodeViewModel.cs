using J113D.TranslationEditor.Data;
using J113D.TranslationEditor.Data.Events;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    public class StringNodeViewModel : NodeViewModel
    {
        private StringNode StringNode
            => (StringNode)_node;

        public string DefaultValue
            => StringNode.DefaultValue;

        public string NodeValue
        {
            get => StringNode.NodeValue;
            set
            {
                if(StringNode.NodeValue == value)
                {
                    return;
                }

                BeginChangeGroup();
                StringNode.NodeValue = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(NodeValue));
                this.AddChangeGroupInvokePropertyChanged(nameof(KeepDefault));
                EndChangeGroup();
            }
        }

        public bool KeepDefault
        {
            get => StringNode.KeepDefault;
            set
            {
                if(StringNode.KeepDefault == value)
                {
                    return;
                }

                BeginChangeGroup();
                StringNode.KeepDefault = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(KeepDefault));
                this.AddChangeGroupInvokePropertyChanged(nameof(NodeValue));
                EndChangeGroup();
            }
        }


        public StringNodeViewModel(FormatViewModel project, StringNode node) 
            : base(project, node) { }


        public void ResetValue()
        {
            StringNode.ResetValue();
        }

        protected override void OnStateChanged(Node node, NodeStateChangedEventArgs args)
        {
            BeginChangeGroup();
            base.OnStateChanged(node, args);
            _project.DecreaseNodeCounter(args.OldState);
            _project.IncreaseNodeCounter(args.NewState);
            EndChangeGroup();
        }

        public override void RefreshNodeValues()
        {
            BeginChangeGroup();
            this.AddChangeGroupInvokePropertyChanged(nameof(KeepDefault));
            this.AddChangeGroupInvokePropertyChanged(nameof(NodeValue));
            this.AddChangeGroupInvokePropertyChanged(nameof(State));
            EndChangeGroup();
        }
    }
}
