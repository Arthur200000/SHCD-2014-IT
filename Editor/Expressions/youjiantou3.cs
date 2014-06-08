using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class youjiantou3 : dh3
    {
        public youjiantou3(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.右箭头3;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) (base.InputLocation.Y + 5f), (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) (base.InputLocation.Y + 5f));
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) 5f, (float) ((base.InputLocation.X + base.Region.Width) - 5f), (float) (base.InputLocation.Y + 2f));
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) (base.InputLocation.Y + 5f), (float) ((base.InputLocation.X + base.Region.Width) - 5f), (float) (base.InputLocation.Y + 8f));
            pen.Dispose();
        }
    }
}

