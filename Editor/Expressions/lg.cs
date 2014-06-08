using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class lg : hanshu
    {
        public lg(lineexpression parent, Color color) : base("lg", parent, color)
        {
            base.Type = FunctionType.对数10;
        }
    }
}

