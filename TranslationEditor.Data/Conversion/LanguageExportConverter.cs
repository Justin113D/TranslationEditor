using J113D.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace J113D.TranslationEditor.Data.Conversion
{
    public static class LanguageExportConverter
    {
        public static string ConvertToKeyExport(this Format format)
        {
            StringBuilder builder = new();

            foreach(string key in format.StringNodes.Keys.OrderBy(x => x))
            {
                builder.AppendLine(key);
            }

            return builder.ToString();
        }

        public static string ConvertToValueExport(this Format format)
        {
            StringBuilder builder = new();
            builder.AppendLine(format.Name);
            builder.AppendLine(format.Version.ToString());
            builder.AppendLine(format.Language);
            builder.AppendLine(format.Author);

            foreach(KeyValuePair<string, StringNode> item in format.StringNodes.OrderBy(x => x.Key))
            {
                builder.AppendLine(item.Value.NodeValue.Escape());
            }

            return builder.ToString();
        }
    }
}
