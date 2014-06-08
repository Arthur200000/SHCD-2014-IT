namespace Qisi.Editor.Controls
{
    using Qisi.Editor.Documents;
	using Qisi.Editor.Documents.Elements;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
	/// <summary>
	/// Form option info.
	/// </summary>
    public class FormOptionInfo : Form
    {
        private Button button1;
        private IContainer components = null;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label Label1;
        private NumericUpDown numericUpDown1;
		private List<SuperBox> superBoxList = new List<SuperBox>();

        public FormOptionInfo()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            this.Label1 = new Label();
            this.numericUpDown1 = new NumericUpDown();
            this.flowLayoutPanel1 = new FlowLayoutPanel();
            this.button1 = new Button();
            this.numericUpDown1.BeginInit();
            base.SuspendLayout();
            this.Label1.AutoSize = true;
            this.Label1.Location = new Point(12, 0x13);
            this.Label1.Name = "Label1";
            this.Label1.Size = new Size(0x2f, 12);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "选项数:";
            this.numericUpDown1.Location = new Point(0x41, 0x11);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new Size(0x33, 0x15);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
            int[] bits = new int[4];
            bits[0] = 4;
            this.numericUpDown1.Value = new decimal(bits);
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new Point(12, 0x2c);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new Size(0x1b2, 0xce);
            this.flowLayoutPanel1.TabIndex = 2;
            this.button1.Location = new Point(0x165, 0x11b);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 3;
            this.button1.Text = "插入";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1ca, 0x13e);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.flowLayoutPanel1);
            base.Controls.Add(this.numericUpDown1);
            base.Controls.Add(this.Label1);
            base.Name = "FormOptionInfo";
            this.Text = "选项信息";
            this.numericUpDown1.EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.superBoxList.Count >= this.numericUpDown1.Value)
            {
                while (this.superBoxList.Count > this.numericUpDown1.Value)
                {
                    this.superBoxList.RemoveAt(Convert.ToInt32(this.numericUpDown1.Value));
                    this.flowLayoutPanel1.Controls.RemoveAt(Convert.ToInt32(this.numericUpDown1.Value));
                }
            }
            else
            {
                for (int i = this.superBoxList.Count; i < this.numericUpDown1.Value; i++)
                {
                    SuperBox item = new SuperBox(this.flowLayoutPanel1.ClientSize.Width) {
                        Height = 100,
                        BackColor = Color.Aqua,
                        Visible = true
                    };
                    this.superBoxList.Add(item);
                    this.flowLayoutPanel1.Controls.Add(item);
                    this.flowLayoutPanel1.SetFlowBreak(item, true);
                }
            }
        }

        internal List<Element> Elements
        {
            get
            {
                List<Element> list = new List<Element>();
                for (int i = 0; i < this.superBoxList.Count; i++)
                {
                    list.AddRange(this.superBoxList[i].Elements);
                }
                return list;
            }
        }

        internal Qisi.Editor.Documents.Options Options
        {
            get
            {
                Qisi.Editor.Documents.Options options = new Qisi.Editor.Documents.Options(false, false);
                for (int i = 0; i < this.superBoxList.Count; i++)
                {
                    char ch = (char) (0x41 + i);
                    Option item = new Option(0, this.superBoxList[i].Elements.Count, ch.ToString());
                    options.OptionList.Add(item);
                }
                return options;
            }
        }
    }
}

