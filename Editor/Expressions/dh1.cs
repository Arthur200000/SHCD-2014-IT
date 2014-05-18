using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;

    internal abstract class dh1 : structexpression
    {
        public dh1(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            this.Fenzi.ParentExpression = this;
            this.Fenmu.ParentExpression = this;
            this.Fenzi.DownLineExpression = this.Fenmu;
            this.Fenmu.UpLineExpression = this.Fenzi;
        }

        public override void RefreshInputLocation()
        {
            this.Fenzi.InputLocation = new PointF(base.InputLocation.X + ((base.Region.Width - this.Fenzi.Region.Width) / 2f), base.InputLocation.Y);
            this.Fenzi.RefreshInputLocation();
            this.Fenmu.InputLocation = new PointF(base.InputLocation.X + ((base.Region.Width - this.Fenmu.Region.Width) / 2f), (base.InputLocation.Y + this.Fenzi.Region.Height) + 10f);
            this.Fenmu.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            foreach (lineexpression lineexpression in base.Child)
            {
                lineexpression.ChangeFontSize(this.Font.Size / 2f);
                lineexpression.RefreshRegion(g);
            }
            float width = Math.Max(Math.Max(this.Fenzi.Region.Width, this.Fenmu.Region.Width), this.Font.Size) + 12f;
            float height = (this.Fenzi.Region.Height + this.Fenmu.Region.Height) + 10f;
            base.Region = new SizeF(width, height);
            float num3 = CommonMethods.CalcAscentPixel(this.Font);
            base.BaseLine = ((this.Fenzi.Region.Height + 5f) - (this.Font.Size / 2f)) + num3;
        }

        protected lineexpression Fenmu
        {
            get
            {
                return base.Child[1];
            }
        }

        protected lineexpression Fenzi
        {
            get
            {
                return base.Child[0];
            }
        }
    }
}

