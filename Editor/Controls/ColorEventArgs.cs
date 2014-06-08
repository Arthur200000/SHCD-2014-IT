namespace Qisi.Editor.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
	/// <summary>
	/// Color event arguments.
	/// </summary>
    public class ColorEventArgs : EventArgs
    {
        public ColorEventArgs(System.Drawing.Color color)
        {
            this.Color = color;
        }
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
        public System.Drawing.Color Color { get; set; }
    }
}

