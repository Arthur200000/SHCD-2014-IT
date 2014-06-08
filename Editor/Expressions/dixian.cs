using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal class dixian : structexpression
    {
        public dixian(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Type = FunctionType.底线;
            this.Benti.ParentExpression = this;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, base.InputLocation.X, (base.InputLocation.Y + base.Region.Height) - (this.DotWidth / 2f), base.InputLocation.X + base.Region.Width, (base.InputLocation.Y + base.Region.Height) - (this.DotWidth / 2f));
            pen.Dispose();
        }

        public override void RefreshInputLocation()
        {
            this.Benti.InputLocation = new PointF(base.InputLocation.X, (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine);
            this.Benti.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            this.Benti.ChangeFontSize(this.Font.Size);
            this.Benti.RefreshRegion(g);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            this.DotWidth = g.MeasureString("A", this.Font, 0, genericTypographic).Height / 3f;
            base.Region = new SizeF(this.Benti.Region.Width, this.Benti.Region.Height + this.DotWidth);
            base.BaseLine = this.Benti.BaseLine;
        }

        protected lineexpression Benti
        {
            get
            {
                return base.Child[0];
            }
        }

        private float DotWidth { get; set; }
    }
}

