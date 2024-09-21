using J113D.Common;
using J113D.UndoRedo.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static J113D.UndoRedo.GlobalChangeTracker;
using J113D.TranslationEditor.Data.JSON;

namespace J113D.TranslationEditor.Data
{
    public class Format
    {
        #region private fields
#pragma warning disable IDE0044

        private string _name;
        private string _author;
        private string _language;

#pragma warning restore IDE0044

        private StringNode? _currentlyChangingName;
        private readonly TrackDictionary<string, StringNode> _stringNodes;
        private readonly TrackList<Version> _versions;
        #endregion

        #region Properties 

        /// <summary>
        /// Format name
        /// </summary>
        public string Name
        {
            get => _name;
            set => TrackFieldChange(this, nameof(_name), value.Trim(), "Format.Name");
        }

        /// <summary>
        /// Project author
        /// </summary>
        public string Author
        {
            get => _author;
            set => TrackFieldChange(this, nameof(_author), value.Trim(), "Format.Author");
        }

        /// <summary>
        /// Project language
        /// </summary>
        public string Language
        {
            get => _language;
            set => TrackFieldChange(this, nameof(_language), value.Trim(), "Format.Language");
        }

        public ParentNode RootNode { get; }

        /// <summary>
        /// Contains all stringnodes that appear in the hierarchy
        /// </summary>
        public ReadOnlyDictionary<string, StringNode> StringNodes { get; }

        /// <summary>
        /// All past Versions of the header
        /// </summary>
        public ReadOnlyCollection<Version> Versions { get; }

        /// <summary>
        /// The headers current active version
        /// </summary>
        public Version Version
        {
            get => _versions[^1];
            set
            {
                if(value <= Version)
                {
                    BlankChange("Format.Version");
                    return;
                }

                BeginChangeGroup("Format.Version");

                RemoveUnusedVersions();
                _versions.Add(value);

                EndChangeGroup();
            }
        }

        #endregion


        public StringNode this[string key]
            => _stringNodes.TryGetValue(key.ToLower(), out StringNode? result)
                ? result
                : throw new KeyNotFoundException($"Key {key} not found!");

        public Format() : this("New Format", string.Empty, "English", [new(0, 0, 1)], []) { }

        internal Format(string name, string author, string language, List<Version> versions, List<Node> childNodes)
        {
            _name = name;
            _author = author;
            _language = language;

            RootNode = new RootNode(this, childNodes);

            _versions = new(versions);
            Versions = new(versions);

            Dictionary<string, StringNode> nodes = [];
            foreach(StringNode node in RootNode.OfType<StringNode>())
            {
                if(!nodes.TryAdd(node.Name, node))
                {
                    throw new ArgumentException($"Node \"{node.Name}\" exists twice!");
                }
            }

            _stringNodes = new(nodes);
            StringNodes = new(_stringNodes);
        }

        public int GetMostRecentUsedVersionIndex()
        {
            bool[] usedVersions = new bool[Versions.Count];

            foreach(StringNode node in StringNodes.Values)
            {
                usedVersions[node.VersionIndex] = true;
            }

            int resultIndex = Versions.Count - 1;
            while(resultIndex >= 0 && !usedVersions[resultIndex])
            {
                resultIndex--;
            }

            return resultIndex;
        }

        private void RemoveUnusedVersions()
        {
            int newestVersionIndex = GetMostRecentUsedVersionIndex();
            if(newestVersionIndex == Versions.Count - 1)
            {
                return;
            }

            BeginChangeGroup("Format.RemoveUnusedVersions");

            for(int i = Versions.Count - 1; i > newestVersionIndex; i-- )
            {
                _versions.RemoveAt(i);
            }

            EndChangeGroup();
        }


        public bool TryGetStringNode(string key, [MaybeNullWhen(false)] out StringNode result)
        {
            return _stringNodes.TryGetValue(key.ToLower(), out result);
        }

        /// <summary>
        /// Resets all string in the stringnodes of the headers hierarchy
        /// </summary>
        public void ResetAllStrings()
        {
            BeginChangeGroup("Format.ResetAllStrings");

            foreach(StringNode n in _stringNodes.Values)
            {
                n.ResetValue();
            }

            EndChangeGroup();
        }

        public string GetFreeStringNodeName(string name)
        {
            name = new string(name.Where(c => !char.IsWhiteSpace(c)).ToArray());

            return _stringNodes.FindNextFreeKey(name, true);
        }


        internal void RemoveBranchStringNodes(Node node)
        {
            BeginChangeGroup("Format.RemoveBranchStringNodes");

            StringNode[] stringNodes = node.GetStringNodes();
            foreach(StringNode stringNode in stringNodes)
            {
                string key = stringNode.Name.ToLower();
                _stringNodes.Remove(key);
            }

            EndChangeGroup();
        }

        internal void AddBranchStringNodes(Node node, bool updateVersionIndex)
        {
            BeginChangeGroup("Format.AddBranchStringNodes");

            StringNode[] stringNodes = node.GetStringNodes();
            int currentVersionIndex = _versions.Count - 1;
            foreach(StringNode stringNode in stringNodes)
            {
                string newName = GetFreeStringNodeName(stringNode.Name);
                if(newName != stringNode.Name)
                {
                    _currentlyChangingName = stringNode;
                    stringNode.Name = newName;
                    _currentlyChangingName = null;
                }

                string key = newName.ToLower();
                _stringNodes.Add(key, stringNode);

                if(updateVersionIndex)
                {
                    stringNode.VersionIndex = currentVersionIndex;
                }
            }

            EndChangeGroup();
        }

        internal void StringNodeChangeKey(StringNode node, string oldName)
        {
            if(_currentlyChangingName == node)
            {
                return;
            }

            string lowerCase = oldName.ToLower();

            if(_stringNodes[lowerCase] != node)
            {
                throw new ArgumentException($"Old name \"{lowerCase}\" doesnt match the node \"{node.Name}\" passed. Matches: \"{_stringNodes[lowerCase].Name}\"");
            }

            string newName = node.Name.ToLower();

            BeginChangeGroup("Format.StringNodeChangeKey");
            _stringNodes.Remove(lowerCase);
            _stringNodes.Add(newName, node);
            EndChangeGroup();
        }


        #region Format I/O

        public string WriteFormatToString(bool indented)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = indented
            };

            options.Converters.Add(new JSON.JsonNodeConverter());
            options.Converters.Add(new JSON.JsonFormatConverter());

            return JsonSerializer.Serialize(this, options);
        }

        public void WriteFormatToFile(string filepath, bool indented)
        {
            using(FileStream stream = File.OpenWrite(filepath))
            {
                WriteFormat(stream, indented);
            }
        }

        public void WriteFormat(Stream stream, bool indented)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = indented
            };

            options.Converters.Add(new JsonNodeConverter());
            options.Converters.Add(new JsonFormatConverter());

            JsonSerializer.Serialize(stream, this, options);
        }


        public static Format ReadFormatFromString(string json)
        {
            JsonSerializerOptions options = new();
            options.Converters.Add(new JSON.JsonNodeConverter());
            options.Converters.Add(new JSON.JsonFormatConverter());

            return JsonSerializer.Deserialize<Format>(json, options)!;
        }

        public static Format ReadFormatFromFile(string filePath)
        {
            using(FileStream stream = File.OpenRead(filePath))
            {
                return ReadFormat(stream);
            }
        }

        public static Format ReadFormat(Stream stream)
        {
            JsonSerializerOptions options = new();
            options.Converters.Add(new JsonNodeConverter());
            options.Converters.Add(new JsonFormatConverter());

            return JsonSerializer.Deserialize<Format>(stream, options)!;
        }

        #endregion

        #region Project I/O

        public void WriteProjectToFile(string filepath, bool indented)
        {
            using(FileStream stream = File.OpenWrite(filepath))
            {
                WriteProject(stream, indented);
            }
        }

        public string WriteProjectToString(bool indented)
        {
            using(MemoryStream stream = new())
            {
                WriteProject(stream, indented);
                stream.Position = 0;
                return new StreamReader(stream).ReadToEnd();
            }
        }

        public void WriteProject(Stream stream, bool indented)
        {
            JsonProject project = new(
                Name, Author, Language, Version,
                _stringNodes
                    .Where(x => x.Value.ValueVersionIndex >= 0)
                    .ToDictionary(x => x.Key, x=> new JsonProjectValue(x.Value.NodeValue, x.Value.KeepDefault, x.Value.ValueVersionIndex))
                );
            
            JsonSerializerOptions options = new()
            {
                WriteIndented = indented
            };

            options.Converters.Add(new JsonProjectConverter());
            options.Converters.Add(new JsonProjectValueConverter());

            JsonSerializer.Serialize(stream, project, options);
        }


        public void ReadProjectFromFile(string filepath)
        {
            using(FileStream stream = File.OpenRead(filepath))
            {
                ReadProject(stream);
            }
        }

        public void ReadProjectFromString(string project)
        {
            using(MemoryStream stream = new(Encoding.UTF8.GetBytes(project)))
            {
                ReadProject(stream);
            }
        }


        public void ReadProject(Stream stream)
        {
            JsonProject project = ReadJsonProject(stream);

            BeginChangeGroup("ReadProject");

            Author = project.Author;
            Language = project.Language;

            foreach(KeyValuePair<string, StringNode> item in _stringNodes)
            {
                if(project.Values.TryGetValue(item.Key, out JsonProjectValue projectValue))
                {
                    item.Value.ImportValue(
                        projectValue.Value,
                        projectValue.ValueVersionIndex,
                        projectValue.KeepDefault);
                }
                else
                {
                    item.Value.ResetValue();
                }
            }

            EndChangeGroup();
        }


        public void ImportProjectValuesFromFile(string filepath)
        {
            using(FileStream stream = File.OpenRead(filepath))
            {
                ImportProjectValues(stream);
            }
        }

        public void ImportProjectValuesFromString(string project)
        {
            using(MemoryStream stream = new(Encoding.UTF8.GetBytes(project)))
            {
                ImportProjectValues(stream);
            }
        }

        public void ImportProjectValues(Stream stream)
        {
            JsonProject project = ReadJsonProject(stream);

            BeginChangeGroup("ImportProjectValues");

            foreach(KeyValuePair<string, JsonProjectValue> projectValue in project.Values)
            {
                if(StringNodes.TryGetValue(projectValue.Key, out StringNode? node) 
                    && projectValue.Value.ValueVersionIndex >= node.ValueVersionIndex)
                {
                    node.ImportValue(
                            projectValue.Value.Value,
                            projectValue.Value.ValueVersionIndex,
                            projectValue.Value.KeepDefault);
                }
            }

            EndChangeGroup();
        }


        private JsonProject ReadJsonProject(Stream stream)
        {
            JsonSerializerOptions options = new();
            options.Converters.Add(new JsonProjectConverter());
            options.Converters.Add(new JsonProjectValueConverter());

            JsonProject project = JsonSerializer.Deserialize<JsonProject>(stream, options)!;

            if(project.Name != Name)
            {
                throw new InvalidDataException($"Formats dont match! Format Name: {Name}, Project specified for: {project.Name}");
            }

            int versionIndex = _versions.FindIndex(x => x.Equals(project.Version));

            if(versionIndex == -1)
            {
                throw new InvalidDataException($"Version invalid! The projects version does not match any version inside the format!");
            }

            return project;
        }

        #endregion

        #region Export

        public void WriteExportToFiles(string keysFilePath, string stringsFilePath)
        {
            using(FileStream keysFileStream = File.OpenWrite(keysFilePath))
            {
                using(FileStream stringsFileStream = File.OpenWrite(stringsFilePath))
                {
                    WriteExport(keysFileStream, stringsFileStream);
                }
            }
        }

        public void WriteExportToStrings(out string keys, out string strings)
        {
            using(MemoryStream keysStream = new())
            {
                using(MemoryStream stringsStream = new())
                {
                    WriteExport(keysStream, stringsStream);
                    keysStream.Position = 0;
                    stringsStream.Position = 0;
                    keys = new StreamReader(keysStream).ReadToEnd();
                    strings = new StreamReader(stringsStream).ReadToEnd();
                }
            }
        }

        public void WriteExport(Stream keysFileStream, Stream stringsFileSteam)
        {
            StreamWriter keysWriter = new(keysFileStream);
            StreamWriter stringsWriter = new(stringsFileSteam);

            stringsWriter.WriteLine(Name);
            stringsWriter.WriteLine(Version);
            stringsWriter.WriteLine(Language);
            stringsWriter.WriteLine(Author);
            stringsWriter.Flush();

            foreach(KeyValuePair<string, StringNode> stringNode in _stringNodes)
            {
                keysWriter.WriteLine(stringNode.Key);
                stringsWriter.WriteLine(stringNode.Value.NodeValue.Escape());
                keysWriter.Flush();
                stringsWriter.Flush();
            }
        }

        #endregion
    }
}
