namespace Qisi.Editor.Controls
{
    using Qisi.Editor.Properties;
    using Qisi.General.Controls;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
	/// <summary>
	/// Form format select.
	/// </summary>
    public class FormFormatSelect : Form
    {
        private IContainer components = null;
        private CrystalButtonLeftRight crystalButtonLeftRight1;
        private ListBox listBox1;
        public int selectdindex = -1;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.Editor.Controls.FormFormatSelect"/> class.
		/// </summary>
		/// <param name="strs">Strings.</param>
        public FormFormatSelect(List<string> strs)
        {
            this.InitializeComponent();
            foreach (string str in strs)
            {
                this.listBox1.Items.Add(str);
            }
        }

        private void crystalButtonLeftRight1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count >= 0)
            {
                this.selectdindex = this.listBox1.SelectedIndex;
            }
            else
            {
                this.selectdindex = -1;
            }
            base.DialogResult = DialogResult.OK;
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
            ComponentResourceManager manager = new ComponentResourceManager(typeof(FormFormatSelect));
            this.listBox1 = new ListBox();
            this.crystalButtonLeftRight1 = new CrystalButtonLeftRight();
            base.SuspendLayout();
            this.listBox1.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 0x17;
            this.listBox1.Location = new Point(0x10, 15);
            this.listBox1.Margin = new Padding(4, 4, 4, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new Size(0x200, 0x101);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
            this.crystalButtonLeftRight1.ButtonText = "确定";
            this.crystalButtonLeftRight1.Cursor = Cursors.Hand;
            this.crystalButtonLeftRight1.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.crystalButtonLeftRight1.Image = Resources.confirm;
            this.crystalButtonLeftRight1.Location = new Point(0xcb, 0x131);
            this.crystalButtonLeftRight1.Margin = new Padding(5, 6, 5, 6);
            this.crystalButtonLeftRight1.Name = "crystalButtonLeftRight1";
            this.crystalButtonLeftRight1.Size = new Size(0x7b, 0x2d);
            this.crystalButtonLeftRight1.Speed = 10;
            this.crystalButtonLeftRight1.TabIndex = 1;
            this.crystalButtonLeftRight1.Click += new EventHandler(this.crystalButtonLeftRight1_Click);
            base.AutoScaleMode = AutoScaleMode.None;
            base.ClientSize = new Size(0x221, 0x170);
            base.Controls.Add(this.crystalButtonLeftRight1);
            base.Controls.Add(this.listBox1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Margin = new Padding(4, 4, 4, 4);
            base.Name = "FormFormatSelect";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "选择粘贴的数据类型";
            base.ResumeLayout(false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count >= 0)
            {
                this.crystalButtonLeftRight1.Enabled = true;
            }
            else
            {
                this.crystalButtonLeftRight1.Enabled = false;
            }
        }
    }
}

