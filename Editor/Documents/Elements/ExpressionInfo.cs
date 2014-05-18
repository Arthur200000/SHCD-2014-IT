namespace Qisi.Editor.Documents.Elements
{
    using Qisi.Editor.Expression;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal class ExpressionInfo : Element
    {
        private containerexpression containerExpr;
        internal static System.Windows.Forms.Padding padding = new System.Windows.Forms.Padding(2, 0, 2, 0);

        internal ExpressionInfo(containerexpression expr, Font font) : base(font)
        {
            this.ContainerExpression = expr;
            this.ContainerExpression.Info = this;
            this.Sized = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.containerExpr.Dispose();
            }
            this.containerExpr = null;
            base.Dispose(disposing);
        }

        internal override void Draw(Graphics g)
        {
            if (this.containerExpr != null)
            {
                base.Draw(g);
                this.containerExpr.DrawExpression(g);
            }
        }

        ~ExpressionInfo()
        {
            this.Dispose(false);
        }

        internal override float BaseLine
        {
            get
            {
                if (this.containerExpr != null)
                {
                    return (this.containerExpr.BaseLine + this.Padding.Top);
                }
                return 0f;
            }
        }

        internal System.Drawing.Color Color
        {
            get
            {
                return this.containerExpr.Color;
            }
            set
            {
                this.containerExpr.Color = value;
            }
        }

        internal containerexpression ContainerExpression
        {
            get
            {
                return this.containerExpr;
            }
            set
            {
                this.containerExpr = value;
            }
        }

        internal override PointF Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
                if (this.containerExpr != null)
                {
                    this.containerExpr.InputLocation = new PointF(value.X + this.Padding.Left, value.Y + this.Padding.Top);
                    this.containerExpr.RefreshInputLocation();
                }
            }
        }

        internal System.Windows.Forms.Padding Padding
        {
            get
            {
                return padding;
            }
        }

        internal override SizeF Size
        {
            get
            {
                if (this.containerExpr != null)
                {
                    return new SizeF(this.containerExpr.Region.Width + this.Padding.Horizontal, this.containerExpr.Region.Height + this.Padding.Vertical);
                }
                return new SizeF(0f, 0f);
            }
        }
    }
}

