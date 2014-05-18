namespace Qisi.Editor.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public class ColorEventArgs : EventArgs
    {
        public ColorEventArgs(System.Drawing.Color color)
        {
            this.Color = color;
        }

        public System.Drawing.Color Color { get; set; }
    }
}

