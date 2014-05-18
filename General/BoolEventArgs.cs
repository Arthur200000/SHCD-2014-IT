namespace Qisi.General
{
    using System;

    public class BoolEventArgs : EventArgs
    {
        private bool _message;

        public BoolEventArgs(bool message)
        {
            this._message = message;
        }

        public bool Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }
    }
}

