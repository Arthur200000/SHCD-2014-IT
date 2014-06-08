using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class jifen2 : type2
    {
        public jifen2(lineexpression parent, Color color) : base(Resources.jfhao, parent, color, true)
        {
            base.Type = FunctionType.积分2;
        }
    }
}

