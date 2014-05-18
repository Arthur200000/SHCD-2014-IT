namespace Qisi.Editor.Documents.Table
{
    using System;
    using System.Collections.Generic;

    internal class Column : IDisposable
    {
        private List<Cell> cells = new List<Cell>();
        private float width = 0f;

        internal Column()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            this.cells = null;
        }

        ~Column()
        {
            this.Dispose(false);
        }

        internal List<Cell> Cells
        {
            get
            {
                return this.cells;
            }
            set
            {
                this.cells = value;
            }
        }

        internal float Width
        {
            get
            {
                this.width = 0f;
                foreach (Cell cell in this.Cells)
                {
                    this.width = Math.Max(this.width, cell.OutWidth);
                }
                return this.width;
            }
        }
    }
}

