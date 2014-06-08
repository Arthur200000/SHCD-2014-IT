namespace Qisi.Editor.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
	/// <summary>
	/// Image event arguments.
	/// </summary>
    public class ImageEventArgs : EventArgs
    {
        public ImageEventArgs(System.Drawing.Image image)
        {
            this.Image = image;
        }

        public System.Drawing.Image Image { get; set; }
    }
}

