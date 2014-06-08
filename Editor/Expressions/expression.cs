using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal abstract class expression : IDisposable
    {
        protected static float minfontsize = 8f;

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public abstract void DrawExpression(Graphics g);
        ~expression()
        {
            this.Dispose(false);
        }

        public abstract expression PointInChild(Point point);
        public expression PointInExpression(Point point)
        {
            if (this.PointInOrNot(point))
            {
                return this.PointInChild(point);
            }
            return null;
        }

        public bool PointInOrNot(Point point)
        {
            return ((((this.InputLocation.X <= point.X) && (((this.InputLocation.X + this.Region.Width) - 1f) >= point.X)) && (this.InputLocation.Y <= point.Y)) && (((this.InputLocation.Y + this.Region.Height) - 1f) >= point.Y));
        }

        public abstract void RefreshInputLocation();
        public abstract void RefreshRegion(Graphics g);
        public abstract string ToString();
        public abstract string ToXml();

        public float BaseLine { get; set; }

        public List<expression> Child { get; set; }

        public abstract System.Drawing.Color Color { get; set; }

        public abstract System.Drawing.Font Font { get; }

        public PointF InputLocation { get; set; }

        public expression ParentExpression { get; set; }

        public SizeF Region { get; set; }

        public FunctionType Type { get; set; }
    }
}

