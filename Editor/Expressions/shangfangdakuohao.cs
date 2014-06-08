using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shangfangdakuohao : dexpression
    {
        public shangfangdakuohao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.上方大括号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawArc(pen, base.InputLocation.X, base.InputLocation.Y + (base.DotWidth / 3f), (base.DotWidth / 3f) * 4f, (base.DotWidth / 3f) * 4f, -90f, -90f);
            g.DrawLine(pen, (float) (base.InputLocation.X + ((base.DotWidth * 2f) / 3f)), (float) (base.InputLocation.Y + (base.DotWidth / 3f)), (float) ((base.InputLocation.X + (base.Benti.Region.Width / 2f)) - ((1.732f * base.DotWidth) / 3f)), (float) (base.InputLocation.Y + (base.DotWidth / 3f)));
            g.DrawArc(pen, (float) ((base.InputLocation.X + (base.Benti.Region.Width / 2f)) - ((3.732f * base.DotWidth) / 3f)), (float) (base.InputLocation.Y - base.DotWidth), (float) ((base.DotWidth / 3f) * 4f), (float) ((base.DotWidth / 3f) * 4f), (float) 30f, (float) 60f);
            g.DrawArc(pen, (float) ((base.InputLocation.X + base.Benti.Region.Width) - ((4f * base.DotWidth) / 3f)), (float) (base.InputLocation.Y + (base.DotWidth / 3f)), (float) ((base.DotWidth / 3f) * 4f), (float) ((base.DotWidth / 3f) * 4f), (float) 0f, (float) -90f);
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Benti.Region.Width) - ((base.DotWidth * 2f) / 3f)), (float) (base.InputLocation.Y + (base.DotWidth / 3f)), (float) ((base.InputLocation.X + (base.Benti.Region.Width / 2f)) + ((1.732f * base.DotWidth) / 3f)), (float) (base.InputLocation.Y + (base.DotWidth / 3f)));
            g.DrawArc(pen, (float) ((base.InputLocation.X + (base.Benti.Region.Width / 2f)) - ((0.286f * base.DotWidth) / 3f)), (float) (base.InputLocation.Y - base.DotWidth), (float) ((base.DotWidth / 3f) * 4f), (float) ((base.DotWidth / 3f) * 4f), (float) 90f, (float) 60f);
            pen.Dispose();
        }
    }
}

