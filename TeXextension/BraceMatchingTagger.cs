using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace TeXeditor
{
    internal class BraceMatchingTagger : ITagger<TextMarkerTag>
    {
        ITextView View { get; set; }
        ITextBuffer SourceBuffer { get; set; }
        SnapshotPoint? CurrentChar { get; set; }

        internal BraceMatchingTagger(ITextView view, ITextBuffer sourceBuffer)
        {
            //here the keys are the open braces, and the values are the close braces
            this.View = view;
            this.SourceBuffer = sourceBuffer;
            this.CurrentChar = null;

            this.View.Caret.PositionChanged += CaretPositionChanged;
            this.View.LayoutChanged += ViewLayoutChanged;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot) //make sure that there has really been a change
            {
                UpdateAtCaretPosition(View.Caret.Position);
            }
        }

        void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        void UpdateAtCaretPosition(CaretPosition caretPosition)
        {
            CurrentChar = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

            if (!CurrentChar.HasValue)
                return;

            var tempEvent = TagsChanged;
            if (tempEvent != null)
                tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0,
                    SourceBuffer.CurrentSnapshot.Length)));
        }

        private bool IsOpenString(SnapshotPoint startPoint, out SnapshotSpan openSpan, out string open, out string close, bool inner)
        {
            ITextSnapshotLine line = startPoint.GetContainingLine();
            string lineText = line.GetText();
            int offset = startPoint.Position - line.Start.Position;
            if (offset >= line.Length)
            {
                openSpan = new SnapshotSpan(startPoint.Snapshot, 0, 0);
                open = "";
                close = "";
                return false;
            }
            switch (lineText[offset])
            {
                case '\\':
                    if (offset + 1 < line.Length)
                    {
                        if (lineText[offset + 1] == '{')
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 2);
                            open = "\\{";
                            close = "\\}";
                            return true;
                        }
                        if (offset + 4 < line.Length
                            && lineText[offset + 1] == 'l'
                            && lineText[offset + 2] == 'e'
                            && lineText[offset + 3] == 'f'
                            && lineText[offset + 4] == 't')
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 5);
                            open = "\\left";
                            close = "\\right";
                            return true;
                        }
                    }
                    break;
                case 'l':
                    if (inner && offset > 1 && offset + 3 < line.Length)
                    {
                        if (lineText[offset - 1] == '\\' &&
                            lineText[offset + 1] == 'e' &&
                            lineText[offset + 2] == 'f' &&
                            lineText[offset + 3] == 't')
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 1, 5);
                            open = "\\left";
                            close = "\\right";
                            return true;
                        }
                    }
                    break;
                case 'e':
                    if (inner && offset > 2 && offset + 2 < line.Length)
                    {
                        if (lineText[offset - 2] == '\\' &&
                            lineText[offset - 1] == 'l' &&
                            lineText[offset + 1] == 'f' &&
                            lineText[offset + 2] == 't')
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 2, 5);
                            open = "\\left";
                            close = "\\right";
                            return true;
                        }
                    }
                    break;
                case 'f':
                    if (inner && offset > 3 && offset + 1 < line.Length)
                    {
                        if (lineText[offset - 3] == '\\' &&
                            lineText[offset - 2] == 'l' &&
                            lineText[offset - 1] == 'e' &&
                            lineText[offset + 1] == 't')
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 3, 5);
                            open = "\\left";
                            close = "\\right";
                            return true;
                        }
                    }
                    break;
                case 't':
                    if (inner && offset > 4)
                    {
                        if (lineText[offset - 4] == '\\' &&
                            lineText[offset - 3] == 'l' &&
                            lineText[offset - 2] == 'e' &&
                            lineText[offset - 1] == 'f')
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 4, 5);
                            open = "\\left";
                            close = "\\right";
                            return true;
                        }
                    }
                    break;
                case '{':
                    if(offset > 0 && lineText[offset - 1] == '\\')
                    {
                        if (inner)
                        {
                            openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 1, 2);
                            open = "\\{";
                            close = "\\}";
                            return true;
                        }
                        break;
                    }
                    openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                    open = "{";
                    close = "}";
                    return true;
                case '[':
                    openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                    open = "[";
                    close = "]";
                    return true;
                case '(':
                    openSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                    open = "(";
                    close = ")";
                    return true;
            }
            openSpan = new SnapshotSpan(startPoint.Snapshot, 0, 0);
            open = "";
            close = "";
            return false;
        }

        private bool IsCloseString(SnapshotPoint startPoint, out SnapshotSpan closeSpan, out string open, out string close, bool inner)
        {
            ITextSnapshotLine line = startPoint.GetContainingLine();
            string lineText = line.GetText();
            int offset = startPoint.Position - line.Start.Position - 1;
            if(offset < 0)
            {
                closeSpan = new SnapshotSpan(startPoint.Snapshot, 0, 0);
                open = "";
                close = "";
                return false;
            }
            switch (lineText[offset])
            {
                case '\\':
                    if (inner && offset + 1 < line.Length)
                    {
                        if (lineText[offset + 1] == '}')
                        {
                            closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 2);
                            open = "\\{";
                            close = "\\}";
                            return true;
                        }
                    }
                    if (inner && offset + 5 < line.Length)
                    {
                        if (lineText[offset + 1] == 'r' &&
                            lineText[offset + 2] == 'i' &&
                            lineText[offset + 3] == 'g' &&
                            lineText[offset + 4] == 'h' &&
                            lineText[offset + 5] == 't')
                        {
                            closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 6);
                            open = "\\left";
                            close = "\\right";
                            return true;
                        }
                    }
                    break;
                case 'r':
                    if (inner && offset + 4 < line.Length && offset > 0
                        && lineText[offset - 1] == '\\'
                        && lineText[offset + 1] == 'i'
                        && lineText[offset + 2] == 'g'
                        && lineText[offset + 3] == 'h'
                        && lineText[offset + 4] == 't')
                    {
                        closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 1, 6);
                        open = "\\left";
                        close = "\\right";
                        return true;
                    }
                    break;
                case 'i':
                    if (inner && offset + 3 < line.Length && offset > 1
                        && lineText[offset - 2] == '\\'
                        && lineText[offset - 1] == 'r'
                        && lineText[offset + 1] == 'g'
                        && lineText[offset + 2] == 'h'
                        && lineText[offset + 3] == 't')
                    {
                        closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 2, 6);
                        open = "\\left";
                        close = "\\right";
                        return true;
                    }
                    break;
                case 'g':
                    if (inner && offset + 2 < line.Length && offset > 2
                        && lineText[offset - 3] == '\\'
                        && lineText[offset - 2] == 'r'
                        && lineText[offset - 1] == 'i'
                        && lineText[offset + 1] == 'h'
                        && lineText[offset + 2] == 't')
                    {
                        closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 3, 6);
                        open = "\\left";
                        close = "\\right";
                        return true;
                    }
                    break;
                case 'h':
                    if (inner && offset + 1 < line.Length && offset > 3
                        && lineText[offset - 4] == '\\'
                        && lineText[offset - 3] == 'r'
                        && lineText[offset - 2] == 'i'
                        && lineText[offset - 1] == 'g'
                        && lineText[offset + 1] == 't')
                    {
                        closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 4, 6);
                        open = "\\left";
                        close = "\\right";
                        return true;
                    }
                    break;
                case 't':
                    if (offset > 4
                        && lineText[offset - 5] == '\\'
                        && lineText[offset - 4] == 'r'
                        && lineText[offset - 3] == 'i'
                        && lineText[offset - 2] == 'g'
                        && lineText[offset - 1] == 'h')
                    {
                        closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 5, 6);
                        open = "\\left";
                        close = "\\right";
                        return true;
                    }
                    break;
                case '}':
                    if (offset > 0)
                    {
                        if (lineText[offset - 1] == '\\')
                        {
                            closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset - 1, 2);
                            open = "\\{";
                            close = "\\}";
                            return true;
                        }
                    }
                    closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                    open = "{";
                    close = "}";
                    return true;
                case ']':
                    closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                    open = "[";
                    close = "]";
                    return true;
                case ')':
                    closeSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                    open = "(";
                    close = ")";
                    return true;
            }
            closeSpan = new SnapshotSpan(startPoint.Snapshot, 0, 0);
            open = "";
            close = "";
            return false;
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)   //there is no content in the buffer
                yield break;

            //don't do anything if the current SnapshotPoint is not initialized or at the end of the buffer
            if (!CurrentChar.HasValue || CurrentChar.Value.Position >= CurrentChar.Value.Snapshot.Length)
                yield break;

            //hold on to a snapshot of the current character
            SnapshotPoint currentChar = CurrentChar.Value;

            //if the requested snapshot isn't the same as the one the brace is on, translate our spans to the expected snapshot
            if (spans[0].Snapshot != currentChar.Snapshot)
            {
                currentChar = currentChar.TranslateTo(spans[0].Snapshot, PointTrackingMode.Positive);
            }

            //get the current char and the previous char
            var currentLine = currentChar.GetContainingLine();
            SnapshotSpan openSpan;
            SnapshotSpan closeSpan;
            string openString;
            string closeString;
            bool inner = true;
            //両方返せるように
            if (IsOpenString(currentChar, out openSpan, out openString, out closeString, inner))   //the key is the open brace
            {
                if (BraceMatchingTagger.FindMatchingCloseChar(currentChar, openString, closeString, View.TextViewLines.Count, out closeSpan, openSpan) == true)
                {
                    yield return new TagSpan<TextMarkerTag>(openSpan, new TextMarkerTag("blue"));
                    yield return new TagSpan<TextMarkerTag>(closeSpan, new TextMarkerTag("blue"));
                }
            }
            if (IsCloseString(currentChar, out closeSpan, out openString, out closeString, inner))    //the value is the close brace, which is the *previous* character 
            {
                if (BraceMatchingTagger.FindMatchingOpenChar(currentChar, openString, closeString, 0, out openSpan, closeSpan) == true)
                {
                    yield return new TagSpan<TextMarkerTag>(openSpan, new TextMarkerTag("blue"));
                    yield return new TagSpan<TextMarkerTag>(closeSpan, new TextMarkerTag("blue"));
                }
            }
        }

        private static bool FindMatchingCloseChar(SnapshotPoint startPoint, string open, string close, int maxLines, out SnapshotSpan pairSpan, SnapshotSpan Span)
        {
            ITextSnapshotLine line = startPoint.GetContainingLine();
            string lineText = line.GetText();
            int lineNumber = line.LineNumber;
            int offset = Span.End - line.Start;
            int stopLineNumber = startPoint.Snapshot.LineCount - 1;
            if (maxLines > 0)
                stopLineNumber = Math.Min(stopLineNumber, lineNumber + maxLines);
            int openCount = 0;
            while (true)
            {
                //walk the entire line
                while (offset < line.Length)
                {
                    var closePos = lineText.IndexOf(close, offset);
                    var openPos = lineText.IndexOf(open, offset);
                    if (closePos == -1)//no close
                    {
                        if (openPos == -1)
                        {
                            break;//next line
                        }
                        do
                        {
                            openCount++;
                            if (openPos + open.Length > line.Length)
                                break;
                            openPos = lineText.IndexOf(open, openPos + open.Length);
                        }
                        while (openPos > -1);
                        break;//next line
                    }
                    if (openPos == -1 || closePos < openPos)
                    {
                        if(openCount == 0)//ok
                        {
                            pairSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + closePos, close.Length);
                            return true;
                        }
                    }
                    else do
                    {
                        openCount++;
                        if (openPos + open.Length > line.Length)
                            break;
                        openPos = lineText.IndexOf(open, openPos + open.Length);
                    }
                    while (openPos > -1 && openPos < closePos);
                    openCount--;//>0 or ++ed
                    offset = closePos + close.Length;
                }

                //move on to the next line
                if (++lineNumber > stopLineNumber)
                    break;

                line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                lineText = line.GetText();
                offset = 0;
            }

            pairSpan = new SnapshotSpan(startPoint.Snapshot, 0, 0);
            return false;
        }

        private static bool FindMatchingOpenChar(SnapshotPoint startPoint, string open, string close, int maxLines, out SnapshotSpan pairSpan, SnapshotSpan Span)
        {
            ITextSnapshotLine line = startPoint.GetContainingLine();

            int lineNumber = line.LineNumber;
            int offset = Span.Start - line.Start; //move the offset to the character before this one
            //if the offset is negative, move to the previous line
            if (offset < 0)
            {
                line = line.Snapshot.GetLineFromLineNumber(--lineNumber);
                offset = line.Length;
            }
            string lineText = line.GetText();
            int stopLineNumber = 0;
            if (maxLines > 0)
                stopLineNumber = Math.Max(stopLineNumber, lineNumber - maxLines);
            int closeCount = 0;
            while (true)
            {
                // Walk the entire line
                while (offset > 0)
                {
                    var openPos = lineText.LastIndexOf(open, offset - 1);
                    var closePos = lineText.LastIndexOf(close, offset - 1);
                    if (openPos == -1)//no open
                    {
                        if (closePos == -1)
                        {
                            break;//next line
                        }
                        do
                        {
                            closeCount++;
                            if (closePos < 1)
                                break;
                            closePos = lineText.LastIndexOf(close, closePos - 1);
                        }
                        while (closePos > -1);
                        break;//next line
                    }
                    if (closePos == -1 || closePos < openPos)
                    {
                        if (closeCount == 0)//ok
                        {
                            pairSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + openPos, open.Length);
                            return true;
                        }
                    }
                    else do
                    {
                        closeCount++;
                        if (closePos < 1)
                            break;
                        closePos = lineText.LastIndexOf(close, closePos - 1);
                    }
                    while (closePos > -1 && openPos < closePos);
                    closeCount--;//>0 or ++ed
                    offset = openPos;
                }

                // Move to the previous line
                if (--lineNumber < stopLineNumber)
                    break;

                line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                lineText = line.GetText();
                offset = line.Length;
            }

            pairSpan = new SnapshotSpan(startPoint.Snapshot, 0, 0);
            return false;
        }
    }
}
