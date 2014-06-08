using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class shuangxiangjiantou1 : dh1
    {
        public shuangxiangjiantou1(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FunctionType.双向箭头1;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 5f), (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 5f));
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 5f), (float) ((base.InputLocation.X + base.Region.Width) - 5f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 2f));
            g.DrawLine(pen, (float) ((base.InputLocation.X + base.Region.Width) - 2f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 5f), (float) ((base.InputLocation.X + base.Region.Width) - 5f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 8f));
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 5f), (float) (base.InputLocation.X + 4f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 2f));
            g.DrawLine(pen, (float) (base.InputLocation.X + 1f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 5f), (float) (base.InputLocation.X + 4f), (float) ((base.InputLocation.Y + base.Fenzi.Region.Height) + 8f));
            pen.Dispose();
        }
    }
}

