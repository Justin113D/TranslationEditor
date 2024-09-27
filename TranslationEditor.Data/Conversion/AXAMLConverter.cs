using System.Security;
using System.Text;

namespace J113D.TranslationEditor.Data.Conversion
{
    public static class AXAMLConverter
    {
        public static string ConvertToAXAMLResource(this Format format, bool grouped)
        {
            StringBuilder builder = new();

            builder.AppendLine("﻿<ResourceDictionary");
            builder.AppendLine("\txmlns=\"https://github.com/avaloniaui\"");
            builder.AppendLine("\txmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
            builder.AppendLine();

            if(grouped)
            {
                WriteGroupContents(builder, format.RootNode);
            }
            else
            {
                foreach(StringNode node in format.StringNodes.Values)
                {
                    WriteStringElement(builder, node);
                }
            }

            builder.AppendLine("</ResourceDictionary>");

            return builder.ToString();
        }

        private static void WriteGroupElement(StringBuilder builder, ParentNode node)
        {
            string commentText = EscapeToXML(node.Name);
            builder.AppendLine($"\t<!-- {commentText} -->");
            WriteGroupContents(builder, node);
            builder.AppendLine();
        }

        private static void WriteGroupContents(StringBuilder builder, ParentNode node)
        {
            foreach(Node childNode in node.ChildNodes)
            {
                switch(childNode)
                {
                    case ParentNode:
                        WriteGroupElement(builder, (ParentNode)childNode);
                        break;
                    case StringNode:
                        WriteStringElement(builder, (StringNode)childNode);
                        break;
                }
            }
        }

        private static void WriteStringElement(StringBuilder builder, StringNode node)
        {
            string innertext = EscapeToXML(node.NodeValue);
            builder.AppendLine($"\t<x:String x:Key=\"{node.Name}\">{innertext}</x:String>");
        }

        private static string EscapeToXML(string value)
        {
            return SecurityElement.Escape(value).Replace("\r", "&#xD;").Replace("\n", "&#xA;");
        }
    }
}
