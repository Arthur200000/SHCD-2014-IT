using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    internal class ehuafu : dexpression
    {
        public ehuafu(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.颚化符;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            float width = g.MeasureString("~", this.Font, 0, genericTypographic).Width;
            SolidBrush brush = new SolidBrush(this.Color);
            float sx = Math.Min((float) (base.Benti.Region.Width / width), (float) 4f);
            g.ScaleTransform(sx, 1f, MatrixOrder.Prepend);
            g.DrawString("~", this.Font, brush, ((base.InputLocation.X + (base.Benti.Region.Width / 2f)) - ((width * sx) / 2f)) / sx, base.InputLocation.Y - ((width / 3f) * 2f), genericTypographic);
            g.ResetTransform();
            brush.Dispose();
        }
    }
}

