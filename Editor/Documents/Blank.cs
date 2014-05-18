namespace Qisi.Editor.Documents
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class Blank : IDisposable
    {
        private GraphicsPath graphicspath = new GraphicsPath();
        private Pen pen = new Pen(Color.Black, 2f);
        private System.Drawing.Region region = new System.Drawing.Region();
        private List<UnderLine> underlines = new List<UnderLine>();

        internal Blank(int startIndex, int count, int maxCharsCount, float minLength, bool cr = false)
        {
            this.StartIndex = startIndex;
            this.Count = count;
            this.MaxCharsCount = maxCharsCount;
            this.MinLength = minLength;
            this.Refreshed = false;
            this.AllowCR = cr;
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
                if (this.graphicspath != null)
                {
                    this.graphicspath.Dispose();
                }
                if (this.region != null)
                {
                    this.region.Dispose();
                }
                if (this.pen != null)
                {
                    this.pen.Dispose();
                }
                if (this.underlines != null)
                {
                    foreach (UnderLine line in this.underlines)
                    {
                        line.Dispose();
                    }
                }
            }
            if (this.graphicspath != null)
            {
                this.graphicspath = null;
            }
            if (this.region != null)
            {
                this.region = null;
            }
            if (this.pen != null)
            {
                this.pen = null;
            }
            if (this.underlines != null)
            {
                for (int i = 0; i < this.underlines.Count; i++)
                {
                    this.underlines[i] = null;
                }
            }
            this.underlines = null;
        }

        internal void Draw(Graphics g)
        {
            if (this.pen == null)
            {
                this.pen = new Pen(Color.Black, 2f);
                this.pen.StartCap = LineCap.Flat;
                this.pen.EndCap = LineCap.Flat;
            }
            g.DrawPath(this.pen, this.Path);
        }

        ~Blank()
        {
            this.Dispose(false);
        }

        internal bool AllowCR { get; set; }

        internal int Count { get; set; }

        internal int MaxCharsCount { get; set; }

        internal float MinLength { get; set; }

        internal GraphicsPath Path
        {
            get
            {
                this.graphicspath = new GraphicsPath();
                foreach (UnderLine line in this.UnderLines)
                {
                    this.graphicspath.AddLine((float) line.StartX, line.Line.Top + line.Line.Height, (float) line.EndX, line.Line.Top + line.Line.Height);
                    this.graphicspath.CloseFigure();
                }
                return this.graphicspath;
            }
        }

        internal bool Refreshed { get; set; }

        internal System.Drawing.Region Region
        {
            get
            {
                this.region = new System.Drawing.Region();
                this.region.MakeEmpty();
                foreach (UnderLine line in this.UnderLines)
                {
                    this.region.Union(new RectangleF((float) line.StartX, line.Line.Top, (float) (line.EndX - line.StartX), line.Line.Height));
                }
                return this.region;
            }
        }

        internal int StartIndex { get; set; }

        internal List<UnderLine> UnderLines
        {
            get
            {
                return this.underlines;
            }
            set
            {
                this.underlines = value;
            }
        }
    }
}

