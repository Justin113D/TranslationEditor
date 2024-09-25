using System.Text.Json;

namespace J113D.TranslationEditor.Data.Json
{
    public static class JsonConverterHelper
    {
        public static void AddConverters(JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonNodeConverter());
            options.Converters.Add(new JsonFormatConverter());
            options.Converters.Add(new JsonProjectConverter());
            options.Converters.Add(new JsonProjectValueConverter());
        }

        public static JsonSerializerOptions CreateOptions(bool indent)
        {
            JsonSerializerOptions result = new()
            {
                WriteIndented = indent
            };

            AddConverters(result);
            return result;
        }
    }
}
