using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal class logx : structexpression
    {
        public logx(lineexpression parent, Color color) : base(parent, color, true)
        {
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            base.Child.Add(new lineexpression(this.Font));
            this.Hanshu.ParentExpression = this;
            this.Benti.ParentExpression = this;
            this.Xiabiao.ParentExpression = this;
            this.Hanshu.Child = new List<structexpression>();
            foreach (char ch in "log".ToCharArray())
            {
                this.Hanshu.Child.Add(new charexpression(ch.ToString(), this.Hanshu, color));
            }
        }

        public override void RefreshInputLocation()
        {
            if (this.Xiabiao.Region.Height < this.Hanshu.Region.Height)
            {
                this.Hanshu.InputLocation = new PointF(base.InputLocation.X + 2f, (base.InputLocation.Y + base.BaseLine) - this.Hanshu.BaseLine);
                this.Xiabiao.InputLocation = new PointF((base.InputLocation.X + this.Hanshu.Region.Width) + 2f, base.InputLocation.Y + this.Hanshu.Region.Height);
            }
            else
            {
                this.Hanshu.InputLocation = new PointF(base.InputLocation.X + 2f, (base.InputLocation.Y + base.BaseLine) - this.Hanshu.BaseLine);
                this.Xiabiao.InputLocation = new PointF((base.InputLocation.X + this.Hanshu.Region.Width) + 2f, base.InputLocation.Y + this.Xiabiao.Region.Height);
            }
            this.Benti.InputLocation = new PointF((((base.InputLocation.X + this.Hanshu.Region.Width) + this.Xiabiao.Region.Width) + 2f) + 5f, (base.InputLocation.Y + base.BaseLine) - this.Benti.BaseLine);
            this.Xiabiao.RefreshInputLocation();
            this.Benti.RefreshInputLocation();
            this.Hanshu.RefreshInputLocation();
        }

        public override void RefreshRegion(Graphics g)
        {
            this.Benti.ChangeFontSize(this.Font.Size);
            this.Benti.RefreshRegion(g);
            float size = this.Benti.Font.Size;
            if (this.Xiabiao != null)
            {
                this.Xiabiao.ChangeFontSize(size);
            }
            this.Xiabiao.RefreshRegion(g);
            while ((this.Xiabiao.Region.Height > (this.Hanshu.Region.Height / 3f)) && (size > expression.minfontsize))
            {
                size -= 0.5f;
                if (this.Xiabiao != null)
                {
                    this.Xiabiao.ChangeFontSize(size);
                }
                this.Xiabiao.RefreshRegion(g);
            }
            this.Hanshu.ChangeFontSize(this.Font.Size);
            this.Hanshu.RefreshRegion(g);
            float width = ((this.Benti.Region.Width + this.Xiabiao.Region.Width) + this.Hanshu.Region.Width) + 9f;
            float height = Math.Max((float) (this.Xiabiao.Region.Height + (this.Hanshu.Region.Height / 2f)), (float) (this.Hanshu.Region.Height + (this.Xiabiao.Region.Height / 2f)));
            height = Math.Max(this.Hanshu.BaseLine, this.Benti.BaseLine) + Math.Max((float) (height - this.Hanshu.BaseLine), (float) (this.Benti.Region.Height - this.Benti.BaseLine));
            base.Region = new SizeF(width, height);
            base.BaseLine = Math.Max(this.Hanshu.BaseLine, this.Benti.BaseLine);
        }

        public override string ToString()
        {
            return (@"\" + this.Hanshu.ToString() + "_{" + this.Xiabiao.ToString() + "}{" + this.Benti.ToString() + "}");
        }

        public lineexpression Benti
        {
            get
            {
                return base.Child[2];
            }
        }

        public lineexpression Hanshu
        {
            get
            {
                return base.Child[0];
            }
        }

        public lineexpression Xiabiao
        {
            get
            {
                return base.Child[1];
            }
        }
    }
}

