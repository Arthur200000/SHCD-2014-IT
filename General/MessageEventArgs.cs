namespace Qisi.General
{
    using System;

    public class MessageEventArgs : EventArgs
    {
        private string _message;

        public MessageEventArgs(string Message)
        {
            this._message = Message;
        }

        public string Message
        {
            get
            {
                return this._message;
            }
        }
    }
}

