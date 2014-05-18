using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class cot : hanshu
    {
        public cot(lineexpression parent, Color color) : base("cot", parent, color)
        {
            base.Type = FType.余切;
        }
    }
}

