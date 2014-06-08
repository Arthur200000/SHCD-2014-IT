using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class arccos : hanshu
    {
        public arccos(lineexpression parent, Color color) : base("arccos", parent, color)
        {
            base.Type = FunctionType.反余弦;
        }
    }
}

