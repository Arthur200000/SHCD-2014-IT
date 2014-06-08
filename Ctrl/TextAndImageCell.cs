namespace Qisi.General.Controls
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;
	/// <summary>
	/// Text and image cell.
	/// </summary>
    public class TextAndImageCell : DataGridViewTextBoxCell
    {
        private System.Drawing.Color ColorValue = System.Drawing.Color.Transparent;

        public TextAndImageCell()
        {
            base.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
		/// <summary>
		/// Clone this instance.
		/// </summary>
        public override object Clone()
        {
            TextAndImageCell cell = (TextAndImageCell) base.Clone();
            cell.ColorValue = this.ColorValue;
            return cell;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, "", formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            Bitmap image = new Bitmap((int) (cellBounds.Height * 1.2), cellBounds.Height);
            Graphics.FromImage(image).FillRectangle(new SolidBrush(this.ColorValue), 4, 4, image.Width - 8, image.Height - 8);
            GraphicsContainer container = graphics.BeginContainer();
            graphics.SetClip(cellBounds);
            graphics.DrawImageUnscaled(image, cellBounds.Location);
            SolidBrush brush = new SolidBrush(cellStyle.ForeColor);
            SizeF ef = graphics.MeasureString(value.ToString(), cellStyle.Font);
            graphics.DrawString(value.ToString(), cellStyle.Font, brush, (float) (image.Width - 8), (cellBounds.Height - ef.Height) / 2f);
            graphics.EndContainer(container);
        }
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
        public System.Drawing.Color Color
        {
            get
            {
                return this.ColorValue;
            }
            set
            {
                this.ColorValue = value;
            }
        }

        private DataGridViewAutoFilterTextAndPictureBoxColumn OwningTextAndImageColumn
        {
            get
            {
                return (DataGridViewAutoFilterTextAndPictureBoxColumn) base.OwningColumn;
            }
        }
    }
}

