using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class dakuohao : kuohaoexpression
    {
        public dakuohao(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.大括号;
        }

        public override void DrawExpression(Graphics g)
        {
            base.DrawExpression(g);
            Pen pen = new Pen(this.Color);
            g.DrawArc(pen, (base.InputLocation.X + (base.GSize / 3f)) + 2f, base.InputLocation.Y, (base.GSize / 3f) * 4f, (base.GSize / 3f) * 4f, -90f, -90f);
            g.DrawLine(pen, (float) ((base.InputLocation.X + (base.GSize / 3f)) + 2f), (float) (base.InputLocation.Y + ((base.GSize * 2f) / 3f)), (float) ((base.InputLocation.X + (base.GSize / 3f)) + 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - ((1.732f * base.GSize) / 3f)));
            g.DrawArc(pen, (float) ((base.InputLocation.X - base.GSize) + 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - ((3.732f * base.GSize) / 3f)), (float) ((base.GSize / 3f) * 4f), (float) ((base.GSize / 3f) * 4f), (float) 0f, (float) 60f);
            g.DrawArc(pen, (float) ((base.InputLocation.X + (base.GSize / 3f)) + 2f), (float) ((base.InputLocation.Y + base.Region.Height) - ((4f * base.GSize) / 3f)), (float) ((base.GSize / 3f) * 4f), (float) ((base.GSize / 3f) * 4f), (float) 90f, (float) 90f);
            g.DrawLine(pen, (float) ((base.InputLocation.X + (base.GSize / 3f)) + 2f), (float) ((base.InputLocation.Y + base.Region.Height) - ((base.GSize * 2f) / 3f)), (float) ((base.InputLocation.X + (base.GSize / 3f)) + 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) + ((1.732f * base.GSize) / 3f)));
            g.DrawArc(pen, (float) ((base.InputLocation.X - base.GSize) + 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - ((0.286f * base.GSize) / 3f)), (float) ((base.GSize / 3f) * 4f), (float) ((base.GSize / 3f) * 4f), (float) 0f, (float) -60f);
            g.DrawArc(pen, ((base.InputLocation.X + base.Region.Width) - ((base.GSize / 3f) * 5f)) - 2f, base.InputLocation.Y, (base.GSize / 3f) * 4f, (base.GSize / 3f) * 4f, -90f, 90f);
            g.DrawLine(pen, (float) (((base.InputLocation.X + base.Region.Width) - (base.GSize / 3f)) - 2f), (float) (base.InputLocation.Y + ((base.GSize * 2f) / 3f)), (float) (((base.InputLocation.X + base.Region.Width) - (base.GSize / 3f)) - 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - ((1.732f * base.GSize) / 3f)));
            g.DrawArc(pen, (float) (((base.InputLocation.X + base.Region.Width) - (base.GSize / 3f)) - 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - ((3.732f * base.GSize) / 3f)), (float) ((base.GSize / 3f) * 4f), (float) ((base.GSize / 3f) * 4f), (float) 180f, (float) -60f);
            g.DrawArc(pen, (float) (((base.InputLocation.X + base.Region.Width) - ((base.GSize / 3f) * 5f)) - 2f), (float) ((base.InputLocation.Y + base.Region.Height) - ((4f * base.GSize) / 3f)), (float) ((base.GSize / 3f) * 4f), (float) ((base.GSize / 3f) * 4f), (float) 90f, (float) -90f);
            g.DrawLine(pen, (float) (((base.InputLocation.X + base.Region.Width) - (base.GSize / 3f)) - 2f), (float) ((base.InputLocation.Y + base.Region.Height) - ((base.GSize * 2f) / 3f)), (float) (((base.InputLocation.X + base.Region.Width) - (base.GSize / 3f)) - 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) + ((1.732f * base.GSize) / 3f)));
            g.DrawArc(pen, (float) (((base.InputLocation.X + base.Region.Width) - (base.GSize / 3f)) - 2f), (float) ((base.InputLocation.Y + (base.Region.Height / 2f)) - ((0.286f * base.GSize) / 3f)), (float) ((base.GSize / 3f) * 4f), (float) ((base.GSize / 3f) * 4f), (float) 180f, (float) 60f);
            pen.Dispose();
        }
    }
}

