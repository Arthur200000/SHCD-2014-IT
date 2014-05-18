namespace Qisi.Editor.Documents
{
    using Qisi.Editor.Documents.Elements;
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal class Line : IDisposable
    {
        private Document container;

        internal Line(float top, Font font, int startIndex, Document document, float lineWidth, float left)
        {
            this.Top = top;
            this.Height = font.Height;
            FontFamily fontFamily = font.FontFamily;
            int cellAscent = fontFamily.GetCellAscent(font.Style);
            this.BaseLine = (font.Size * cellAscent) / ((float) fontFamily.GetEmHeight(font.Style));
            this.StartIndex = startIndex;
            this.ElementCount = 0;
            this.LineWidth = lineWidth;
            this.Left = left;
            this.Right = left;
            this.container = document;
            this.container.Lines.Add(this);
            this.container.DocHeight = ((this.Top + this.Height) - document.Margin.Top) - document.DocLocation.Y;
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
            this.container = null;
        }

        ~Line()
        {
            this.Dispose(false);
        }

        internal void ResetVertical()
        {
            int num4;
            float height = this.Height;
            float num2 = 0f;
            float num3 = 0f;
            for (num4 = 0; num4 < this.ElementCount; num4++)
            {
                Element element = this.container.Elements[num4 + this.StartIndex];
                num2 = Math.Max(element.BaseLine, num2);
                num3 = Math.Max(element.Size.Height - element.BaseLine, num3);
            }
            float num5 = num2 + num3;
            this.BaseLine = num2;
            this.container.DocHeight += num5 - height;
            this.Height = num5;
            for (num4 = 0; num4 < this.ElementCount; num4++)
            {
                Element element2 = this.container.Elements[num4 + this.StartIndex];
                element2.Location = new PointF(element2.Location.X, (this.Top + this.BaseLine) - element2.BaseLine);
            }
        }

        internal void ResetVertical(Element elementToAdd)
        {
            int num4;
            float height = this.Height;
            float num2 = 0f;
            float num3 = 0f;
            for (num4 = 0; num4 < this.ElementCount; num4++)
            {
                Element element = this.container.Elements[num4 + this.StartIndex];
                num2 = Math.Max(element.BaseLine, num2);
                num3 = Math.Max(element.Size.Height - element.BaseLine, num3);
            }
            num2 = Math.Max(elementToAdd.BaseLine, num2);
            num3 = Math.Max(elementToAdd.Size.Height - elementToAdd.BaseLine, num3);
            float num5 = num2 + num3;
            this.BaseLine = num2;
            this.container.DocHeight += num5 - height;
            this.Height = num5;
            for (num4 = 0; num4 < this.ElementCount; num4++)
            {
                Element element2 = this.container.Elements[num4 + this.StartIndex];
                element2.Location = new PointF(element2.Location.X, (this.Top + this.BaseLine) - element2.BaseLine);
            }
            elementToAdd.Location = new PointF(elementToAdd.Location.X, (this.Top + this.BaseLine) - elementToAdd.BaseLine);
        }

        internal void Separate()
        {
        }

        internal float BaseLine { get; set; }

        internal Document Container
        {
            get
            {
                return this.container;
            }
            set
            {
                this.container = value;
            }
        }

        internal int ElementCount { get; set; }

        internal float Height { get; set; }

        internal float Left { get; set; }

        internal float LineWidth { get; set; }

        internal float Right { get; set; }

        internal int StartIndex { get; set; }

        internal float Top { get; set; }
    }
}

