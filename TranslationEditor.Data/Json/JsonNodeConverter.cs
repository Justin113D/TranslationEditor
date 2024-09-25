using J113D.Common.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace J113D.TranslationEditor.Data.Json
{
    public class JsonNodeConverter : SimpleJsonObjectConverter<Node>
    {
        private const string _name = nameof(Node.Name);
        private const string _description = nameof(Node.Description);
        private const string _defaultValue = nameof(StringNode.DefaultValue);
        private const string _versionIndex = nameof(StringNode.VersionIndex);
        private const string _childNodes = nameof(ParentNode.ChildNodes);

        public override ReadOnlyDictionary<string, PropertyDefinition> PropertyDefinitions { get; } = new(new Dictionary<string, PropertyDefinition>()
        {
            { _name, new(PropertyTokenType.String, null) },
            { _description, new(PropertyTokenType.String, null, true) },
            { _defaultValue, new(PropertyTokenType.String, null) },
            { _versionIndex, new(PropertyTokenType.Number, 0) },
            { _childNodes, new(PropertyTokenType.Array, null) },
        });

        protected override object? ReadValue(ref Utf8JsonReader reader, string propertyName, ReadOnlyDictionary<string, object?> values, JsonSerializerOptions options)
        {
            switch(propertyName)
            {
                case _name:
                case _description:
                case _defaultValue:
                    return reader.GetString();
                case _versionIndex:
                    return reader.GetInt32();
                case _childNodes:
                    return JsonSerializer.Deserialize<List<Node>>(ref reader, options);
                default:
                    throw new InvalidPropertyException();
            }
        }

        protected override Node Create(ReadOnlyDictionary<string, object?> values)
        {
            string name = (string?)values[_name]
                ?? throw new InvalidDataException("Node has no name!");

            string description = (string?)values[_description] ?? string.Empty;

            bool isStringNode = values[_defaultValue] is string;
            bool isParentNode = values[_childNodes] is List<Node>;

            if(isStringNode && isParentNode)
            {
                throw new InvalidDataException("Node has children and default value, unable to determine whether parent node or string node!");
            }
            else if(isStringNode)
            {
                string defaultValue = (string)values[_defaultValue]!;
                int versionIndex = (int)values[_versionIndex]!;
                return new StringNode(name, defaultValue, versionIndex, description);
            }
            else if(isParentNode)
            {
                List<Node> children = (List<Node>)values[_childNodes]!;
                return new ParentNode(name, description, children);
            }
            else
            {
                throw new InvalidDataException("Node has neither children nor default value, unable to determine whether parent node or string node!");
            }
        }

        protected override void WriteValues(Utf8JsonWriter writer, Node value, JsonSerializerOptions options)
        {
            writer.WriteString(_name, value.Name);

            if(!string.IsNullOrWhiteSpace(value.Description))
            {
                writer.WriteString(_description, value.Description);
            }

            if(value is StringNode stringNode)
            {
                writer.WriteString(_defaultValue, stringNode.DefaultValue);
                
                if(stringNode.VersionIndex > 0)
                {
                    writer.WriteNumber(_versionIndex, stringNode.VersionIndex);
                }
            }
            else if(value is ParentNode parentNode)
            {
                writer.WritePropertyName(_childNodes);
                JsonSerializer.Serialize(writer, parentNode.ChildNodes, options);
            }
        }
    }
}
