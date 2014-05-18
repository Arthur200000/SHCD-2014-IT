using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;

    internal class fenshi : structexpression
    {
        public fenshi(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Type = FType.分式;
            this.Fenzi.ParentExpression = this;
            this.Fenmu.ParentExpression = this;
            this.Fenzi.DownLineExpression = this.Fenmu;
            this.Fenmu.UpLineExpression = this.Fenzi;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, (float) (base.InputLocation.X + 2f), (float) ((this.Fenzi.InputLocation.Y + this.Fenzi.Region.Height) + 2f), (float) ((base.InputLocation.X + base.Region.Width) - 5f), (float) ((this.Fenzi.InputLocation.Y + this.Fenzi.Region.Height) + 2f));
            pen.Dispose();
        }

        public override void RefreshInputLocation()
        {
            this.Fenzi.InputLocation = new PointF(base.InputLocation.X + ((base.Region.Width - this.Fenzi.Region.Width) / 2f), base.InputLocation.Y + 3f);
            this.Fenzi.RefreshInputLocation();
            this.Fenmu.InputLocation = new PointF(base.InputLocation.X + ((base.Region.Width - this.Fenmu.Region.Width) / 2f), ((base.InputLocation.Y + base.Region.Height) - 3f) - base.Child[1].Region.Height);
            this.Fenmu.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            foreach (expression expression in base.Child)
            {
                expression.RefreshRegion(g);
            }
            float width = Math.Max(base.Child[0].Region.Width, base.Child[1].Region.Width) + Math.Max((float) 12f, (float) ((12f * this.Font.Size) / 12f));
            float height = (base.Child[0].Region.Height + base.Child[1].Region.Height) + 10f;
            base.Region = new SizeF(width, height);
            base.BaseLine = ((this.Fenzi.Region.Height + 5f) - (this.Font.Size / 2f)) + CommonMethods.CalcAscentPixel(this.Font);
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

