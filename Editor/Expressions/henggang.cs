using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class henggang : dexpression
    {
        public henggang(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.横杠;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, base.InputLocation.X, base.InputLocation.Y + (base.DotWidth / 2f), base.InputLocation.X + base.Region.Width, base.InputLocation.Y + (base.DotWidth / 2f));
            pen.Dispose();
        }
    }
}

