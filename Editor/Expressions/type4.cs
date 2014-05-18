using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    internal abstract class type4 : imageexpression
    {
        public type4(Image f, lineexpression parent, Color color) : base(f, parent, color)
        {
            base.Child.Insert(0, new lineexpression(this.Font));
            this.Xiabiao.ParentExpression = this;
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
            float num = Math.Max(base.GSize.Width, this.Xiabiao.Region.Width);
            base.Benti.InputLocation = new PointF(((base.InputLocation.X + num) + 2f) + 5f, (base.InputLocation.Y + base.BaseLine) - base.Benti.BaseLine);
            base.FuhaoLoc = new PointF(((base.InputLocation.X + (num / 2f)) + 2f) - (base.GSize.Width / 2f), (base.InputLocation.Y + base.BaseLine) - base.FuhaoBaseLine);
            this.Xiabiao.InputLocation = new PointF(((base.InputLocation.X + (num / 2f)) + 2f) - (this.Xiabiao.Region.Width / 2f), base.FuhaoLoc.Y + base.GSize.Height);
            this.Xiabiao.RefreshInputLocation();
            base.Benti.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            base.Benti.ChangeFontSize(this.Font.Size);
            base.Benti.RefreshRegion(g);
            float size = base.Benti.Font.Size;
            if (this.Xiabiao != null)
            {
                this.Xiabiao.ChangeFontSize(size);
            }
            this.Xiabiao.RefreshRegion(g);
            while ((this.Xiabiao.Region.Height > (base.GSize.Height / 3f)) && (size > expression.minfontsize))
            {
                size -= 0.5f;
                if (this.Xiabiao != null)
                {
                    this.Xiabiao.ChangeFontSize(size);
                }
                this.Xiabiao.RefreshRegion(g);
            }
            float width = Math.Max(base.GSize.Width, this.Xiabiao.Region.Width);
            float height = base.GSize.Height + this.Xiabiao.Region.Height;
            float num4 = CommonMethods.CalcAscentPixel(this.Font);
            base.FuhaoBaseLine = ((base.GSize.Height / 2f) - (this.Font.Size / 2f)) + num4;
            width += (base.Benti.Region.Width + 5f) + 4f;
            height = Math.Max(base.FuhaoBaseLine, base.Benti.BaseLine) + Math.Max((float) (height - base.FuhaoBaseLine), (float) (base.Benti.Region.Height - base.Benti.BaseLine));
            base.Region = new SizeF(width, height);
            base.BaseLine = Math.Max(base.FuhaoBaseLine, base.Benti.BaseLine);
        }

        public lineexpression Xiabiao
        {
            get
            {
                return base.Child[0];
            }
        }
    }
}

