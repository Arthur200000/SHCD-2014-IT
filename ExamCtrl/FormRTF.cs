namespace ExamClientControlsLibrary
{
    using Qisi.General.Controls;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
	/// <summary>
	/// RTF Form.
	/// </summary>
    public class FormRTF : Form
    {
        private IContainer components;
        private ReadOnlyRichTextBox readOnlyRichTextBox1;

        public FormRTF()
        {
            this.components = null;
            this.InitializeComponent();
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="ExamClientControlsLibrary.FormRTF"/> class.
		/// </summary>
		/// <param name="rtfPath">RTF path.</param>
        public FormRTF(string rtfPath)
        {
            this.components = null;
            this.InitializeComponent();
            this.readOnlyRichTextBox1.LoadFile(rtfPath);
        }
		/// <summary>
		/// Appends the text.
		/// </summary>
		/// <param name="text">Text.</param>
        public void AppendText(string text)
        {
            this.readOnlyRichTextBox1.AppendText(text);
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
            this.readOnlyRichTextBox1 = new ReadOnlyRichTextBox();
            base.SuspendLayout();
            this.readOnlyRichTextBox1.BorderStyle = BorderStyle.None;
            this.readOnlyRichTextBox1.Cursor = Cursors.Arrow;
            this.readOnlyRichTextBox1.Dock = DockStyle.Fill;
            this.readOnlyRichTextBox1.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.readOnlyRichTextBox1.Location = new Point(0, 0);
            this.readOnlyRichTextBox1.Margin = new Padding(4);
            this.readOnlyRichTextBox1.Name = "readOnlyRichTextBox1";
            this.readOnlyRichTextBox1.ReadOnly = true;
            this.readOnlyRichTextBox1.Size = new Size(0x2fd, 0x148);
            this.readOnlyRichTextBox1.TabIndex = 0;
            this.readOnlyRichTextBox1.Text = "参考答案：";
            base.AutoScaleDimensions = new SizeF(8f, 15f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x2fd, 0x148);
            base.Controls.Add(this.readOnlyRichTextBox1);
            this.DoubleBuffered = true;
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Margin = new Padding(4, 4, 4, 4);
            base.Name = "FormRTF";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "参考答案";
            base.ResumeLayout(false);
        }
    }
}

