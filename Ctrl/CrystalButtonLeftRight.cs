namespace Qisi.General.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	/// <summary>
	/// Crystal button, left-right.
	/// </summary>
    public class CrystalButtonLeftRight : CrystalButton
    {
		public CrystalButtonLeftRight() : base(TextImageRelation.ImageBeforeText)
		{
		    base.Size = new Size(60, 20);
		}
    }
}

