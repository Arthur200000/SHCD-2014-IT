using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class cosh : hanshu
    {
        public cosh(lineexpression parent, Color color) : base("cosh", parent, color)
        {
            base.Type = FunctionType.双曲余弦;
        }
    }
}

