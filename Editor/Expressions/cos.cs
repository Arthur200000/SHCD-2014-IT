using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class cos : hanshu
    {
        public cos(lineexpression parent, Color color) : base("cos", parent, color)
        {
            base.Type = FType.余弦;
        }
    }
}

