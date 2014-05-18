namespace Qisi.Editor.Documents
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal class Option : IDisposable
    {
        private System.Drawing.Font font;
        private Qisi.Editor.Documents.Line line;
        private int optionFontSize = 20;
        private System.Drawing.Region region;

        public Option(int start, int count, string value)
        {
            this.font = new System.Drawing.Font("微软雅黑", (float) this.optionFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            this.region = new System.Drawing.Region();
            this.line = null;
            this.StartIndex = start;
            this.Count = count;
            this.Checked = false;
            this.Region = new System.Drawing.Region();
            this.Value = value;
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
                if (this.font != null)
                {
                    this.font.Dispose();
                }
                if (this.region != null)
                {
                    this.region.Dispose();
                }
            }
            this.region = null;
            this.font = null;
            this.line = null;
        }

        public void Draw(Graphics g, string s)
        {
            FontFamily fontFamily = this.Font.FontFamily;
            int cellAscent = fontFamily.GetCellAscent(this.Font.Style);
            float num2 = (this.Font.Size * cellAscent) / ((float) fontFamily.GetEmHeight(this.Font.Style));
            PointF point = new PointF(this.Left, (this.Line.Top + this.Line.BaseLine) - num2);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            if (this.Answer)
            {
                g.DrawString(s + ".", this.Font, Brushes.Red, point, genericTypographic);
            }
            else
            {
                g.DrawString(s + ".", this.Font, Brushes.Black, point, genericTypographic);
            }
            SizeF size = g.MeasureString(s + ".", this.Font, 0, genericTypographic);
            point.X -= size.Width * 0.3f;
            size.Width *= 1.6f;
            if (this.Checked)
            {
                Pen pen = new Pen(Color.Blue, 3f);
                g.DrawEllipse(pen, new RectangleF(point, size));
                pen.Dispose();
            }
        }

        ~Option()
        {
            this.Dispose(false);
        }

        internal bool Answer { get; set; }

        internal bool Checked { get; set; }

        internal int Count { get; set; }

        internal System.Drawing.Font Font
        {
            get
            {
                return this.font;
            }
        }

        internal float Left { get; set; }

        public Qisi.Editor.Documents.Line Line
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

        public System.Drawing.Region Region
        {
            get
            {
                return this.region;
            }
            set
            {
                this.region = value;
            }
        }

        internal int StartIndex { get; set; }

        internal string Value { get; set; }

        internal float Width { get; set; }
    }
}

