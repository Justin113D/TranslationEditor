using J113D.Common.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace J113D.TranslationEditor.Data.Json
{
    public class JsonProjectConverter : SimpleJsonObjectConverter<JsonProject>
    {
        private const string _name = nameof(JsonProject.Name);
        private const string _author = nameof(JsonProject.Author);
        private const string _language = nameof(JsonProject.Language);
        private const string _version = nameof(JsonProject.Version);
        private const string _values = nameof(JsonProject.Values);

        public override ReadOnlyDictionary<string, PropertyDefinition> PropertyDefinitions { get; } = new(new Dictionary<string, PropertyDefinition>()
        {
            { _name, new(PropertyTokenType.String, string.Empty) },
            { _author, new(PropertyTokenType.String, string.Empty) },
            { _language, new(PropertyTokenType.String, string.Empty) },
            { _version, new(PropertyTokenType.String, null) },
            { _values, new(PropertyTokenType.Object, null) },
        });

        protected override object? ReadValue(ref Utf8JsonReader reader, string propertyName, ReadOnlyDictionary<string, object?> values, JsonSerializerOptions options)
        {
            switch(propertyName)
            {
                case _name:
                case _author:
                case _language:
                    return reader.GetString();
                case _version:
                    return JsonSerializer.Deserialize<Version>(ref reader, options);
                case _values:
                    return JsonSerializer.Deserialize<Dictionary<string, JsonProjectValue>>(ref reader, options);
                default:
                    throw new InvalidPropertyException();
            }
        }

        protected override JsonProject Create(ReadOnlyDictionary<string, object?> values)
        {
            string author = (string)values[_author]!;
            string language = (string)values[_language]!;

            string name = (string?)values[_name]
                ?? throw new InvalidDataException("Project has no name!");

            Version version = (Version?)values[_version]
                ?? throw new InvalidDataException("Project has no version!");

            Dictionary<string, JsonProjectValue> projectValues = (Dictionary<string, JsonProjectValue>?)values[_values]
                ?? throw new InvalidDataException("Format has no values!");

            return new(name, author, language, version, projectValues);
        }

        protected override void WriteValues(Utf8JsonWriter writer, JsonProject value, JsonSerializerOptions options)
        {
            writer.WriteString(_name, value.Name);

            if(!string.IsNullOrWhiteSpace(value.Author))
            {
                writer.WriteString(_author, value.Author);
            }

            if(!string.IsNullOrWhiteSpace(value.Language))
            {
                writer.WriteString(_language, value.Language);
            }

            writer.WritePropertyName(_version);
            JsonSerializer.Serialize(writer, value.Version, options);

            writer.WritePropertyName(_values);
            JsonSerializer.Serialize(writer, value.Values, options);
        }
    }
}
