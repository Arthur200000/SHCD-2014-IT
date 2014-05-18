namespace Qisi.Editor.Documents.Elements
{
    using Qisi.Editor.Documents;
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    internal class Pic_Tab : Element
    {
        internal static Padding margin = new Padding(3, 3, 3, 3);

        internal Pic_Tab(Font font) : base(font)
        {
            this.Align = ImageAlign.Embedding;
            this.AloneSelected = false;
            this.Sized = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        ~Pic_Tab()
        {
            this.Dispose(false);
        }

        public ImageAlign Align { get; set; }

        public bool AloneSelected { get; set; }

        internal override float BaseLine
        {
            get
            {
                return this.Size.Height;
            }
        }

        internal Padding Margin
        {
            get
            {
                return margin;
            }
        }
    }
}

