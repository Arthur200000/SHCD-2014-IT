using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class tanh_2 : hanshushangbiao
    {
        public tanh_2(lineexpression parent, Color color) : base("tanh", parent, color)
        {
            base.Type = FType.双曲正切_2;
        }
    }
}

