using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class denghao1 : dh1
    {
        public denghao1(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.等号1;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 3f), (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 3f));
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 7f), (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 7f));
            pen.Dispose();
        }
    }
}

