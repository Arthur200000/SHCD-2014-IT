using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class erchongjifen2 : type2
    {
        public erchongjifen2(lineexpression parent, Color color) : base(Resources.erchongfuhao, parent, color, true)
        {
            base.Type = FunctionType.二重积分2;
        }
    }
}

