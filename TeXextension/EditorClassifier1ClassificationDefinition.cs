//------------------------------------------------------------------------------
// <copyright file="EditorClassifier1ClassificationDefinition.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace TeXeditor
{
    /// <summary>
    /// Classification type definition export for EditorClassifier1
    /// </summary>
    internal static class EditorClassifier1ClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        /// <summary>
        /// Defines the "EditorClassifier1" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor plain text (<U+FF)")]
        private static ClassificationTypeDefinition TeXtypePlain;//
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor control sequence (\\...)")]
        private static ClassificationTypeDefinition TeXtypeControl;//\test
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor registered sequence (\\alpha,...)")]
        private static ClassificationTypeDefinition TeXtypeRegistered;//\alpha, \beta, \section
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor comment (%...)")]
        private static ClassificationTypeDefinition TeXtypeComment;//%
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor operator (=, +, -, /, *)")]
        private static ClassificationTypeDefinition TeXtypeOperator;//%
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor special character (&, #, ~)")]
        private static ClassificationTypeDefinition TeXtypeSpecial;//&, #, ~
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor environment (\\begin, \\end, \\left, \\right, \\label)")]
        private static ClassificationTypeDefinition TeXtypeEnvironment;//\begin{}, \end{}, \left, \right
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor bracket (\\{, \\}, [, ], (, ))")]
        private static ClassificationTypeDefinition TeXtypeBracket;//()[]\{\}
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor inline formula ($, \\(, \\))")]
        private static ClassificationTypeDefinition TeXtypeInline;//$, \(\)
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor display formula ($$, \\[, \\])")]
        private static ClassificationTypeDefinition TeXtypeDisplay;//$$, \[\]
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("TeXeditor group ({, }, ^, _)")]
        private static ClassificationTypeDefinition TeXtypeGroup;//{}^_

#pragma warning restore 169
    }
}
