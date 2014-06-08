namespace Qisi.Editor.Controls
{
    using Qisi.Editor;
    using Qisi.Editor.Expression;
    using Qisi.Editor.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
	/// <summary>
	/// Function label.
	/// </summary>
	internal class FunctionLabel : Control
    {
        private FunctionType _ftype;
        private Image image;
        private Rectangle recttodraw;
        private bool withhotkey;

        internal event AppendExpressionHandler AppendExpression;

        public FunctionLabel() : this(FunctionType.包含, false)
        {
        }

        internal FunctionLabel(FunctionType f, bool hotkey = false)
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.recttodraw = new Rectangle(0, 0, CommonMethods.height, CommonMethods.height);
            base.Margin = new Padding(0, 0, 0, 0);
            base.Padding = new Padding(0, 0, 0, 0);
            this.Font = new Font("微软雅黑", 10f, FontStyle.Regular, GraphicsUnit.Pixel, 0x86);
            this.DoubleBuffered = true;
            this.AutoSize = false;
            this.Text = "";
            this.Cursor = Cursors.Hand;
            base.Click += new EventHandler(this.FLabel_Click);
            base.MouseEnter += new EventHandler(this.FLabel_MouseEnter);
            base.MouseLeave += new EventHandler(this.FLabel_MouseLeave);
            this.withhotkey = hotkey;
            base.Width = CommonMethods.height;
            if (this.withhotkey)
            {
                base.Height = CommonMethods.height + (CommonMethods.height / 2);
            }
            else
            {
                base.Height = CommonMethods.height;
            }
            this.Ftype = f;
        }

        private void FLabel_Click(object sender, EventArgs e)
        {
            try
            {
                this.AppendExpression(this, new ExpressionEventArgs(this.Ftype));
            }
            catch
            {
            }
        }

        private void FLabel_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.PaleTurquoise;
        }

        private void FLabel_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.Transparent;
        }
		/// <summary>
		/// Fresh Image.
		/// </summary>
        public void freshimage()
        {
            if (this.image.Width > this.image.Height)
            {
                this.recttodraw = new Rectangle(0, (CommonMethods.height - ((this.image.Height * CommonMethods.height) / this.image.Width)) / 2, CommonMethods.height, (this.image.Height * CommonMethods.height) / this.image.Width);
            }
            else
            {
                this.recttodraw = new Rectangle((CommonMethods.height - ((this.image.Width * CommonMethods.height) / this.image.Height)) / 2, 0, (this.image.Width * CommonMethods.height) / this.image.Height, CommonMethods.height);
            }
        }

        private void generateimage(string str)
        {
            this.image = new Bitmap(CommonMethods.height, CommonMethods.height);
            Graphics graphics = Graphics.FromImage(this.image);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            float height = CommonMethods.height;
            Font cambriaFont = CommonMethods.GetCambriaFont(height, FontStyle.Regular);
            SizeF ef = graphics.MeasureString(str, cambriaFont, 0, genericTypographic);
            while ((ef.Width >= (CommonMethods.height * 0.8)) || (ef.Height >= (CommonMethods.height * 0.8)))
            {
                height -= 10f;
                cambriaFont = CommonMethods.GetCambriaFont(height, cambriaFont.Style);
                ef = graphics.MeasureString(str, cambriaFont, 0, genericTypographic);
            }
            graphics.DrawString(str, cambriaFont, Brushes.Black, (CommonMethods.height / 2) - (ef.Width / 2f), (CommonMethods.height / 2) - (ef.Height / 2f), genericTypographic);
            graphics.Dispose();
            cambriaFont.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.image != null)
            {
                Rectangle recttodraw = this.recttodraw;
                if (this.withhotkey)
                {
                    string str;
                    StringFormat genericTypographic = StringFormat.GenericTypographic;
                    genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    if (this.HotKey != Keys.None)
                    {
                        str = "Alt+" + this.HotKey.ToString();
                    }
                    else
                    {
                        str = "无热键";
                    }
                    e.Graphics.DrawString(str, this.Font, Brushes.Black, new PointF(0f, 0f));
                    recttodraw = new Rectangle(this.recttodraw.Left, this.recttodraw.Top + (CommonMethods.height / 2), this.recttodraw.Width, this.recttodraw.Height);
                }
                e.Graphics.DrawImage(this.image, recttodraw, new Rectangle(0, 0, this.image.Width, this.image.Height), GraphicsUnit.Pixel);
            }
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
        }

        [DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), Localizable(true), Description("公式的类别"), DisplayName("FType"), Category("Text")]
		/// <summary>
		/// Gets or sets the function type.
		/// </summary>
		/// <value>The function type.</value>
        public FunctionType Ftype
        {
            get
            {
                return this._ftype;
            }
            set
            {
                if (this._ftype != value)
                {
                    this._ftype = value;
					string specialChar = CommonMethods.GetSpecialChar(this._ftype.ToString());
                    if (string.IsNullOrEmpty(specialChar))
                    {
                        ResourceManager resourceManager = Resources.ResourceManager;
                        this.image = (Image) resourceManager.GetObject(this._ftype.ToString());
                        if (this.image == null)
                        {
                            this.image = new Bitmap(CommonMethods.height, CommonMethods.height);
                        }
                        this.freshimage();
                    }
                    else
                    {
                        this.generateimage(specialChar);
                    }
                }
            }
        }
		/// <summary>
		/// Gets or sets the hot key.
		/// </summary>
		/// <value>The hot key.</value>
        public Keys HotKey { get; set; }
		/// <summary>
		/// Gets or sets the hot key identifier.
		/// </summary>
		/// <value>The hot key identifier.</value>
        public int HotKeyId { get; set; }

        internal delegate void AppendExpressionHandler(object sender, ExpressionEventArgs e);
    }
}

