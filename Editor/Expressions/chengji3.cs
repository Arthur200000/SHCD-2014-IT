using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class chengji3 : type3
    {
        public chengji3(lineexpression parent, Color color) : base(Resources.chengjihao, parent, color, false)
        {
            base.Type = FunctionType.乘积3;
        }
    }
}

