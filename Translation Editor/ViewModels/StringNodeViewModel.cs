using J113D.TranslationEditor.Data;
using J113D.TranslationEditor.Data.Events;
using PropertyChanged;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    internal sealed class StringNodeViewModel : NodeViewModel
    {
        private string? _tmpNodeValue;

        private StringNode StringNode
            => (StringNode)_node;

        public string DefaultValue
            => StringNode.DefaultValue;

        public string NodeValue
        {
            get => _tmpNodeValue ?? StringNode.NodeValue;
            set
            {
                if(StringNode.NodeValue == value)
                {
                    return;
                }

                BeginChangeGroup();

                string prevValue = StringNode.NodeValue;
                StringNode.NodeValue = value;

                EndChangeGroup();

                if(prevValue == StringNode.NodeValue && value != StringNode.NodeValue)
                {
                    _tmpNodeValue = value;
                    InvokePropertyChanged(nameof(NodeValue));
                    _tmpNodeValue = null;
                    InvokePropertyChanged(nameof(NodeValue));
                }
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
                EndChangeGroup();
            }
        }


        public StringNodeViewModel(FormatViewModel project, StringNode node) 
            : base(project, node)
        {
            node.ValueChanged += OnValueChanged;
        }

        [SuppressPropertyChangedWarnings]
        private void OnValueChanged(Node source, NodeValueChangedEventArgs args)
        {
            this.AddChangeGroupInvokePropertyChanged(nameof(NodeValue));
        }

        public void ResetValue()
        {
            BeginChangeGroup();
            StringNode.ResetValue();
            this.AddChangeGroupInvokePropertyChanged(nameof(KeepDefault));
            EndChangeGroup();
        }

        [SuppressPropertyChangedWarnings]
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
