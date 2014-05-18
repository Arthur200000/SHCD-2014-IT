using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class sin : hanshu
    {
        public sin(lineexpression parent, Color color) : base("sin", parent, color)
        {
            base.Type = FType.正弦;
        }
    }
}

