using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace TeXeditor
{
    internal class CompletionSource : ICompletionSource
    {
        private CompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;
        private List<Completion> m_compList;

        public CompletionSource(CompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            m_sourceProvider = sourceProvider;
            m_textBuffer = textBuffer;
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            List<string> strList = new List<string>();
            strList.Add("\\{\\}");
            strList.Add("\\ref{}");
            strList.Add("\\cite{}");
            strList.Add("\\label{}");
            strList.Add("\\bibitem{}");
            strList.Add("\\frac{}{}");
            strList.Add("\\part{}");
            strList.Add("\\chapter{}");
            strList.Add("\\section{}");
            strList.Add("\\subsection{}");
            strList.Add("\\subsubsection{}");
            strList.Add("\\begin{}\n\\end{}");
            strList.Add("\\begin{}");
            //strList.Add("\\begin{cases}");
            //strList.Add("\\begin{align}");
            //strList.Add("\\begin{align*}");
            //strList.Add("\\begin{bmatrix}");
            strList.Add("\\end{}");
            //strList.Add("\\end{cases}");
            //strList.Add("\\end{align}");
            //strList.Add("\\end{align*}");
            //strList.Add("\\end{bmatrix}");
            strList.Sort();
            m_compList = new List<Completion>();
            foreach (string str in strList)
                m_compList.Add(new Completion(str, str, str, null, null));

            completionSets.Add(new CompletionSet(
                "TeX",    //the non-localized title of the tab
                "TeX",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                    session),
                m_compList,
                null));
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            var navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            var extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }
}
