using J113D.TranslationEditor.Data.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.Data
{
    /// <summary>
    /// Base value container of a translation format
    /// </summary>
    public abstract class Node
    {
        #region private fields
#pragma warning disable IDE0044

        private string _name;
        private string _description;
        protected internal NodeState _state;
        internal ParentNode? _parent;

#pragma warning restore IDE0044
        #endregion

        #region events

        /// <summary>
        /// On changing <see cref="Name"/> <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeNameChangedEventHandler? NameChanged;

        /// <summary>
        /// On changing <see cref="Parent"/> <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeParentChangedEventHandler? ParentChanged;

        /// <summary>
        /// On <see cref="Format"/> has changed <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeHeaderChangedEventHandler? HeaderChanged;

        /// <summary>
        /// On changing <see cref="State"/> <br/>
        /// Does not get invoked on undo/redo!
        /// </summary>
        public event NodeStateChangedEventHandler? NodeStateChanged;

        #endregion

        #region properties

        /// <summary>
        /// Node label/unique identifier
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                value = GetAdjustedName(value);

                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cant be empty or whitespace!", nameof(value));
                }

                if(value == _name)
                {
                    BlankChange("Node.Name");
                    return;
                }

                value = VerifyName(value) ?? value;
                string oldNodeName = _name;

                BeginChangeGroup("Node.Name");

                TrackFieldChange(this, nameof(_name), value, "Node._name");
                InternalOnNameChanged(oldNodeName);
                NameChanged?.Invoke(this, new(oldNodeName, value));

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Gets and sets the nodes description accordingly
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                value = value.Trim();

                if(value == _description)
                {
                    BlankChange("Node.Description");
                    return;
                }

                TrackFieldChange(this, nameof(_description), value, "Node._description");
            }
        }

        /// <summary>
        /// Update state of the string
        /// </summary>
        public NodeState State
        {
            get => _state;
            protected set
            {
                if(value == _state)
                {
                    BlankChange();
                    return;
                }

                NodeState oldState = _state;

                BeginChangeGroup("Node.State");

                TrackFieldChange(this, nameof(_state), value, "Node._state");
                Parent?.EvaluateState(this);
                NodeStateChanged?.Invoke(this, new(oldState, value));

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Parent Node containing this node
        /// </summary>
        public virtual ParentNode? Parent
            => _parent;

        /// <summary>
        /// Format that the node belongs to
        /// </summary>
        public virtual Format? Format
            => _parent?.Format;

        #endregion

        /// <summary>
        /// Create a node with a name and a descripton
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        protected Node(string name, string description, NodeState defaultState)
        {
            _name = GetAdjustedName(name);
            _description = description.Trim();
            _state = defaultState;
        }

        protected virtual string GetAdjustedName(string name)
        {
            return name.Trim();
        }

        protected virtual string? VerifyName(string name)
        {
            return null;
        }

        /// <summary>
        /// Called before the name change event is invoked
        /// </summary>
        /// <param name="oldName"></param>
        protected virtual void InternalOnNameChanged(string oldName) { }


        public void SetParent(ParentNode? parent)
        {
            if(parent == _parent)
            {
                BlankChange("Node.SetParent");
                return;
            }

            if(parent != null)
            {
                parent.AddChildNode(this);
            }
            else
            {
                _parent?.RemoveChildNode(this);
            }
        }

        /// <summary>
        /// Internal parent setter
        /// </summary>
        /// <param name="newParent"></param>
        internal void InternalSetParent(ParentNode? newParent, bool updateVersionIndex, int oldParentIndex, int newParentIndex)
        {
            BeginChangeGroup("Node.InternalSetParent");

            ParentNode? oldParent = _parent;
            TrackFieldChange(this, nameof(_parent), newParent, "Node._parent");

            if(oldParent?.Format != newParent?.Format)
            {
                oldParent?.Format?.RemoveBranchStringNodes(this);
                newParent?.Format?.AddBranchStringNodes(this, updateVersionIndex);
                InvokeHeaderChanged(new(oldParent?.Format, newParent?.Format));
            }

            ParentChanged?.Invoke(this, new(oldParent, newParent));

            oldParent?.InvokeChildrenChanged(oldParentIndex, -1, this);
            newParent?.InvokeChildrenChanged(-1, newParentIndex, this);

            EndChangeGroup();
        }

        /// <summary>
        /// Used for relaying the parents header changed event back to this node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="args"></param>
        internal virtual void InvokeHeaderChanged(NodeHeaderChangedEventArgs args)
        {
            HeaderChanged?.Invoke(this, args);
        }


        public static ParentNode? GetCommonAncestor(Node a, Node b, out bool aIsAboveB)
        {
            if(a.Format != b.Format)
            {
                throw new ArgumentException("Nodes are from different formats!");
            }
            else if(a == b)
            {
                aIsAboveB = false;
                return null;
            }

            List<Node> hierarchyOfA = [];
            Node? current = a;
            while(current != null)
            {
                hierarchyOfA.Add(current);
                current = current.Parent;
            }

            Node? previous = null;
            Node? result = b;
            while(result != null && !hierarchyOfA.Contains(result))
            {
                previous = result;
                result = result.Parent;
            }

            int index = hierarchyOfA.IndexOf(result!);

            if(previous == null)
            {
                aIsAboveB = false;
            }
            else if(index == 0)
            {
                aIsAboveB = true;
            }
            else
            {
                Node sibling = hierarchyOfA[index - 1];
                    
                int indexOfA = ((ParentNode)result!).ChildNodes.IndexOf(sibling);
                int indexOfB = ((ParentNode)result!).ChildNodes.IndexOf(previous);

                aIsAboveB = indexOfA < indexOfB;
            }
            

            return (ParentNode)result!;
        }

        public StringNode[] GetStringNodes()
        {
            if(this is ParentNode parent)
            {
                return parent.OfType<StringNode>().ToArray();
            }
            else if(this is StringNode stringNode)
            {
                return [stringNode];
            }

            throw new ArgumentException($"Node \"{Name}\" is not of valid type!");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
