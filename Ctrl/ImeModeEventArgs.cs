namespace Qisi.General.Controls
{
    using System;
    using System.Windows.Forms;

    public class ImeModeEventArgs : EventArgs
    {
        private System.Windows.Forms.ImeMode _imemode;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.Controls.ImeModeEventArgs"/> class.
		/// </summary>
		/// <param name="imemode">Imemode.</param>
        public ImeModeEventArgs(System.Windows.Forms.ImeMode imemode)
        {
            this._imemode = imemode;
        }
		/// <summary>
		/// Gets the IME mode.
		/// </summary>
		/// <value>The IME mode.</value>
        public System.Windows.Forms.ImeMode ImeMode
        {
            get
            {
                return this._imemode;
            }
        }
    }
}

