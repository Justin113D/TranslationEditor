using Avalonia;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.ProjectApp.Views.Export
{
    internal class XAMLExportFileHandler : BaseFileHandler
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


        protected override bool AskForResetConfirmation => false;

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


        public XAMLExportFileHandler(Visual visual) : base(visual, null) { }


        protected override void InternalSave(Uri filePath)
        {
            string text = ExportType switch
            {
                XAMLType.AvaloniaResource => ViewModel.ExportAXAML(Grouped),
                _ => ViewModel.ExportXAML(Grouped),
            };

            File.WriteAllText(filePath.AbsolutePath, text);
        }
    }
}
