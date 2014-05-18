using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class log : hanshu
    {
        public log(lineexpression parent, Color color) : base("log", parent, color)
        {
            base.Type = FType.对数2;
        }
    }
}

