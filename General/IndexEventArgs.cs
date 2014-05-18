namespace Qisi.General
{
    using System;

    public class IndexEventArgs : EventArgs
    {
        private int index;

        public IndexEventArgs(int i)
        {
            this.index = i;
        }

        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }
    }
}

