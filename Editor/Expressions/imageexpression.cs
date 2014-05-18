using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal abstract class imageexpression : structexpression
    {
        protected Image fuhao;

        protected imageexpression(Image f, lineexpression parent, Color color) : base(parent, color, true)
        {
            this.fuhao = f;
            base.Child = new List<lineexpression>();
            base.Child.Add(new lineexpression(this.Font));
            this.Benti.ParentExpression = this;
        }

        protected lineexpression Benti
        {
            get
            {
                return base.Child[base.Child.Count - 1];
            }
        }

        protected float FuhaoBaseLine { get; set; }

        protected PointF FuhaoLoc { get; set; }

        protected SizeF GSize
        {
            get
            {
                return new SizeF((this.fuhao.Width * this.Font.Size) / 70f, (this.fuhao.Height * this.Font.Size) / 70f);
            }
        }
    }
}

