﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.Data.Tests
{
    [TestClass]
    public class Tests_StringNode
    {
        private static StringNode CreateNode()
        {
            return new("Test", "TestValue", 3);
        }

        [TestMethod]
        public void StringNode_Constructor()
        {
            const string testValue = "Test Value";
            const string testValueUntrimmed = $"  {testValue}  ";
            const int version = 3;

            UndoRedo.ChangeTracker.Pin pin = PinCurrentChange();

            StringNode node = new("Test", testValue, version);
            StringNode trimmedNode = new("Test", testValueUntrimmed, version);

            Assert.AreEqual(node.DefaultValue, testValue, "Failed to set default value");
            Assert.AreEqual(node.NodeValue, testValue, "Failed to set value");
            Assert.AreEqual(node.VersionIndex, version, "Failed to set state");
            Assert.AreEqual(node.ValueVersionIndex, -1, "Changed version index supposed to be -1");
            Assert.AreEqual(node.State, NodeState.Untranslated, "State supposed to be \"Untranslated\"");

            Assert.AreEqual(trimmedNode.DefaultValue, testValue, "Failed to trim default value");
            Assert.AreEqual(trimmedNode.NodeValue, testValue, "Failed to trim value");

            Assert.IsTrue(pin.IsValid, "A Change has been tracked");
        }

        #region Version Index

        [TestMethod]
        public void StringNode_VersionIndex_Set()
        {
            const int version = 1;
            StringNode node = CreateNode();
            node.VersionIndex = version;
            Assert.AreEqual(node.VersionIndex, version, "Setting version failed");
        }

        [TestMethod]
        public void StringNode_VersionIndex_Undo()
        {
            StringNode node = CreateNode();
            int previousVersion = node.VersionIndex;
            node.VersionIndex++;
            UndoChange();
            Assert.AreEqual(node.VersionIndex, previousVersion, "Undo failed");
        }

        [TestMethod]
        public void StringNode_VersionIndex_Redo()
        {
            const int version = 10;
            StringNode node = CreateNode();
            node.VersionIndex = version;
            UndoChange();
            RedoChange();

            Assert.AreEqual(node.VersionIndex, version, "Setting version failed");
        }

        [TestMethod]
        public void StringNode_VersionIndex_EnsureChange()
        {
            StringNode node = CreateNode();
            UndoRedo.ChangeTracker.Pin pin = PinCurrentChange();
            node.VersionIndex = node.VersionIndex;
            Assert.IsFalse(pin.IsValid, "No Change created");
        }

        #endregion

        #region Changed-Version-Index

        [TestMethod]
        public void StringNode_ChangedVersion_Set()
        {
            StringNode node = CreateNode();
            node.ValueVersionIndex = node.VersionIndex;
            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Setting changed-version failed");
        }

        [TestMethod]
        public void StringNode_ChangedVersion_CheckState()
        {
            StringNode untranslated = CreateNode();
            StringNode outdated = CreateNode();
            StringNode translated = CreateNode();

            untranslated.ValueVersionIndex = 0;
            untranslated.ValueVersionIndex = -1;
            outdated.ValueVersionIndex = 0;
            translated.ValueVersionIndex = translated.VersionIndex;

            Assert.AreEqual(untranslated.State, NodeState.Untranslated, "Nodestate expected to be \"Untranslated\"");
            Assert.AreEqual(outdated.State, NodeState.Outdated, "Nodestate expected to be \"Outdated\"");
            Assert.AreEqual(translated.State, NodeState.Translated, "Nodestate expected to be \"Translated\"");
        }

        [TestMethod]
        public void StringNode_ChangedVersion_KeepKeepDefault()
        {
            StringNode node = CreateNode();
            node.KeepDefault = true;
            node.ValueVersionIndex = -1;

            Assert.IsTrue(node.KeepDefault, "Failed to keep keepdefault");
        }

        [TestMethod]
        public void StringNode_ChangedVersion_Undo()
        {
            StringNode node = CreateNode();
            int previousChangedVersion = node.ValueVersionIndex;
            node.ValueVersionIndex = node.VersionIndex;
            UndoChange();
            Assert.AreEqual(node.ValueVersionIndex, previousChangedVersion, "Undo failed");
        }

        [TestMethod]
        public void StringNode_ChangedVersion_Redo()
        {
            StringNode node = CreateNode();
            node.ValueVersionIndex = node.VersionIndex;
            UndoChange();
            RedoChange();
            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Redo failed");
        }

        [TestMethod]
        public void StringNode_ChangedVersion_EnsureChange()
        {
            StringNode node = CreateNode();
            UndoRedo.ChangeTracker.Pin pin = PinCurrentChange();
            node.ValueVersionIndex = node.ValueVersionIndex;
            Assert.IsFalse(pin.IsValid, "No change tracked");
        }

        #endregion

        #region KeepDefault

        [TestMethod]
        public void StringNode_KeepDefault_Set()
        {
            StringNode node = CreateNode();
            node.KeepDefault = true;
            Assert.IsTrue(node.KeepDefault, "Failed to set value");
            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Version index not set correctly");
            Assert.AreEqual(node.State, NodeState.Translated, "State not set to translated");
        }

        [TestMethod]
        public void StringNode_KeepDefault_DirectlyUnset()
        {
            StringNode node = CreateNode();
            node.KeepDefault = true;
            node.KeepDefault = false;

            Assert.AreEqual(node.ValueVersionIndex, -1, "Version index not set correctly");
            Assert.AreEqual(node.State, NodeState.Untranslated, "State not set to translated");
        }

        [TestMethod]
        public void StringNode_KeepDefault_InDirectlyUnset()
        {
            StringNode node = CreateNode();
            node.KeepDefault = true;
            node.NodeValue += "+";

            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Version index not set correctly");
            Assert.AreEqual(node.State, NodeState.Translated, "State not set to translated");
            Assert.IsFalse(node.KeepDefault, "Keepdefault not reset");
        }

        [TestMethod]
        public void StringNode_KeepDefault_Undo()
        {
            StringNode node = CreateNode();

            bool oldvalue = node.KeepDefault;
            int oldChangedVersionIndex = node.ValueVersionIndex;
            NodeState oldState = node.State;

            node.KeepDefault = !node.KeepDefault;
            UndoChange();

            Assert.AreEqual(node.KeepDefault, oldvalue, "Undo failed");
            Assert.AreEqual(node.ValueVersionIndex, oldChangedVersionIndex, "Failed to undo ChangedVersionIndex");
            Assert.AreEqual(node.State, oldState, "Failed to undo State");
        }

        [TestMethod]
        public void StringNode_KeepDefault_Redo()
        {
            const bool newValue = true;
            StringNode node = CreateNode();
            node.KeepDefault = newValue;
            UndoChange();
            RedoChange();
            Assert.AreEqual(node.KeepDefault, newValue);
            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Failed to redo ChangedVersionIndex");
            Assert.AreEqual(node.State, NodeState.Translated, "Failed to redo State");
        }

        #endregion

        #region Nodevalue

        [TestMethod]
        public void StringNode_NodeValue_Set()
        {
            StringNode node = CreateNode();
            string newValue = node.NodeValue + "+";
            node.NodeValue = newValue;

            Assert.AreEqual(node.NodeValue, newValue, "Failed to set node value");
            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Changedversion wasnt updated accordingly");
        }

        [TestMethod]
        public void StringNode_NodeValue_CheckTrim()
        {
            StringNode node = CreateNode();
            string newValue = $"  {node.NodeValue}  ";
            string targetValue = newValue.Trim();
            node.NodeValue = newValue;
            Assert.AreEqual(node.NodeValue, targetValue, "Failed to trim");
        }


        [TestMethod]
        public void StringNode_NodeValue_Undo()
        {
            StringNode node = CreateNode();
            string oldValue = node.NodeValue;
            int oldChangedVersionIndex = node.ValueVersionIndex;
            node.NodeValue += "+";

            UndoChange();

            Assert.AreEqual(node.NodeValue, oldValue, "Failed to undo Node Value");
            Assert.AreEqual(node.ValueVersionIndex, oldChangedVersionIndex, "Failed to undo Changed version index");
        }

        [TestMethod]
        public void StringNode_NodeValue_Redo()
        {
            StringNode node = CreateNode();
            string newValue = node.NodeValue + "+";
            node.NodeValue = newValue;

            UndoChange();
            RedoChange();

            Assert.AreEqual(node.NodeValue, newValue, "Failed to redo node value");
            Assert.AreEqual(node.ValueVersionIndex, node.VersionIndex, "Failed to redo Changed version indedx");
        }

        [TestMethod]
        public void StringNode_NodeValue_EnsureChange()
        {
            StringNode node = CreateNode();
            UndoRedo.ChangeTracker.Pin pin = PinCurrentChange();
            node.NodeValue = node.NodeValue;
            Assert.IsFalse(pin.IsValid, "Failed to track change");
        }

        #endregion
    }
}
