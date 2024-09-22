using J113D.TranslationEditor.Data;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.FormatApp.ViewModels
{
    internal sealed class StringNodeViewModel : NodeViewModel
    {
        #region private fields
#pragma warning disable IDE0044

        private string _realDefaultValue;

#pragma warning restore IDE0044
        #endregion

        private StringNode StringNode
            => (StringNode)_node;


        public string DefaultValue
        {
            get => StringNode.DefaultValue;
            set
            {
                if(_realDefaultValue == value)
                {
                    return;
                }

                BeginChangeGroup();
                TrackFieldChange(this, nameof(_realDefaultValue), value);
                StringNode.DefaultValue = value;
                EndChangeGroup();
            }
        }

        public StringNodeViewModel(FormatViewModel project, StringNode node)
            : base(project, node)
        {
            _realDefaultValue = node.DefaultValue;
        }
    }
}
