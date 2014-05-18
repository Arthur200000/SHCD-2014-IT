using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class sec : hanshu
    {
        public sec(lineexpression parent, Color color) : base("sec", parent, color)
        {
            base.Type = FType.正割;
        }
    }
}

