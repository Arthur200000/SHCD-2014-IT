namespace Qisi.Editor.Documents.Table
{
    using System;
    using System.Collections.Generic;

    internal class Row
    {
        private List<Cell> cells = new List<Cell>();
        private float height = 0f;

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

        ~Row()
        {
            this.Dispose(false);
        }

        public List<Cell> Cells
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

        public float Height
        {
            get
            {
                this.height = 0f;
                foreach (Cell cell in this.Cells)
                {
                    this.height = Math.Max(this.height, cell.OutHeight);
                }
                return this.height;
            }
        }
    }
}

