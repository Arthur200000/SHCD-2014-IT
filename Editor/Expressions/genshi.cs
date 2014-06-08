using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.CompilerServices;

    internal class genshi : structexpression
    {
        public genshi(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Type = FunctionType.一般根式;
            this.Benti.ParentExpression = this;
            this.Gen.ParentExpression = this;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            PointF tf = new PointF(this.Benti.InputLocation.X - this.GSize.Width, this.Benti.InputLocation.Y - this.Gintv);
            PointF[] points = new PointF[] { new PointF(tf.X, tf.Y + (0.7f * this.GSize.Height)), new PointF(tf.X + (0.2f * this.GSize.Width), tf.Y + (0.6f * this.GSize.Height)), new PointF(tf.X + (0.5f * this.GSize.Width), tf.Y + this.GSize.Height), new PointF(tf.X + this.GSize.Width, tf.Y), new PointF((tf.X + this.GSize.Width) + this.Benti.Region.Width, tf.Y) };
            Pen pen = new Pen(this.Color);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLines(pen, points);
            pen.Dispose();
        }

        public override void RefreshInputLocation()
        {
            PointF tf = new PointF();
            PointF tf2 = new PointF();
            if ((this.Gen.Region.Height + ((this.GSize.Height / 5f) * 4f)) > this.GSize.Height)
            {
                tf.Y = (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine;
                tf2.Y = base.InputLocation.Y;
            }
            else
            {
                tf.Y = (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine;
                tf2.Y = (base.InputLocation.Y + ((this.GSize.Height * 2f) / 5f)) - this.Gen.Region.Height;
            }
            if ((this.Gen.Region.Width + ((this.GSize.Width * 2f) / 3f)) > this.GSize.Width)
            {
                tf.X = (base.InputLocation.X + this.Gen.Region.Width) + ((this.GSize.Width * 2f) / 3f);
                tf2.X = base.InputLocation.X;
            }
            else
            {
                tf.X = base.InputLocation.X + this.GSize.Width;
                tf2.X = (base.InputLocation.X + (this.GSize.Width / 3f)) - this.Gen.Region.Width;
            }
            this.Benti.InputLocation = tf;
            this.Benti.RefreshInputLocation();
            this.Gen.InputLocation = tf2;
            this.Gen.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            this.Benti.ChangeFontSize(this.Font.Size);
            this.Benti.RefreshRegion(g);
            float size = this.Benti.Font.Size;
            if (this.Gen != null)
            {
                this.Gen.ChangeFontSize(size);
            }
            this.Gen.RefreshRegion(g);
			while ((this.Gen.Region.Height > (this.Benti.Region.Height / 3f)) && (size > 8f)) // expression.cs: protected static float minfontsize = 8f;
            {
                size -= 0.5f;
                if (this.Gen != null)
                {
                    this.Gen.ChangeFontSize(size);
                }
                this.Gen.RefreshRegion(g);
            }
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            this.GSize = g.MeasureString("1", this.Font, 0, genericTypographic);
            this.Gintv = 0f;
            if (this.Benti.Child != null)
            {
                foreach (expression expression in this.Benti.Child)
                {
                    if (((expression.Type == FunctionType.一般根式) || (expression.Type == FunctionType.平方根)) || (expression.Type == FunctionType.立方根))
                    {
                        this.Gintv = this.GSize.Height / 2f;
                        break;
                    }
                }
            }
            this.GSize = new SizeF(this.GSize.Width, this.Benti.Region.Height + this.Gintv);
            float width = Math.Max(this.Gen.Region.Width + ((this.GSize.Width * 2f) / 3f), this.GSize.Width) + this.Benti.Region.Width;
            float num3 = Math.Max(this.Gen.Region.Height + ((this.GSize.Height / 5f) * 4f), this.GSize.Height);
            base.Region = new SizeF(width, num3 + this.Gintv);
            base.BaseLine = (base.Region.Height - this.Benti.Region.Height) + this.Benti.BaseLine;
        }

        public lineexpression Benti
        {
            get
            {
                return base.Child[1];
            }
        }

        public lineexpression Gen
        {
            get
            {
                return base.Child[0];
            }
        }

        private float Gintv { get; set; }

        private SizeF GSize { get; set; }
    }
}

