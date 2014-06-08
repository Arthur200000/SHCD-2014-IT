using Qisi.Editor.Properties;


﻿namespace Qisi.Editor.Expression
{
    using Qisi.Editor;
    using System;
    using System.Drawing;

    internal class juzheng : structexpression
    {
        private Point Size;

        public juzheng(Point P, lineexpression parent, Color color) : base(parent, color, true)
        {
            int num;
            this.Size = new Point();
            this.Size = P;
            for (num = 0; num < (P.X * P.Y); num++)
            {
                base.Child.Add(new lineexpression(this.Font));
            }
            base.Type = FunctionType.矩阵;
            for (num = 0; num < (P.X * P.Y); num++)
            {
                base.Child[num].ParentExpression = this;
            }
            this.refreshUpDown();
        }

        public void addLine(int count)
        {
            this.Size = new Point(this.Size.X + count, this.Size.Y);
            for (int i = 0; i < (count * this.Size.Y); i++)
            {
                base.Child.Add(new lineexpression(this.Font));
                base.Child[base.Child.Count - 1].ParentExpression = this;
            }
            this.refreshUpDown();
        }

        public void deleteCol(int Index)
        {
            int num = Index % this.Size.Y;
            int num2 = Index / this.Size.Y;
            for (int i = this.Size.X - 1; i >= 0; i--)
            {
                base.Child.RemoveAt((i * this.Size.Y) + num);
            }
            this.Size = new Point(this.Size.X, this.Size.Y - 1);
            if ((this.Size.X == 0) || (this.Size.Y == 0))
            {
                base.Child = null;
            }
            this.refreshUpDown();
        }

        public void deleteRow(int Index)
        {
            int index = (Index / this.Size.Y) * this.Size.Y;
            for (int i = 0; i < this.Size.Y; i++)
            {
                base.Child.RemoveAt(index);
            }
            this.Size = new Point(this.Size.X - 1, this.Size.Y);
            if ((this.Size.X == 0) || (this.Size.Y == 0))
            {
                base.Child = null;
            }
            this.refreshUpDown();
        }

        public void insertColumnAfter(int Index)
        {
            int num = (Index % this.Size.Y) + 1;
            this.Size = new Point(this.Size.X, this.Size.Y + 1);
            for (int i = 0; i < this.Size.X; i++)
            {
                base.Child.Insert((i * this.Size.Y) + num, new lineexpression(this.Font));
                base.Child[(i * this.Size.Y) + num].ParentExpression = this;
            }
            this.refreshUpDown();
        }

        public void insertColumnBefore(int Index)
        {
            int num = Index % this.Size.Y;
            this.Size = new Point(this.Size.X, this.Size.Y + 1);
            for (int i = 0; i < this.Size.X; i++)
            {
                base.Child.Insert((i * this.Size.Y) + num, new lineexpression(this.Font));
                base.Child[(i * this.Size.Y) + num].ParentExpression = this;
            }
            this.refreshUpDown();
        }

        public void insertRowAfter(int Index)
        {
            int index = ((Index / this.Size.Y) * this.Size.Y) + this.Size.Y;
            for (int i = 0; i < this.Size.Y; i++)
            {
                base.Child.Insert(index, new lineexpression(this.Font));
                base.Child[index].ParentExpression = this;
            }
            this.Size = new Point(this.Size.X + 1, this.Size.Y);
            this.refreshUpDown();
        }

        public void insertRowBefore(int Index)
        {
            int index = (Index / this.Size.Y) * this.Size.Y;
            for (int i = 0; i < this.Size.Y; i++)
            {
                base.Child.Insert(index, new lineexpression(this.Font));
                base.Child[index].ParentExpression = this;
            }
            this.Size = new Point(this.Size.X + 1, this.Size.Y);
            this.refreshUpDown();
        }

        public override void RefreshInputLocation()
        {
            int num6;
            float x = base.InputLocation.X;
            float y = base.InputLocation.Y;
            int num3 = 0;
            while (num3 < this.Size.X)
            {
                float baseLine = 0f;
                float num5 = 0f;
                num6 = 0;
                while (num6 < this.Size.Y)
                {
                    if (baseLine < base.Child[(num3 * this.Size.Y) + num6].BaseLine)
                    {
                        baseLine = base.Child[(num3 * this.Size.Y) + num6].BaseLine;
                    }
                    if (num5 < (base.Child[(num3 * this.Size.Y) + num6].Region.Height - base.Child[(num3 * this.Size.Y) + num6].BaseLine))
                    {
                        num5 = base.Child[(num3 * this.Size.Y) + num6].Region.Height - base.Child[(num3 * this.Size.Y) + num6].BaseLine;
                    }
                    num6++;
                }
                for (num6 = 0; num6 < this.Size.Y; num6++)
                {
                    base.Child[(num3 * this.Size.Y) + num6].InputLocation = new PointF(base.Child[(num3 * this.Size.Y) + num6].InputLocation.X, (y + baseLine) - base.Child[(num3 * this.Size.Y) + num6].BaseLine);
                }
                y += (baseLine + num5) + (this.Font.Height / 2);
                num3++;
            }
            num6 = 0;
            while (num6 < this.Size.Y)
            {
                float width = 0f;
                num3 = 0;
                while (num3 < this.Size.X)
                {
                    if (width < base.Child[(num3 * this.Size.Y) + num6].Region.Width)
                    {
                        width = base.Child[(num3 * this.Size.Y) + num6].Region.Width;
                    }
                    num3++;
                }
                num3 = 0;
                while (num3 < this.Size.X)
                {
                    base.Child[(num3 * this.Size.Y) + num6].InputLocation = new PointF(x, base.Child[(num3 * this.Size.Y) + num6].InputLocation.Y);
                    num3++;
                }
                x += width + (this.Font.Height / 2);
                num6++;
            }
            for (num3 = 0; num3 < this.Size.X; num3++)
            {
                for (num6 = 0; num6 < this.Size.Y; num6++)
                {
                    base.Child[(num3 * this.Size.Y) + num6].RefreshInputLocation();
                }
            }
        }

        public override void RefreshRegion(Graphics g)
        {
            int num6;
            foreach (lineexpression lineexpression in base.Child)
            {
                lineexpression.ChangeFontSize(this.Font.Size);
                lineexpression.RefreshRegion(g);
            }
            float width = 0f;
            float height = 0f;
            int num3 = 0;
            while (num3 < this.Size.X)
            {
                float baseLine = 0f;
                float num5 = 0f;
                num6 = 0;
                while (num6 < this.Size.Y)
                {
                    if (baseLine < base.Child[(num3 * this.Size.Y) + num6].BaseLine)
                    {
                        baseLine = base.Child[(num3 * this.Size.Y) + num6].BaseLine;
                    }
                    if (num5 < (base.Child[(num3 * this.Size.Y) + num6].Region.Height - base.Child[(num3 * this.Size.Y) + num6].BaseLine))
                    {
                        num5 = base.Child[(num3 * this.Size.Y) + num6].Region.Height - base.Child[(num3 * this.Size.Y) + num6].BaseLine;
                    }
                    num6++;
                }
                height += baseLine + num5;
                num3++;
            }
            for (num6 = 0; num6 < this.Size.Y; num6++)
            {
                float num7 = 0f;
                for (num3 = 0; num3 < this.Size.X; num3++)
                {
                    if (num7 < base.Child[(num3 * this.Size.Y) + num6].Region.Width)
                    {
                        num7 = base.Child[(num3 * this.Size.Y) + num6].Region.Width;
                    }
                }
                width += num7;
            }
            width += ((this.Size.Y - 1) * this.Font.Height) / 2;
            height += ((this.Size.X - 1) * this.Font.Height) / 2;
            base.Region = new SizeF(width, height);
            float num8 = CommonMethods.CalcAscentPixel(this.Font);
            base.BaseLine = ((base.Region.Height / 2f) - (this.Font.Size / 2f)) + num8;
        }

        private void refreshUpDown()
        {
            for (int i = 0; i < (this.Size.X * this.Size.Y); i++)
            {
                base.Child[i].UpLineExpression = ((i - this.Size.Y) >= 0) ? base.Child[i - this.Size.Y] : null;
                base.Child[i].DownLineExpression = ((i + this.Size.Y) < (this.Size.X * this.Size.Y)) ? base.Child[i + this.Size.Y] : null;
            }
        }

        public override string ToString()
        {
            string newValue = "";
            for (int i = 0; i < (base.Child.Count - 1); i++)
            {
                if (((i + 1) % this.Size.Y) == 0)
                {
                    newValue = newValue + base.Child[i].ToString() + @"\";
                }
                else if (((i + 1) % this.Size.Y) == 0)
                {
                    newValue = newValue + base.Child[i].ToString() + "&";
                }
            }
            newValue = newValue + base.Child[base.Child.Count - 1].ToString();
            return CommonMethods.ExprToString(base.Type.ToString()).Replace("<0>", newValue);
        }

        public override string ToXml()
        {
            string str = "";
            str = "<" + base.Type.ToString() + "X=\"" + this.Size.X.ToString() + "\" Y=\"" + this.Size.Y.ToString() + "\" Color=\"" + this.Color.ToArgb().ToString("x8") + "\">";
            if (base.Child != null)
            {
                foreach (lineexpression lineexpression in base.Child)
                {
                    str = str + lineexpression.ToXml();
                }
            }
            return (str + "</" + base.Type.ToString() + ">");
        }
    }
}

