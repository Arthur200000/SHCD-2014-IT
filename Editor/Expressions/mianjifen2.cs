using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class mianjifen2 : type2
    {
        public mianjifen2(lineexpression parent, Color color) : base(Resources.mjhao, parent, color, true)
        {
            base.Type = FunctionType.面积分2;
        }
    }
}

