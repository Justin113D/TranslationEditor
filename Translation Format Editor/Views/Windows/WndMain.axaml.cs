using Avalonia.Controls;
using PropertyChanged;
using System;

namespace J113D.TranslationEditor.FormatApp.Views.Windows
{
    [DoNotNotify]
    internal partial class WndMain : J113D.Avalonia.Controls.Window
    {
        private bool _ignoreResetConfirmation;

        public WndMain()
        {
            InitializeComponent();
        }

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            if(!_ignoreResetConfirmation && Toolbar.MenuBar.HasFileChanged)
            {
                e.Cancel = true;
                if(await Toolbar.MenuBar.CloseConfirmation())
                {
                    _ignoreResetConfirmation = true;
                    Close();
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            //Toolbar.MenuBar.HelpWindow?.Close();
            base.OnClosed(e);
        }


    }
}