using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shuangxiangjiantouzaishang : dexpression
    {
        public shuangxiangjiantouzaishang(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.双向箭头在上;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, base.InputLocation.X, base.InputLocation.Y + (base.DotWidth / 2f), base.InputLocation.X + base.Region.Width, base.InputLocation.Y + (base.DotWidth / 2f));
            g.DrawLine(pen, (base.InputLocation.X + base.Region.Width) - (base.DotWidth / 2f), base.InputLocation.Y, base.InputLocation.X + base.Region.Width, base.InputLocation.Y + (base.DotWidth / 2f));
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Region.Width) - (base.DotWidth / 2f)), (float) (base.InputLocation.Y + base.DotWidth), (float) (base.InputLocation.X + base.Region.Width), (float) (base.InputLocation.Y + (base.DotWidth / 2f)));
            g.DrawLine(pen, base.InputLocation.X, base.InputLocation.Y + (base.DotWidth / 2f), base.InputLocation.X + (base.DotWidth / 2f), base.InputLocation.Y);
            g.DrawLine(pen, base.InputLocation.X, base.InputLocation.Y + (base.DotWidth / 2f), base.InputLocation.X + (base.DotWidth / 2f), base.InputLocation.Y + base.DotWidth);
            pen.Dispose();
        }
    }
}

