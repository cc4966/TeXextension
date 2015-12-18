//------------------------------------------------------------------------------
// <copyright file="EditorClassifier1.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace TeXeditor
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "EditorClassifier1" classification type.
    /// </summary>
    internal class EditorClassifier1 : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType _plain;
        private readonly IClassificationType _control;
        private readonly IClassificationType _registered;
        private readonly IClassificationType _comment;
        private readonly IClassificationType _operator;
        private readonly IClassificationType _special;
        private readonly IClassificationType _environment;
        private readonly IClassificationType _bracket;
        private readonly IClassificationType _inline;
        private readonly IClassificationType _display;
        private readonly IClassificationType _group;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorClassifier1"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal EditorClassifier1(IClassificationTypeRegistryService registry)
        {
            this._plain = registry.GetClassificationType("TeXeditor plain text (<U+FF)");
            this._control = registry.GetClassificationType("TeXeditor control sequence (\\...)");
            this._registered = registry.GetClassificationType("TeXeditor registered sequence (\\alpha,...)");
            this._comment = registry.GetClassificationType("TeXeditor comment (%...)");
            this._operator = registry.GetClassificationType("TeXeditor operator (=, +, -, /, *)");
            this._special = registry.GetClassificationType("TeXeditor special character (&, #, ~)");
            this._environment = registry.GetClassificationType("TeXeditor environment (\\begin, \\end, \\left, \\right, \\label)");
            this._bracket = registry.GetClassificationType("TeXeditor bracket (\\{, \\}, [, ], (, ))");
            this._inline = registry.GetClassificationType("TeXeditor inline formula ($, \\(, \\))");
            this._display = registry.GetClassificationType("TeXeditor display formula ($$, \\[, \\])");
            this._group = registry.GetClassificationType("TeXeditor group ({, }, ^, _)");
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var classifications = new List<ClassificationSpan>();
            var text = span.GetText();
            for (int charIndex = 0; charIndex < text.Length; charIndex++)
            {
                int start = charIndex;
                switch (text[charIndex])
                {
                    default:
                        if (text[charIndex] > 0xff)
                        {
                            for (charIndex++; charIndex < text.Length; charIndex++)
                            {
                                if (text[charIndex] <= 0xff)
                                {
                                    break;
                                }
                            }
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, charIndex - start)), _plain));
                            charIndex--;
                        }
                        break;
                    case '%':
                        for (charIndex++; charIndex < text.Length; charIndex++)
                        {
                            if (text[charIndex] != '%')
                            {
                                break;
                            }
                        }
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, charIndex - start)), _special));
                        if (span.Length > charIndex)
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + charIndex, span.Length-charIndex)), _comment));
                        charIndex = span.Length;
                        continue;
                    case '=':
                    case '+':
                    case '-':
                    case '/':
                    case '*':
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 1)), _operator));
                        continue;
                    case '\\':
                        charIndex++;
                        if (charIndex == text.Length)
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 1)), _control));
                            continue;
                        }
                        if (text[charIndex] == ' ' || text[charIndex] == '%' || text[charIndex] == '^' || text[charIndex] == '_' || text[charIndex] == '&' || text[charIndex] == '#' || text[charIndex] == '$')
                        {
                            continue;
                        }
                        if (text[charIndex] == '{' || text[charIndex] == '}')
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 2)), _bracket));
                            continue;
                        }
                        if (text[charIndex] == '[' || text[charIndex] == ']')
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 2)), _display));
                            continue;
                        }
                        if (text[charIndex] == '(' || text[charIndex] == ')')
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 2)), _inline));
                            continue;
                        }
                        if (text[charIndex] == '\\')
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 2)), _control));
                            continue;
                        }
                        for (; charIndex < text.Length; charIndex++)
                        {
                            if (text[charIndex] < 'A' || 'z' < text[charIndex] || ('Z' < text[charIndex] && text[charIndex] < 'a'))
                            {
                                break;
                            }
                        }
                        var len = charIndex - start;
                        var seq = text.Substring(start + 1, len - 1);
                        charIndex--;
                        if (seq == "begin" || seq == "end")
                        {
                            if(charIndex + 1 < text.Length && text[charIndex + 1] == '{')
                            {
                                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                                continue;
                            }
                        }
                        if (seq == "label")
                        {
                            if (charIndex + 1 < text.Length && text[charIndex + 1] == '{')
                            {
                                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                                continue;
                            }
                        }
                        if (seq == "include")
                        {
                            if (charIndex + 1 < text.Length && text[charIndex + 1] == '{')
                            {
                                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                                continue;
                            }
                        }
                        if (seq == "includegraphics")
                        {
                            if (charIndex + 1 < text.Length && (text[charIndex + 1] == '{' || text[charIndex + 1] == '['))
                            {
                                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                                continue;
                            }
                        }
                        if (seq == "if" || seq == "fi")
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                            continue;
                        }
                        if (seq == "left" || seq == "right")
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                            continue;
                        }
                        if (seq == "part" || seq == "chapter" || seq == "section" || seq == "subsection" || seq == "subsubsection")
                        {
                            if (charIndex + 1 < text.Length && text[charIndex + 1] == '{')
                            {
                                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _environment));
                                continue;
                            }
                        }
                        else if (
                            seq == "alpha" || 
                            seq == "beta" || 
                            seq == "gamma" || 
                            seq == "delta" || 
                            seq == "epsilon" || 
                            seq == "zeta" || 
                            seq == "eta" || 
                            seq == "theta" || 
                            seq == "iota" || 
                            seq == "kappa" || 
                            seq == "lambda" || 
                            seq == "mu" || 
                            seq == "nu" ||
                            //seq == "omicron" ||
                            seq == "xi" ||
                            seq == "pi" ||
                            seq == "rho" ||
                            seq == "sigma" ||
                            seq == "tau" ||
                            seq == "upsilon" ||
                            seq == "phi" ||
                            seq == "chi" ||
                            seq == "psi" ||
                            seq == "psi" ||
                            seq == "omega" ||
                            seq == "varepsilon" ||
                            seq == "vartheta" ||
                            seq == "varpi" ||
                            seq == "varrho" ||
                            seq == "varsigma" ||
                            seq == "varphi" ||
                            seq == "Gamma" ||
                            seq == "Delta" ||
                            seq == "Theta" ||
                            seq == "Lambda" ||
                            seq == "Pi" ||
                            seq == "Sigma" ||
                            seq == "Upsilon" ||
                            seq == "Phi" ||
                            seq == "Psi" ||
                            seq == "Omega" ||
                            seq == "varGamma" ||
                            seq == "varDelta" ||
                            seq == "varSigma" ||
                            seq == "varUpsilon" ||
                            seq == "varTheta" ||
                            seq == "varPhi" ||
                            seq == "varLambda" ||
                            seq == "varPsi" ||
                            seq == "varXi" ||
                            seq == "varOmega" ||
                            seq == "varPi" ||
                            seq == "digamma" ||
                            seq == "beth" ||
                            seq == "deleth" ||
                            seq == "gimel")
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _registered));
                            continue;
                        }
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, len)), _control));
                        continue;
                    case '^':
                    case '_':
                    case '{':
                    case '}':
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 1)), _group));
                        continue;
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 1)), _bracket));
                        continue;
                    case '~':
                    case '&':
                    case '#':
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 1)), _special));
                        continue;
                    case '$':
                        charIndex++;
                        if (charIndex < text.Length && text[charIndex] == '$')
                        {
                            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 2)), _display));
                            continue;
                        }
                        charIndex--;
                        classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start + start, 1)), _inline));
                        continue;
                }
            }
            return classifications;
        }

        #endregion
    }
}
