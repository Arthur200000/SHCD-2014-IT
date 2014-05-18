namespace Qisi.Editor.Documents.Elements
{
    using Qisi.Editor.Documents;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.CompilerServices;

    internal class Element : IDisposable
    {
        private Document document;
        private System.Drawing.Font font;
        private Line lineContainer;
        private int lineIndex;
        private System.Drawing.Region region;

        internal Element(System.Drawing.Font f)
        {
            this.Font = f;
            this.region = new System.Drawing.Region();
            this.lineContainer = null;
            this.document = null;
            this.Settled = false;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.region != null)
                {
                    this.region.Dispose();
                }
                if (this.font != null)
                {
                    this.font.Dispose();
                }
            }
            this.region = null;
            this.font = null;
            this.lineContainer = null;
            this.document = null;
        }

        internal virtual void Draw(Graphics g)
        {
        }

        internal virtual void DrawHighLight(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
        }

        ~Element()
        {
            this.Dispose(false);
        }

        internal virtual float BaseLine { get; set; }

        internal Document DocumentContainer
        {
            get
            {
                return this.document;
            }
            set
            {
                this.document = value;
            }
        }

        internal System.Drawing.Font Font
        {
            get
            {
                return this.font;
            }
            set
            {
                this.font = value;
            }
        }

        internal bool InBlank
        {
            get
            {
                if (this.document != null)
                {
                    foreach (Blank blank in this.document.Blanks)
                    {
                        if ((blank.StartIndex <= this.Index) && ((blank.StartIndex + blank.Count) > this.Index))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        internal int Index
        {
            get
            {
                return this.document.Elements.IndexOf(this);
            }
        }

        internal Line LineContainer
        {
            get
            {
                return this.lineContainer;
            }
            set
            {
                this.lineContainer = value;
            }
        }

        internal int LineIndex
        {
            get
            {
                if (this.lineContainer != null)
                {
                    return (this.Index - this.lineContainer.StartIndex);
                }
                return -1;
            }
        }

        internal virtual PointF Location { get; set; }

        internal PointF OutLocation
        {
            get
            {
                if (this.lineContainer != null)
                {
                    return new PointF(this.Location.X, this.lineContainer.Top);
                }
                return new PointF(this.Location.X, 0f);
            }
        }

        internal SizeF OutSize
        {
            get
            {
                if (this.lineContainer != null)
                {
                    return new SizeF(this.OutWidth, this.lineContainer.Height);
                }
                return new SizeF(this.OutWidth, 0f);
            }
        }

        internal float OutWidth { get; set; }

        internal virtual System.Drawing.Region Region
        {
            get
            {
                this.region = new System.Drawing.Region(new RectangleF(this.OutLocation, this.OutSize));
                return this.region;
            }
        }

        internal bool Settled { get; set; }

        internal virtual SizeF Size { get; set; }

        internal virtual bool Sized { get; set; }
    }
}

