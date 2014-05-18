using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal abstract class type3 : imageexpression
    {
        private bool offset;

        public type3(Image f, lineexpression parent, Color color, bool off = false) : base(f, parent, color)
        {
            this.offset = off;
            base.Child.Insert(0, new lineexpression(this.Font));
            base.Child.Insert(0, new lineexpression(this.Font));
            this.Shangbiao.ParentExpression = this;
            this.Xiabiao.ParentExpression = this;
            this.Shangbiao.DownLineExpression = this.Xiabiao;
            this.Xiabiao.UpLineExpression = this.Shangbiao;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            ImageAttributes imageAttr = new ImageAttributes();
            ColorMap map = new ColorMap {
                OldColor = Color.Black,
                NewColor = this.Color
            };
            ColorMap[] mapArray = new ColorMap[] { map };
            imageAttr.SetRemapTable(mapArray, ColorAdjustType.Bitmap);
            g.DrawImage(base.fuhao, new Rectangle((int) base.FuhaoLoc.X, (int) base.FuhaoLoc.Y, (int) base.GSize.Width, (int) base.GSize.Height), 0, 0, base.fuhao.Width, base.fuhao.Height, GraphicsUnit.Pixel, imageAttr);
        }

        public override void RefreshInputLocation()
        {
            PointF tf = new PointF();
            PointF tf2 = new PointF();
            PointF tf3 = new PointF();
            tf.X = (-this.Shangbiao.Region.Width / 2f) + this.offsetX;
            tf.Y = tf.X + this.Shangbiao.Region.Width;
            tf2.X = (-this.Xiabiao.Region.Width / 2f) - this.offsetX;
            tf2.Y = tf2.X + this.Xiabiao.Region.Width;
            tf3.X = -base.GSize.Width / 2f;
            tf3.Y = tf3.X + base.GSize.Width;
            float introduced6 = Math.Max(tf3.Y, Math.Max(tf.Y, tf2.Y));
            float num = introduced6 - Math.Min(Math.Min(tf3.X, tf.X), tf2.X);
            base.FuhaoLoc = new PointF(((base.InputLocation.X + 2f) + (num / 2f)) - (base.GSize.Width / 2f), ((base.InputLocation.Y + base.BaseLine) - base.FuhaoBaseLine) + this.Shangbiao.Region.Height);
            base.Benti.InputLocation = new PointF((base.InputLocation.X + 7f) + num, (base.InputLocation.Y + base.BaseLine) - base.Benti.BaseLine);
            this.Shangbiao.InputLocation = new PointF((((base.InputLocation.X + 2f) + (num / 2f)) - (this.Shangbiao.Region.Width / 2f)) + this.offsetX, base.FuhaoLoc.Y - this.Shangbiao.Region.Height);
            this.Xiabiao.InputLocation = new PointF((((base.InputLocation.X + 2f) + (num / 2f)) - (this.Xiabiao.Region.Width / 2f)) - this.offsetX, base.FuhaoLoc.Y + base.GSize.Height);
            this.Shangbiao.RefreshInputLocation();
            this.Xiabiao.RefreshInputLocation();
            base.Benti.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            base.Benti.ChangeFontSize(this.Font.Size);
            base.Benti.RefreshRegion(g);
            float size = this.Font.Size;
            if (this.Shangbiao != null)
            {
                this.Shangbiao.ChangeFontSize(size);
            }
            if (this.Xiabiao != null)
            {
                this.Xiabiao.ChangeFontSize(size);
            }
            this.Shangbiao.RefreshRegion(g);
            this.Xiabiao.RefreshRegion(g);
            while ((this.Shangbiao.Region.Height > (base.GSize.Height / 3f)) && (size > expression.minfontsize))
            {
                size -= 0.5f;
                if (this.Shangbiao != null)
                {
                    this.Shangbiao.ChangeFontSize(size);
                }
                this.Shangbiao.RefreshRegion(g);
            }
            size = base.Benti.Font.Size;
            while ((this.Xiabiao.Region.Height > (base.GSize.Height / 3f)) && (size > expression.minfontsize))
            {
                size -= 0.5f;
                if (this.Xiabiao != null)
                {
                    this.Xiabiao.ChangeFontSize(size);
                }
                this.Xiabiao.RefreshRegion(g);
            }
            if (this.Xiabiao.Font.Size < this.Shangbiao.Font.Size)
            {
                this.Shangbiao.ChangeFontSize(this.Xiabiao.Font.Size);
                this.Shangbiao.RefreshRegion(g);
            }
            else
            {
                this.Xiabiao.ChangeFontSize(this.Shangbiao.Font.Size);
                this.Xiabiao.RefreshRegion(g);
            }
            if (this.offset)
            {
                this.offsetX = (16f * this.Font.Size) / 50f;
            }
            else
            {
                this.offsetX = 0f;
            }
            PointF tf = new PointF();
            PointF tf2 = new PointF();
            PointF tf3 = new PointF();
            tf.X = (-this.Shangbiao.Region.Width / 2f) + this.offsetX;
            tf.Y = tf.X + this.Shangbiao.Region.Width;
            tf2.X = (-this.Xiabiao.Region.Width / 2f) - this.offsetX;
            tf2.Y = tf2.X + this.Xiabiao.Region.Width;
            tf3.X = -base.GSize.Width / 2f;
            tf3.Y = tf3.X + base.GSize.Width;
            float introduced9 = Math.Max(tf3.Y, Math.Max(tf.Y, tf2.Y));
            float width = introduced9 - Math.Min(Math.Min(tf3.X, tf.X), tf2.X);
            float height = (base.GSize.Height + this.Shangbiao.Region.Height) + this.Xiabiao.Region.Height;
            float num4 = CommonMethods.CalcAscentPixel(this.Font);
            base.FuhaoBaseLine = (((base.GSize.Height / 2f) + this.Shangbiao.Region.Height) - (this.Font.Size / 2f)) + num4;
            width += (base.Benti.Region.Width + 5f) + 4f;
            height = Math.Max((float) (height - base.FuhaoBaseLine), (float) (base.Benti.Region.Height - base.Benti.BaseLine)) + Math.Max(base.FuhaoBaseLine, base.Benti.BaseLine);
            base.Region = new SizeF(width, height);
            base.BaseLine = Math.Max(base.FuhaoBaseLine, base.Benti.BaseLine);
        }

        public float offsetX { get; set; }

        public lineexpression Shangbiao
        {
            get
            {
                return base.Child[0];
            }
        }

        public lineexpression Xiabiao
        {
            get
            {
                return base.Child[1];
            }
        }
    }
}

