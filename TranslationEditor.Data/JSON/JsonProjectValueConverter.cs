using J113D.Common.JSON;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace J113D.TranslationEditor.Data.JSON
{
    public class JsonProjectValueConverter : SimpleJsonObjectConverter<JsonProjectValue>
    {
        private const string _value = nameof(JsonProjectValue.Value);
        private const string _keepDefault = nameof(JsonProjectValue.KeepDefault);
        private const string _valueVersionIndex = nameof(JsonProjectValue.ValueVersionIndex);

        public override ReadOnlyDictionary<string, PropertyDefinition> PropertyDefinitions { get; } = new(new Dictionary<string, PropertyDefinition>()
        {
            { _value, new(PropertyTokenType.String, null) },
            { _keepDefault, new(PropertyTokenType.Bool, false) },
            { _valueVersionIndex, new(PropertyTokenType.Number, 0) },
        });

        protected override object? ReadValue(ref Utf8JsonReader reader, string propertyName, ReadOnlyDictionary<string, object?> values, JsonSerializerOptions options)
        {
            switch(propertyName)
            {
                case _value:
                    return reader.GetString();
                case _keepDefault:
                    return reader.GetBoolean();
                case _valueVersionIndex:
                    return reader.GetInt32();
                default:
                    throw new InvalidPropertyException();
            }
        }

        protected override JsonProjectValue Create(ReadOnlyDictionary<string, object?> values)
        {
            bool keepDefault = (bool)values[_keepDefault]!;
            int valueVersionIndex = (int)values[_valueVersionIndex]!;

            string value = (string?)values[_value]
                ?? throw new InvalidDataException("Entry has no value!");

            return new(value, keepDefault, valueVersionIndex);
        }

        protected override void WriteValues(Utf8JsonWriter writer, JsonProjectValue value, JsonSerializerOptions options)
        {
            writer.WriteString(nameof(JsonProjectValue.Value), value.Value);

            if(value.ValueVersionIndex > 0)
            {
                writer.WriteNumber(nameof(JsonProjectValue.ValueVersionIndex), value.ValueVersionIndex);
            }

            if(value.KeepDefault)
            {
                writer.WriteBoolean(nameof(JsonProjectValue.KeepDefault), value.KeepDefault);
            }
        }
    }
}
