namespace Qisi.General.Controls
{
    using System;
    using System.Windows.Forms;

    public class ImeModeEventArgs : EventArgs
    {
        private System.Windows.Forms.ImeMode _imemode;

        public ImeModeEventArgs(System.Windows.Forms.ImeMode imemode)
        {
            this._imemode = imemode;
        }

        public System.Windows.Forms.ImeMode ImeMode
        {
            get
            {
                return this._imemode;
            }
        }
    }
}

