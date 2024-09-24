using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.Data.JSON
{
    public readonly struct JsonProject(string name, string author, string language, Version version, Dictionary<string, JsonProjectValue> values)
    {
        public string Name { get; } = name;
        public string Author { get; } = author;
        public string Language { get; } = language;
        public Version Version { get; } = version;
        public Dictionary<string, JsonProjectValue> Values { get; } = values;
    }
}
