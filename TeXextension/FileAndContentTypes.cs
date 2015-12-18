using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TeXeditor
{
    internal static class FileAndContentTypeDefinitions
    {
        [Export]
        [Name("TeX")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition hiddenTeXContentTypeDefinition;

        [Export]
        [FileExtension(".tex")]
        [ContentType("tex")]
        internal static FileExtensionToContentTypeDefinition hiddenTeXFileExtensionDefinition;
    }
}
