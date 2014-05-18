using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class denghao3 : dh3
    {
        public denghao3(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.等号3;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) (base.InputLocation.Y + 3f), (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) (base.InputLocation.Y + 3f));
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) (base.InputLocation.Y + 7f), (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) (base.InputLocation.Y + 7f));
            pen.Dispose();
        }
    }
}

