using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal abstract class kuohaoexpression : structexpression
    {
        public kuohaoexpression(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            this.Benti.ParentExpression = this;
        }

        public override void RefreshInputLocation()
        {
            this.Benti.InputLocation = new PointF((base.InputLocation.X + this.GSize) + 2f, (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine);
            this.Benti.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            this.Benti.ChangeFontSize(this.Font.Size);
            this.Benti.RefreshRegion(g);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            this.GSize = g.MeasureString("{", this.Font, 0, genericTypographic).Width;
            if (((7.464 * this.GSize) / 3.0) > this.Benti.Region.Height)
            {
                this.GSize = ((this.Benti.Region.Height * 3f) / 2f) / 3.732f;
            }
            float width = (this.Benti.Region.Width + (this.GSize * 2f)) + 4f;
            float num2 = CommonMethods.CalcAscentPixel(this.Font);
            float num3 = (this.Benti.BaseLine - num2) + (this.Font.Size / 2f);
            float height = Math.Max((float) (num3 * 2f), (float) ((this.Benti.Region.Height - num3) * 2f));
            base.Region = new SizeF(width, height);
            base.BaseLine = ((base.Region.Height / 2f) - (this.Font.Size / 2f)) + num2;
        }

        protected lineexpression Benti
        {
            get
            {
                return base.Child[0];
            }
        }

        protected float GSize { get; set; }
    }
}

