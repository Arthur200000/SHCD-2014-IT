using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;
	using Properties;

    internal class tijifen1 : type1
    {
        public tijifen1(lineexpression parent, Color color) : base(Resources.tijifenhao, parent, color)
        {
            base.Type = FunctionType.体积分1;
        }
    }
}

