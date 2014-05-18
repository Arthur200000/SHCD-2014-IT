using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class sech : hanshu
    {
        public sech(lineexpression parent, Color color) : base("sech", parent, color)
        {
            base.Type = FType.双曲正割;
        }
    }
}

