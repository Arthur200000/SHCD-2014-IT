namespace Qisi.Editor.Controls
{
    using Qisi.Editor.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;
	/// <summary>
	/// Text bar.
	/// </summary>
    public class TextBar : UserControl
    {
        private System.Drawing.Color _color;
        private System.Drawing.Font _font;
        private ColorDialog colorDialog1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private IContainer components;
        private Label Label1;
        private Label Label2;
        private Label Label3;
        private Label Label4;
        private Label Label5;
        private Label Label6;
        private OpenFileDialog openFileDialog1;

        public event ColorChangedEventHandler ColorChanged;

        public event FontChangedEventHandler FontChanged;

        public event InsertImageEventHandler InsertImage;

        public event InsertTableEventHandler InsertTable;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.Editor.Controls.TextBar"/> class.
		/// </summary>
        public TextBar() : this(new System.Drawing.Font("宋体", 20f, FontStyle.Regular, GraphicsUnit.Pixel), System.Drawing.Color.Black)
        {
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.Editor.Controls.TextBar"/> class.
		/// </summary>
		/// <param name="defaultfont">Default Font.</param>
		/// <param name="color">Color.</param>
        public TextBar(System.Drawing.Font defaultfont, System.Drawing.Color color)
        {
            this._font = null;
            this.components = null;
            this.InitializeComponent();
            this.Font = defaultfont;
            this.Color = color;
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
            {
                this.comboBox1.Items.Add(family.Name);
            }
            float[] numArray = new float[] { 
                5f, 5.5f, 6.5f, 7.5f, 8f, 9f, 10f, 10.5f, 11f, 12f, 14f, 16f, 18f, 20f, 22f, 24f, 
                26f, 28f, 36f, 48f, 72f
             };
            foreach (float num in numArray)
            {
                this.comboBox2.Items.Add(num.ToString());
            }
            if (!this.comboBox1.Items.Contains(this.Font.FontFamily.Name))
            {
                this.comboBox1.Items.Add(this.Font.FontFamily.Name);
            }
            this.comboBox1.SelectedItem = this.Font.FontFamily.Name;
            if (!this.comboBox2.Items.Contains(this.Font.Size.ToString()))
            {
                this.comboBox2.Items.Add(this.Font.Size.ToString());
            }
            this.comboBox2.Text = this.Font.Size.ToString();
            this.Label1.ForeColor = this._color;
            this.Label1.BackColor = System.Drawing.Color.FromArgb(0x7f, 0xff - this._color.R, 0xff - this._color.G, 0xff - this._color.B);
            if (this.Font.Bold)
            {
                this.Label2.BackColor = System.Drawing.Color.Yellow;
            }
            if (this.Font.Italic)
            {
                this.Label3.BackColor = System.Drawing.Color.Yellow;
            }
            if (this.Font.Underline)
            {
                this.Label4.BackColor = System.Drawing.Color.Yellow;
            }
            this.comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox2.KeyPress += new KeyPressEventHandler(this.comboBox2_KeyPress);
            this.comboBox2.SelectedIndexChanged += new EventHandler(this.comboBox2_SelectedIndexChanged);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (this.comboBox1.Items.Contains(this.comboBox1.Text))
                {
                    this.comboBox1.SelectedIndex = this.comboBox1.Items.IndexOf(this.comboBox1.Text);
                }
                else
                {
                    this.comboBox1.Text = this.Font.FontFamily.Name;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Font = new System.Drawing.Font(this.comboBox1.SelectedItem.ToString(), this.Font.Size, this.Font.Style, GraphicsUnit.Pixel);
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                float result = 0f;
                if (float.TryParse(this.comboBox2.Text, out result))
                {
                    this.Font = new System.Drawing.Font(this.Font.FontFamily, result, this.Font.Style, GraphicsUnit.Pixel);
                }
                else
                {
                    this.comboBox2.Text = this.Font.Size.ToString();
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Font = new System.Drawing.Font(this.Font.FontFamily, Convert.ToSingle(this.comboBox2.SelectedItem), this.Font.Style, GraphicsUnit.Pixel);
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
            this.comboBox1 = new ComboBox();
            this.comboBox2 = new ComboBox();
            this.Label1 = new Label();
            this.Label2 = new Label();
            this.Label3 = new Label();
            this.Label4 = new Label();
            this.Label5 = new Label();
            this.Label6 = new Label();
            this.colorDialog1 = new ColorDialog();
            this.openFileDialog1 = new OpenFileDialog();
            base.SuspendLayout();
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new Point(0, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new Size(0x79, 20);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.KeyPress += new KeyPressEventHandler(this.comboBox1_KeyPress);
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new Point(0x7f, 0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new Size(0x2d, 20);
            this.comboBox2.TabIndex = 1;
            this.Label1.Cursor = Cursors.Hand;
            this.Label1.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label1.ForeColor = SystemColors.ControlText;
            this.Label1.Location = new Point(0xb2, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new Size(0x16, 0x16);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "A";
            this.Label1.TextAlign = ContentAlignment.MiddleCenter;
            this.Label1.ForeColorChanged += new EventHandler(this.Label1_ForeColorChanged);
            this.Label1.Click += new EventHandler(this.Label1_Click);
            this.Label2.Cursor = Cursors.Hand;
            this.Label2.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label2.Location = new Point(0xce, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new Size(0x16, 0x16);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "B";
            this.Label2.TextAlign = ContentAlignment.MiddleCenter;
            this.Label2.Click += new EventHandler(this.Label2_Click);
            this.Label3.Cursor = Cursors.Hand;
            this.Label3.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Italic | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label3.Location = new Point(0xea, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new Size(0x16, 0x16);
            this.Label3.TabIndex = 4;
            this.Label3.Text = "I";
            this.Label3.TextAlign = ContentAlignment.MiddleCenter;
            this.Label3.Click += new EventHandler(this.Label3_Click);
            this.Label4.Cursor = Cursors.Hand;
            this.Label4.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label4.Location = new Point(0x106, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new Size(0x16, 0x16);
            this.Label4.TabIndex = 5;
            this.Label4.Text = "U";
            this.Label4.TextAlign = ContentAlignment.MiddleCenter;
            this.Label4.Click += new EventHandler(this.Label4_Click);
            this.Label5.Cursor = Cursors.Hand;
            this.Label5.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label5.Image = Resources.picture;
            this.Label5.Location = new Point(0x13e, 0);
            this.Label5.Name = "Label5";
            this.Label5.Size = new Size(0x16, 0x16);
            this.Label5.TabIndex = 8;
            this.Label5.Click += new EventHandler(this.Label5_Click);
            this.Label6.Cursor = Cursors.Hand;
            this.Label6.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.Label6.Image = Resources.table;
            this.Label6.Location = new Point(290, 0);
            this.Label6.Name = "Label6";
            this.Label6.Size = new Size(0x16, 0x16);
            this.Label6.TabIndex = 7;
            this.Label6.Click += new EventHandler(this.Label6_Click);
            this.openFileDialog1.FileName = "openFileDialog1";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.Label5);
            base.Controls.Add(this.Label6);
            base.Controls.Add(this.Label4);
            base.Controls.Add(this.Label3);
            base.Controls.Add(this.Label2);
            base.Controls.Add(this.Label1);
            base.Controls.Add(this.comboBox2);
            base.Controls.Add(this.comboBox1);
            base.Name = "TextBar";
            base.Size = new Size(350, 0x16);
            base.ResumeLayout(false);
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.Color = this.colorDialog1.Color;
            }
        }

        private void Label1_ForeColorChanged(object sender, EventArgs e)
        {
            if (this.ColorChanged != null)
            {
                this.ColorChanged(this, new ColorEventArgs(this.Label1.ForeColor));
            }
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            FontStyle regular;
            if (this.Font.Bold)
            {
                regular = FontStyle.Regular;
            }
            else
            {
                regular = FontStyle.Bold;
            }
            if (this.Font.Italic && this.Font.Underline)
            {
                regular |= FontStyle.Underline | FontStyle.Italic;
            }
            else if (this.Font.Italic)
            {
                regular |= FontStyle.Italic;
            }
            else if (this.Font.Underline)
            {
                regular |= FontStyle.Underline;
            }
            this.Font = new System.Drawing.Font(this.Font.FontFamily, this.Font.Size, regular, GraphicsUnit.Pixel);
        }

        private void Label3_Click(object sender, EventArgs e)
        {
            FontStyle regular;
            if (this.Font.Italic)
            {
                regular = FontStyle.Regular;
            }
            else
            {
                regular = FontStyle.Italic;
            }
            if (this.Font.Bold && this.Font.Underline)
            {
                regular |= FontStyle.Underline | FontStyle.Bold;
            }
            else if (this.Font.Bold)
            {
                regular |= FontStyle.Bold;
            }
            else if (this.Font.Underline)
            {
                regular |= FontStyle.Underline;
            }
            this.Font = new System.Drawing.Font(this.Font.FontFamily, this.Font.Size, regular, GraphicsUnit.Pixel);
        }

        private void Label4_Click(object sender, EventArgs e)
        {
            FontStyle regular;
            if (this.Font.Underline)
            {
                regular = FontStyle.Regular;
            }
            else
            {
                regular = FontStyle.Underline;
            }
            if (this.Font.Italic && this.Font.Bold)
            {
                regular |= FontStyle.Italic | FontStyle.Bold;
            }
            else if (this.Font.Italic)
            {
                regular |= FontStyle.Italic;
            }
            else if (this.Font.Bold)
            {
                regular |= FontStyle.Bold;
            }
            this.Font = new System.Drawing.Font(this.Font.FontFamily, this.Font.Size, regular, GraphicsUnit.Pixel);
        }

        private void Label5_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "图片文件(*.jpg,*.gif,*.bmp,*.png)|*.jpg;*.gif;*.bmp;*.png";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image image = Image.FromFile(this.openFileDialog1.FileName);
                    this.InsertImage(this, new ImageEventArgs(image));
                }
                catch
                {
                }
            }
        }

        private void Label6_Click(object sender, EventArgs e)
        {
            FormInsertTable table = new FormInsertTable();
            if (table.ShowDialog() == DialogResult.OK)
            {
                this.InsertTable(this, new TableEventArgs(table.TableSize));
            }
        }

        [Category("外观"), DisplayName("Color"), Description("字体颜色"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), Localizable(true)]
        public System.Drawing.Color Color
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
                this.Label1.ForeColor = this._color;
                this.Label1.BackColor = System.Drawing.Color.FromArgb(0x7f, 0xff - this._color.R, 0xff - this._color.G, 0xff - this._color.B);
            }
        }
		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font.</value>
        public System.Drawing.Font Font
        {
            get
            {
                return this._font;
            }
            set
            {
                this._font = value;
                if (this.FontChanged != null)
                {
                    this.FontChanged(this, new FontEventArgs(this._font));
                }
                if (this.Font.Bold)
                {
                    this.Label2.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    this.Label2.BackColor = System.Drawing.Color.Transparent;
                }
                if (this.Font.Italic)
                {
                    this.Label3.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    this.Label3.BackColor = System.Drawing.Color.Transparent;
                }
                if (this.Font.Underline)
                {
                    this.Label4.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    this.Label4.BackColor = System.Drawing.Color.Transparent;
                }
            }
        }

        public delegate void ColorChangedEventHandler(object sender, ColorEventArgs e);

        public delegate void FontChangedEventHandler(object sender, FontEventArgs e);

        public delegate void InsertImageEventHandler(object sender, ImageEventArgs e);

        public delegate void InsertTableEventHandler(object sender, TableEventArgs e);
    }
}

