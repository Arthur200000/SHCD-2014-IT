using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.CompilerServices;
    using System.Xml;

    internal class lineexpression : expression
    {
        private System.Drawing.Font _font;

        public lineexpression(System.Drawing.Font font)
        {
            this._font = font;
            this.Child = new List<structexpression>();
        }

        public lineexpression(XmlNode node, System.Drawing.Font font)
        {
            this._font = font;
            if (node.HasChildNodes)
            {
                this.Child = new List<structexpression>();
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    System.Drawing.Color black;
                    if (node2.Attributes["Color"] != null)
                    {
                        string str = node2.Attributes["Color"].Value;
                        try
                        {
                            black = System.Drawing.Color.FromArgb(Convert.ToInt32(str, 0x10));
                        }
                        catch
                        {
                            black = System.Drawing.Color.Black;
                        }
                    }
                    else
                    {
                        black = System.Drawing.Color.Black;
                    }
                    int matrixX = 2;
                    int matrixY = 1;
                    if (node2.Name == "矩阵")
                    {
                        if (node2.Attributes["X"] != null)
                        {
                            matrixX = Convert.ToInt32(node2.Attributes["X"].Value);
                        }
                        if (node2.Attributes["Y"] != null)
                        {
                            matrixY = Convert.ToInt32(node2.Attributes["Y"].Value);
                        }
                    }
                    structexpression item = CommonMethods.CreateExpr(node2.Name, this, black, node2.InnerText, matrixX, matrixY);
                    this.Child.Add(item);
                    if (node2.HasChildNodes && (item.Child != null))
                    {
                        for (int i = 0; i < Math.Min(node2.ChildNodes.Count, item.Child.Count); i++)
                        {
                            XmlNode node3 = node2.ChildNodes[i];
                            FontStyle regular = FontStyle.Regular;
                            if (node3.Attributes["Style"] != null)
                            {
                                string str2 = node3.Attributes["Style"].Value;
                                if (str2.Contains("Bold"))
                                {
                                    regular |= FontStyle.Bold;
                                }
                                if (str2.Contains("Italic"))
                                {
                                    regular |= FontStyle.Italic;
                                }
                                if (str2.Contains("Strikeout"))
                                {
                                    regular |= FontStyle.Strikeout;
                                }
                                if (str2.Contains("Underline"))
                                {
                                    regular |= FontStyle.Underline;
                                }
                            }
                            System.Drawing.Font font2 = new System.Drawing.Font(font.Name, font.Size, regular, font.Unit);
                            lineexpression lineexpression = new lineexpression(node3, font2);
                            item.Child[i] = lineexpression;
                            lineexpression.ParentExpression = item;
                        }
                    }
                }
            }
        }

        public void ChangeFontSize(float size)
        {
            this._font = new System.Drawing.Font(this._font.Name, Math.Max(size, expression.minfontsize), this._font.Style, this._font.Unit);
        }

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._font.Dispose();
            }
        }

        public override void DrawExpression(Graphics g)
        {
            if (this.Child.Count == 0)
            {
                Pen pen = new Pen(this.Color) {
                    DashStyle = DashStyle.Dot
                };
                g.DrawRectangle(pen, base.InputLocation.X, base.InputLocation.Y, base.Region.Width, base.Region.Height);
                pen.Dispose();
            }
            else
            {
                foreach (structexpression structexpression in this.Child)
                {
                    structexpression.DrawExpression(g);
                }
            }
        }

        ~lineexpression()
        {
            this.Dispose(false);
        }

        public override expression PointInChild(Point point)
        {
            if (this.Child != null)
            {
                foreach (structexpression structexpression in this.Child)
                {
                    if (structexpression.PointInOrNot(point))
                    {
                        return structexpression.PointInChild(point);
                    }
                }
            }
            return this;
        }

        public override void RefreshInputLocation()
        {
            if (this.Child.Count != 0)
            {
                this.Child[0].InputLocation = new PointF(base.InputLocation.X, (base.InputLocation.Y + base.BaseLine) - this.Child[0].BaseLine);
                this.Child[0].RefreshInputLocation();
                for (int i = 1; i < this.Child.Count; i++)
                {
                    this.Child[i].InputLocation = new PointF(this.Child[i - 1].InputLocation.X + this.Child[i - 1].Region.Width, (base.InputLocation.Y + base.BaseLine) - this.Child[i].BaseLine);
                    this.Child[i].RefreshInputLocation();
                }
            }
        }

        public override void RefreshRegion(Graphics g)
        {
            if (this.Child.Count == 0)
            {
                g.PageUnit = GraphicsUnit.Pixel;
                StringFormat genericTypographic = StringFormat.GenericTypographic;
                genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                base.Region = g.MeasureString("  ", this.Font, 0, genericTypographic);
                base.BaseLine = ((base.Region.Height / 2f) - (this._font.Size / 2f)) + CommonMethods.CalcAscentPixel(this._font);
            }
            else
            {
                float width = 0f;
                float baseLine = 0f;
                float num3 = 0f;
                foreach (expression expression in this.Child)
                {
                    expression.RefreshRegion(g);
                    width += expression.Region.Width;
                    if (expression.BaseLine > baseLine)
                    {
                        baseLine = expression.BaseLine;
                    }
                    if ((expression.Region.Height - expression.BaseLine) > num3)
                    {
                        num3 = expression.Region.Height - expression.BaseLine;
                    }
                }
                float height = baseLine + num3;
                base.Region = new SizeF(width, height);
                base.BaseLine = baseLine;
            }
        }

        public override string ToString()
        {
            if (this.Child == null)
            {
                return "";
            }
            string str = "";
            foreach (expression expression in this.Child)
            {
                str = str + expression.ToString();
            }
            return str;
        }

        public override string ToXml()
        {
            string str = "";
            str = "<线性 Style=\"" + this.Font.Style.ToString() + "\">";
            if (this.Child != null)
            {
                foreach (structexpression structexpression in this.Child)
                {
                    str = str + structexpression.ToXml();
                }
            }
            return (str + "</线性>");
        }

        public List<structexpression> Child { get; set; }

        public override System.Drawing.Color Color
        {
            get
            {
                return ((this.ParentExpression == null) ? System.Drawing.Color.Black : this.ParentExpression.Color);
            }
            set
            {
                foreach (structexpression structexpression in this.Child)
                {
                    structexpression.Color = value;
                }
            }
        }

        public lineexpression DownLineExpression { get; set; }

        public override System.Drawing.Font Font
        {
            get
            {
                return this._font;
            }
        }

        public structexpression ParentExpression { get; set; }

        public lineexpression UpLineExpression { get; set; }
    }
}

