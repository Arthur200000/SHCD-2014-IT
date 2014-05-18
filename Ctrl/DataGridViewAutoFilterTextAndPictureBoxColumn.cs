namespace Qisi.General.Controls
{
    using System;
    using System.Windows.Forms;

    public class DataGridViewAutoFilterTextAndPictureBoxColumn : DataGridViewAutoFilterTextBoxColumn
    {
        public DataGridViewAutoFilterTextAndPictureBoxColumn()
        {
            this.CellTemplate = new TextAndImageCell();
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public override object Clone()
        {
            return (DataGridViewAutoFilterTextAndPictureBoxColumn) base.Clone();
        }

        private TextAndImageCell TextAndImageCellTemplate
        {
            get
            {
                return (TextAndImageCell) this.CellTemplate;
            }
        }
    }
}

