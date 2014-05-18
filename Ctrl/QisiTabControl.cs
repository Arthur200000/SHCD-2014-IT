namespace Qisi.General.Controls
{
    using System;
    using System.Windows.Forms;

    public class QisiTabControl : TabControl
    {
        public QisiTabControl()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x2000000;
                return createParams;
            }
        }
    }
}

