using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class type2 : imageexpression
    {
        private bool offset;

        public type2(Image f, lineexpression parent, Color color, bool off = false) : base(f, parent, color)
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
            base.FuhaoLoc = new PointF(base.InputLocation.X + 2f, ((base.InputLocation.Y + base.BaseLine) - base.FuhaoBaseLine) + Math.Max((float) (this.Shangbiao.Region.Height - (base.GSize.Height / 2f)), (float) (this.Shangbiao.Region.Height / 2f)));
            base.Benti.InputLocation = new PointF((((base.InputLocation.X + base.GSize.Width) + Math.Max(this.Shangbiao.Region.Width, this.Xiabiao.Region.Width - this.offsetX)) + 2f) + 5f, (base.InputLocation.Y + base.BaseLine) - base.Benti.BaseLine);
            this.Shangbiao.InputLocation = new PointF((base.InputLocation.X + base.GSize.Width) + 2f, (base.FuhaoLoc.Y + (Math.Min(base.GSize.Height, this.Shangbiao.Region.Height) / 2f)) - this.Shangbiao.Region.Height);
            this.Xiabiao.InputLocation = new PointF(((base.InputLocation.X + base.GSize.Width) + 2f) - this.offsetX, (base.FuhaoLoc.Y + base.GSize.Height) - (Math.Min(base.GSize.Height, this.Xiabiao.Region.Height) / 2f));
            this.Shangbiao.RefreshInputLocation();
            this.Xiabiao.RefreshInputLocation();
            base.Benti.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            float num3;
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
                this.offsetX = (20f * this.Font.Size) / 50f;
            }
            else
            {
                this.offsetX = 0f;
            }
            float width = (((base.GSize.Width + Math.Max(this.Shangbiao.Region.Width, this.Xiabiao.Region.Width - this.offsetX)) + base.Benti.Region.Width) + 5f) + 4f;
            if (this.Shangbiao.Region.Height < base.GSize.Height)
            {
                num3 = this.Shangbiao.Region.Height / 2f;
            }
            else
            {
                num3 = this.Shangbiao.Region.Height - (base.GSize.Height / 2f);
            }
            float num4 = CommonMethods.CalcAscentPixel(this.Font);
            base.FuhaoBaseLine = ((num3 + (base.GSize.Height / 2f)) - (this.Font.Size / 2f)) + num4;
            if (this.Xiabiao.Region.Height < base.GSize.Height)
            {
                num3 += this.Xiabiao.Region.Height / 2f;
            }
            else
            {
                num3 += this.Xiabiao.Region.Height - (base.GSize.Height / 2f);
            }
            num3 += base.GSize.Height;
            num3 = Math.Max(base.Benti.BaseLine, base.FuhaoBaseLine) + Math.Max((float) (base.Benti.Region.Height - base.Benti.BaseLine), (float) (num3 - base.FuhaoBaseLine));
            base.Region = new SizeF(width, num3);
            base.BaseLine = Math.Max(base.Benti.BaseLine, base.FuhaoBaseLine);
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

