using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class kuohao : kuohaoexpression
    {
        public kuohao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.括号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            float num = (base.GSize / 2f) + (((base.Region.Height * base.Region.Height) / base.GSize) / 8f);
            float num2 = (float) ((Math.Atan((double) ((base.Region.Height / 2f) / ((((base.Region.Height * base.Region.Height) / 8f) / base.GSize) - (base.GSize / 2f)))) / 3.1415926535897931) * 180.0);
            g.DrawArc(pen, (float) (base.InputLocation.X + 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - num), (float) (num * 2f), (float) (num * 2f), (float) (-180f + num2), (float) (-2f * num2));
            g.DrawArc(pen, ((base.InputLocation.X + base.Region.Width) - 2f) - (num * 2f), (base.InputLocation.Y + (base.Region.Height / 2f)) - num, num * 2f, num * 2f, -num2, 2f * num2);
            pen.Dispose();
        }
    }
}

