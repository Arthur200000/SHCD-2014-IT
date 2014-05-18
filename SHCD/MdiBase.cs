namespace SHCD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class MdiBase : UserControl
    {
        private IContainer components = null;

        public MdiBase()
        {
            this.InitializeComponent();
            base.Visible = false;
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
            this.BackColor = SystemColors.GradientActiveCaption;
            this.DoubleBuffered = true;
            base.Name = "MdiBase";
            base.Size = new Size(0x1d3, 0xff);
            base.Paint += new PaintEventHandler(this.Login_Paint);
            base.ResumeLayout(false);
        }

        private void Login_Paint(object sender, PaintEventArgs e)
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
            Graphics graphics = e.Graphics;
            Color color = Color.FromArgb(0x88, 0xc4, 0xff);
            Color white = Color.White;
            Brush brush = new LinearGradientBrush(base.ClientRectangle, white, color, LinearGradientMode.Vertical);
            graphics.FillRectangle(brush, base.ClientRectangle);
            graphics.FillRectangle(Brushes.Blue, new Rectangle(0, 0, base.Width, SystemInformation.MenuHeight));
        }
    }
}

