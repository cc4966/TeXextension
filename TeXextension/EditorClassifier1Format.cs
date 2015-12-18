//------------------------------------------------------------------------------
// <copyright file="EditorClassifier1Format.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TeXeditor
{
    /// <summary>
    /// Defines an editor format for the EditorClassifier1 type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor plain text (<U+FF)")]
    [Name("TeXeditor plain text (<U+FF)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypePlainFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypePlainFormat"/> class.
        /// </summary>
        public TeXtypePlainFormat()
        {
            this.DisplayName = "TeXeditor plain text (<U+FF)"; // Human readable version of the name
            //this.BackgroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor control sequence (\\...)")]
    [Name("TeXeditor control sequence (\\...)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeControlFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeControlFormat"/> class.
        /// </summary>
        public TeXtypeControlFormat()
        {
            this.DisplayName = "TeXeditor control sequence (\\...)"; // Human readable version of the name
            this.ForegroundColor = Colors.Tan;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor registered sequence (\\alpha,...)")]
    [Name("TeXeditor registered sequence (\\alpha,...)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeRegisteredFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeRegisteredFormat"/> class.
        /// </summary>
        public TeXtypeRegisteredFormat()
        {
            this.DisplayName = "TeXeditor registered sequence (\\alpha,...)"; // Human readable version of the name
            this.ForegroundColor = Colors.Aquamarine;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor comment (%...)")]
    [Name("TeXeditor comment (%...)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeCommentFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeCommentFormat"/> class.
        /// </summary>
        public TeXtypeCommentFormat()
        {
            this.DisplayName = "TeXeditor comment (%...)"; // Human readable version of the name
            this.ForegroundColor = Colors.Gray;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor operator (=, +, -, /, *)")]
    [Name("TeXeditor operator (=, +, -, /, *)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeOperatorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeOperatorFormat"/> class.
        /// </summary>
        public TeXtypeOperatorFormat()
        {
            this.DisplayName = "TeXeditor operator (=, +, -, /, *)"; // Human readable version of the name
            this.ForegroundColor = Colors.LightSkyBlue;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor special character (&, #, ~)")]
    [Name("TeXeditor special character (&, #, ~)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeSpecialFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeSpecialFormat"/> class.
        /// </summary>
        public TeXtypeSpecialFormat()
        {
            this.DisplayName = "TeXeditor special character (&, #, ~)"; // Human readable version of the name
            this.ForegroundColor = Colors.Orange;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor environment (\\begin, \\end, \\left, \\right, \\label)")]
    [Name("TeXeditor environment (\\begin, \\end, \\left, \\right, \\label)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeEnvironmentFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeEnvironmentFormat"/> class.
        /// </summary>
        public TeXtypeEnvironmentFormat()
        {
            this.DisplayName = "TeXeditor environment (\\begin, \\end, \\left, \\right, \\label)"; // Human readable version of the name
            this.ForegroundColor = Colors.Cyan;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor bracket (\\{, \\}, [, ], (, ))")]
    [Name("TeXeditor bracket (\\{, \\}, [, ], (, ))")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeBracketFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeBracketFormat"/> class.
        /// </summary>
        public TeXtypeBracketFormat()
        {
            this.DisplayName = "TeXeditor bracket (\\{, \\}, [, ], (, ))"; // Human readable version of the name
            this.ForegroundColor = Colors.SeaGreen;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor inline formula ($, \\(, \\))")]
    [Name("TeXeditor inline formula ($, \\(, \\))")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeInlineFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeInlineFormat"/> class.
        /// </summary>
        public TeXtypeInlineFormat()
        {
            this.DisplayName = "TeXeditor inline formula ($, \\(, \\))"; // Human readable version of the name
            this.ForegroundColor = Colors.BurlyWood;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor display formula ($$, \\[, \\])")]
    [Name("TeXeditor display formula ($$, \\[, \\])")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeDisplayFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeDisplayFormat"/> class.
        /// </summary>
        public TeXtypeDisplayFormat()
        {
            this.DisplayName = "TeXeditor display formula ($$, \\[, \\])"; // Human readable version of the name
            this.ForegroundColor = Colors.CornflowerBlue;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TeXeditor group ({, }, ^, _)")]
    [Name("TeXeditor group ({, }, ^, _)")]
    [UserVisible(true)] // This should be visible to the end user
    [Order(Before = Priority.Default)] // Set the priority to be after the default classifiers
    internal sealed class TeXtypeGroupFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeXtypeGroupFormat"/> class.
        /// </summary>
        public TeXtypeGroupFormat()
        {
            this.DisplayName = "TeXeditor group ({, }, ^, _)"; // Human readable version of the name
            this.ForegroundColor = Colors.IndianRed;
        }
    }
}
