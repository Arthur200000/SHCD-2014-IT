namespace Qisi.Editor.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public class ImageEventArgs : EventArgs
    {
        public ImageEventArgs(System.Drawing.Image image)
        {
            this.Image = image;
        }

        public System.Drawing.Image Image { get; set; }
    }
}

