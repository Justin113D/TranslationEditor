using Avalonia.Controls;
using Avalonia.Data;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using PropertyChanged;

namespace J113D.TranslationEditor.ProjectApp.Views.NodeTree
{
    [DoNotNotify]
    internal partial class UcNodeTree : UserControl
    {
        public UcNodeTree()
        {
            InitializeComponent();
        }

        public void OnNodeValueTextReset(TextBox textbox)
        {
            BindingExpressionBase binding = BindingOperations.GetBindingExpressionBase(textbox, TextBox.TextProperty)!;
            binding.UpdateSource();
            ((StringNodeViewModel)textbox.DataContext!).ResetValue();
        }

        public void OnNodeValueTextUndo(TextBox textbox)
        {
            BindingExpressionBase binding = BindingOperations.GetBindingExpressionBase(textbox, TextBox.TextProperty)!;
            binding.UpdateSource();
            ((MainViewModel)TopLevel.GetTopLevel(this)!.DataContext!).Undo();
        }
    }
}