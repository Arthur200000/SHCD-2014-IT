using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class csc : hanshu
    {
        public csc(lineexpression parent, Color color) : base("csc", parent, color)
        {
            base.Type = FunctionType.余割;
        }
    }
}

