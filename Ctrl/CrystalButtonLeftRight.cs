namespace Qisi.General.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

    public class CrystalButtonLeftRight : CrystalButton
    {
		public CrystalButtonLeftRight() : base(TextImageRelation.ImageBeforeText)
		{
		    base.Size = new Size(60, 20);
		}
    }
}

