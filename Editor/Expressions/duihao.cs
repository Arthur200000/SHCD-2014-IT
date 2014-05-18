using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    internal class duihao : dexpression
    {
        public duihao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.对号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            float width = g.MeasureString("^", this.Font, 0, genericTypographic).Width;
            SolidBrush brush = new SolidBrush(this.Color);
            float sx = Math.Min((float) (base.Benti.Region.Width / width), (float) 4f);
            g.ScaleTransform(sx, -1f, MatrixOrder.Prepend);
            g.DrawString("^", this.Font, brush, ((base.InputLocation.X + (base.Benti.Region.Width / 2f)) / sx) - (width / 2f), (-base.InputLocation.Y - base.DotWidth) - (width / 2f), genericTypographic);
            g.ResetTransform();
            brush.Dispose();
        }
    }
}

