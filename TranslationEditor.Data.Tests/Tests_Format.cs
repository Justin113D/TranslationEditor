using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static J113D.UndoRedo.GlobalChangeTracker;

namespace J113D.TranslationEditor.Data.Tests
{
    [TestClass]
    public class Tests_Format
    {
        private static Format TestFormat()
        {
            Format format = new();

            // single node
            StringNode single = new("Single", "Test");

            // under parent
            ParentNode parent = new("Parent");
            StringNode childNode1 = new("ChildNode1", "Test");
            StringNode childNode2 = new("ChildNode2", "Test");

            // parenting this to the already added parent
            ParentNode subParent = new("Subparent");
            StringNode subchildNode1 = new("SubChildNode1", "Test");
            StringNode subchildNode2 = new("SubChildNode2", "Test");

            parent.AddChildNode(childNode1);
            parent.AddChildNode(childNode2);

            subParent.AddChildNode(subchildNode1);
            subParent.AddChildNode(subchildNode2);

            format.RootNode.AddChildNode(single);
            format.RootNode.AddChildNode(parent);
            parent.AddChildNode(subParent);

            return format;
        }

        [TestMethod]
        public void Header_StringNodeParenting()
        {
            Format format = new();

            // single node
            StringNode single = new("Single", "Test");

            // under parent
            ParentNode parent = new("Parent");
            StringNode childNode1 = new("ChildNode1", "Test");
            StringNode childNode2 = new("ChildNode2", "Test");

            // parenting this to the already added parent
            ParentNode subParent = new("Subparent");
            StringNode subchildNode1 = new("SubChildNode1", "Test");
            StringNode subchildNode2 = new("SubChildNode2", "Test");

            parent.AddChildNode(childNode1);
            parent.AddChildNode(childNode2);

            subParent.AddChildNode(subchildNode1);
            subParent.AddChildNode(subchildNode2);

            format.RootNode.AddChildNode(single);
            format.RootNode.AddChildNode(parent);
            parent.AddChildNode(subParent);

            // check if all nodes exist
            Assert.AreEqual(single, format[single.Name]);
            Assert.AreEqual(childNode1, format[childNode1.Name]);
            Assert.AreEqual(childNode2, format[childNode2.Name]);
            Assert.AreEqual(subchildNode1, format[subchildNode1.Name]);
            Assert.AreEqual(subchildNode2, format[subchildNode2.Name]);

            format.RootNode.RemoveChildNode(parent);

            Assert.IsFalse(format.TryGetStringNode(childNode1.Name, out _));
            Assert.IsFalse(format.TryGetStringNode(childNode2.Name, out _));
            Assert.IsFalse(format.TryGetStringNode(subchildNode1.Name, out _));
            Assert.IsFalse(format.TryGetStringNode(subchildNode2.Name, out _));
        }

        [TestMethod]
        public void Header_StringNodeNameAltering()
        {
            const string Name = "String";
            const string NameNumbered = Name + ".001";
            const string NameNumbered2 = Name + ".002";
            const string NameLower = "string";
            const string NameLowerNumbered = NameLower + ".003";
            const string AltName = "AltString";
            const string AltNameNumbered = AltName + ".001";
            const string AltNameNumbered2 = AltName + ".002";

            Format format = new();

            StringNode stringNode = new(Name, "Test");
            StringNode stringNode2 = new(Name, "Test");
            StringNode stringNode3 = new(NameNumbered, "Test");
            StringNode stringNode4 = new(NameLower, "Test");

            StringNode altStringNode = new(AltName, "Test");
            StringNode altStringNode2 = new(AltName, "Test");
            StringNode altStringNode3 = new(AltName, "Test");

            format.RootNode.AddChildNode(stringNode); // regular adding
            format.RootNode.AddChildNode(stringNode2); // adding .001
            format.RootNode.AddChildNode(stringNode3); // counting .001 up to .002
            format.RootNode.AddChildNode(stringNode4); // even though its lower case, it should have a .003 added

            format.RootNode.AddChildNode(altStringNode);
            altStringNode.Name = AltNameNumbered; // renaming an already added node

            format.RootNode.AddChildNode(altStringNode2); // adding node of previous existing name
            format.RootNode.AddChildNode(altStringNode3); // adding .002


            Assert.AreEqual(stringNode, format[Name]);
            Assert.AreEqual(stringNode2, format[NameNumbered]);
            Assert.AreEqual(stringNode3, format[NameNumbered2]);
            Assert.AreEqual(stringNode4, format[NameLowerNumbered]);
            Assert.AreEqual(stringNode.Name, Name);
            Assert.AreEqual(stringNode2.Name, NameNumbered);
            Assert.AreEqual(stringNode3.Name, NameNumbered2);
            Assert.AreEqual(stringNode4.Name, NameLowerNumbered);

            Assert.AreEqual(altStringNode, format[AltNameNumbered]);
            Assert.AreEqual(altStringNode2, format[AltName]);
            Assert.AreEqual(altStringNode3, format[AltNameNumbered2]);
            Assert.AreEqual(altStringNode.Name, AltNameNumbered);
            Assert.AreEqual(altStringNode2.Name, AltName);
            Assert.AreEqual(altStringNode3.Name, AltNameNumbered2);
        }

        [TestMethod]
        public void Header_StringNodeParenting_UndoRedo()
        {
            const string ChildNodeName = "ChildNode";
            const string ChildNodeNameNumbered = ChildNodeName + ".001";
            const string SubChildNodeName = "SubChildNode";
            const string SubChildNodeNameNumbered = SubChildNodeName + ".001";

            Format format = new();

            // single node
            StringNode single = new("Single", "Test");

            // under parent
            ParentNode parent = new("Parent");
            StringNode childNode1 = new(ChildNodeName, "Test");
            StringNode childNode2 = new(ChildNodeName, "Test");

            // parenting this to the already added parent
            ParentNode subParent = new("Subparent");
            StringNode subchildNode1 = new(SubChildNodeName, "Test");
            StringNode subchildNode2 = new(SubChildNodeName, "Test");

            parent.AddChildNode(childNode1);
            parent.AddChildNode(childNode2);

            subParent.AddChildNode(subchildNode1);
            subParent.AddChildNode(subchildNode2);

            format.RootNode.AddChildNode(single);
            format.RootNode.AddChildNode(parent);
            parent.AddChildNode(subParent);

            UndoChange();
            Assert.IsFalse(format.TryGetStringNode(subchildNode1.Name, out _));
            Assert.IsFalse(format.TryGetStringNode(subchildNode2.Name, out _));
            Assert.AreEqual(subchildNode1.Name, SubChildNodeName);
            Assert.AreEqual(subchildNode2.Name, SubChildNodeName);

            UndoChange();
            Assert.IsFalse(format.TryGetStringNode(childNode1.Name, out _));
            Assert.IsFalse(format.TryGetStringNode(childNode2.Name, out _));
            Assert.AreEqual(childNode1.Name, ChildNodeName);
            Assert.AreEqual(childNode2.Name, ChildNodeName);

            UndoChange();
            Assert.IsFalse(format.TryGetStringNode(single.Name, out _));

            RedoChange();
            Assert.AreEqual(single, format[single.Name]);

            RedoChange();
            Assert.AreEqual(childNode1, format[childNode1.Name]);
            Assert.AreEqual(childNode2, format[childNode2.Name]);
            Assert.AreEqual(childNode1.Name, ChildNodeName);
            Assert.AreEqual(childNode2.Name, ChildNodeNameNumbered);

            RedoChange();
            Assert.AreEqual(subchildNode1, format[subchildNode1.Name]);
            Assert.AreEqual(subchildNode2, format[subchildNode2.Name]);
            Assert.AreEqual(subchildNode1.Name, SubChildNodeName);
            Assert.AreEqual(subchildNode2.Name, SubChildNodeNameNumbered);
        }

        [TestMethod]
        public void Header_StringNode_ChangeHeader()
        {
            Format format = new();
            Format otherHeader = new();

            StringNode node = new("Test", "test");

            node.SetParent(format.RootNode);
            node.SetParent(otherHeader.RootNode);

            Assert.IsFalse(format.TryGetStringNode(node.Name, out _));
            Assert.AreEqual(node, otherHeader[node.Name]);
        }

        [TestMethod]
        public void Header_StringNode_ChangeHeader_UndoRedo()
        {
            Format format = new();
            Format otherHeader = new();

            StringNode node = new("Test", "test");

            node.SetParent(format.RootNode);
            node.SetParent(otherHeader.RootNode);

            UndoChange();

            Assert.IsFalse(otherHeader.TryGetStringNode(node.Name, out _));
            Assert.AreEqual(node, format[node.Name]);

            RedoChange();

            Assert.IsFalse(format.TryGetStringNode(node.Name, out _));
            Assert.AreEqual(node, otherHeader[node.Name]);
        }

        [TestMethod]
        public void Header_Version_Set()
        {
            Version OneZero = new(1, 0);
            Version ZeroFive = new(0, 5);
            Version TwoZero = new(2, 0);
            Version ThreeZero = new(3, 0);
            Version FourZero = new(4, 0);

            Format format = new();
            StringNode version0 = new("String", "test");
            StringNode version0Again = new("String", "test");
            StringNode version1 = new("String", "test");
            StringNode version2 = new("String", "test");

            format.Version = OneZero;
            format.RootNode.AddChildNode(version0);

            format.Version = ZeroFive;
            format.RootNode.AddChildNode(version0Again);

            format.Version = TwoZero;
            format.RootNode.AddChildNode(version1);

            format.Version = ThreeZero;

            format.Version = FourZero;
            format.RootNode.AddChildNode(version2);

            Assert.AreEqual(format.Versions.Count, 3);
            Assert.AreEqual(version0.VersionIndex, 0);
            Assert.AreEqual(version0Again.VersionIndex, 0);
            Assert.AreEqual(version1.VersionIndex, 1);
            Assert.AreEqual(version2.VersionIndex, 2);

            Assert.AreEqual(format.Versions[0], OneZero);
            Assert.AreEqual(format.Versions[1], TwoZero);
            Assert.AreEqual(format.Versions[2], FourZero);
        }

        [TestMethod]
        public void Header_JsonConversion()
        {
            Format format = TestFormat();

            string formatString = format.WriteFormatToString(false);

            Format readHeader = Format.ReadFormatFromString(formatString);

            Assert.AreEqual(readHeader.Language, format.Language);
            Assert.AreEqual(readHeader.Author, format.Author);
            Assert.AreEqual(readHeader.Name, format.Name);

            Assert.AreEqual(readHeader.StringNodes.Count, format.StringNodes.Count);
        }

        [TestMethod]
        public void Header_Project_CompileLoad()
        {
            Format format = TestFormat();
            StringNode node1 = format["single"];
            StringNode node2 = format["childnode1"];
            StringNode node3 = format["subchildnode1"];

            node1.NodeValue += "+";
            node2.NodeValue += "\n\t+";
            node3.KeepDefault = true;

            string project = format.WriteProjectToString(true);

            Format loadHeader = TestFormat();
            loadHeader.ReadProjectFromString(project);

            Assert.AreEqual(loadHeader[node1.Name].NodeValue, node1.NodeValue);
            Assert.AreEqual(loadHeader[node2.Name].NodeValue, node2.NodeValue);
            Assert.AreEqual(loadHeader[node3.Name].NodeValue, node3.NodeValue);
        }
    }
}
