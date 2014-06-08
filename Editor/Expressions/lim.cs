using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class lim : hanshuxiabiao
    {
        public lim(lineexpression parent, Color color) : base("lim", parent, color)
        {
            base.Type = FunctionType.极限;
        }
    }
}

