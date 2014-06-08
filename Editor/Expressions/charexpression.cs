using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using Qisi.Editor.Documents;
    using System;
    using System.Drawing;

    internal class charexpression : structexpression
    {
        protected string content;

        public charexpression(string str, lineexpression parent, Color color) : base(parent, color, false)
        {
            this.content = "";
            this.content = Document.FromEscape(str);
            base.Type = FunctionType.字符;
        }

        public override void DrawExpression(Graphics g)
        {
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            SolidBrush brush = new SolidBrush(this.Color);
            g.DrawString(this.content, base.ParentExpression.Font, brush, base.InputLocation, genericTypographic);
            brush.Dispose();
        }

        public override void RefreshInputLocation()
        {
        }

        public override void RefreshRegion(Graphics g)
        {
            g.PageUnit = GraphicsUnit.Pixel;
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            base.Region = g.MeasureString(this.content, this.Font, 0, genericTypographic);
            base.BaseLine = CommonMethods.CalcAscentPixel(base.ParentExpression.Font);
        }

        public override string ToString()
        {
            return this.content;
        }

        public override string ToXml()
        {
            string str3 = "<" + base.Type.ToString() + " Color=\"" + this.Color.ToArgb().ToString("x8") + "\">";
            return (str3 + this.content + "</" + base.Type.ToString() + ">");
        }
    }
}

