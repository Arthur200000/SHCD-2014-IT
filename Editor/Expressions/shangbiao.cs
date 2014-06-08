using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shangbiao : structexpression
    {
        public shangbiao(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Type = FunctionType.上标;
            this.Benti.ParentExpression = this;
            this.Shangbiao.ParentExpression = this;
        }

        public override void RefreshInputLocation()
        {
            this.Benti.InputLocation = new PointF(base.InputLocation.X, (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine);
            this.Benti.RefreshInputLocation();
            this.Shangbiao.InputLocation = new PointF(base.InputLocation.X + this.Benti.Region.Width, base.InputLocation.Y);
            this.Shangbiao.RefreshInputLocation();
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
            this.Shangbiao.RefreshRegion(g);
            while ((this.Shangbiao.Region.Height > (this.Benti.Region.Height / 3f)) && (size > expression.minfontsize))
            {
                size -= 0.5f;
                if (this.Shangbiao != null)
                {
                    this.Shangbiao.ChangeFontSize(size);
                }
                this.Shangbiao.RefreshRegion(g);
            }
            float width = base.Child[0].Region.Width + base.Child[1].Region.Width;
            float height = Math.Max((float) (base.Child[1].Region.Height + (base.Child[0].Region.Height / 2f)), (float) ((base.Child[1].Region.Height / 2f) + base.Child[0].Region.Height));
            base.Region = new SizeF(width, height);
            base.BaseLine = (base.Region.Height - this.Benti.Region.Height) + this.Benti.BaseLine;
        }

        public lineexpression Benti
        {
            get
            {
                return base.Child[0];
            }
        }

        public lineexpression Shangbiao
        {
            get
            {
                return base.Child[1];
            }
        }
    }
}

