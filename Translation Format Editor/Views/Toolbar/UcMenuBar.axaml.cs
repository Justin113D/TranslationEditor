using Avalonia.Controls;
using Avalonia.Interactivity;
using J113D.TranslationEditor.FormatApp.ViewModels;
using Avalonia.Threading;
using PropertyChanged;
using J113D.Avalonia.MessageBox;
using J113D.TranslationEditor.FormatApp.Views.Windows;
using System;
using System.Threading.Tasks;
using J113D.Avalonia.Utilities.IO;
using J113D.UndoRedo;
using static System.Net.Mime.MediaTypeNames;

namespace J113D.TranslationEditor.FormatApp.Views.Toolbar
{
    [DoNotNotify]
    internal partial class UcMenuBar : UserControl, IFileChangeTracker
    {
        private ChangeTracker.Pin? _fileChangePin;
        private readonly FormatFileHandler _formatFileHandler;
        private readonly FormatFileAppendHandler _formatFileAppendHandler;
        //public WndHelp? HelpWindow { get; private set; }

        private MainViewModel ViewModel
            => (MainViewModel)DataContext!;

        public bool HasFileChanged => _fileChangePin?.IsValid == false;

        public UcMenuBar()
        {
            InitializeComponent();
            _formatFileHandler = new(this);
            _formatFileAppendHandler = new(this);
        }


        public void StoreCurrentState()
        {
            _fileChangePin = ViewModel.FormatTracker.PinCurrent();
        }

        public void ResetCurrentState()
        {
            _fileChangePin = null;
        }

        public void CopyState(IFileChangeTracker source)
        {
            throw new NotSupportedException();
        }


        public Task<bool> CloseConfirmation()
        {
            return _formatFileHandler.ResetConfirmation();
        }


        public void OnNewFormat(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _formatFileHandler.Reset());
        }

        public void OnOpenFormat(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _formatFileHandler.Open());
        }

        public void OnSaveFormat(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _formatFileHandler.Save(false));
        }

        public void OnSaveFormatAs(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _formatFileHandler.Save(true));
        }

        public void OnAppendFormat(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _formatFileAppendHandler.Open());
        }


        private void OnUndo(object? sender, RoutedEventArgs e)
        {
            ViewModel.Undo();
        }

        private void OnRedo(object? sender, RoutedEventArgs e)
        {
            ViewModel.Redo();
        }

        private void OnCopy(object? sender, RoutedEventArgs e)
        {
            string? text = ViewModel.Copy();
            if(text != null)
            {
                Dispatcher.UIThread.Post(async () => await TopLevel.GetTopLevel(this)!.Clipboard!.SetTextAsync(text));
            }
        }

        private void OnPaste(object? sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                string? clipboard = await TopLevel.GetTopLevel(this)!.Clipboard!.GetTextAsync();
                ViewModel.Paste(clipboard);
            });
        }


        private void OnExpandAll(object sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                Window window = (Window)TopLevel.GetTopLevel(this)!;
                MessageBoxResult? result = await window.MessageBoxDialog(
                    "Expand All?",
                    "Depending on how big the format is,\nthis operation could take a while,\nif not crash the application outright.\n\nProceed?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if(result != MessageBoxResult.Yes)
                {
                    return;
                }

                ViewModel.Format!.RootNode.ExpandAll();
            });
        }

        private void OnCollapseAll(object sender, RoutedEventArgs e)
        {
            ViewModel.Format!.RootNode.CollapseAll();
        }


        private void OnSettingsOpen(object sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                Window topLevel = (Window)TopLevel.GetTopLevel(this)!;
                WndSettings window = new();
                await window.ShowDialog(topLevel);
            });
        }

        private void OnOpenHelp(object sender, RoutedEventArgs e)
        {
            //if(HelpWindow != null)
            //{
            //    HelpWindow.Focus();
            //    return;
            //}

            //Window topLevel = (Window)TopLevel.GetTopLevel(this)!;
            //HelpWindow = new();
            //HelpWindow.Show();
            //HelpWindow.Closed += OnCloseHelp;
        }

        private void OnCloseHelp(object? sender, EventArgs e)
        {
            //HelpWindow = null;
        }
    }
}