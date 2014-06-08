using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class yiyinfuhao : dexpression
    {
        public yiyinfuhao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.抑音符号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            float width = g.MeasureString("`", this.Font, 0, genericTypographic).Width;
            SolidBrush brush = new SolidBrush(this.Color);
            g.DrawString("`", this.Font, brush, (base.InputLocation.X + (base.Benti.Region.Width / 2f)) - (width / 2f), base.InputLocation.Y - (width / 2f), genericTypographic);
            g.ResetTransform();
            brush.Dispose();
        }
    }
}

