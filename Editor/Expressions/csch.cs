using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class csch : hanshu
    {
        public csch(lineexpression parent, Color color) : base("csch", parent, color)
        {
            base.Type = FunctionType.双曲余割;
        }
    }
}

