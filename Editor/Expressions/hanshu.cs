using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class hanshu : structexpression
    {
        public hanshu(string h, lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            this.Hanshu.ParentExpression = this;
            this.Benti.ParentExpression = this;
            this.Hanshu.Child = new List<structexpression>();
            foreach (char ch in h.ToCharArray())
            {
                this.Hanshu.Child.Add(new charexpression(ch.ToString(), this.Hanshu, color));
            }
        }

        public override void RefreshInputLocation()
        {
            this.Hanshu.InputLocation = new PointF(base.InputLocation.X + 2f, (base.InputLocation.Y + base.BaseLine) - this.Hanshu.BaseLine);
            this.Hanshu.RefreshInputLocation();
            this.Benti.InputLocation = new PointF(((base.InputLocation.X + this.Hanshu.Region.Width) + 2f) + 5f, (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine);
            this.Benti.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            this.Benti.ChangeFontSize(this.Font.Size);
            this.Benti.RefreshRegion(g);
            this.Hanshu.RefreshRegion(g);
            this.Hanshu.ChangeFontSize(this.Font.Size);
            float width = (this.Benti.Region.Width + this.Hanshu.Region.Width) + 9f;
            float height = Math.Max(this.Hanshu.BaseLine, this.Benti.BaseLine) + Math.Max((float) (this.Hanshu.Region.Height - this.Hanshu.BaseLine), (float) (this.Benti.Region.Height - this.Benti.BaseLine));
            base.Region = new SizeF(width, height);
            if (this.Hanshu.BaseLine < this.Benti.BaseLine)
            {
                base.BaseLine = this.Benti.BaseLine;
            }
            else
            {
                base.BaseLine = this.Hanshu.BaseLine;
            }
        }

        public override string ToString()
        {
            return (@"\" + this.Hanshu.ToString() + "{" + this.Benti.ToString() + "}");
        }

        public lineexpression Benti
        {
            get
            {
                return base.Child[1];
            }
        }

        public lineexpression Hanshu
        {
            get
            {
                return base.Child[0];
            }
        }
    }
}

