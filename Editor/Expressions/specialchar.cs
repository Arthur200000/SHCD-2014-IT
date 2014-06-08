using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;

    internal class specialchar : charexpression
    {
        private Font _Font;
        private string specialcontent;

        public specialchar(string str, FunctionType f, string c2, lineexpression parent, Color color) : base(str, parent, color)
        {
            this.specialcontent = c2;
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            this._Font = CommonMethods.GetCambriaFont(12f, FontStyle.Regular);
            base.Type = f;
        }

        public override string ToString()
        {
            return this.specialcontent;
        }
    }
}

