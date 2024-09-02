using System;

namespace J113D.TranslationEditor.Data.Events
{
    public class NodeHeaderChangedEventArgs : EventArgs
    {
        public Format? OldHeader { get; }
        public Format? NewHeader { get; }

        public NodeHeaderChangedEventArgs(Format? oldHeader, Format? newHeader)
        {
            OldHeader = oldHeader;
            NewHeader = newHeader;
        }
    }

    public delegate void NodeHeaderChangedEventHandler(Node source, NodeHeaderChangedEventArgs args);
}
