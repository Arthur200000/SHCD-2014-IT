using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class xiefenshi : structexpression
    {
        public xiefenshi(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Type = FType.斜分式;
            this.Fenzi.ParentExpression = this;
            this.Fenmu.ParentExpression = this;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, (float) ((base.InputLocation.X + this.Fenzi.Region.Width) + (this.Fenzi.Region.Height * 0.6f)), (float) (this.Fenzi.InputLocation.Y + (this.Fenzi.Region.Height / 3f)), (float) (base.InputLocation.X + this.Fenzi.Region.Width), (float) ((this.Fenmu.InputLocation.Y + this.Fenmu.Region.Height) - (this.Fenzi.Region.Height / 3f)));
            pen.Dispose();
        }

        public override void RefreshInputLocation()
        {
            this.Fenzi.InputLocation = new PointF(base.InputLocation.X + 2f, base.InputLocation.Y);
            this.Fenzi.RefreshInputLocation();
            this.Fenmu.InputLocation = new PointF(((base.InputLocation.X + 2f) + this.Fenzi.Region.Width) + (this.Fenzi.Region.Height * 0.6f), base.InputLocation.Y + this.Fenzi.Region.Height);
            this.Fenmu.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            foreach (lineexpression lineexpression in base.Child)
            {
                lineexpression.ChangeFontSize(this.Font.Size);
                lineexpression.RefreshRegion(g);
            }
            float width = ((base.Child[0].Region.Width + base.Child[1].Region.Width) + (this.Fenzi.Region.Height * 0.6f)) + 4f;
            float height = base.Child[0].Region.Height + base.Child[1].Region.Height;
            base.Region = new SizeF(width, height);
            FontFamily fontFamily = this.Font.FontFamily;
            int cellAscent = fontFamily.GetCellAscent(this.Font.Style);
            float num4 = (this.Font.Size * cellAscent) / ((float) fontFamily.GetEmHeight(this.Font.Style));
            base.BaseLine = (base.Child[0].Region.Height + (-this.Font.Size / 2f)) + num4;
        }

        public lineexpression Fenmu
        {
            get
            {
                return base.Child[1];
            }
        }

        public lineexpression Fenzi
        {
            get
            {
                return base.Child[0];
            }
        }
    }
}

