using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;

namespace J113D.TranslationEditor.FormatApp.Views.Export
{
    internal class XAMLExportFileHandler : BaseExportFileHandler
    {
        private static readonly FilePickerFileType[] _xamlFileType = [
            new("WPF Resource Dictionary") {
                Patterns = ["*.xaml"]
            }
        ];

        private static readonly FilePickerFileType[] _axamlFileType = [
            new("Avalonia Resource") {
                Patterns = ["*.axaml"]
            }
        ];

        protected override string FileTypeName => ExportType switch
        {
            XAMLType.AvaloniaResource => "Avalonia Resource Dictionary",
            _ => "WPF Resource Dictionary",
        };

        public override IReadOnlyList<FilePickerFileType>? FileType => ExportType switch
        {
            XAMLType.AvaloniaResource => _axamlFileType,
            _ => _xamlFileType,
        };

        public bool Grouped { get; set; }

        public XAMLType ExportType { get; set; }


        public XAMLExportFileHandler(Window window) : base(window) { }


        protected override string WriteText(Uri filePath)
        {
            return ExportType switch
            {
                XAMLType.AvaloniaResource => ViewModel.ExportAXAML(Grouped),
                _ => ViewModel.ExportXAML(Grouped),
            };
        }
    }
}
