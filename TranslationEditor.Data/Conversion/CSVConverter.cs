using Csv;
using J113D.TranslationEditor.Data.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace J113D.TranslationEditor.Data.Conversionj
{
    public static class CSVConverter
    {
        public static string ConvertToCSV(this Format format, bool includeFormatProject, IEnumerable<JsonProject> projects)
        {
            List<string> columnsNames = ["keys"];

            if(includeFormatProject)
            {
                columnsNames.Add(format.Language);
            }

            foreach(JsonProject project in projects)
            {
                columnsNames.Add(project.Language);
            }

            List<string[]> rows = [];

            foreach(KeyValuePair<string, StringNode> stringNode in format.StringNodes)
            {
                List<string> row = [stringNode.Key];
                
                if(includeFormatProject)
                {
                    row.Add(stringNode.Value.NodeValue);
                }

                foreach(JsonProject project in projects)
                {
                    if(project.Values.TryGetValue(stringNode.Key, out JsonProjectValue value))
                    {
                        row.Add(value.Value);
                    }
                    else
                    {
                        row.Add(stringNode.Value.DefaultValue);
                    }
                }

                rows.Add([.. row]);
            }

            return CsvWriter.WriteToText([.. columnsNames], rows);
        }

        public static string ConvertToCSV(this Format format, bool includeFormatProject, IEnumerable<string> jsonProjects)
        {
            List<JsonProject> projects = [];
            JsonSerializerOptions options = JsonConverterHelper.CreateOptions(false);

            int i = 0;
            foreach(string jsonProject in jsonProjects)
            {
                try
                {
                    projects.Add(JsonSerializer.Deserialize<JsonProject>(jsonProject, options)!);
                }
                catch(JsonException)
                {
                    throw new InvalidDataException($"Json project {i} failed to be read");
                }

                i++;
            }

            return ConvertToCSV(format, includeFormatProject, projects);
        }

    }
}
