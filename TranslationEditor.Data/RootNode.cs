using System.Collections.Generic;

namespace J113D.TranslationEditor.Data
{
    internal class RootNode : ParentNode
    {
        public override Format? Format { get; }

        internal RootNode(Format format, List<Node> childNodes) 
            : base("Root", null, childNodes)
        {
            Format = format;
        }
    }
}
