using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class sin_2 : hanshushangbiao
    {
        public sin_2(lineexpression parent, Color color) : base("sin", parent, color)
        {
            base.Type = FunctionType.正弦_2;
        }
    }
}

