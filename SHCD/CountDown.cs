namespace SHCD
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class CountDown : MdiBase
    {
        private IContainer components = null;
        private Label Label1;

        public CountDown()
        {
            this.InitializeComponent();
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
            this.Label1 = new Label();
            base.SuspendLayout();
            this.Label1.AutoSize = true;
            this.Label1.BackColor = Color.Transparent;
            this.Label1.Font = new Font("宋体", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label1.Location = new Point(60, 0xcc);
            this.Label1.Name = "Label1";
            this.Label1.Size = new Size(0xad, 0x13);
            this.Label1.TabIndex = 0x11;
            this.Label1.Text = "正在打开试卷....";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.Label1);
            base.Name = "CountDown";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public override string Text
        {
            set
            {
                this.Label1.Text = value;
            }
        }
    }
}

