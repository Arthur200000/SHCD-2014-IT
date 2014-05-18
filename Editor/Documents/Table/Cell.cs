namespace Qisi.Editor.Documents.Table
{
    using Qisi.Editor.Documents;
    using Qisi.Editor.Documents.Elements;
    using System;
    using System.Drawing;

    internal class Cell : Document
    {
        private float _minheight;
        public int hspan;
        public bool ismerged;
        public int wspan;

        public Cell(Font F, TableInfo con, float docwidth, PointF Location, Color backColor) : base(con.Margin, F, con, docwidth, Location, backColor)
        {
            this.hspan = 1;
            this.wspan = 1;
            this.ismerged = false;
        }

        internal override float DocHeight
        {
            get
            {
                return base.DocHeight;
            }
            set
            {
                if (base.DocHeight != value)
                {
                    base.DocHeight = value;
                    TableInfo parent = base.Parent as TableInfo;
                    if (parent != null)
                    {
                        float y = (parent.Location.Y + parent.Margin.Top) + parent.LineWidth;
                        for (int i = 0; i < parent.TableSize.X; i++)
                        {
                            float x = (parent.Location.X + parent.Margin.Left) + parent.LineWidth;
                            for (int j = 0; j < parent.TableSize.Y; j++)
                            {
                                if ((parent.Rows.Count > i) && (parent.Rows[i].Cells.Count > j))
                                {
                                    parent.Rows[i].Cells[j].DocLocation = new PointF(x, y);
                                    x += parent.LineWidth + parent.Columns[j].Width;
                                }
                            }
                            y += parent.Rows[i].Height + parent.LineWidth;
                        }
                    }
                }
            }
        }

        internal override float DocWidth
        {
            get
            {
                return base.DocWidth;
            }
            set
            {
                if (base.DocWidth != value)
                {
                    base.DocWidth = value;
                    TableInfo parent = base.Parent as TableInfo;
                    if (parent != null)
                    {
                        float y = (parent.Location.Y + parent.Margin.Top) + parent.LineWidth;
                        for (int i = 0; i < parent.TableSize.X; i++)
                        {
                            float x = (parent.Location.X + parent.Margin.Left) + parent.LineWidth;
                            for (int j = 0; j < parent.TableSize.Y; j++)
                            {
                                if ((parent.Rows.Count > i) && (parent.Rows[i].Cells.Count > j))
                                {
                                    parent.Rows[i].Cells[j].DocLocation = new PointF(x, y);
                                    x += parent.LineWidth + parent.Columns[j].Width;
                                }
                            }
                            y += parent.Rows[i].Height + parent.LineWidth;
                        }
                    }
                }
            }
        }

        public float MinHeight
        {
            get
            {
                return this._minheight;
            }
            set
            {
                if (this._minheight != value)
                {
                    this._minheight = value;
                    float y = (((TableInfo) base.Parent).Location.Y + ((TableInfo) base.Parent).Margin.Top) + ((TableInfo) base.Parent).LineWidth;
                    for (int i = 0; i < ((TableInfo) base.Parent).TableSize.X; i++)
                    {
                        float x = (((TableInfo) base.Parent).Location.X + ((TableInfo) base.Parent).Margin.Left) + ((TableInfo) base.Parent).LineWidth;
                        for (int j = 0; j < ((TableInfo) base.Parent).TableSize.Y; j++)
                        {
                            if ((((TableInfo) base.Parent).Rows.Count > i) && (((TableInfo) base.Parent).Rows[i].Cells.Count > j))
                            {
                                ((TableInfo) base.Parent).Rows[i].Cells[j].DocLocation = new PointF(x, y);
                                x += ((TableInfo) base.Parent).LineWidth + ((TableInfo) base.Parent).Columns[j].Width;
                            }
                        }
                        y += ((TableInfo) base.Parent).Rows[i].Height + ((TableInfo) base.Parent).LineWidth;
                    }
                }
            }
        }

        internal override float OutHeight
        {
            get
            {
                return Math.Max(this.MinHeight, this.DocHeight + base.Margin.Vertical);
            }
        }
    }
}

