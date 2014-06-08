using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shangxiabiaozuo : structexpression
    {
        public shangxiabiaozuo(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Type = FunctionType.上下标左;
            base.Child[0].ParentExpression = this;
            base.Child[1].ParentExpression = this;
            base.Child[2].ParentExpression = this;
            this.Shangbiao.DownLineExpression = this.Xiabiao;
            this.Xiabiao.UpLineExpression = this.Shangbiao;
        }

        public override void RefreshInputLocation()
        {
            float num = Math.Max(this.Shangbiao.Region.Width, this.Xiabiao.Region.Width);
            this.Benti.InputLocation = new PointF(base.InputLocation.X + num, (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine);
            this.Shangbiao.InputLocation = new PointF((base.InputLocation.X + num) - this.Shangbiao.Region.Width, base.InputLocation.Y);
            this.Xiabiao.InputLocation = new PointF((base.InputLocation.X + num) - this.Xiabiao.Region.Width, (base.InputLocation.Y + base.Region.Height) - this.Xiabiao.Region.Height);
            this.Benti.RefreshInputLocation();
            this.Shangbiao.RefreshInputLocation();
            this.Xiabiao.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            this.Benti.ChangeFontSize(this.Font.Size);
            this.Benti.RefreshRegion(g);
            float size = this.Benti.Font.Size;
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
            while ((this.Shangbiao.Region.Height > (this.Benti.Region.Height / 3f)) && (size > expression.minfontsize))
            {
                size -= 0.5f;
                if (this.Shangbiao != null)
                {
                    this.Shangbiao.ChangeFontSize(size);
                }
                this.Shangbiao.RefreshRegion(g);
            }
            size = this.Benti.Font.Size;
            while ((this.Xiabiao.Region.Height > (this.Benti.Region.Height / 3f)) && (size > expression.minfontsize))
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
            float width = base.Child[2].Region.Width + Math.Max(base.Child[1].Region.Width, base.Child[0].Region.Width);
            float height = 0f;
            if (this.Shangbiao.Region.Height < this.Benti.Region.Height)
            {
                height = this.Shangbiao.Region.Height / 2f;
            }
            else
            {
                height = this.Shangbiao.Region.Height - (this.Benti.Region.Height / 2f);
            }
            base.BaseLine = height + this.Benti.BaseLine;
            if (this.Xiabiao.Region.Height < this.Benti.Region.Height)
            {
                height += this.Xiabiao.Region.Height / 2f;
            }
            else
            {
                height += this.Xiabiao.Region.Height - (this.Benti.Region.Height / 2f);
            }
            height += this.Benti.Region.Height;
            base.Region = new SizeF(width, height);
        }

        public lineexpression Benti
        {
            get
            {
                return base.Child[2];
            }
        }

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

