namespace Qisi.General.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class QisiTreeView : TreeView
    {
        public QisiTreeView()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.DrawMode = TreeViewDrawMode.OwnerDrawText;
            base.DrawNode += new DrawTreeNodeEventHandler(this.PaperTree_DrawNode);
            base.HideSelection = false;
            base.HotTracking = true;
            this.BackColor = Color.White;
        }

        private void PaperTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null)
                {
                    nodeFont = this.Font;
                }
                e.Graphics.PageUnit = nodeFont.Unit;
                SizeF size = e.Graphics.MeasureString(e.Node.Text, nodeFont, 0, genericTypographic);
                RectangleF layoutRectangle = new RectangleF((PointF) e.Node.Bounds.Location, size);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, layoutRectangle, genericTypographic);
            }
            else if ((e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot)
            {
                Font font = e.Node.NodeFont;
                if (font == null)
                {
                    font = this.Font;
                }
                e.Graphics.PageUnit = font.Unit;
                SizeF ef3 = e.Graphics.MeasureString(e.Node.Text, font, 0, genericTypographic);
                RectangleF ef4 = new RectangleF((PointF) e.Node.Bounds.Location, ef3);
                e.Graphics.FillRectangle(SystemBrushes.HotTrack, e.Bounds);
                e.Graphics.DrawString(e.Node.Text, font, Brushes.White, ef4, genericTypographic);
            }
            else
            {
                Font font3 = e.Node.NodeFont;
                if (font3 == null)
                {
                    font3 = this.Font;
                }
                e.Graphics.PageUnit = font3.Unit;
                SizeF ef5 = e.Graphics.MeasureString(e.Node.Text, font3, 0, genericTypographic);
                RectangleF ef6 = new RectangleF((PointF) e.Node.Bounds.Location, ef5);
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                e.Graphics.DrawString(e.Node.Text, font3, Brushes.Black, ef6, genericTypographic);
            }
            genericTypographic.Dispose();
        }
    }
}

