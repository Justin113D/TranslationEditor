using J113D.TranslationEditor.Data;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal sealed class StringNodeViewModel : NodeViewModel
    {
        private string? _tmpDefaultValue;

        private StringNode StringNode
            => (StringNode)Node;


        public string DefaultValue
        {
            get => _tmpDefaultValue ?? StringNode.DefaultValue;
            set
            {
                if(StringNode.DefaultValue == value)
                {
                    return;
                }

                BeginChangeGroup();

                string prevValue = StringNode.DefaultValue;
                StringNode.DefaultValue = value;
                this.AddChangeGroupInvokePropertyChanged(nameof(DefaultValue));

                EndChangeGroup();

                if(prevValue == StringNode.DefaultValue && value != StringNode.DefaultValue)
                {
                    _tmpDefaultValue = value;
                    InvokePropertyChanged(nameof(DefaultValue));
                    _tmpDefaultValue = null;
                    InvokePropertyChanged(nameof(DefaultValue));
                }
            }
        }

        public StringNodeViewModel(FormatViewModel project, StringNode node)
            : base(project, node) { }
    }
}
