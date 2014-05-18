using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class arcsin : hanshu
    {
        public arcsin(lineexpression parent, Color color) : base("arcsin", parent, color)
        {
            base.Type = FType.反正弦;
        }
    }
}

