using System;

namespace J113D.TranslationEditor.Data.Events
{
    public class NodeValueChangedEventArgs : EventArgs
    {
        public string OldValue { get; }
        public string NewValue { get; }

        public NodeValueChangedEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public delegate void NodeValueChangedEventHandler(Node source, NodeValueChangedEventArgs args);
}
