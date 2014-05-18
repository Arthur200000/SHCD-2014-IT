namespace Qisi.Editor.Documents.Elements
{
    using Qisi.Editor.Documents.Table;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class TableInfo : Pic_Tab
    {
        private List<Column> columns;
        private float height;
        private List<Cell> items;
        private List<Row> rows;
        private float width;

        internal TableInfo(Point tableSize, float totalWidth, Font font, float lineWidth = 1f) : base(font)
        {
            int num2;
            this.TableSize = tableSize;
            this.LineWidth = lineWidth;
            float num = (font.Height * tableSize.Y) + ((base.Margin.Vertical + this.LineWidth) * (tableSize.Y + 1));
            this.width = 0f;
            this.height = 0f;
            this.items = new List<Cell>();
            this.rows = new List<Row>();
            this.columns = new List<Column>();
            for (num2 = 0; num2 < tableSize.X; num2++)
            {
                this.rows.Add(new Row());
                this.rows[num2].Cells = new List<Cell>();
            }
            int num3 = 0;
            while (num3 < tableSize.Y)
            {
                this.columns.Add(new Column());
                this.columns[num3].Cells = new List<Cell>();
                num3++;
            }
            float y = (this.Location.Y + base.Margin.Top) + this.LineWidth;
            for (num2 = 0; num2 < tableSize.X; num2++)
            {
                float x = (this.Location.X + base.Margin.Left) + this.LineWidth;
                for (num3 = 0; num3 < tableSize.Y; num3++)
                {
                    Cell item = new Cell(font, this, totalWidth / ((float) tableSize.Y), new PointF(x, y), Color.Transparent);
                    this.rows[num2].Cells.Add(item);
                    this.columns[num3].Cells.Add(item);
                    this.items.Add(item);
                    x += this.LineWidth + this.columns[num3].Width;
                }
                y += this.Rows[num2].Height + this.LineWidth;
            }
        }

        protected override void Dispose(bool disposing)
        {
            int num;
            if (disposing)
            {
                if (this.items != null)
                {
                    foreach (Cell cell in this.items)
                    {
                        cell.Dispose();
                    }
                }
                if (this.rows != null)
                {
                    foreach (Row row in this.rows)
                    {
                        row.Dispose();
                    }
                }
                if (this.columns != null)
                {
                    foreach (Column column in this.columns)
                    {
                        column.Dispose();
                    }
                }
            }
            if (this.items != null)
            {
                for (num = 0; num < this.items.Count; num++)
                {
                    this.items[num] = null;
                }
            }
            if (this.rows != null)
            {
                for (num = 0; num < this.rows.Count; num++)
                {
                    this.rows[num] = null;
                }
            }
            if (this.columns != null)
            {
                for (num = 0; num < this.columns.Count; num++)
                {
                    this.columns[num] = null;
                }
            }
            this.items = null;
            this.rows = null;
            this.columns = null;
            base.Dispose(disposing);
        }

        internal override void Draw(Graphics g)
        {
            if (((this.items != null) && (this.rows != null)) && (this.columns != null))
            {
                base.Draw(g);
                float y = this.Location.Y + base.Margin.Top;
                float num2 = y;
                float x = this.Location.X + base.Margin.Left;
                float num4 = x;
                float height = this.Size.Height - base.Margin.Vertical;
                float width = this.Size.Width - base.Margin.Horizontal;
                SolidBrush brush = new SolidBrush(Color.Black);
                for (int i = 0; i < this.TableSize.X; i++)
                {
                    g.FillRectangle(brush, x, num2, width, this.LineWidth);
                    num2 += this.rows[i].Height + this.LineWidth;
                }
                g.FillRectangle(brush, x, num2, width, this.LineWidth);
                for (int j = 0; j < this.TableSize.Y; j++)
                {
                    g.FillRectangle(brush, num4, y, this.LineWidth, height);
                    num4 += this.columns[j].Width + this.LineWidth;
                }
                g.FillRectangle(brush, num4, y, this.LineWidth, height);
                foreach (Cell cell in this.items)
                {
                    cell.Draw(g);
                }
                brush.Dispose();
            }
        }

        internal override void DrawHighLight(Graphics g)
        {
            if (((this.items != null) && (this.rows != null)) && (this.columns != null))
            {
                base.Draw(g);
                float y = this.Location.Y + base.Margin.Top;
                float num2 = y;
                float x = this.Location.X + base.Margin.Left;
                float num4 = x;
                float height = this.Size.Height - base.Margin.Vertical;
                float width = this.Size.Width - base.Margin.Horizontal;
                SolidBrush brush = new SolidBrush(Color.Black);
                for (int i = 0; i < this.TableSize.X; i++)
                {
                    g.FillRectangle(brush, x, num2, width, this.LineWidth);
                    num2 += this.rows[i].Height + this.LineWidth;
                }
                g.FillRectangle(brush, x, num2, width, this.LineWidth);
                for (int j = 0; j < this.TableSize.Y; j++)
                {
                    g.FillRectangle(brush, num4, y, this.LineWidth, height);
                    num4 += this.columns[j].Width + this.LineWidth;
                }
                g.FillRectangle(brush, num4, y, this.LineWidth, height);
                foreach (Cell cell in this.items)
                {
                    cell.DrawHighLight(g);
                }
                brush.Dispose();
            }
        }

        ~TableInfo()
        {
            this.Dispose(false);
        }

        internal Cell GetItem(int rowID, int columnID)
        {
            if ((rowID >= 0) && (columnID >= 0))
            {
                if (((this.rows == null) || (this.columns == null)) || (this.items == null))
                {
                    return null;
                }
                Cell cell = this.rows[rowID].Cells[columnID];
                if (!cell.ismerged)
                {
                    return cell;
                }
                if (((rowID - 1) >= 0) && (this.rows[rowID - 1].Cells[columnID].hspan > 1))
                {
                    return this.GetItem(rowID - 1, columnID);
                }
                if (((columnID - 1) >= 0) && (this.rows[rowID].Cells[columnID - 1].wspan > 1))
                {
                    return this.GetItem(rowID, columnID - 1);
                }
            }
            return null;
        }

        internal Point GetRowColumn(Cell cell)
        {
            if (((this.rows == null) || (this.columns == null)) || (this.items == null))
            {
                return new Point(-1, -1);
            }
            int index = this.Items.IndexOf(cell);
            if (index < 0)
            {
                return new Point(-1, -1);
            }
            return new Point(index / this.TableSize.Y, index % this.TableSize.Y);
        }

        public void LayOut(Graphics g)
        {
            foreach (Cell cell in this.items)
            {
                cell.PrepareToDraw(g);
            }
        }

        internal List<Column> Columns
        {
            get
            {
                return this.columns;
            }
            set
            {
                this.columns = value;
            }
        }

        internal float Height
        {
            get
            {
                this.height = 0f;
                foreach (Row row in this.Rows)
                {
                    this.height += row.Height + this.LineWidth;
                }
                this.height += base.Margin.Vertical + this.LineWidth;
                return this.height;
            }
        }

        internal List<Cell> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        internal float LineWidth { get; set; }

        internal override PointF Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                if (base.Location != value)
                {
                    base.Location = value;
                    float y = (base.Location.Y + base.Margin.Top) + this.LineWidth;
                    for (int i = 0; i < this.TableSize.X; i++)
                    {
                        float x = (base.Location.X + base.Margin.Left) + this.LineWidth;
                        for (int j = 0; j < this.TableSize.Y; j++)
                        {
                            this.rows[i].Cells[j].DocLocation = new PointF(x, y);
                            x += this.LineWidth + this.columns[j].Width;
                        }
                        y += this.rows[i].Height + this.LineWidth;
                    }
                }
            }
        }

        internal List<Row> Rows
        {
            get
            {
                return this.rows;
            }
            set
            {
                this.rows = value;
            }
        }

        internal override SizeF Size
        {
            get
            {
                return new SizeF(this.Width, this.Height);
            }
        }

        internal override bool Sized
        {
            get
            {
                foreach (Cell cell in this.items)
                {
                    foreach (Element element in cell.Elements)
                    {
                        if (!element.Sized)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        internal Point TableSize { get; set; }

        internal float Width
        {
            get
            {
                this.width = 0f;
                foreach (Column column in this.columns)
                {
                    this.width += column.Width + this.LineWidth;
                }
                this.width += base.Margin.Horizontal + this.LineWidth;
                return this.width;
            }
        }
    }
}

