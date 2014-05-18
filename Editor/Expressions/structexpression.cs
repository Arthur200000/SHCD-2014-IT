using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal abstract class structexpression : expression
    {
        private System.Drawing.Color _color;

        public structexpression(lineexpression parent, System.Drawing.Color color, bool haschild = true)
        {
            this.ParentExpression = parent;
            if (haschild)
            {
                this.Child = new List<lineexpression>();
            }
            else
            {
                this.Child = null;
            }
            this.Color = color;
        }

        public override void DrawExpression(Graphics g)
        {
            foreach (lineexpression lineexpression in this.Child)
            {
                lineexpression.DrawExpression(g);
            }
        }

        ~structexpression()
        {
            this.Dispose(false);
        }

        public override expression PointInChild(Point point)
        {
            if (this.Child != null)
            {
                foreach (lineexpression lineexpression in this.Child)
                {
                    if (lineexpression.PointInOrNot(point))
                    {
                        return lineexpression.PointInChild(point);
                    }
                }
            }
            return this;
        }

        public override string ToString()
        {
            string str = CommonMethods.ExprToString(base.Type.ToString());
            if (this.Child != null)
            {
                for (int i = 0; i < this.Child.Count; i++)
                {
                    str = str.Replace("<" + i.ToString() + ">", this.Child[i].ToString());
                }
            }
            return str;
        }

        public override string ToXml()
        {
            string str = "";
            str = "<" + base.Type.ToString() + " Color=\"" + this.Color.ToArgb().ToString("x8") + "\">";
            if (this.Child != null)
            {
                foreach (lineexpression lineexpression in this.Child)
                {
                    str = str + lineexpression.ToXml();
                }
            }
            return (str + "</" + base.Type.ToString() + ">");
        }

        public List<lineexpression> Child { get; set; }

        public override System.Drawing.Color Color
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
                if (this.Child != null)
                {
                    foreach (lineexpression lineexpression in this.Child)
                    {
                        lineexpression.Color = value;
                    }
                }
            }
        }

        public lineexpression DefaultChild
        {
            get
            {
                if ((this.Child != null) && (this.Child.Count > 0))
                {
                    return this.Child[0];
                }
                return null;
            }
        }

        public override System.Drawing.Font Font
        {
            get
            {
                return ((this.ParentExpression != null) ? this.ParentExpression.Font : SystemFonts.DefaultFont);
            }
        }

        public lineexpression ParentExpression { get; set; }
    }
}

