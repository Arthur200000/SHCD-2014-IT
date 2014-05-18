using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shuangdian : dexpression
    {
        public shuangdian(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.双点;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            SolidBrush brush = new SolidBrush(this.Color);
            g.FillEllipse(brush, (base.Benti.InputLocation.X + (base.Benti.Region.Width / 2f)) - ((base.DotWidth / 2f) * 3f), ((base.InputLocation.Y + (base.Region.Height / 4f)) - (base.Benti.Region.Height / 4f)) - (base.DotWidth / 2f), base.DotWidth, base.DotWidth);
            g.FillEllipse(brush, (base.Benti.InputLocation.X + (base.Benti.Region.Width / 2f)) + (base.DotWidth / 2f), ((base.InputLocation.Y + (base.Region.Height / 4f)) - (base.Benti.Region.Height / 4f)) - (base.DotWidth / 2f), base.DotWidth, base.DotWidth);
            brush.Dispose();
        }
    }
}

