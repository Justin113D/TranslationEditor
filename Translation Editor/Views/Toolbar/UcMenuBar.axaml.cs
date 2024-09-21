using Avalonia.Controls;
using Avalonia.Interactivity;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using Avalonia.Threading;
using PropertyChanged;
using J113D.Avalonia.MessageBox;
using J113D.TranslationEditor.ProjectApp.Views.Windows;
using System;
using System.Threading.Tasks;
using J113D.Avalonia.Utilities.IO;
using J113D.UndoRedo;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    [DoNotNotify]
    internal partial class UcMenuBar : UserControl, IFileChangeTracker
    {
        private ChangeTracker.Pin? _fileChangePin;
        private readonly FormatFileHandler _formatFileHandler;
        private readonly ProjectFileHandler _projectFileHandler;
        private readonly ImportProjectFileHandler _importProjectFileHandler;
        private readonly ExportFileHandler _exportFileHandler;
        public WndHelp? HelpWindow { get; private set; }

        private MainViewModel ViewModel
            => (MainViewModel)DataContext!;

        public bool HasFileChanged => _fileChangePin?.IsValid == false;

        public UcMenuBar()
        {
            InitializeComponent();

            _formatFileHandler = new(this);
            _projectFileHandler = new(this);
            _importProjectFileHandler = new(this);
            _exportFileHandler = new(this);
        }


        public void StoreCurrentState()
        {
            _fileChangePin = ViewModel.ProjectTracker.PinCurrent();
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
            return _projectFileHandler.ResetConfirmation();
        }

        public void LoadFormatDirectly(Uri filepath)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                if(!await _formatFileHandler.OpenNoDialog(filepath))
                {
                    return;
                }

                _projectFileHandler?.ForgetFilePath();
            });
        }

        public void OnLoadFormat(object sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                if(!await _formatFileHandler.Open())
                {
                    return;
                }

                _projectFileHandler?.ForgetFilePath();
            });
        }

        public void OnNewProject(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _projectFileHandler.Reset());
        }

        public void OnOpenProject(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _projectFileHandler.Open());
        }

        public void OnSaveProject(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _projectFileHandler.Save(false));
        }

        public void OnSaveProjectAs(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _projectFileHandler.Save(true));
        }

        public void OnImportProjectValues(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _importProjectFileHandler.Open());
        }

        public void OnExportLanguageFile(object sender, RoutedEventArgs e)
        {
            if(ViewModel.Format == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(async () => await _exportFileHandler.Save(true));
        }


        private void OnUndo(object? sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext!).Undo();
        }

        private void OnRedo(object? sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext!).Redo();
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
            if(HelpWindow != null)
            {
                HelpWindow.Focus();
                return;
            }

            Window topLevel = (Window)TopLevel.GetTopLevel(this)!;
            HelpWindow = new();
            HelpWindow.Show();
            HelpWindow.Closed += OnCloseHelp;
        }

        private void OnCloseHelp(object? sender, EventArgs e)
        {
            HelpWindow = null;
        }
    }
}