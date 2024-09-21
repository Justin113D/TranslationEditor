using J113D.UndoRedo;
using System.ComponentModel;

namespace J113D.TranslationEditor.ProjectApp.ViewModels
{
    internal abstract class ViewModelBase : IInvokeNotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
