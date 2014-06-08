using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class tan : hanshu
    {
        public tan(lineexpression parent, Color color) : base("tan", parent, color)
        {
            base.Type = FunctionType.正切;
        }
    }
}

