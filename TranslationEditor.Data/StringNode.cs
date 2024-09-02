﻿using System;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.Data
{
    /// <summary>
    /// A node holding a text value, the main focus of the language/text file
    /// </summary>
    public class StringNode : Node
    {
        #region private fields
#pragma warning disable IDE0044

        private string _nodeValue;
        private string _defaultValue;
        private int _versionIndex;
        private int _valueVersionIndex;
        private bool _keepDefault;

#pragma warning restore IDE0044
        #endregion

        #region Properties

        /// <summary>
        /// The default value for this node
        /// </summary>
        public string DefaultValue
        {
            get => _defaultValue;
            set
            {
                BeginChangeGroup("StringNode.DefaultValue");

                TrackFieldChange(this, nameof(_defaultValue), value.Trim(), "StringNode._defaultValue");

                Format? header = Format;
                if(header != null)
                {
                    VersionIndex = header.Versions.Count - 1;
                }

                if(_valueVersionIndex == -1)
                {
                    TrackNodeValueChange(_defaultValue);
                }

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Version index of this node in the format
        /// </summary>
        public int VersionIndex
        {
            get => _versionIndex;
            set => TrackFieldChange(this, nameof(_versionIndex), value, "StringNode._versionIndex");
        }

        /// <summary>
        /// Index of when the value was last changed by the translator <br/>
        /// -1 = untranslated
        /// </summary>
        public int ValueVersionIndex
        {
            get => _valueVersionIndex;
            set
            {
                value = Math.Clamp(value, -1, VersionIndex);

                if(value == _valueVersionIndex)
                {
                    BlankChange("StringNode.ValueVersionIndex");
                    return;
                }

                BeginChangeGroup("StringNode.ValueVersionIndex");

                TrackFieldChange(this, nameof(_valueVersionIndex), value, "StringNode._valueVersionIndex");

                if(value == -1)
                {
                    State = NodeState.Untranslated;
                }
                else if(value == VersionIndex)
                {
                    State = NodeState.Translated;
                }
                else // value > -1 && value < VersionIndex
                {
                    State = NodeState.Outdated;
                }

                EndChangeGroup();
            }
        }


        /// <summary>
        /// The node value - holding the (un)translated text
        /// </summary>
        public string NodeValue
        {
            get => _nodeValue;
            set
            {
                if(_nodeValue == value)
                {
                    BlankChange("StringNode.VodeValue");
                    return;
                }

                BeginChangeGroup("StringNode.NodeValue");

                TrackNodeValueChange(value);

                if(KeepDefault)
                {
                    TrackKeepDefaultChange(false);
                }

                ValueVersionIndex = value == DefaultValue ? -1 : VersionIndex;

                EndChangeGroup();
            }
        }

        public bool KeepDefault
        {
            get => _keepDefault;
            set
            {
                if(value == _keepDefault)
                {
                    BlankChange("StringNode.KeepDefault");
                    return;
                }

                BeginChangeGroup("StringNode.NodeValuKeepDefault");

                if(value)
                {
                    TrackNodeValueChange(DefaultValue);
                    ValueVersionIndex = VersionIndex;
                }

                TrackKeepDefaultChange(value);

                if(!value)
                {
                    ValueVersionIndex = -1;
                }

                EndChangeGroup();
            }
        }

        #endregion

        public StringNode(string name, string value, int versionIndex = 0, string? description = null)
            : base(name, description, NodeState.Untranslated)
        {
            _nodeValue = _defaultValue = value.Trim();
            _versionIndex = versionIndex;
            _valueVersionIndex = -1;
        }


        private void TrackNodeValueChange(string value)
        {
            TrackFieldChange(this, nameof(_nodeValue), value.Trim(), "StringNode._nodeValue");
        }

        private void TrackKeepDefaultChange(bool value)
        {
            TrackFieldChange(this, nameof(_keepDefault), value, "StringNode._keepDefault");
        }

        internal void ImportValue(string value, int changedVersionIndex, bool keepDefault)
        {
            BeginChangeGroup("StringNode.ImportValue");

            TrackNodeValueChange(value);
            TrackKeepDefaultChange(keepDefault);
            ValueVersionIndex = changedVersionIndex;

            EndChangeGroup();
        }


        /// <summary>
        /// Sets the node value to the default value
        /// </summary>
        public void ResetValue()
        {
            BeginChangeGroup("StringNode.ResetValue");
            TrackNodeValueChange(DefaultValue);
            ValueVersionIndex = -1;
            TrackKeepDefaultChange(false);
            EndChangeGroup();
        }


        protected override string ValidateName(string name)
        {
            name = base.ValidateName(name);
            return Format?.GetFreeStringNodeName(name) ?? name;
        }

        protected override void InternalOnNameChanged(string oldName)
        {
            Format?.StringNodeChangeKey(this, oldName);
        }

    }
}
