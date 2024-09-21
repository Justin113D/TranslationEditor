﻿using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using J113D.TranslationEditor.ProjectApp.ViewModels;
using PropertyChanged;
using System;
using System.Windows.Input;

namespace J113D.TranslationEditor.ProjectApp.Views
{
    [DoNotNotify]
    internal class UndoRedoTextBox : TextBox, ICommand
    {
        protected override Type StyleKeyOverride => typeof(TextBox);

        public UndoRedoTextBox() : base()
        {
            IsUndoEnabled = false;

            KeyBindings.Add(new()
            {
                Gesture = new(Key.Z, KeyModifiers.Control),
                Command = this
            });
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            BindingOperations.GetBindingExpressionBase(this, TextProperty)?.UpdateSource();
            ((MainViewModel)TopLevel.GetTopLevel(this)!.DataContext!).Undo();
        }
    }
}