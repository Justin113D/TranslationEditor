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


        protected NodeViewModel(FormatViewModel format, Node node)
        {
            _node = node;
            _format = format;
        }


    }
}
