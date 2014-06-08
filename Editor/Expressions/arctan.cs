using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class arctan : hanshu
    {
        public arctan(lineexpression parent, Color color) : base("arctan", parent, color)
        {
            base.Type = FunctionType.反正切;
        }
    }
}

