using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class lifanggen : genshi
    {
        public lifanggen(lineexpression parent, Color color) : base(parent, color)
        {
            base.Type = FType.立方根;
            base.Gen.Child = new List<structexpression>();
            base.Gen.Child.Add(new charexpression("3", base.Gen, color));
        }
    }
}

