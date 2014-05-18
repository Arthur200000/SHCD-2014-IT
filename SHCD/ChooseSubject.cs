namespace SHCD
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    public class ChooseSubject : MdiBase
    {
        private Button button1;
        private IContainer components = null;
        private Label label1;
        private ListBox listBox1;

        public event EventHandler SubjectSelected;

        public ChooseSubject()
        {
            this.InitializeComponent();
            this.button1.Enabled = false;
        }

        public void AddItem(string text)
        {
            this.listBox1.Items.Clear();
            foreach (string str in text.Split(new char[] { '_' }))
            {
                this.listBox1.Items.Add(str);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.SubjectSelected(this, new EventArgs());
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
            this.listBox1 = new ListBox();
            this.label1 = new Label();
            this.button1 = new Button();
            base.SuspendLayout();
            this.listBox1.Font = new Font("黑体", 15f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new Point(0x37, 0x33);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new Size(0x159, 0x90);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
            this.label1.AutoSize = true;
            this.label1.BackColor = Color.Transparent;
            this.label1.Font = new Font("黑体", 15f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label1.Location = new Point(0x33, 0x1c);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0xb1, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "请选择选考模块：";
            this.button1.AutoSize = true;
            this.button1.Cursor = Cursors.Hand;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Font = new Font("黑体", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.button1.Location = new Point(0xb2, 210);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x59, 0x1f);
            this.button1.TabIndex = 5;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.button1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.listBox1);
            this.DoubleBuffered = true;
            base.Name = "ChooseSubject";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null)
            {
                this.button1.Enabled = true;
            }
        }

        public string SelectSubject
        {
            get
            {
                return this.listBox1.SelectedItem.ToString();
            }
        }
    }
}

