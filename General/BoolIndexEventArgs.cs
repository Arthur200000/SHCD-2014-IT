namespace Qisi.General
{
    using System;
    using System.Runtime.CompilerServices;

    public class BoolIndexEventArgs : BoolEventArgs
    {
        public BoolIndexEventArgs(int index, bool n) : base(n)
        {
            this.Index = index;
        }

        public int Index { get; set; }
    }
}

