using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class min : hanshuxiabiao
    {
        public min(lineexpression parent, Color color) : base("min", parent, color)
        {
            base.Type = FType.最小值;
        }
    }
}

