using J113D.Common.JSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace J113D.TranslationEditor.Data.JSON
{
    internal class JsonFormatConverter : SimpleJsonObjectConverter<Format>
    {
        private const string _name = nameof(Format.Name);
        private const string _author = nameof(Format.Author);
        private const string _language = nameof(Format.Language);
        private const string _versions = nameof(Format.Versions);
        private const string _childNodes = nameof(ParentNode.ChildNodes);

        public override ReadOnlyDictionary<string, PropertyDefinition> PropertyDefinitions { get; } = new(new Dictionary<string, PropertyDefinition>()
        {
            { _name, new(PropertyTokenType.String, string.Empty) },
            { _author, new(PropertyTokenType.String, string.Empty) },
            { _language, new(PropertyTokenType.String, string.Empty) },
            { _versions, new(PropertyTokenType.Array, null) },
            { _childNodes, new(PropertyTokenType.Array, null) },
        });

        protected override object? ReadValue(ref Utf8JsonReader reader, string propertyName, ReadOnlyDictionary<string, object?> values, JsonSerializerOptions options)
        {
            switch(propertyName)
            {
                case _name:
                case _author:
                case _language:
                    return reader.GetString();
                case _versions:
                    return JsonSerializer.Deserialize<List<Version>>(ref reader, options);
                case _childNodes:
                    return JsonSerializer.Deserialize<List<Node>>(ref reader, options);
                default:
                    throw new InvalidPropertyException();
            }
        }

        protected override Format Create(ReadOnlyDictionary<string, object?> values)
        {
            string name = (string)values[_name]!;
            string author = (string)values[_author]!;
            string language = (string)values[_language]!;

            List<Version> versions = (List<Version>?)values[_versions]
                ?? throw new InvalidDataException("Format has no versions!");

            List<Node> childNodes = (List<Node>?)values[_childNodes]
                ?? throw new InvalidDataException("Format has no child nodes!");

            return new(name, author, language, versions, childNodes);
        }

        protected override void WriteValues(Utf8JsonWriter writer, Format value, JsonSerializerOptions options)
        {
            if(!string.IsNullOrWhiteSpace(value.Name))
            {
                writer.WriteString(_name, value.Name);
            }

            if(!string.IsNullOrWhiteSpace(value.Author))
            {
                writer.WriteString(_author, value.Author);
            }

            if(!string.IsNullOrWhiteSpace(value.Language))
            {
                writer.WriteString(_language, value.Language);
            }

            writer.WritePropertyName(_versions);
            JsonSerializer.Serialize(writer, value.Versions, options);

            writer.WritePropertyName(_childNodes);
            JsonSerializer.Serialize(writer, value.RootNode.ChildNodes, options);
        }
    }
}
