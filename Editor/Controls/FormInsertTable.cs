namespace Qisi.Editor.Controls
{
    using Qisi.Editor.Properties;
    using Qisi.General.Controls;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
	/// <summary>
	/// Form insert table.
	/// </summary>
    internal class FormInsertTable : Form
    {
        private Point _tablesize = new Point();
        private IContainer components = null;
        private CrystalButtonLeftRight crystalButtonLeftRight1;
        private CrystalButtonLeftRight crystalButtonLeftRight2;
        private Label Label1;
        private Label Label2;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;

        public FormInsertTable()
        {
            this.InitializeComponent();
            base.Icon = SystemIcons.Information;
        }

        private void crystalButtonLeftRight1_Click(object sender, EventArgs e)
        {
            this._tablesize = new Point(Convert.ToInt32(this.numericUpDown2.Value), Convert.ToInt32(this.numericUpDown1.Value));
            base.DialogResult = DialogResult.OK;
        }

        private void crystalButtonLeftRight2_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
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
            this.Label2 = new Label();
            this.numericUpDown1 = new NumericUpDown();
            this.numericUpDown2 = new NumericUpDown();
            this.crystalButtonLeftRight2 = new CrystalButtonLeftRight();
            this.crystalButtonLeftRight1 = new CrystalButtonLeftRight();
            this.numericUpDown1.BeginInit();
            this.numericUpDown2.BeginInit();
            base.SuspendLayout();
            this.Label1.AutoSize = true;
            this.Label1.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label1.Location = new Point(12, 0x10);
            this.Label1.Name = "Label1";
            this.Label1.Size = new Size(0x2e, 0x15);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "行数:";
            this.Label2.AutoSize = true;
            this.Label2.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label2.Location = new Point(12, 0x2d);
            this.Label2.Name = "Label2";
            this.Label2.Size = new Size(0x2e, 0x15);
            this.Label2.TabIndex = 1;
            this.Label2.Text = "列数:";
            this.numericUpDown1.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.numericUpDown1.Location = new Point(0x97, 14);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new Size(0x40, 0x1d);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.TextAlign = HorizontalAlignment.Right;
            this.numericUpDown1.ThousandsSeparator = true;
            int[] bits = new int[4];
            bits[0] = 2;
            this.numericUpDown1.Value = new decimal(bits);
            this.numericUpDown2.Font = new Font("微软雅黑", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.numericUpDown2.Location = new Point(0x97, 0x2b);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new Size(0x40, 0x1d);
            this.numericUpDown2.TabIndex = 3;
            this.numericUpDown2.TextAlign = HorizontalAlignment.Right;
            this.numericUpDown2.ThousandsSeparator = true;
            bits = new int[4];
            bits[0] = 5;
            this.numericUpDown2.Value = new decimal(bits);
            this.crystalButtonLeftRight2.ButtonText = "取消";
            this.crystalButtonLeftRight2.Cursor = Cursors.Hand;
            this.crystalButtonLeftRight2.Font = new Font("微软雅黑", 10f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.crystalButtonLeftRight2.Image = Resources.cancel;
            this.crystalButtonLeftRight2.Location = new Point(0x83, 0x56);
            this.crystalButtonLeftRight2.Margin = new Padding(4, 5, 4, 5);
            this.crystalButtonLeftRight2.Name = "crystalButtonLeftRight2";
            this.crystalButtonLeftRight2.Size = new Size(0x4a, 0x19);
            this.crystalButtonLeftRight2.Speed = 10;
            this.crystalButtonLeftRight2.TabIndex = 5;
            this.crystalButtonLeftRight2.Click += new EventHandler(this.crystalButtonLeftRight2_Click);
            this.crystalButtonLeftRight1.ButtonText = "确定";
            this.crystalButtonLeftRight1.Cursor = Cursors.Hand;
            this.crystalButtonLeftRight1.Font = new Font("微软雅黑", 10f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.crystalButtonLeftRight1.Image = Resources.confirm;
            this.crystalButtonLeftRight1.Location = new Point(0x27, 0x56);
            this.crystalButtonLeftRight1.Margin = new Padding(4, 5, 4, 5);
            this.crystalButtonLeftRight1.Name = "crystalButtonLeftRight1";
            this.crystalButtonLeftRight1.Size = new Size(0x4a, 0x19);
            this.crystalButtonLeftRight1.Speed = 10;
            this.crystalButtonLeftRight1.TabIndex = 4;
            this.crystalButtonLeftRight1.Click += new EventHandler(this.crystalButtonLeftRight1_Click);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0xf8, 0x76);
            base.Controls.Add(this.crystalButtonLeftRight2);
            base.Controls.Add(this.crystalButtonLeftRight1);
            base.Controls.Add(this.numericUpDown2);
            base.Controls.Add(this.numericUpDown1);
            base.Controls.Add(this.Label2);
            base.Controls.Add(this.Label1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "FrmInsertTable";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "插入表格";
            this.numericUpDown1.EndInit();
            this.numericUpDown2.EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
		/// <summary>
		/// Gets the size of the table.
		/// </summary>
		/// <value>The size of the table.</value>
        public Point TableSize
        {
            get
            {
                return this._tablesize;
            }
        }
    }
}

