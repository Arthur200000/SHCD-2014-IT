using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor.Documents.Elements;
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Xml;

    internal class containerexpression : lineexpression
    {
        public containerexpression(Font font) : base(font)
        {
        }

        public containerexpression(XmlNode node, Font font) : base(node, font)
        {
        }

        public ExpressionInfo Info { get; set; }
    }
}

