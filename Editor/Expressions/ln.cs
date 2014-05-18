using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class ln : hanshu
    {
        public ln(lineexpression parent, Color color) : base("ln", parent, color)
        {
            base.Type = FType.自然对数;
        }
    }
}

