using J113D.TranslationEditor.Data.Events;
using J113D.UndoRedo.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.Data
{
    /// <summary>
    /// A node which holds more nodes as children (hierarchy node)
    /// </summary>
    public class ParentNode : Node, IEnumerable<Node>
    {
        protected readonly TrackList<Node> _childNodes;

        /// <summary>
        /// The children of this node
        /// </summary>
        public ReadOnlyCollection<Node> ChildNodes { get; }


        public event NodeChildrenChangedEventHandler? ChildrenChanged;


        internal ParentNode(string name, string description, List<Node> childNodes)
            : base(name, description, NodeState.None)
        {
            _childNodes = new(childNodes);
            ChildNodes = new(_childNodes);

            foreach(Node item in _childNodes)
            {
                item._parent = this;
            }

            EvaluateState(false);
        }


        /// <summary>
        /// Creates a parent node
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public ParentNode(string name, string description = "") : this(name, description, []) { }



        public void AddChildNode(Node node, bool updateVersionIndex = true)
        {
            InternalAddNode(node, _childNodes.Count, updateVersionIndex);
        }

        public void RemoveChildNode(Node node)
        {
            if(node.Parent != this)
            {
                BlankChange("ParentNode.RemoveChildNode");
                return;
            }

            BeginChangeGroup("ParentNode.RemoveChildNode");

            int index = _childNodes.IndexOf(node);
            InternalRemoveNode(index);

            node.InternalSetParent(null, false, index, -1);

            EndChangeGroup();
        }

        public void InsertChildNodeAt(Node node, int index)
        {
            InternalAddNode(node, index);
        }

        public void MoveChildNode(int fromIndex, int toIndex)
        {
            if(fromIndex < 0 || fromIndex > _childNodes.Count
                || toIndex < 0 || toIndex > _childNodes.Count)
            {
                throw new IndexOutOfRangeException("One or both indices are out of range!");
            }

            if(fromIndex == toIndex)
            {
                BlankChange("ParentNode.MoveChildNode");
                return;
            }

            if(fromIndex < toIndex)
            {
                toIndex--;
            }

            Node target = _childNodes[fromIndex];

            BeginChangeGroup("ParentNode.MoveChildNode");

            _childNodes.RemoveAt(fromIndex);
            _childNodes.Insert(toIndex, target);

            InvokeChildrenChanged(fromIndex, toIndex, target);

            EndChangeGroup();
        }


        private void InternalAddNode(Node node, int index, bool updateVersionIndex = true)
        {
            if(node.Parent == this)
            {
                BlankChange("ParentNode.InternalAddNode");
                return;
            }

            if(index < 0 || index > _childNodes.Count)
            {
                throw new IndexOutOfRangeException("Index out of range!");
            }

            BeginChangeGroup("ParentNode.InternalAddNode");

            int otherIndex = -1;
            if(node.Parent != null)
            {
                ParentNode otherParent = node.Parent;
                otherIndex = otherParent._childNodes.IndexOf(node);
                otherParent.InternalRemoveNode(otherIndex);
            }

            _childNodes.Insert(index, node);

            if(node.State > State)
            {
                State = node.State;
            }

            node.InternalSetParent(this, updateVersionIndex, otherIndex, index);

            EndChangeGroup();
        }

        private void InternalRemoveNode(int index)
        {
            _childNodes.RemoveAt(index);
            EvaluateState(true);
        }


        internal void EvaluateState(Node changedNode)
        {
            if(changedNode.State > State)
            {
                State = changedNode.State;
            }
            else if(changedNode.State < State)
            {
                EvaluateState(true);
            }
        }

        internal void EvaluateState(bool track)
        {
            NodeState state = _childNodes.Count > 0
                ? ChildNodes.Max(x => x.State)
                : NodeState.None;

            if(track)
            {
                State = state;
            }
            else
            {
                _state = state;
            }
        }


        internal override void InvokeHeaderChanged(NodeHeaderChangedEventArgs args)
        {
            base.InvokeHeaderChanged(args);

            foreach(Node n in _childNodes)
            {
                n.InvokeHeaderChanged(args);
            }
        }

        internal void InvokeChildrenChanged(int fromIndex, int toIndex, Node removedNode)
        {
            ChildrenChanged?.Invoke(this, new(fromIndex, toIndex, removedNode));
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return new NodeHierarchyEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
