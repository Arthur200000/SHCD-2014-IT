using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class tanh : hanshu
    {
        public tanh(lineexpression parent, Color color) : base("tanh", parent, color)
        {
            base.Type = FunctionType.双曲正切;
        }
    }
}

