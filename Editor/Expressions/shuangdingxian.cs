using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shuangdingxian : dexpression
    {
        public shuangdingxian(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.双顶线;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, base.InputLocation.X, base.InputLocation.Y + (base.DotWidth / 3f), base.InputLocation.X + base.Region.Width, base.InputLocation.Y + (base.DotWidth / 3f));
            g.DrawLine(pen, base.InputLocation.X, base.InputLocation.Y + ((base.DotWidth / 3f) * 2f), base.InputLocation.X + base.Region.Width, base.InputLocation.Y + ((base.DotWidth / 3f) * 2f));
            pen.Dispose();
        }
    }
}

