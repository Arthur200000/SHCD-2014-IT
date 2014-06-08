using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class arccot : hanshu
    {
        public arccot(lineexpression parent, Color color) : base("arccot", parent, color)
        {
            base.Type = FunctionType.反余切;
        }
    }
}

