using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class fangkuohao : kuohaoexpression
    {
        public fangkuohao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.方括号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, base.InputLocation.X + 2f, base.InputLocation.Y, ((base.InputLocation.X + 2f) + base.GSize) - 1f, base.InputLocation.Y);
            g.DrawLine(pen, base.InputLocation.X + 2f, base.InputLocation.Y, base.InputLocation.X + 2f, (base.InputLocation.Y + base.Region.Height) - 1f);
            g.DrawLine(pen, (float) (base.InputLocation.X + 2f), (float) ((base.InputLocation.Y + base.Region.Height) - 1f), (float) (((base.InputLocation.X + 2f) + base.GSize) - 1f), (float) ((base.InputLocation.Y + base.Region.Height) - 1f));
            g.DrawLine(pen, (base.InputLocation.X + base.Region.Width) - 2f, base.InputLocation.Y, (((base.InputLocation.X + base.Region.Width) - 2f) - base.GSize) + 1f, base.InputLocation.Y);
            g.DrawLine(pen, (base.InputLocation.X + base.Region.Width) - 2f, base.InputLocation.Y, (base.InputLocation.X + base.Region.Width) - 2f, (base.InputLocation.Y + base.Region.Height) - 1f);
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) ((base.InputLocation.Y + base.Region.Height) - 1f), (float) ((((base.InputLocation.X + base.Region.Width) - 2f) - base.GSize) + 1f), (float) ((base.InputLocation.Y + base.Region.Height) - 1f));
            pen.Dispose();
        }
    }
}

