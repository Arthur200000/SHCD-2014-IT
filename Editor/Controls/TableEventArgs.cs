namespace Qisi.Editor.Controls
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public class TableEventArgs : EventArgs
    {
        public TableEventArgs(Point size)
        {
            this.TableSize = size;
        }

        public Point TableSize { get; set; }
    }
}

