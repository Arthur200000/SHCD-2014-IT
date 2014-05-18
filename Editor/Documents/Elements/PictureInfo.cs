namespace Qisi.Editor.Documents.Elements
{
    using Qisi.Editor.Documents;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class PictureInfo : Pic_Tab
    {
        private List<Document> documents;
        private System.Drawing.Image image;
        private SizeF imageShowSize;
        private SizeF size;

        internal PictureInfo(System.Drawing.Image ima, Font font) : base(font)
        {
            this.image = ima;
            this.documents = new List<Document>();
            this.ImageShowSize = (SizeF) ima.Size;
        }

        internal PictureInfo(System.Drawing.Image image, Font font, SizeF showSize) : base(font)
        {
            this.Image = image;
            this.documents = new List<Document>();
            this.ImageShowSize = showSize;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.image != null)
                {
                    this.Image.Dispose();
                }
                if (this.documents != null)
                {
                    foreach (Document document in this.documents)
                    {
                        document.Dispose();
                    }
                }
            }
            this.image = null;
            if (this.documents != null)
            {
                for (int i = 0; i < this.documents.Count; i++)
                {
                    this.documents[i] = null;
                }
            }
            this.documents = null;
            base.Dispose(disposing);
        }

        internal override void Draw(Graphics g)
        {
            if (this.image != null)
            {
                base.Draw(g);
                float width = 6f;
                if (!base.AloneSelected)
                {
                    g.DrawImage(this.Image, new RectangleF(this.Location.X + base.Margin.Left, this.Location.Y + base.Margin.Top, this.ImageShowSize.Width, this.ImageShowSize.Height), new RectangleF(new PointF(0f, 0f), (SizeF) this.Image.Size), GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(this.Image, new RectangleF(this.Location.X + base.Margin.Left, this.Location.Y + base.Margin.Top, this.ImageShowSize.Width, this.ImageShowSize.Height), new RectangleF(new PointF(0f, 0f), (SizeF) this.Image.Size), GraphicsUnit.Pixel);
                    g.DrawRectangle(Pens.Black, this.Location.X + base.Margin.Left, this.Location.Y + base.Margin.Top, this.ImageShowSize.Width, this.ImageShowSize.Height);
                    g.FillRectangle(Brushes.DarkBlue, (this.Location.X + base.Margin.Left) - (width / 2f), ((this.Location.Y + base.Margin.Top) + (this.ImageShowSize.Height / 2f)) - (width / 2f), width, width);
                    g.FillRectangle(Brushes.DarkBlue, ((this.Location.X + base.Margin.Left) + (this.ImageShowSize.Width / 2f)) - (width / 2f), (this.Location.Y + base.Margin.Top) - (width / 2f), width, width);
                    g.FillRectangle(Brushes.DarkBlue, ((this.Location.X + base.Margin.Left) + this.ImageShowSize.Width) - (width / 2f), ((this.Location.Y + base.Margin.Top) + (this.ImageShowSize.Height / 2f)) - (width / 2f), width, width);
                    g.FillRectangle(Brushes.DarkBlue, ((this.Location.X + base.Margin.Left) + (this.ImageShowSize.Width / 2f)) - (width / 2f), ((this.Location.Y + base.Margin.Top) + this.ImageShowSize.Height) - (width / 2f), width, width);
                }
            }
        }

        internal override void DrawHighLight(Graphics g)
        {
            this.Draw(g);
        }

        ~PictureInfo()
        {
            this.Dispose(false);
        }

        internal List<Document> Documents
        {
            get
            {
                return this.documents;
            }
            set
            {
                this.documents = value;
            }
        }

        internal System.Drawing.Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        internal SizeF ImageShowSize
        {
            get
            {
                return this.imageShowSize;
            }
            set
            {
                if (this.imageShowSize != value)
                {
                    this.imageShowSize = value;
                    this.size = new SizeF(this.imageShowSize.Width + base.Margin.Horizontal, this.imageShowSize.Height + base.Margin.Vertical);
                }
            }
        }

        internal SizeF OriginalSize
        {
            get
            {
                if (this.image != null)
                {
                    return (SizeF) this.Image.Size;
                }
                return new SizeF(0f, 0f);
            }
        }

        internal override SizeF Size
        {
            get
            {
                return this.size;
            }
        }
    }
}

