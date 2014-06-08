namespace SHCD
{
	using SHCD.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class Key : UserControl
    {
        private IContainer components = null;
        private int keyType = 1;
        private bool OneLine = false;
        private string text = "S";
        private Keys Value;

        public Key()
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
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            base.Margin = new Padding(0);
            base.Name = "Key";
            base.Size = new Size(40, 40);
            base.Paint += new PaintEventHandler(this.Key_Paint);
            base.ResumeLayout(false);
        }

        private void Key_Paint(object sender, PaintEventArgs e)
        {
            List<Point> list = new List<Point>();
            int width = base.Width;
            int height = base.Height;
            list.Add(new Point(0, 5));
            list.Add(new Point(1, 5));
            list.Add(new Point(1, 3));
            list.Add(new Point(2, 3));
            list.Add(new Point(2, 2));
            list.Add(new Point(3, 2));
            list.Add(new Point(3, 1));
            list.Add(new Point(5, 1));
            list.Add(new Point(5, 0));
            list.Add(new Point(width - 6, 0));
            list.Add(new Point(width - 6, 1));
            list.Add(new Point(width - 4, 1));
            list.Add(new Point(width - 4, 2));
            list.Add(new Point(width - 3, 2));
            list.Add(new Point(width - 3, 3));
            list.Add(new Point(width - 2, 3));
            list.Add(new Point(width - 2, 5));
            list.Add(new Point(width - 1, 5));
            list.Add(new Point(width - 1, height - 6));
            list.Add(new Point(width - 2, height - 6));
            list.Add(new Point(width - 2, height - 4));
            list.Add(new Point(width - 3, height - 4));
            list.Add(new Point(width - 3, height - 3));
            list.Add(new Point(width - 4, height - 3));
            list.Add(new Point(width - 4, height - 2));
            list.Add(new Point(width - 6, height - 2));
            list.Add(new Point(width - 6, height - 1));
            list.Add(new Point(5, height - 1));
            list.Add(new Point(5, height - 2));
            list.Add(new Point(3, height - 2));
            list.Add(new Point(3, height - 3));
            list.Add(new Point(2, height - 3));
            list.Add(new Point(2, height - 4));
            list.Add(new Point(1, height - 4));
            list.Add(new Point(1, height - 6));
            list.Add(new Point(0, height - 6));
            Point[] points = list.ToArray();
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(points);
            base.Region = new Region(path);
            e.Graphics.DrawPath(Pens.Gray, path);
            List<Point> list2 = new List<Point>();
            int num3 = base.Width - 3;
            int num4 = base.Height - 4;
            int x = 2;
            int y = 2;
            list2.Add(new Point(x, y + 5));
            list2.Add(new Point(x + 1, y + 5));
            list2.Add(new Point(x + 1, y + 3));
            list2.Add(new Point(x + 2, y + 3));
            list2.Add(new Point(x + 2, y + 2));
            list2.Add(new Point(x + 3, y + 2));
            list2.Add(new Point(x + 3, y + 1));
            list2.Add(new Point(x + 5, y + 1));
            list2.Add(new Point(x + 5, y));
            list2.Add(new Point(num3 - 6, y));
            list2.Add(new Point(num3 - 6, y + 1));
            list2.Add(new Point(num3 - 4, y + 1));
            list2.Add(new Point(num3 - 4, y + 2));
            list2.Add(new Point(num3 - 3, y + 2));
            list2.Add(new Point(num3 - 3, y + 3));
            list2.Add(new Point(num3 - 2, y + 3));
            list2.Add(new Point(num3 - 2, y + 5));
            list2.Add(new Point(num3 - 1, y + 5));
            list2.Add(new Point(num3 - 1, num4 - 6));
            list2.Add(new Point(num3 - 2, num4 - 6));
            list2.Add(new Point(num3 - 2, num4 - 4));
            list2.Add(new Point(num3 - 3, num4 - 4));
            list2.Add(new Point(num3 - 3, num4 - 3));
            list2.Add(new Point(num3 - 4, num4 - 3));
            list2.Add(new Point(num3 - 4, num4 - 2));
            list2.Add(new Point(num3 - 6, num4 - 2));
            list2.Add(new Point(num3 - 6, num4 - 1));
            list2.Add(new Point(x + 5, num4 - 1));
            list2.Add(new Point(x + 5, num4 - 2));
            list2.Add(new Point(x + 3, num4 - 2));
            list2.Add(new Point(x + 3, num4 - 3));
            list2.Add(new Point(x + 2, num4 - 3));
            list2.Add(new Point(x + 2, num4 - 4));
            list2.Add(new Point(x + 1, num4 - 4));
            list2.Add(new Point(x + 1, num4 - 6));
            list2.Add(new Point(x, num4 - 6));
            StringFormat format = new StringFormat {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
            Point[] pointArray2 = list2.ToArray();
            GraphicsPath path2 = new GraphicsPath();
            path2.AddPolygon(pointArray2);
            e.Graphics.DrawPath(Pens.Gray, path2);
            string text = this.text;
            if (!this.OneLine)
            {
                for (int i = this.text.Length - 1; i > 0; i--)
                {
                    text = text.Insert(i, "\r\n");
                }
            }
            if (this.keyType == 1)
            {
                e.Graphics.DrawString(text, this.Font, Brushes.Black, new RectangleF(3f, 3f, (float) (base.ClientSize.Width - 3), (float) (base.ClientSize.Height - 3)), format);
            }
            else
            {
                e.Graphics.DrawImage(Resources.win, new Rectangle(10, 10, 20, 20));
            }
        }

        public string KeyText
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public Keys KeyValue
        {
            get
            {
                return this.Value;
            }
            set
            {
                this.Value = value;
            }
        }

        public int myType
        {
            get
            {
                return this.keyType;
            }
            set
            {
                this.keyType = value;
            }
        }

        public bool one
        {
            get
            {
                return this.OneLine;
            }
            set
            {
                this.OneLine = value;
            }
        }
    }
}

