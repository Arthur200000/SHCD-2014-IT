using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class sinh : hanshu
    {
        public sinh(lineexpression parent, Color color) : base("sinh", parent, color)
        {
            base.Type = FunctionType.双曲正弦;
        }
    }
}

