using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class max : hanshuxiabiao
    {
        public max(lineexpression parent, Color color) : base("max", parent, color)
        {
            base.Type = FType.最大值;
        }
    }
}

