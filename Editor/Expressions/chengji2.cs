using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class chengji2 : type2
    {
        public chengji2(lineexpression parent, Color color) : base(Resources.chengjihao, parent, color, false)
        {
            base.Type = FunctionType.乘积2;
        }
    }
}

