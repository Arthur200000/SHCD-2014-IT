namespace Qisi.General.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class CrystalButtonUpDown : CrystalButton
    {
        private IContainer components;

        public CrystalButtonUpDown() : base(TextImageRelation.ImageAboveText)
        {
            base.Size = new Size(60, 60);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            base.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}

