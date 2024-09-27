using Avalonia;
using Avalonia.Platform.Storage;
using J113D.Avalonia.Utilities.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace J113D.TranslationEditor.ProjectApp.Views.Toolbar
{
    internal sealed class ProjectFileHandler : BaseFileHandler
    {
        protected override string FileTypeName 
            => "Language project";

        public override IReadOnlyList<FilePickerFileType>? FileType { get; } = 
        [
            new("Language Project") {
                Patterns = ["*.json"]
            }
        ];


        public ProjectFileHandler(Visual visual, IFileChangeTracker fileChangeTracker) : base(visual, fileChangeTracker) { }


        protected override void InternalReset()
        {
            ViewModel.NewProject();
        }

        protected override void InternalLoad(Uri filePath)
        {
            ViewModel.ReadProject(File.ReadAllText(filePath.AbsolutePath));
        }

        protected override void InternalSave(Uri filePath)
        {
            File.WriteAllText(filePath.AbsolutePath, ViewModel.WriteProject());
        }
    }
}
