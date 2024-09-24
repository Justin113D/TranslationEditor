using System;

namespace J113D.TranslationEditor.Data.Events
{
    public class NodeChildrenChangedEventArgs : EventArgs
    {
        public int FromIndex { get; }

        public int ToIndex { get; }

        public Node Child { get; }

        public NodeChildrenChangedEventArgs(int fromIndex, int toIndex, Node removedNode)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
            Child = removedNode;
        }
    }

    public delegate void NodeChildrenChangedEventHandler(ParentNode source, NodeChildrenChangedEventArgs args);
}
