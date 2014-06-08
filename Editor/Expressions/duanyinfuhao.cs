using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    internal class duanyinfuhao : dexpression
    {
        public duanyinfuhao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.短音符号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            SizeF ef = g.MeasureString("(", this.Font, 0, genericTypographic);
            SolidBrush brush = new SolidBrush(this.Color);
            g.RotateTransform(-90f);
            float sy = Math.Min((float) (base.Benti.Region.Width / ef.Height), (float) 4f);
            g.ScaleTransform(1f, sy, MatrixOrder.Prepend);
            g.DrawString("(", this.Font, brush, -base.InputLocation.Y - ef.Width, (((base.InputLocation.X + (base.Benti.Region.Width / 2f)) / sy) - (ef.Height / 2f)) - (ef.Width / 3f), genericTypographic);
            g.ResetTransform();
            brush.Dispose();
        }
    }
}

