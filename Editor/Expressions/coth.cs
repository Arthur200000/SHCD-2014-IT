using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Drawing;

    internal class coth : hanshu
    {
        public coth(lineexpression parent, Color color) : base("coth", parent, color)
        {
            base.Type = FType.双曲余切;
        }
    }
}

