namespace Qisi.Editor.Documents
{
    using System;

    internal class UnderLine : IDisposable
    {
        private int end;
        private Qisi.Editor.Documents.Line line;
        private int start;

        internal UnderLine(Qisi.Editor.Documents.Line ln, float num1, float num2)
        {
            this.Line = ln;
            this.StartX = (int) num1;
            this.EndX = (int) num2;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            this.line = null;
        }

        ~UnderLine()
        {
            this.Dispose(false);
        }

        internal int EndX
        {
            get
            {
                return this.end;
            }
            set
            {
                this.end = value;
            }
        }

        internal Qisi.Editor.Documents.Line Line
        {
            get
            {
                return this.line;
            }
            set
            {
                this.line = value;
            }
        }

        internal int StartX
        {
            get
            {
                return this.start;
            }
            set
            {
                this.start = value;
            }
        }
    }
}

