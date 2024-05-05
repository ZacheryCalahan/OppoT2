using System.Collections;

namespace OppoT2Assembler.ErrorHandler
{
    public class SyntaxErrors : IEnumerable
    {
        List<SyntaxInfo> errorLines;
        public readonly struct SyntaxInfo
        {
            public readonly string line;
            public readonly string? label;
            public readonly Error error;

            public SyntaxInfo(string line, Error error)
            {
                this.line = line;
                this.error = error;
            }

            public SyntaxInfo(string line, Error error, string label)
            {
                this.line = line;
                this.error = error;
                this.label = label;
            }
        }

        public enum Error
        {
            // Line Errors
            InvalidTokenCount,
            InvalidLabel,
            NoLabelFound,
            InvalidInstruction,
            InvalidAddress,
            InvalidDirective,

            // Token Errors
            InvalidToken
        }

        public SyntaxErrors()
        {
            errorLines = new List<SyntaxInfo>();
        }

        public List<SyntaxInfo> GetErrorList()
        {
            return errorLines;
        }

        public bool IsEmpty()
        {
            return errorLines.Count == 0;
        }

        public void AddError(string line, Error error)
        {
            errorLines.Add(new SyntaxInfo(line, error));            
        }

        public void AddError(string line, Error error, string label)
        {
            errorLines.Add(new SyntaxInfo(line, error, label));
        }

        public IEnumerator<SyntaxInfo> GetEnumerator()
        {
            foreach (SyntaxInfo lineInfo in errorLines)
            {
                yield return lineInfo;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
