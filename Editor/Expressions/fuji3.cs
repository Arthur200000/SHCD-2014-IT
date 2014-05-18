using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class fuji3 : type3
    {
        public fuji3(lineexpression parent, Color color) : base(Resources.fujihao, parent, color, false)
        {
            base.Type = FType.副积3;
        }
    }
}

