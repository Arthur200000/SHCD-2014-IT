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

    public class TextBar : UserControl
    {
        private System.Drawing.Color _color;
        private System.Drawing.Font _font;
        private ColorDialog colorDialog1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private IContainer components;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private OpenFileDialog openFileDialog1;

        public event ColorChangedEventHandler ColorChanged;

        public event FontChangedEventHandler FontChanged;

        public event InsertImageEventHandler InsertImage;

        public event InsertTableEventHandler InsertTable;

        public TextBar() : this(new System.Drawing.Font("宋体", 20f, FontStyle.Regular, GraphicsUnit.Pixel), System.Drawing.Color.Black)
        {
        }

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
            this.label1.ForeColor = this._color;
            this.label1.BackColor = System.Drawing.Color.FromArgb(0x7f, 0xff - this._color.R, 0xff - this._color.G, 0xff - this._color.B);
            if (this.Font.Bold)
            {
                this.label2.BackColor = System.Drawing.Color.Yellow;
            }
            if (this.Font.Italic)
            {
                this.label3.BackColor = System.Drawing.Color.Yellow;
            }
            if (this.Font.Underline)
            {
                this.label4.BackColor = System.Drawing.Color.Yellow;
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
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();
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
            this.label1.Cursor = Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label1.ForeColor = SystemColors.ControlText;
            this.label1.Location = new Point(0xb2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x16, 0x16);
            this.label1.TabIndex = 2;
            this.label1.Text = "A";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label1.ForeColorChanged += new EventHandler(this.label1_ForeColorChanged);
            this.label1.Click += new EventHandler(this.label1_Click);
            this.label2.Cursor = Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label2.Location = new Point(0xce, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x16, 0x16);
            this.label2.TabIndex = 3;
            this.label2.Text = "B";
            this.label2.TextAlign = ContentAlignment.MiddleCenter;
            this.label2.Click += new EventHandler(this.label2_Click);
            this.label3.Cursor = Cursors.Hand;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Italic | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label3.Location = new Point(0xea, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x16, 0x16);
            this.label3.TabIndex = 4;
            this.label3.Text = "I";
            this.label3.TextAlign = ContentAlignment.MiddleCenter;
            this.label3.Click += new EventHandler(this.label3_Click);
            this.label4.Cursor = Cursors.Hand;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label4.Location = new Point(0x106, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x16, 0x16);
            this.label4.TabIndex = 5;
            this.label4.Text = "U";
            this.label4.TextAlign = ContentAlignment.MiddleCenter;
            this.label4.Click += new EventHandler(this.label4_Click);
            this.label5.Cursor = Cursors.Hand;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label5.Image = Resources.picture;
            this.label5.Location = new Point(0x13e, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x16, 0x16);
            this.label5.TabIndex = 8;
            this.label5.Click += new EventHandler(this.label5_Click);
            this.label6.Cursor = Cursors.Hand;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 12f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label6.Image = Resources.table;
            this.label6.Location = new Point(290, 0);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x16, 0x16);
            this.label6.TabIndex = 7;
            this.label6.Click += new EventHandler(this.label6_Click);
            this.openFileDialog1.FileName = "openFileDialog1";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.label5);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.comboBox2);
            base.Controls.Add(this.comboBox1);
            base.Name = "TextBar";
            base.Size = new Size(350, 0x16);
            base.ResumeLayout(false);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.Color = this.colorDialog1.Color;
            }
        }

        private void label1_ForeColorChanged(object sender, EventArgs e)
        {
            if (this.ColorChanged != null)
            {
                this.ColorChanged(this, new ColorEventArgs(this.label1.ForeColor));
            }
        }

        private void label2_Click(object sender, EventArgs e)
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

        private void label3_Click(object sender, EventArgs e)
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

        private void label4_Click(object sender, EventArgs e)
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

        private void label5_Click(object sender, EventArgs e)
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

        private void label6_Click(object sender, EventArgs e)
        {
            FrmInsertTable table = new FrmInsertTable();
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
                this.label1.ForeColor = this._color;
                this.label1.BackColor = System.Drawing.Color.FromArgb(0x7f, 0xff - this._color.R, 0xff - this._color.G, 0xff - this._color.B);
            }
        }

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
                    this.label2.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    this.label2.BackColor = System.Drawing.Color.Transparent;
                }
                if (this.Font.Italic)
                {
                    this.label3.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    this.label3.BackColor = System.Drawing.Color.Transparent;
                }
                if (this.Font.Underline)
                {
                    this.label4.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    this.label4.BackColor = System.Drawing.Color.Transparent;
                }
            }
        }

        public delegate void ColorChangedEventHandler(object sender, ColorEventArgs e);

        public delegate void FontChangedEventHandler(object sender, FontEventArgs e);

        public delegate void InsertImageEventHandler(object sender, ImageEventArgs e);

        public delegate void InsertTableEventHandler(object sender, TableEventArgs e);
    }
}

