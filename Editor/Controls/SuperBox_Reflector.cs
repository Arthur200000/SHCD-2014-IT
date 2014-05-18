namespace Qisi.Editor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Qisi.General;
    using Qisi.Editor;
    using Qisi.Editor.Documents;
    using Qisi.Editor.Documents.Elements;
    using Qisi.Editor.Documents.Table;
    using Qisi.Editor.Expression;
    using Qisi.Editor.Properties;

    [ComVisible(true)]
    public class SuperBox : Control
    {
        private object beginSelectContainer;
        private float beginSelectIndex;
        private object boundDragedObject;
        private object caretContainer;
        private float caretHeight;
        private int caretIndex;
        private PointF caretLocation;
        private ContextMenuStrip contextMenuStrip1;
        private expression currentMatrix;
        private int currentMatrixItemIndex;
        private long downArrowTime;
        private Bound dragBound;
        private object dragedObject;
        private bool freshed;
        private bool hasCaret;
        private const string inputableChars = "，。“”‘’、…：；！？";
        private Queue<char> inputChars;
        private Queue<FType> inputExpressions;
        private Queue<Image> inputImages;
        private Queue<Point> inputTables;
        private long leftArrowTime;
        private Document mainDocument;
        private object mouseContainer;
        private float mouseIndex;
        private MouseState mouseState;
        private long rightArrowTime;
        private List<object> selectedContainers;
        private Pic_Tab selectedObject;
        private List<int> selectedTextLast;
        private List<int> selectedTextNew;
        private ToolTip tip;
        private ToolStripMenuType toolStripMenuType;
        private long upArrowTime;
        private float virtualCaretX;

        public event EventHandler ContentChanged;

        public event EventHandler OperateClicked;

        public event MessageEventHandler OperateDone;

        public SuperBox() : this(200)
        {
        }

        public SuperBox(int width)
        {
            this.selectedTextLast = new List<int>();
            this.selectedTextNew = new List<int>();
            this.selectedContainers = new List<object>();
            this.inputChars = new Queue<char>();
            this.inputExpressions = new Queue<FType>();
            this.inputImages = new Queue<Image>();
            this.inputTables = new Queue<Point>();
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.tip = new ToolTip();
            base.SizeChanged += new EventHandler(this.SuperBox_SizeChanged);
            base.KeyDown += new KeyEventHandler(this.SuperBox_KeyDown);
            base.KeyPress += new KeyPressEventHandler(this.SuperBox_KeyPress);
            base.MouseDown += new MouseEventHandler(this.SuperBox_MouseDown);
            base.MouseMove += new MouseEventHandler(this.SuperBox_MouseMove);
            base.MouseUp += new MouseEventHandler(this.SuperBox_MouseUp);
            base.Click += new EventHandler(this.SuperBox_Click);
            base.MouseHover += new EventHandler(this.SuperBox_MouseHover);
            this.contextMenuStrip1 = new ContextMenuStrip();
            base.TabStop = false;
            base.Width = width;
            this.DoubleBuffered = true;
            this.Font = new Font("宋体", 20f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.caretHeight = 0f;
            this.mouseState = MouseState.None;
            this.hasCaret = false;
            this.mainDocument = new Document(new Padding(10, 0, 10, 10), this.Font, null, (float) base.ClientSize.Width, new PointF(0f, 0f), this.BackColor);
            this.mainDocument.ContentChanged += new EventHandler(this.mainDocument_ContentChanged);
            this.mainDocument.HeightChanged += new EventHandler(this.mainDocument_HeightChanged);
            this.mainDocument.OperateClicked += new EventHandler(this.mainDocument_OperateClicked);
            this.mainDocument.OperateDone += new EventHandler(this.mainDocument_OperateDone);
            base.Height = (int) this.mainDocument.OutHeight;
            this.Insert();
        }

        private void AddLine(object sender, EventArgs e)
        {
        }

        internal void AppendF(FType type)
        {
            this.inputExpressions.Enqueue(type);
            this.Insert();
        }

        public void AppendImage(Image img)
        {
            this.inputImages.Enqueue(img);
            this.Insert();
        }

        public void AppendT(Point p)
        {
            this.inputTables.Enqueue(p);
            this.Insert();
        }

        private void CancelSelection()
        {
            if (this.selectedObject != null)
            {
                this.selectedObject.AloneSelected = false;
                this.selectedObject = null;
            }
            this.selectedContainers = new List<object>();
            this.selectedTextLast = new List<int>();
            this.selectedTextNew = new List<int>();
        }

        public void Clear()
        {
            this.selectedContainers = new List<object>();
            this.selectedTextLast = new List<int>();
            this.selectedTextNew = new List<int>();
            this.mainDocument.ClearAll();
            base.Invalidate();
        }

        private void Copy(object sender, EventArgs e)
        {
        }

        private void Cut(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.mainDocument.Dispose();
                this.contextMenuStrip1.Dispose();
            }
            base.Dispose(disposing);
        }

        public void DoOperate(object obj, Image stuImg, string stuInfo, string dataPath, int examLeftTime, int tipTime, string stdanswer)
        {
            if ((obj != null) && (obj is OperationInfo))
            {
                (obj as OperationInfo).Do(stuImg, stuInfo, dataPath, examLeftTime, tipTime, stdanswer);
            }
        }

        public void Draw()
        {
            Graphics g = base.CreateGraphics();
            this.mainDocument.Draw(g);
        }

        public void FillIn(string[] strs, string filepath, string id)
        {
            this.mainDocument.FillIn(strs, filepath, id);
            base.Invalidate();
        }

        ~SuperBox()
        {
            this.Dispose(false);
        }

        private void findClosestElementDown(lineexpression lexpr)
        {
            int num3;
            structexpression structexpression;
            float num4;
            float num5;
            float num6;
            int num = -1;
            float positiveInfinity = float.PositiveInfinity;
            for (num3 = 0; num3 < lexpr.Child.Count; num3++)
            {
                structexpression = lexpr.Child[num3];
                num4 = (this.virtualCaretX - structexpression.InputLocation.X) - structexpression.Region.Width;
                num5 = this.virtualCaretX - structexpression.InputLocation.X;
                num6 = ((num4 * num5) > 0f) ? Math.Abs((float) (num4 + num5)) : 0f;
                if (num6 < positiveInfinity)
                {
                    positiveInfinity = num6;
                    num = num3;
                }
            }
            if (num == -1)
            {
                this.caretContainer = lexpr;
                this.caretIndex = lexpr.Child.Count;
                this.UpdateCaretLocation();
            }
            else
            {
                structexpression = lexpr.Child[num];
                if ((structexpression.Child != null) && (structexpression.Child.Count != 0))
                {
                    lineexpression lineexpression = structexpression.Child[0];
                    for (num3 = 1; num3 < structexpression.Child.Count; num3++)
                    {
                        if (lineexpression.InputLocation.Y > structexpression.Child[num3].InputLocation.Y)
                        {
                            lineexpression = structexpression.Child[num3];
                        }
                        else if (lineexpression.InputLocation.Y == structexpression.Child[num3].InputLocation.Y)
                        {
                            float num7 = (this.virtualCaretX - lineexpression.InputLocation.X) - lineexpression.Region.Width;
                            float num8 = this.virtualCaretX - lineexpression.InputLocation.X;
                            float num9 = ((num7 * num8) > 0f) ? Math.Abs((float) (num7 + num8)) : 0f;
                            num4 = (this.virtualCaretX - structexpression.Child[num3].InputLocation.X) - structexpression.Child[num3].Region.Width;
                            num5 = this.virtualCaretX - structexpression.Child[num3].InputLocation.X;
                            num6 = ((num4 * num5) > 0f) ? Math.Abs((float) (num4 + num5)) : 0f;
                            if (num6 < num9)
                            {
                                lineexpression = structexpression.Child[num3];
                            }
                            else if ((num6 == num9) && (structexpression.Child[num3].Region.Width < lineexpression.Region.Width))
                            {
                                lineexpression = structexpression.Child[num3];
                            }
                        }
                    }
                    this.findClosestElementDown(lineexpression);
                }
                else
                {
                    this.caretContainer = lexpr;
                    if (((this.virtualCaretX - structexpression.InputLocation.X) / structexpression.Region.Width) > 0.5)
                    {
                        this.caretIndex = num + 1;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.caretIndex = num;
                        this.UpdateCaretLocation();
                    }
                }
            }
        }

        private void findClosestElementDown(Document document, int index)
        {
            Document document2;
            int num;
            float num2;
            float num3;
            float num4;
            float num5;
            float num6;
            float num7;
            Line line;
            int num8;
            float positiveInfinity;
            if (document.Elements[index] is PictureInfo)
            {
                PictureInfo info = document.Elements[index] as PictureInfo;
                if ((info.Documents != null) && (info.Documents.Count != 0))
                {
                    document2 = info.Documents[0];
                    for (num = 1; num < info.Documents.Count; num++)
                    {
                        if (document2.DocLocation.Y > info.Documents[num].DocLocation.Y)
                        {
                            document2 = info.Documents[num];
                        }
                        else if (document2.DocLocation.Y == info.Documents[num].DocLocation.Y)
                        {
                            num2 = (this.virtualCaretX - document2.DocLocation.X) - document2.OutWidth;
                            num3 = this.virtualCaretX - document2.DocLocation.X;
                            num4 = ((num2 * num3) > 0f) ? Math.Abs((float) (num2 + num3)) : 0f;
                            num5 = (this.virtualCaretX - info.Documents[num].DocLocation.X) - info.Documents[num].OutWidth;
                            num6 = this.virtualCaretX - info.Documents[num].DocLocation.X;
                            num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                            if (num7 < num4)
                            {
                                document2 = info.Documents[num];
                            }
                            else if ((num7 == num4) && (info.Documents[num].DocWidth < document2.DocWidth))
                            {
                                document2 = info.Documents[num];
                            }
                        }
                    }
                    line = document2.Lines[0];
                    num8 = -1;
                    positiveInfinity = float.PositiveInfinity;
                    for (num = line.StartIndex; num < (line.StartIndex + line.ElementCount); num++)
                    {
                        num5 = (this.virtualCaretX - document.Elements[num].OutLocation.X) - document.Elements[num].OutSize.Width;
                        num6 = this.virtualCaretX - document.Elements[num].OutLocation.X;
                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                        if (num7 < positiveInfinity)
                        {
                            positiveInfinity = num7;
                            num8 = num;
                        }
                    }
                    if (num8 == -1)
                    {
                        this.caretContainer = document2;
                        this.caretIndex = line.StartIndex + line.ElementCount;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.findClosestElementDown(document2, num8);
                    }
                }
                else
                {
                    this.caretContainer = document;
                    if (((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
                    {
                        this.caretIndex = index + 1;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.caretIndex = index;
                        this.UpdateCaretLocation();
                    }
                }
            }
            else if (document.Elements[index] is TableInfo)
            {
                TableInfo info2 = document.Elements[index] as TableInfo;
                if ((info2.Items != null) && (info2.Items.Count != 0))
                {
                    document2 = info2.Items[info2.Items.Count - 1];
                    for (num = 1; num < info2.Items.Count; num++)
                    {
                        if (document2.DocLocation.Y > info2.Items[num].DocLocation.Y)
                        {
                            document2 = info2.Items[num];
                        }
                        else if (document2.DocLocation.Y == info2.Items[num].DocLocation.Y)
                        {
                            num2 = (this.virtualCaretX - document2.DocLocation.X) - document2.OutWidth;
                            num3 = this.virtualCaretX - document2.DocLocation.X;
                            num4 = ((num2 * num3) > 0f) ? Math.Abs((float) (num2 + num3)) : 0f;
                            num5 = (this.virtualCaretX - info2.Items[num].DocLocation.X) - info2.Items[num].OutWidth;
                            num6 = this.virtualCaretX - info2.Items[num].DocLocation.X;
                            num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                            if (num7 < num4)
                            {
                                document2 = info2.Items[num];
                            }
                            else if ((num7 == num4) && (info2.Items[num].DocWidth < document2.DocWidth))
                            {
                                document2 = info2.Items[num];
                            }
                        }
                    }
                    line = document2.Lines[0];
                    num8 = -1;
                    positiveInfinity = float.PositiveInfinity;
                    for (num = line.StartIndex; num < (line.StartIndex + line.ElementCount); num++)
                    {
                        num5 = (this.virtualCaretX - document.Elements[num].OutLocation.X) - document.Elements[num].OutSize.Width;
                        num6 = this.virtualCaretX - document.Elements[num].OutLocation.X;
                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                        if (num7 < positiveInfinity)
                        {
                            positiveInfinity = num7;
                            num8 = num;
                        }
                    }
                    if (num8 == -1)
                    {
                        this.caretContainer = document2;
                        this.caretIndex = line.StartIndex + line.ElementCount;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.findClosestElementDown(document2, num8);
                    }
                }
                else
                {
                    this.caretContainer = document;
                    if (((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
                    {
                        this.caretIndex = index + 1;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.caretIndex = index;
                        this.UpdateCaretLocation();
                    }
                }
            }
            else if (document.Elements[index] is ExpressionInfo)
            {
                ExpressionInfo info3 = document.Elements[index] as ExpressionInfo;
                this.findClosestElementDown(info3.ContainerExpression);
            }
            else
            {
                this.caretContainer = document;
                if (((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
                {
                    this.caretIndex = index + 1;
                    this.UpdateCaretLocation();
                }
                else
                {
                    this.caretIndex = index;
                    this.UpdateCaretLocation();
                }
            }
        }

        private void findClosestElementUp(lineexpression lexpr)
        {
            int num3;
            structexpression structexpression;
            float num4;
            float num5;
            float num6;
            int num = -1;
            float positiveInfinity = float.PositiveInfinity;
            for (num3 = lexpr.Child.Count - 1; num3 >= 0; num3--)
            {
                structexpression = lexpr.Child[num3];
                num4 = (this.virtualCaretX - structexpression.InputLocation.X) - structexpression.Region.Width;
                num5 = this.virtualCaretX - structexpression.InputLocation.X;
                num6 = ((num4 * num5) > 0f) ? Math.Abs((float) (num4 + num5)) : 0f;
                if (num6 < positiveInfinity)
                {
                    positiveInfinity = num6;
                    num = num3;
                }
            }
            if (num == -1)
            {
                this.caretContainer = lexpr;
                this.caretIndex = lexpr.Child.Count;
                this.UpdateCaretLocation();
            }
            else
            {
                structexpression = lexpr.Child[num];
                if ((structexpression.Child != null) && (structexpression.Child.Count != 0))
                {
                    lineexpression lineexpression = structexpression.Child[structexpression.Child.Count - 1];
                    for (num3 = structexpression.Child.Count - 2; num3 >= 0; num3--)
                    {
                        if ((lineexpression.Region.Height + lineexpression.InputLocation.Y) < (structexpression.Child[num3].Region.Height + structexpression.Child[num3].InputLocation.Y))
                        {
                            lineexpression = structexpression.Child[num3];
                        }
                        else if ((lineexpression.Region.Height + lineexpression.InputLocation.Y) == (structexpression.Child[num3].Region.Height + structexpression.Child[num3].InputLocation.Y))
                        {
                            float num7 = (this.virtualCaretX - lineexpression.InputLocation.X) - lineexpression.Region.Width;
                            float num8 = this.virtualCaretX - lineexpression.InputLocation.X;
                            float num9 = ((num7 * num8) > 0f) ? Math.Abs((float) (num7 + num8)) : 0f;
                            num4 = (this.virtualCaretX - structexpression.Child[num3].InputLocation.X) - structexpression.Child[num3].Region.Width;
                            num5 = this.virtualCaretX - structexpression.Child[num3].InputLocation.X;
                            num6 = ((num4 * num5) > 0f) ? Math.Abs((float) (num4 + num5)) : 0f;
                            if (num6 < num9)
                            {
                                lineexpression = structexpression.Child[num3];
                            }
                            else if ((num6 == num9) && (structexpression.Child[num3].Region.Width < lineexpression.Region.Width))
                            {
                                lineexpression = structexpression.Child[num3];
                            }
                        }
                    }
                    this.findClosestElementUp(lineexpression);
                }
                else
                {
                    this.caretContainer = lexpr;
                    if (((this.virtualCaretX - structexpression.InputLocation.X) / structexpression.Region.Width) > 0.5)
                    {
                        this.caretIndex = num + 1;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.caretIndex = num;
                        this.UpdateCaretLocation();
                    }
                }
            }
        }

        private void findClosestElementUp(Document document, int index)
        {
            Document document2;
            int num;
            float num2;
            float num3;
            float num4;
            float num5;
            float num6;
            float num7;
            Line lastLine;
            int num8;
            float positiveInfinity;
            if (document.Elements[index] is PictureInfo)
            {
                PictureInfo info = document.Elements[index] as PictureInfo;
                if ((info.Documents != null) && (info.Documents.Count != 0))
                {
                    document2 = info.Documents[info.Documents.Count - 1];
                    for (num = info.Documents.Count - 2; num >= 0; num--)
                    {
                        if ((document2.OutHeight + document2.DocLocation.Y) < (info.Documents[num].OutHeight + info.Documents[num].DocLocation.Y))
                        {
                            document2 = info.Documents[num];
                        }
                        else if ((document2.OutHeight + document2.DocLocation.Y) == (info.Documents[num].OutHeight + info.Documents[num].DocLocation.Y))
                        {
                            num2 = (this.virtualCaretX - document2.DocLocation.X) - document2.OutWidth;
                            num3 = this.virtualCaretX - document2.DocLocation.X;
                            num4 = ((num2 * num3) > 0f) ? Math.Abs((float) (num2 + num3)) : 0f;
                            num5 = (this.virtualCaretX - info.Documents[num].DocLocation.X) - info.Documents[num].OutWidth;
                            num6 = this.virtualCaretX - info.Documents[num].DocLocation.X;
                            num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                            if (num7 < num4)
                            {
                                document2 = info.Documents[num];
                            }
                            else if ((num7 == num4) && (info.Documents[num].DocWidth < document2.DocWidth))
                            {
                                document2 = info.Documents[num];
                            }
                        }
                    }
                    lastLine = document2.LastLine;
                    num8 = -1;
                    positiveInfinity = float.PositiveInfinity;
                    for (num = (lastLine.StartIndex + lastLine.ElementCount) - 1; num >= lastLine.StartIndex; num--)
                    {
                        num5 = (this.virtualCaretX - document.Elements[num].OutLocation.X) - document.Elements[num].OutSize.Width;
                        num6 = this.virtualCaretX - document.Elements[num].OutLocation.X;
                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                        if (num7 < positiveInfinity)
                        {
                            positiveInfinity = num7;
                            num8 = num;
                        }
                    }
                    if (num8 == -1)
                    {
                        this.caretContainer = document2;
                        this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.findClosestElementUp(document2, num8);
                    }
                }
                else
                {
                    this.caretContainer = document;
                    if (((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
                    {
                        this.caretIndex = index + 1;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.caretIndex = index;
                        this.UpdateCaretLocation();
                    }
                }
            }
            else if (document.Elements[index] is TableInfo)
            {
                TableInfo info2 = document.Elements[index] as TableInfo;
                if ((info2.Items != null) && (info2.Items.Count != 0))
                {
                    document2 = info2.Items[info2.Items.Count - 1];
                    for (num = info2.Items.Count - 2; num >= 0; num--)
                    {
                        if ((document2.OutHeight + document2.DocLocation.Y) < (info2.Items[num].OutHeight + info2.Items[num].DocLocation.Y))
                        {
                            document2 = info2.Items[num];
                        }
                        else if ((document2.OutHeight + document2.DocLocation.Y) == (info2.Items[num].OutHeight + info2.Items[num].DocLocation.Y))
                        {
                            num2 = (this.virtualCaretX - document2.DocLocation.X) - document2.OutWidth;
                            num3 = this.virtualCaretX - document2.DocLocation.X;
                            num4 = ((num2 * num3) > 0f) ? Math.Abs((float) (num2 + num3)) : 0f;
                            num5 = (this.virtualCaretX - info2.Items[num].DocLocation.X) - info2.Items[num].OutWidth;
                            num6 = this.virtualCaretX - info2.Items[num].DocLocation.X;
                            num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                            if (num7 < num4)
                            {
                                document2 = info2.Items[num];
                            }
                            else if ((num7 == num4) && (info2.Items[num].DocWidth < document2.DocWidth))
                            {
                                document2 = info2.Items[num];
                            }
                        }
                    }
                    lastLine = document2.LastLine;
                    num8 = -1;
                    positiveInfinity = float.PositiveInfinity;
                    for (num = (lastLine.StartIndex + lastLine.ElementCount) - 1; num >= lastLine.StartIndex; num--)
                    {
                        num5 = (this.virtualCaretX - document.Elements[num].OutLocation.X) - document.Elements[num].OutSize.Width;
                        num6 = this.virtualCaretX - document.Elements[num].OutLocation.X;
                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                        if (num7 < positiveInfinity)
                        {
                            positiveInfinity = num7;
                            num8 = num;
                        }
                    }
                    if (num8 == -1)
                    {
                        this.caretContainer = document2;
                        this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.findClosestElementUp(document2, num8);
                    }
                }
                else
                {
                    this.caretContainer = document;
                    if (((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
                    {
                        this.caretIndex = index + 1;
                        this.UpdateCaretLocation();
                    }
                    else
                    {
                        this.caretIndex = index;
                        this.UpdateCaretLocation();
                    }
                }
            }
            else if (document.Elements[index] is ExpressionInfo)
            {
                ExpressionInfo info3 = document.Elements[index] as ExpressionInfo;
                this.findClosestElementUp(info3.ContainerExpression);
            }
            else
            {
                this.caretContainer = document;
                if (((this.virtualCaretX - document.Elements[index].OutLocation.X) / document.Elements[index].OutSize.Width) > 0.5)
                {
                    this.caretIndex = index + 1;
                    this.UpdateCaretLocation();
                }
                else
                {
                    this.caretIndex = index;
                    this.UpdateCaretLocation();
                }
            }
        }

        private void findDownLine(Document document)
        {
            if (document.Parent != null)
            {
                Document document2;
                Line line;
                int num2;
                float positiveInfinity;
                int num4;
                float num5;
                float num6;
                float num7;
                Document documentContainer;
                Line lineContainer;
                int num8;
                Line line3;
                int num9;
                if (document.Parent is TableInfo)
                {
                    TableInfo parent = document.Parent as TableInfo;
                    if ((parent.Items != null) && (parent.Items.Count > 1))
                    {
                        document2 = null;
                        int index = parent.Items.IndexOf(document as Cell);
                        if ((index + parent.TableSize.Y) < parent.Items.Count)
                        {
                            document2 = parent.Items[index + parent.TableSize.Y];
                        }
                        if (document2 != null)
                        {
                            line = document2.Lines[0];
                            num2 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = line.StartIndex; num4 < (line.StartIndex + line.ElementCount); num4++)
                            {
                                num5 = (this.virtualCaretX - document2.Elements[num4].OutLocation.X) - document2.Elements[num4].OutSize.Width;
                                num6 = this.virtualCaretX - document2.Elements[num4].OutLocation.X;
                                num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                if (num7 < positiveInfinity)
                                {
                                    positiveInfinity = num7;
                                    num2 = num4;
                                }
                            }
                            if (num2 == -1)
                            {
                                this.caretContainer = document2;
                                this.caretIndex = line.StartIndex + line.ElementCount;
                                this.UpdateCaretLocation();
                            }
                            else
                            {
                                this.findClosestElementDown(document2, num2);
                            }
                        }
                        else
                        {
                            documentContainer = parent.DocumentContainer;
                            lineContainer = parent.LineContainer;
                            num8 = documentContainer.Lines.IndexOf(lineContainer);
                            if (num8 < (documentContainer.Lines.Count - 1))
                            {
                                line3 = documentContainer.Lines[num8 + 1];
                                num9 = -1;
                                positiveInfinity = float.PositiveInfinity;
                                for (num4 = Math.Max(this.caretIndex + 1, line3.StartIndex); num4 < (line3.StartIndex + line3.StartIndex); num4++)
                                {
                                    if (!documentContainer.Elements[num4].InBlank)
                                    {
                                        num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                        num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                        if (num7 < positiveInfinity)
                                        {
                                            positiveInfinity = num7;
                                            num9 = num4;
                                        }
                                    }
                                }
                                if (num9 != -1)
                                {
                                    this.findClosestElementDown(documentContainer, num9);
                                }
                            }
                            else
                            {
                                this.findDownLine(documentContainer);
                            }
                        }
                    }
                    else
                    {
                        documentContainer = parent.DocumentContainer;
                        lineContainer = parent.LineContainer;
                        num8 = documentContainer.Lines.IndexOf(lineContainer);
                        if (num8 < (documentContainer.Lines.Count - 1))
                        {
                            line3 = documentContainer.Lines[num8 + 1];
                            num9 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = Math.Max(this.caretIndex + 1, line3.StartIndex); num4 < (line3.StartIndex + line3.StartIndex); num4++)
                            {
                                if (!documentContainer.Elements[num4].InBlank)
                                {
                                    num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                    num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                    num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                    if (num7 < positiveInfinity)
                                    {
                                        positiveInfinity = num7;
                                        num9 = num4;
                                    }
                                }
                            }
                            if (num9 != -1)
                            {
                                this.findClosestElementDown(documentContainer, num9);
                            }
                        }
                        else
                        {
                            this.findDownLine(documentContainer);
                        }
                    }
                }
                else if (document.Parent is PictureInfo)
                {
                    PictureInfo info2 = document.Parent as PictureInfo;
                    if ((info2.Documents != null) && (info2.Documents.Count > 1))
                    {
                        document2 = null;
                        float y = float.PositiveInfinity;
                        for (num4 = 0; num4 < info2.Documents.Count; num4++)
                        {
                            if ((document != info2.Documents[num4]) && (document.DocLocation.Y < info2.Documents[num4].DocLocation.Y))
                            {
                                if (y > info2.Documents[num4].DocLocation.Y)
                                {
                                    document2 = info2.Documents[num4];
                                    y = info2.Documents[num4].DocLocation.Y;
                                }
                                else if (y == info2.Documents[num4].DocLocation.Y)
                                {
                                    float num11 = (this.virtualCaretX - document2.DocLocation.X) - document2.OutWidth;
                                    float num12 = this.virtualCaretX - document2.DocLocation.X;
                                    float num13 = ((num11 * num12) > 0f) ? Math.Abs((float) (num11 + num12)) : 0f;
                                    num5 = (this.virtualCaretX - info2.Documents[num4].DocLocation.X) - info2.Documents[num4].OutWidth;
                                    num6 = this.virtualCaretX - info2.Documents[num4].DocLocation.X;
                                    num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                    if (num7 < num13)
                                    {
                                        document2 = info2.Documents[num4];
                                    }
                                    else if ((num7 == num13) && (info2.Documents[num4].DocWidth < document2.DocWidth))
                                    {
                                        document2 = info2.Documents[num4];
                                    }
                                }
                            }
                        }
                        if (document2 != null)
                        {
                            line = document2.Lines[0];
                            num2 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = line.StartIndex; num4 < (line.StartIndex + line.ElementCount); num4++)
                            {
                                num5 = (this.virtualCaretX - document2.Elements[num4].OutLocation.X) - document2.Elements[num4].OutSize.Width;
                                num6 = this.virtualCaretX - document2.Elements[num4].OutLocation.X;
                                num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                if (num7 < positiveInfinity)
                                {
                                    positiveInfinity = num7;
                                    num2 = num4;
                                }
                            }
                            if (num2 == -1)
                            {
                                this.caretContainer = document2;
                                this.caretIndex = line.StartIndex + line.ElementCount;
                                this.UpdateCaretLocation();
                            }
                            else
                            {
                                this.findClosestElementDown(document2, num2);
                            }
                        }
                        else
                        {
                            documentContainer = info2.DocumentContainer;
                            lineContainer = info2.LineContainer;
                            num8 = documentContainer.Lines.IndexOf(lineContainer);
                            if (num8 < (documentContainer.Lines.Count - 1))
                            {
                                line3 = documentContainer.Lines[num8 + 1];
                                num9 = -1;
                                positiveInfinity = float.PositiveInfinity;
                                for (num4 = Math.Max(this.caretIndex + 1, line3.StartIndex); num4 < (line3.StartIndex + line3.ElementCount); num4++)
                                {
                                    if (!documentContainer.Elements[num4].InBlank)
                                    {
                                        num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                        num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                        if (num7 < positiveInfinity)
                                        {
                                            positiveInfinity = num7;
                                            num9 = num4;
                                        }
                                    }
                                }
                                if (num9 != -1)
                                {
                                    this.findClosestElementDown(documentContainer, num9);
                                }
                            }
                            else
                            {
                                this.findDownLine(documentContainer);
                            }
                        }
                    }
                    else
                    {
                        documentContainer = info2.DocumentContainer;
                        lineContainer = info2.LineContainer;
                        num8 = documentContainer.Lines.IndexOf(lineContainer);
                        if (num8 < (documentContainer.Lines.Count - 1))
                        {
                            line3 = documentContainer.Lines[num8 + 1];
                            num9 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = Math.Max(this.caretIndex + 1, line3.StartIndex); num4 < (line3.StartIndex + line3.ElementCount); num4++)
                            {
                                if (!documentContainer.Elements[num4].InBlank)
                                {
                                    num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                    num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                    num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                    if (num7 < positiveInfinity)
                                    {
                                        positiveInfinity = num7;
                                        num9 = num4;
                                    }
                                }
                            }
                            if (num9 != -1)
                            {
                                this.findClosestElementDown(documentContainer, num9);
                            }
                        }
                        else
                        {
                            this.findDownLine(documentContainer);
                        }
                    }
                }
            }
        }

        private void FindMouse(Document document, Point mousePosition, bool ignoreMouseState = false)
        {
            int num = 0;
            while (!this.freshed && (num < 5))
            {
                Thread.Sleep(50);
            }
            if (num < 5)
            {
                bool flag = false;
                if (this.ReadOnly)
                {
                    if (this.PointInBlank(document, mousePosition, ignoreMouseState))
                    {
                        flag = true;
                    }
                    else if (this.PointInOptions(document, mousePosition, ignoreMouseState) || this.PointInOperation(document, mousePosition, ignoreMouseState))
                    {
                        return;
                    }
                }
                else if ((this.PointInBlank(document, mousePosition, ignoreMouseState) || this.PointInOptions(document, mousePosition, ignoreMouseState)) || this.PointInOperation(document, mousePosition, ignoreMouseState))
                {
                    return;
                }
                if (!this.ReadOnly || flag)
                {
                    Region region;
                    foreach (Element element in document.Elements)
                    {
                        if ((element is ExpressionInfo) && (element as ExpressionInfo).Region.IsVisible(mousePosition))
                        {
                            ExpressionInfo info = element as ExpressionInfo;
                            expression expression = info.ContainerExpression.PointInExpression(mousePosition);
                            if (expression == null)
                            {
                                if (Math.Abs((float) (mousePosition.X - info.OutLocation.X)) < Math.Abs((float) ((mousePosition.X - info.OutLocation.X) - info.OutSize.Width)))
                                {
                                    this.mouseIndex = info.Index;
                                }
                                else
                                {
                                    this.mouseIndex = info.Index + 1;
                                }
                                this.mouseContainer = info.DocumentContainer;
                            }
                            else if (expression is lineexpression)
                            {
                                lineexpression lineexpression = expression as lineexpression;
                                this.mouseIndex = 0f;
                                for (int i = 0; i < lineexpression.Child.Count; i++)
                                {
                                    if ((mousePosition.X >= lineexpression.Child[i].InputLocation.X) && (mousePosition.X <= (lineexpression.Child[i].InputLocation.X + lineexpression.Child[i].Region.Width)))
                                    {
                                        this.mouseIndex = i + ((mousePosition.X - lineexpression.Child[i].InputLocation.X) / lineexpression.Child[i].Region.Width);
                                        break;
                                    }
                                }
                                this.mouseContainer = lineexpression;
                            }
                            else
                            {
                                structexpression item = expression as structexpression;
                                this.mouseIndex = item.ParentExpression.Child.IndexOf(item) + (Math.Abs((float) (mousePosition.X - item.InputLocation.X)) / item.Region.Width);
                                this.mouseContainer = item.ParentExpression;
                            }
                            if (!ignoreMouseState)
                            {
                                this.mouseState = MouseState.Select;
                            }
                            return;
                        }
                        if ((element is TableInfo) && (element as TableInfo).Region.IsVisible(mousePosition))
                        {
                            Cell cell = this.PointInTable(mousePosition, element as TableInfo, ignoreMouseState);
                            if (cell != null)
                            {
                                this.FindMouse(cell, mousePosition, ignoreMouseState);
                            }
                            return;
                        }
                        if ((element is PictureInfo) && (element as PictureInfo).Region.IsVisible(mousePosition))
                        {
                            PictureInfo info2 = element as PictureInfo;
                            if (info2.AloneSelected)
                            {
                                this.mouseContainer = info2;
                                if ((mousePosition.Y < (info2.Location.Y + (info2.Margin.Top * 2))) && (mousePosition.Y >= info2.Location.Y))
                                {
                                    if (!ignoreMouseState)
                                    {
                                        this.mouseState = MouseState.DragBoundNS;
                                        this.boundDragedObject = info2;
                                        this.dragBound = Bound.Top;
                                    }
                                }
                                else if ((mousePosition.Y < (info2.Location.Y + info2.Size.Height)) && (mousePosition.Y >= ((info2.Location.Y + info2.Size.Height) - (info2.Margin.Bottom * 2))))
                                {
                                    if (!ignoreMouseState)
                                    {
                                        this.mouseState = MouseState.DragBoundNS;
                                        this.boundDragedObject = info2;
                                        this.dragBound = Bound.Bottom;
                                    }
                                }
                                else if ((mousePosition.X < (info2.Location.X + (info2.Margin.Left * 2))) && (mousePosition.X >= info2.Location.X))
                                {
                                    if (!ignoreMouseState)
                                    {
                                        this.mouseState = MouseState.DragBoundWE;
                                        this.boundDragedObject = info2;
                                        this.dragBound = Bound.Left;
                                    }
                                }
                                else if ((mousePosition.X < (info2.Location.X + info2.Size.Width)) && (mousePosition.X >= ((info2.Location.X + info2.Size.Width) - (info2.Margin.Right * 2))))
                                {
                                    if (!ignoreMouseState)
                                    {
                                        this.mouseState = MouseState.DragBoundWE;
                                        this.boundDragedObject = info2;
                                        this.dragBound = Bound.Right;
                                    }
                                }
                                else if (!ignoreMouseState)
                                {
                                    this.mouseState = MouseState.AboveImage;
                                }
                            }
                            else
                            {
                                foreach (Document document2 in info2.Documents)
                                {
                                    region = new Region(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
                                    if (region.IsVisible(mousePosition))
                                    {
                                        this.FindMouse(document2, mousePosition, ignoreMouseState);
                                        return;
                                    }
                                }
                                if (!ignoreMouseState)
                                {
                                    this.mouseState = MouseState.AboveImage;
                                    this.mouseContainer = info2;
                                }
                            }
                            return;
                        }
                    }
                    this.mouseContainer = document;
                    if (!ignoreMouseState)
                    {
                        this.mouseState = MouseState.Select;
                    }
                    if (document.Elements.Count == 0)
                    {
                        this.mouseIndex = 0f;
                    }
                    else if ((document.Lines.Count == 0) || (mousePosition.Y < (document.DocLocation.Y + document.Margin.Top)))
                    {
                        this.mouseIndex = 0f;
                    }
                    else if ((mousePosition.Y > (document.Lines[document.Lines.Count - 1].Top + document.Lines[document.Lines.Count - 1].Height)) || (mousePosition.Y > ((document.DocLocation.Y + document.OutHeight) - document.Margin.Bottom)))
                    {
                        this.mouseIndex = document.Elements.Count;
                    }
                    else
                    {
                        foreach (Line line in document.Lines)
                        {
                            int index = document.Lines.IndexOf(line);
                            if (index < (document.Lines.Count - 1))
                            {
                                Line line2 = document.Lines[index + 1];
                                region = new Region(new RectangleF(document.DocLocation.X, line.Top, document.OutWidth, line2.Top - line.Top));
                            }
                            else
                            {
                                region = new Region(new RectangleF(document.DocLocation.X, line.Top, document.OutWidth, (document.DocLocation.Y + document.OutHeight) - line.Top));
                            }
                            if (region.IsVisible(mousePosition))
                            {
                                int startIndex = line.StartIndex;
                                while (startIndex < (line.StartIndex + line.ElementCount))
                                {
                                    if (document.Elements[startIndex].Region.IsVisible(mousePosition))
                                    {
                                        this.mouseIndex = startIndex + ((mousePosition.X - document.Elements[startIndex].Location.X) / document.Elements[startIndex].OutSize.Width);
                                        break;
                                    }
                                    startIndex++;
                                }
                                if ((line.ElementCount > 0) && (mousePosition.X <= document.Elements[line.StartIndex].OutLocation.X))
                                {
                                    this.mouseIndex = line.StartIndex;
                                    break;
                                }
                                if (line.ElementCount == 0)
                                {
                                    this.mouseIndex = line.StartIndex;
                                    break;
                                }
                                for (startIndex = line.StartIndex; startIndex < (line.StartIndex + line.ElementCount); startIndex++)
                                {
                                    if (mousePosition.X < document.Elements[startIndex].OutLocation.X)
                                    {
                                        this.mouseIndex = startIndex;
                                        break;
                                    }
                                }
                                this.mouseIndex = line.StartIndex + line.ElementCount;
                            }
                        }
                    }
                }
                else
                {
                    this.mouseState = MouseState.None;
                }
            }
        }

        private void findUpperLine(Document document)
        {
            if (document.Parent != null)
            {
                Document document2;
                Line lastLine;
                int num2;
                float positiveInfinity;
                int num4;
                float num5;
                float num6;
                float num7;
                Document documentContainer;
                Line lineContainer;
                int num8;
                Line line3;
                int num9;
                if (document.Parent is TableInfo)
                {
                    TableInfo parent = document.Parent as TableInfo;
                    if ((parent.Items != null) && (parent.Items.Count > 1))
                    {
                        document2 = null;
                        int index = parent.Items.IndexOf(document as Cell);
                        if ((index - parent.TableSize.Y) >= 0)
                        {
                            document2 = parent.Items[index - parent.TableSize.Y];
                        }
                        if (document2 != null)
                        {
                            lastLine = document2.LastLine;
                            num2 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = (lastLine.StartIndex + lastLine.ElementCount) - 1; num4 >= lastLine.StartIndex; num4--)
                            {
                                num5 = (this.virtualCaretX - document2.Elements[num4].OutLocation.X) - document2.Elements[num4].OutSize.Width;
                                num6 = this.virtualCaretX - document2.Elements[num4].OutLocation.X;
                                num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                if (num7 < positiveInfinity)
                                {
                                    positiveInfinity = num7;
                                    num2 = num4;
                                }
                            }
                            if (num2 == -1)
                            {
                                this.caretContainer = document2;
                                this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
                                this.UpdateCaretLocation();
                            }
                            else
                            {
                                this.findClosestElementUp(document2, num2);
                            }
                        }
                        else
                        {
                            documentContainer = parent.DocumentContainer;
                            lineContainer = parent.LineContainer;
                            num8 = documentContainer.Lines.IndexOf(lineContainer);
                            if (num8 > 0)
                            {
                                line3 = documentContainer.Lines[num8 - 1];
                                num9 = -1;
                                positiveInfinity = float.PositiveInfinity;
                                for (num4 = Math.Min((int) (this.caretIndex - 1), (int) ((line3.ElementCount + line3.StartIndex) - 1)); num4 >= line3.StartIndex; num4--)
                                {
                                    if (!documentContainer.Elements[num4].InBlank)
                                    {
                                        num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                        num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                        if (num7 < positiveInfinity)
                                        {
                                            positiveInfinity = num7;
                                            num9 = num4;
                                        }
                                    }
                                }
                                if (num9 != -1)
                                {
                                    this.findClosestElementUp(documentContainer, num9);
                                }
                            }
                            else
                            {
                                this.findUpperLine(documentContainer);
                            }
                        }
                    }
                    else
                    {
                        documentContainer = parent.DocumentContainer;
                        lineContainer = parent.LineContainer;
                        num8 = documentContainer.Lines.IndexOf(lineContainer);
                        if (num8 > 0)
                        {
                            line3 = documentContainer.Lines[num8 - 1];
                            num9 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = Math.Min((int) (this.caretIndex - 1), (int) ((line3.ElementCount + line3.StartIndex) - 1)); num4 >= line3.StartIndex; num4--)
                            {
                                if (!documentContainer.Elements[num4].InBlank)
                                {
                                    num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                    num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                    num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                    if (num7 < positiveInfinity)
                                    {
                                        positiveInfinity = num7;
                                        num9 = num4;
                                    }
                                }
                            }
                            if (num9 != -1)
                            {
                                this.findClosestElementUp(documentContainer, num9);
                            }
                        }
                        else
                        {
                            this.findUpperLine(documentContainer);
                        }
                    }
                }
                else if (document.Parent is PictureInfo)
                {
                    PictureInfo info2 = document.Parent as PictureInfo;
                    if ((info2.Documents != null) && (info2.Documents.Count > 1))
                    {
                        document2 = null;
                        float negativeInfinity = float.NegativeInfinity;
                        for (num4 = info2.Documents.Count - 1; num4 >= 0; num4--)
                        {
                            if ((document != info2.Documents[num4]) && ((document.OutHeight + document.DocLocation.Y) > (info2.Documents[num4].OutHeight + info2.Documents[num4].DocLocation.Y)))
                            {
                                if (negativeInfinity < (info2.Documents[num4].OutHeight + info2.Documents[num4].DocLocation.Y))
                                {
                                    document2 = info2.Documents[num4];
                                    negativeInfinity = info2.Documents[num4].OutHeight + info2.Documents[num4].DocLocation.Y;
                                }
                                else if (negativeInfinity == (info2.Documents[num4].OutHeight + info2.Documents[num4].DocLocation.Y))
                                {
                                    float num11 = (this.virtualCaretX - document2.DocLocation.X) - document2.OutWidth;
                                    float num12 = this.virtualCaretX - document2.DocLocation.X;
                                    float num13 = ((num11 * num12) > 0f) ? Math.Abs((float) (num11 + num12)) : 0f;
                                    num5 = (this.virtualCaretX - info2.Documents[num4].DocLocation.X) - info2.Documents[num4].OutWidth;
                                    num6 = this.virtualCaretX - info2.Documents[num4].DocLocation.X;
                                    num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                    if (num7 < num13)
                                    {
                                        document2 = info2.Documents[num4];
                                    }
                                    else if ((num7 == num13) && (info2.Documents[num4].DocWidth < document2.DocWidth))
                                    {
                                        document2 = info2.Documents[num4];
                                    }
                                }
                            }
                        }
                        if (document2 != null)
                        {
                            lastLine = document2.LastLine;
                            num2 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = (lastLine.StartIndex + lastLine.ElementCount) - 1; num4 >= lastLine.StartIndex; num4--)
                            {
                                num5 = (this.virtualCaretX - document2.Elements[num4].OutLocation.X) - document2.Elements[num4].OutSize.Width;
                                num6 = this.virtualCaretX - document2.Elements[num4].OutLocation.X;
                                num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                if (num7 < positiveInfinity)
                                {
                                    positiveInfinity = num7;
                                    num2 = num4;
                                }
                            }
                            if (num2 == -1)
                            {
                                this.caretContainer = document2;
                                this.caretIndex = lastLine.StartIndex + lastLine.ElementCount;
                                this.UpdateCaretLocation();
                            }
                            else
                            {
                                this.findClosestElementUp(document2, num2);
                            }
                        }
                        else
                        {
                            documentContainer = info2.DocumentContainer;
                            lineContainer = info2.LineContainer;
                            num8 = documentContainer.Lines.IndexOf(lineContainer);
                            if (num8 > 0)
                            {
                                line3 = documentContainer.Lines[num8 - 1];
                                num9 = -1;
                                positiveInfinity = float.PositiveInfinity;
                                for (num4 = Math.Min((int) (this.caretIndex - 1), (int) ((line3.ElementCount + line3.StartIndex) - 1)); num4 >= line3.StartIndex; num4--)
                                {
                                    if (!documentContainer.Elements[num4].InBlank)
                                    {
                                        num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                        num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                        num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                        if (num7 < positiveInfinity)
                                        {
                                            positiveInfinity = num7;
                                            num9 = num4;
                                        }
                                    }
                                }
                                if (num9 != -1)
                                {
                                    this.findClosestElementUp(documentContainer, num9);
                                }
                            }
                            else
                            {
                                this.findUpperLine(documentContainer);
                            }
                        }
                    }
                    else
                    {
                        documentContainer = info2.DocumentContainer;
                        lineContainer = info2.LineContainer;
                        num8 = documentContainer.Lines.IndexOf(lineContainer);
                        if (num8 > 0)
                        {
                            line3 = documentContainer.Lines[num8 - 1];
                            num9 = -1;
                            positiveInfinity = float.PositiveInfinity;
                            for (num4 = Math.Min((int) (this.caretIndex - 1), (int) ((line3.ElementCount + line3.StartIndex) - 1)); num4 >= line3.StartIndex; num4--)
                            {
                                if (!documentContainer.Elements[num4].InBlank)
                                {
                                    num5 = (this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X) - documentContainer.Elements[num4].OutSize.Width;
                                    num6 = this.virtualCaretX - documentContainer.Elements[num4].OutLocation.X;
                                    num7 = ((num5 * num6) > 0f) ? Math.Abs((float) (num5 + num6)) : 0f;
                                    if (num7 < positiveInfinity)
                                    {
                                        positiveInfinity = num7;
                                        num9 = num4;
                                    }
                                }
                            }
                            if (num9 != -1)
                            {
                                this.findClosestElementUp(documentContainer, num9);
                            }
                        }
                        else
                        {
                            this.findUpperLine(documentContainer);
                        }
                    }
                }
            }
        }

        public string getContent()
        {
            return this.mainDocument.getContent();
        }

        public string getContentXml()
        {
            return this.mainDocument.getContentXml();
        }

        private void getNextBlank(Document document, int index, bool startFromBlank)
        {
            PictureInfo parent;
            TableInfo info2;
            if (!startFromBlank)
            {
                foreach (Blank blank in document.Blanks)
                {
                    if (blank.StartIndex == index)
                    {
                        this.caretContainer = document;
                        this.caretIndex = index;
                        this.UpdateCaretLocation();
                        return;
                    }
                }
            }
            if (index < document.Elements.Count)
            {
                if (document.Elements[index] is PictureInfo)
                {
                    parent = document.Elements[index] as PictureInfo;
                    if (parent.Documents != null)
                    {
                        this.getNextBlank(parent.Documents[0], 0, false);
                    }
                    else
                    {
                        this.getNextBlank(document, index + 1, false);
                    }
                }
                else if (document.Elements[index] is TableInfo)
                {
                    info2 = document.Elements[index] as TableInfo;
                    if (info2.Items != null)
                    {
                        this.getNextBlank(info2.Items[0], 0, false);
                    }
                    else
                    {
                        this.getNextBlank(document, index + 1, false);
                    }
                }
                else
                {
                    this.getNextBlank(document, index + 1, false);
                }
            }
            else if (document.Parent != null)
            {
                int num;
                if (document.Parent is PictureInfo)
                {
                    parent = document.Parent as PictureInfo;
                    num = parent.Documents.IndexOf(document);
                    if ((num + 1) < parent.Documents.Count)
                    {
                        this.getNextBlank(parent.Documents[num + 1], 0, false);
                    }
                    else
                    {
                        this.getNextBlank(document.Parent.DocumentContainer, document.Parent.Index + 1, false);
                    }
                }
                else if (document.Parent is TableInfo)
                {
                    info2 = document.Parent as TableInfo;
                    num = info2.Items.IndexOf(document as Cell);
                    if ((num + 1) < info2.Items.Count)
                    {
                        this.getNextBlank(info2.Items[num + 1], 0, false);
                    }
                    else
                    {
                        this.getNextBlank(document.Parent.DocumentContainer, document.Parent.Index + 1, false);
                    }
                }
                else
                {
                    this.getNextBlank(document.Parent.DocumentContainer, document.Parent.Index + 1, false);
                }
            }
        }

        private void getPrevBlank(Document document, int index, bool startFromBlank)
        {
            PictureInfo parent;
            TableInfo info2;
            if (!startFromBlank)
            {
                foreach (Blank blank in document.Blanks)
                {
                    if ((blank.StartIndex + blank.Count) == index)
                    {
                        this.caretContainer = document;
                        this.caretIndex = index;
                        this.UpdateCaretLocation();
                        return;
                    }
                }
            }
            if (index > 0)
            {
                if (document.Elements[index - 1] is PictureInfo)
                {
                    parent = document.Elements[index - 1] as PictureInfo;
                    if (parent.Documents != null)
                    {
                        this.getPrevBlank(parent.Documents[parent.Documents.Count - 1], parent.Documents[parent.Documents.Count - 1].Elements.Count, false);
                    }
                    else
                    {
                        this.getPrevBlank(document, index - 1, false);
                    }
                }
                else if (document.Elements[index - 1] is TableInfo)
                {
                    info2 = document.Elements[index - 1] as TableInfo;
                    if (info2.Items != null)
                    {
                        this.getPrevBlank(info2.Items[info2.Items.Count - 1], info2.Items[info2.Items.Count - 1].Elements.Count, false);
                    }
                    else
                    {
                        this.getPrevBlank(document, index - 1, false);
                    }
                }
                else
                {
                    this.getPrevBlank(document, index - 1, false);
                }
            }
            else if (document.Parent != null)
            {
                int num;
                if (document.Parent is PictureInfo)
                {
                    parent = document.Parent as PictureInfo;
                    num = parent.Documents.IndexOf(document);
                    if ((num - 1) >= 0)
                    {
                        this.getPrevBlank(parent.Documents[num - 1], parent.Documents[num - 1].Elements.Count, false);
                    }
                    else
                    {
                        this.getPrevBlank(document.Parent.DocumentContainer, document.Parent.Index, false);
                    }
                }
                else if (document.Parent is TableInfo)
                {
                    info2 = document.Parent as TableInfo;
                    num = info2.Items.IndexOf(document as Cell);
                    if ((num - 1) >= 0)
                    {
                        this.getPrevBlank(info2.Items[num - 1], info2.Items[num - 1].Elements.Count, false);
                    }
                    else
                    {
                        this.getPrevBlank(document.Parent.DocumentContainer, document.Parent.Index, false);
                    }
                }
                else
                {
                    this.getPrevBlank(document.Parent.DocumentContainer, document.Parent.Index, false);
                }
            }
        }

        private void Insert()
        {
            Document caretContainer;
            int num;
            int num2;
            containerexpression containerexpression;
            lineexpression parentExpression;
            ExpressionInfo info;
            Document document2;
            bool flag = false;
            this.freshed = false;
            while (this.inputExpressions.Count != 0)
            {
                structexpression structexpression;
                flag = true;
                FType type = this.inputExpressions.Peek();
                if (this.caretContainer is Document)
                {
                    caretContainer = this.caretContainer as Document;
                    if ((this.selectedContainers.Count > 0) && (this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer))
                    {
                        num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                        num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                        this.caretIndex = num + caretContainer.DeleteElement(num, num2 - num, this.ReadOnly);
                        this.selectedContainers = new List<object>();
                        this.selectedTextLast = new List<int>();
                        this.selectedTextNew = new List<int>();
                    }
                    containerexpression = new containerexpression(Qisi.Editor.CommonMethods.GetCambriaFont(this.Font.Size, FontStyle.Regular)) {
                        Child = new List<structexpression>()
                    };
                    structexpression = Qisi.Editor.CommonMethods.CreateExpr(type.ToString(), containerexpression, this.ForeColor, "", 2, 1);
                    containerexpression.Child.Add(structexpression);
                    if (caretContainer.InsertExpression(this.caretIndex, containerexpression, this.Font, this.ReadOnly))
                    {
                        if (structexpression.DefaultChild != null)
                        {
                            this.caretContainer = structexpression.DefaultChild;
                            this.caretIndex = 0;
                        }
                        else
                        {
                            this.caretContainer = containerexpression;
                            this.caretIndex = 1;
                        }
                    }
                }
                else if (this.caretContainer is lineexpression)
                {
                    parentExpression = this.caretContainer as lineexpression;
                    while (!(parentExpression is containerexpression))
                    {
                        parentExpression = parentExpression.ParentExpression.ParentExpression;
                    }
                    containerexpression = parentExpression as containerexpression;
                    info = containerexpression.Info;
                    info.Sized = false;
                    parentExpression = this.caretContainer as lineexpression;
                    if ((this.ReadOnly && info.InBlank) || !this.ReadOnly)
                    {
                        if ((this.selectedContainers.Count > 0) && (this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer))
                        {
                            num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                            num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                            if (parentExpression.Child != null)
                            {
                                parentExpression.Child.RemoveRange(num, num2 - num);
                            }
                            this.caretIndex = num;
                            this.selectedContainers = new List<object>();
                            this.selectedTextLast = new List<int>();
                            this.selectedTextNew = new List<int>();
                        }
                        structexpression = Qisi.Editor.CommonMethods.CreateExpr(type.ToString(), parentExpression, this.ForeColor, "", 2, 1);
                        if (parentExpression.Child == null)
                        {
                            parentExpression.Child = new List<structexpression>();
                            parentExpression.Child.Insert(0, structexpression);
                        }
                        else
                        {
                            parentExpression.Child.Insert(this.caretIndex, structexpression);
                        }
                        if (structexpression.DefaultChild != null)
                        {
                            this.caretContainer = structexpression.DefaultChild;
                            this.caretIndex = 0;
                        }
                        else
                        {
                            this.caretIndex++;
                        }
                    }
                }
                this.inputExpressions.Dequeue();
            }
            while (this.inputChars.Count != 0)
            {
                flag = true;
                char chr = this.inputChars.Peek();
                if (chr == '\b')
                {
                    if ((this.caretContainer is Document) && (this.caretIndex > 0))
                    {
                        caretContainer = this.caretContainer as Document;
                        if ((this.selectedContainers.Count > 0) && (this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer))
                        {
                            num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                            num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                            if (num == num2)
                            {
                                if (caretContainer.DeleteElement(this.caretIndex - 1, this.ReadOnly))
                                {
                                    this.caretIndex--;
                                }
                            }
                            else
                            {
                                this.caretIndex = num + caretContainer.DeleteElement(num, num2 - num, this.ReadOnly);
                            }
                            this.selectedContainers = new List<object>();
                            this.selectedTextLast = new List<int>();
                            this.selectedTextNew = new List<int>();
                        }
                        else if ((this.selectedContainers.Count == 0) && caretContainer.DeleteElement(this.caretIndex - 1, this.ReadOnly))
                        {
                            this.caretIndex--;
                        }
                    }
                    else if ((this.caretContainer is lineexpression) && (this.caretIndex > 0))
                    {
                        parentExpression = this.caretContainer as lineexpression;
                        while (!(parentExpression is containerexpression))
                        {
                            parentExpression = parentExpression.ParentExpression.ParentExpression;
                        }
                        containerexpression = parentExpression as containerexpression;
                        info = containerexpression.Info;
                        info.Sized = false;
                        parentExpression = this.caretContainer as lineexpression;
                        if (info.InBlank || !this.ReadOnly)
                        {
                            if ((this.selectedContainers.Count > 0) && (this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer))
                            {
                                num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                                num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                                if (num == num2)
                                {
                                    if (parentExpression.Child != null)
                                    {
                                        parentExpression.Child.RemoveAt(this.caretIndex - 1);
                                    }
                                    if (this.caretIndex > 0)
                                    {
                                        this.caretIndex--;
                                    }
                                }
                                else
                                {
                                    if (parentExpression.Child != null)
                                    {
                                        parentExpression.Child.RemoveRange(num, num2 - num);
                                    }
                                    this.caretIndex = num;
                                }
                                this.selectedContainers = new List<object>();
                                this.selectedTextLast = new List<int>();
                                this.selectedTextNew = new List<int>();
                            }
                            else if (this.selectedContainers.Count == 0)
                            {
                                if (parentExpression.Child != null)
                                {
                                    parentExpression.Child.RemoveAt(this.caretIndex - 1);
                                }
                                if (this.caretIndex > 0)
                                {
                                    this.caretIndex--;
                                }
                            }
                        }
                    }
                }
                else
                {
                    lineexpression lineexpression2;
                    if (chr == '\x007f')
                    {
                        ExpressionInfo info2;
                        if ((this.selectedContainers.Count == 0) || ((this.selectedContainers.Count == 1) && (this.selectedTextLast[0] == this.selectedTextNew[0])))
                        {
                            if (this.caretContainer is Document)
                            {
                                caretContainer = this.caretContainer as Document;
                                if (this.caretIndex < caretContainer.Elements.Count)
                                {
                                    caretContainer.DeleteElement(this.caretIndex, this.ReadOnly);
                                }
                            }
                            else if (this.caretContainer is lineexpression)
                            {
                                parentExpression = this.caretContainer as lineexpression;
                                while (!(parentExpression is containerexpression))
                                {
                                    parentExpression = parentExpression.ParentExpression.ParentExpression;
                                }
                                info2 = (parentExpression as containerexpression).Info;
                                info2.Sized = false;
                                if (info2.InBlank || !this.ReadOnly)
                                {
                                    parentExpression = this.caretContainer as lineexpression;
                                    if ((parentExpression.Child != null) && (this.caretIndex < parentExpression.Child.Count))
                                    {
                                        parentExpression.Child.RemoveAt(this.caretIndex);
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = this.selectedContainers.Count - 1; i >= 0; i--)
                            {
                                int num5;
                                List<int> list;
                                int num6;
                                num = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
                                num2 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
                                if (this.selectedContainers[i] is Document)
                                {
                                    caretContainer = this.selectedContainers[i] as Document;
                                    int num4 = caretContainer.DeleteElement(num, num2 - num, this.ReadOnly);
                                    num5 = 0;
                                    while (num5 < i)
                                    {
                                        if (this.selectedContainers[num5] == this.selectedContainers[i])
                                        {
                                            if (this.selectedTextLast[num5] >= num2)
                                            {
												(list = this.selectedTextLast)[num6 = num5] = list[num6] - ((num2 - num) - num4); // Why! Look at line 1835!
                                            }
                                            else if (this.selectedTextLast[num5] >= num)
                                            {
                                                this.selectedTextLast[num5] = num;
                                            }
                                            if (this.selectedTextNew[num5] >= num2)
                                            {
                                                (list = this.selectedTextNew)[num6 = num5] = list[num6] - ((num2 - num) - num4);
                                            }
                                            else if (this.selectedTextNew[num5] >= num)
                                            {
                                                this.selectedTextNew[num5] = num;
                                            }
                                        }
                                        num5++;
                                    }
                                    this.caretContainer = caretContainer;
                                    this.caretIndex = num + num4;
                                }
                                else if (this.selectedContainers[i] is TableInfo)
                                {
                                    TableInfo info3 = this.selectedContainers[i] as TableInfo;
                                    if (info3.InBlank || !this.ReadOnly)
                                    {
                                        num5 = num;
                                        while (num5 < num2)
                                        {
                                            info3.Items[num5].ClearAll();
                                            num5++;
                                        }
                                        this.caretContainer = info3.Items[num];
                                        this.caretIndex = 0;
                                    }
                                }
                                else if (this.selectedContainers[i] is PictureInfo)
                                {
                                    PictureInfo info4 = this.selectedContainers[i] as PictureInfo;
                                    if (info4.InBlank || !this.ReadOnly)
                                    {
                                        num5 = num;
                                        while (num5 < num2)
                                        {
                                            ((PictureInfo) this.selectedContainers[i]).Documents[num5].ClearAll();
                                            num5++;
                                        }
                                        this.caretContainer = info4.Documents[0];
                                        this.caretIndex = 0;
                                    }
                                }
                                else if (this.selectedContainers[i] is lineexpression)
                                {
                                    lineexpression2 = this.selectedContainers[i] as lineexpression;
                                    while (!(lineexpression2 is containerexpression))
                                    {
                                        lineexpression2 = lineexpression2.ParentExpression.ParentExpression;
                                    }
                                    info2 = (lineexpression2 as containerexpression).Info;
                                    info2.Sized = false;
                                    if (info2.InBlank || !this.ReadOnly)
                                    {
                                        lineexpression2 = this.selectedContainers[i] as lineexpression;
                                        if (lineexpression2.Child != null)
                                        {
                                            lineexpression2.Child.RemoveRange(num, num2 - num);
                                        }
                                        for (num5 = 0; num5 < i; num5++)
                                        {
                                            if (this.selectedContainers[num5] == this.selectedContainers[i])
                                            {
                                                if (this.selectedTextLast[num5] >= num2)
                                                {
                                                    (list = this.selectedTextLast)[num6 = num5] = list[num6] - (num2 - num);
                                                }
                                                else if (this.selectedTextLast[num5] >= num)
                                                {
                                                    this.selectedTextLast[num5] = num;
                                                }
                                                if (this.selectedTextNew[num5] >= num2)
                                                {
                                                    (list = this.selectedTextNew)[num6 = num5] = list[num6] - (num2 - num);
                                                }
                                                else if (this.selectedTextNew[num5] >= num)
                                                {
                                                    this.selectedTextNew[num5] = num;
                                                }
                                            }
                                        }
                                        this.caretContainer = this.selectedContainers[i];
                                        this.caretIndex = num;
                                    }
                                }
                            }
                        }
                        this.selectedTextNew = new List<int>();
                        this.selectedTextLast = new List<int>();
                        this.selectedContainers = new List<object>();
                    }
                    else if (this.caretContainer is Document)
                    {
                        document2 = this.caretContainer as Document;
                        if ((this.selectedContainers.Count > 0) && (this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer))
                        {
                            num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                            num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                            this.caretIndex = num + document2.DeleteElement(num, num2 - num, this.ReadOnly);
                            this.selectedContainers = new List<object>();
                            this.selectedTextLast = new List<int>();
                            this.selectedTextNew = new List<int>();
                        }
                        if (document2.InsertChar(this.caretIndex, chr, this.Font, this.ReadOnly))
                        {
                            this.caretIndex++;
                        }
                    }
                    else if (this.caretContainer is lineexpression)
                    {
                        lineexpression2 = this.caretContainer as lineexpression;
                        while (!(lineexpression2 is containerexpression))
                        {
                            lineexpression2 = lineexpression2.ParentExpression.ParentExpression;
                        }
                        info = ((containerexpression) lineexpression2).Info;
                        info.Sized = false;
                        if ((this.ReadOnly && info.InBlank) || !this.ReadOnly)
                        {
                            lineexpression2 = this.caretContainer as lineexpression;
                            if ((this.selectedContainers.Count > 0) && (this.selectedContainers[this.selectedContainers.Count - 1] == this.caretContainer))
                            {
                                num = Math.Min(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                                num2 = Math.Max(this.selectedTextLast[this.selectedTextLast.Count - 1], this.selectedTextNew[this.selectedTextNew.Count - 1]);
                                if (((expression) this.caretContainer).Child != null)
                                {
                                    ((expression) this.caretContainer).Child.RemoveRange(num, num2 - num);
                                }
                                this.caretIndex = num;
                                this.selectedContainers = new List<object>();
                                this.selectedTextLast = new List<int>();
                                this.selectedTextNew = new List<int>();
                            }
                            structexpression item = Qisi.Editor.CommonMethods.CreateExpr("字符", lineexpression2, this.ForeColor, chr.ToString(), 2, 1);
                            if (lineexpression2.Child == null)
                            {
                                lineexpression2.Child = new List<structexpression>();
                                lineexpression2.Child.Insert(this.caretIndex, item);
                            }
                            else
                            {
                                lineexpression2.Child.Insert(this.caretIndex, item);
                            }
                            while (!(lineexpression2 is containerexpression))
                            {
                                lineexpression2 = lineexpression2.ParentExpression.ParentExpression;
                            }
                            this.caretIndex++;
                        }
                    }
                }
                this.inputChars.Dequeue();
            }
            while (this.inputImages.Count != 0)
            {
                flag = true;
                Image image = this.inputImages.Peek();
                if (this.caretContainer is Document)
                {
                    document2 = this.caretContainer as Document;
                    document2.InsertPic(this.caretIndex, image, this.Font, this.ReadOnly);
                }
                this.inputImages.Dequeue();
            }
            while (this.inputTables.Count != 0)
            {
                flag = true;
                Point tableSize = this.inputTables.Peek();
                if (this.caretContainer is Document)
                {
                    document2 = this.caretContainer as Document;
                    if (document2.InsertTable(this.caretIndex, tableSize, this.Font, this.ReadOnly))
                    {
                        Element element = document2.Elements[this.caretIndex];
                        if (element is TableInfo)
                        {
                            this.caretContainer = (element as TableInfo).Items[0];
                            this.caretIndex = 0;
                        }
                    }
                }
                this.inputTables.Dequeue();
            }
            if (flag)
            {
                this.hasCaret = true;
                base.Invalidate();
                if (this.ContentChanged != null)
                {
                    this.ContentChanged(this, new EventArgs());
                }
            }
            this.freshed = true;
        }

        private void InsertBlankEnterable(object sender, EventArgs e)
        {
            if (this.caretContainer is Document)
            {
                FormBlankInfo info = new FormBlankInfo();
                if (info.ShowDialog() == DialogResult.OK)
                {
                    (this.caretContainer as Document).InsertBlank(this.caretIndex, info.MaxCount, info.MinWidth, true);
                }
            }
            base.Invalidate();
        }

        private void InsertBlankEnterless(object sender, EventArgs e)
        {
            if (this.caretContainer is Document)
            {
                FormBlankInfo info = new FormBlankInfo();
                if (info.ShowDialog() == DialogResult.OK)
                {
                    (this.caretContainer as Document).InsertBlank(this.caretIndex, info.MaxCount, info.MinWidth, false);
                }
            }
            base.Invalidate();
        }

        private void InsertOptions(object sender, EventArgs e)
        {
            if (this.caretContainer is Document)
            {
                FormOptionInfo info = new FormOptionInfo();
                if (info.ShowDialog() == DialogResult.OK)
                {
                    (this.caretContainer as Document).InsertOptions(this.caretIndex, info.Options, info.Elements);
                }
            }
            base.Invalidate();
        }

        private bool JuZheng(expression e, PointF p)
        {
            bool flag = false;
            if ((((e.InputLocation.X <= p.X) && (p.X <= (e.InputLocation.X + e.Region.Width))) && (e.InputLocation.Y <= p.Y)) && (p.Y <= (e.InputLocation.Y + e.Region.Height)))
            {
                if (e.Type == FType.矩阵)
                {
                    if (e.Child == null)
                    {
                        return false;
                    }
                    foreach (expression expression in e.Child)
                    {
                        if ((((expression.InputLocation.X <= p.X) && (p.X <= (expression.InputLocation.X + expression.Region.Width))) && (expression.InputLocation.Y <= p.Y)) && (p.Y <= (expression.InputLocation.Y + expression.Region.Height)))
                        {
                            this.currentMatrixItemIndex = e.Child.IndexOf(expression);
                            this.currentMatrix = e;
                            return true;
                        }
                    }
                    return flag;
                }
                if (e.Child == null)
                {
                    return flag;
                }
                foreach (expression expression2 in e.Child)
                {
                    flag |= this.JuZheng(expression2, p);
                }
            }
            return flag;
        }

        public void LoadFromXml(string File)
        {
            this.selectedContainers = new List<object>();
            this.selectedTextLast = new List<int>();
            this.selectedTextNew = new List<int>();
            this.mainDocument = new Document(new Padding(10, 0, 10, 10), this.Font, null, (float) base.ClientSize.Width, new PointF(0f, 0f), this.BackColor);
            this.mainDocument.ContentChanged += new EventHandler(this.mainDocument_HeightChanged);
            this.mainDocument.HeightChanged += new EventHandler(this.mainDocument_HeightChanged);
            this.mainDocument.OperateClicked += new EventHandler(this.mainDocument_OperateClicked);
            this.mainDocument.OperateDone += new EventHandler(this.mainDocument_OperateDone);
            this.mainDocument.LoadXmlFromFile(File);
            this.Draw();
        }

        public void LoadOptionSTD(string answer)
        {
            this.mainDocument.LoadOptionSTD(answer);
            base.Invalidate();
        }

        private void mainDocument_ContentChanged(object sender, EventArgs e)
        {
        }

        private void mainDocument_HeightChanged(object sender, EventArgs e)
        {
            if (base.Height != ((int) this.mainDocument.OutHeight))
            {
                base.Height = (int) this.mainDocument.OutHeight;
            }
        }

        private void mainDocument_OperateClicked(object sender, EventArgs e)
        {
            if (this.OperateClicked != null)
            {
                this.OperateClicked(sender, e);
            }
        }

        private void mainDocument_OperateDone(object sender, EventArgs e)
        {
            if (this.OperateDone != null)
            {
                if (sender is OperationInfo)
                {
                    this.OperateDone(sender, new MessageEventArgs((sender as OperationInfo).OperationID));
                }
                else
                {
                    this.OperateDone(sender, new MessageEventArgs(""));
                }
            }
        }

        private void moveCaretLeft(Document document, int index)
        {
            if (document.Elements[index - 1] is CharInfo)
            {
                this.caretIndex = index - 1;
                this.UpdateCaretLocation();
            }
            else if (document.Elements[index - 1] is ExpressionInfo)
            {
                ExpressionInfo info = document.Elements[index - 1] as ExpressionInfo;
                containerexpression containerExpression = info.ContainerExpression;
                this.caretContainer = containerExpression;
                this.caretIndex = containerExpression.Child.Count;
                this.UpdateCaretLocation();
                base.Invalidate();
            }
            else if (document.Elements[index - 1] is TableInfo)
            {
                TableInfo info2 = document.Elements[index - 1] as TableInfo;
                this.caretContainer = info2.Items[info2.Items.Count - 1];
                this.caretIndex = info2.Items[info2.Items.Count - 1].Elements.Count;
                this.UpdateCaretLocation();
            }
            else if (document.Elements[index - 1] is PictureInfo)
            {
                PictureInfo info3 = document.Elements[index - 1] as PictureInfo;
                if ((info3.Documents == null) || (info3.Documents.Count == 0))
                {
                    this.caretIndex = index - 1;
                }
                else
                {
                    this.caretContainer = info3.Documents[info3.Documents.Count - 1];
                    this.caretIndex = info3.Documents[info3.Documents.Count - 1].Elements.Count;
                }
                this.UpdateCaretLocation();
            }
        }

        private void moveCaretRight(Document document, int index)
        {
            if (document.Elements[index] is CharInfo)
            {
                this.caretIndex = index + 1;
                this.UpdateCaretLocation();
            }
            else if (document.Elements[index] is ExpressionInfo)
            {
                ExpressionInfo info = document.Elements[index] as ExpressionInfo;
                containerexpression containerExpression = info.ContainerExpression;
                this.caretContainer = containerExpression;
                this.caretIndex = 0;
                this.UpdateCaretLocation();
                base.Invalidate();
            }
            else if (document.Elements[index] is TableInfo)
            {
                TableInfo info2 = document.Elements[index] as TableInfo;
                this.caretContainer = info2.Items[0];
                this.caretIndex = 0;
                this.UpdateCaretLocation();
            }
            else if (document.Elements[index] is PictureInfo)
            {
                PictureInfo info3 = document.Elements[index] as PictureInfo;
                if ((info3.Documents == null) || (info3.Documents.Count == 0))
                {
                    this.caretIndex = index + 1;
                }
                else
                {
                    this.caretContainer = info3.Documents[0];
                    this.caretIndex = 0;
                }
                this.UpdateCaretLocation();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Qisi.Editor.NativeMethods.DestroyCaret();
            Qisi.Editor.NativeMethods.CreateCaret(base.Handle, IntPtr.Zero, 1, (int) this.caretHeight);
            Qisi.Editor.NativeMethods.ShowCaret(base.Handle);
            this.UpdateCaretLocation();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            this.hasCaret = false;
            Qisi.Editor.NativeMethods.DestroyCaret();
            base.Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.mainDocument.Draw(g);
            this.freshed = true;
            this.UpdateCaretLocation();
            this.virtualCaretX = this.caretLocation.X;
            if (this.caretContainer is lineexpression)
            {
                lineexpression caretContainer = this.caretContainer as lineexpression;
                while (!(caretContainer is containerexpression))
                {
                    caretContainer = caretContainer.ParentExpression.ParentExpression;
                }
                ExpressionInfo info = (caretContainer as containerexpression).Info;
                if ((info != null) && (!this.ReadOnly || (this.ReadOnly && info.InBlank)))
                {
                    Document documentContainer = info.DocumentContainer;
                    g.Clip = new Region(new RectangleF(documentContainer.DocLocation.X + documentContainer.Margin.Left, documentContainer.DocLocation.Y + documentContainer.Margin.Top, documentContainer.DocWidth, documentContainer.DocHeight));
                }
                g.FillRectangle(Brushes.LightSteelBlue, new RectangleF((this.caretContainer as lineexpression).InputLocation, (this.caretContainer as lineexpression).Region));
                g.ResetClip();
                g.DrawRectangle(Pens.LightSkyBlue, (float) (caretContainer.InputLocation.X - info.Padding.Left), (float) (caretContainer.InputLocation.Y - info.Padding.Top), (float) (caretContainer.Region.Width + info.Padding.Horizontal), (float) (caretContainer.Region.Height + info.Padding.Vertical));
                info.Draw(g);
            }
            SolidBrush brush = new SolidBrush(this.mainDocument.HighLightColor);
            for (int i = 0; i < this.selectedContainers.Count; i++)
            {
                int start = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
                int num3 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
                float x = 0f;
                float num5 = 0f;
                int index = 0;
                int num7 = 0;
                if (this.selectedContainers[i] is Document)
                {
                    Element element;
                    if (((this.selectedContainers[i] as Document).Elements.Count > 0) && (start != 0))
                    {
                        element = (this.selectedContainers[i] as Document).Elements[start - 1];
                        x = element.Location.X + element.OutSize.Width;
                        index = (this.selectedContainers[i] as Document).Lines.IndexOf(element.LineContainer);
                    }
                    else
                    {
                        x = (this.selectedContainers[i] as Document).DocLocation.X + (this.selectedContainers[i] as Document).Margin.Left;
                        index = 0;
                    }
                    if (((this.selectedContainers[i] as Document).Elements.Count > 0) && (num3 != 0))
                    {
                        element = (this.selectedContainers[i] as Document).Elements[num3 - 1];
                        num5 = element.Location.X + element.OutSize.Width;
                        num7 = (this.selectedContainers[i] as Document).Lines.IndexOf(element.LineContainer);
                    }
                    else
                    {
                        num5 = (this.selectedContainers[i] as Document).DocLocation.X + (this.selectedContainers[i] as Document).Margin.Left;
                        num7 = 0;
                    }
                    if (index == num7)
                    {
                        g.FillRectangle(brush, x, (this.selectedContainers[i] as Document).Lines[index].Top, num5 - x, (this.selectedContainers[i] as Document).Lines[index].Height);
                    }
                    else
                    {
                        g.FillRectangle(brush, x, (this.selectedContainers[i] as Document).Lines[index].Top, (this.selectedContainers[i] as Document).Lines[index].Right - x, (this.selectedContainers[i] as Document).Lines[index].Height);
                        for (int j = index + 1; j < num7; j++)
                        {
                            g.FillRectangle(brush, (this.selectedContainers[i] as Document).Lines[j].Left, (this.selectedContainers[i] as Document).Lines[j].Top, (this.selectedContainers[i] as Document).Lines[j].Right - (this.selectedContainers[i] as Document).Lines[j].Left, (this.selectedContainers[i] as Document).Lines[j].Height);
                        }
                        g.FillRectangle(brush, (this.selectedContainers[i] as Document).Lines[num7].Left, (this.selectedContainers[i] as Document).Lines[num7].Top, num5 - (this.selectedContainers[i] as Document).Lines[num7].Left, (this.selectedContainers[i] as Document).Lines[num7].Height);
                    }
                    (this.selectedContainers[i] as Document).DrawHighLight(g, start, num3 - start);
                }
                else
                {
                    int num9;
                    if (this.selectedContainers[i] is lineexpression)
                    {
                        num9 = start;
                        while (num9 < num3)
                        {
                            if ((((expression) this.selectedContainers[i]).Child != null) && (((expression) this.selectedContainers[i]).Child.Count > num9))
                            {
                                g.FillRectangle(brush, new RectangleF(((expression) this.selectedContainers[i]).Child[num9].InputLocation, ((expression) this.selectedContainers[i]).Child[num9].Region));
                                (this.selectedContainers[i] as lineexpression).DrawExpression(g);
                            }
                            num9++;
                        }
                    }
                    else if (this.selectedContainers[i] is TableInfo)
                    {
                        for (num9 = start; num9 < num3; num9++)
                        {
                            Cell cell = ((TableInfo) this.selectedContainers[i]).Items[num9];
                            g.FillRectangle(brush, new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
                            cell.Draw(g);
                        }
                    }
                    else if (this.selectedContainers[i] is PictureInfo)
                    {
                    }
                }
            }
            brush.Dispose();
        }

        private void Paste(object sender, EventArgs e)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            string fullName = typeof(Document).FullName;
            if (dataObject.GetDataPresent(fullName))
            {
                Document data = dataObject.GetData(fullName) as Document;
            }
            if (dataObject.GetDataPresent(DataFormats.Bitmap))
            {
                Bitmap img = dataObject.GetData(DataFormats.Bitmap) as Bitmap;
                this.AppendImage(img);
            }
            else if (dataObject.GetDataPresent(DataFormats.Text))
            {
                string str2 = dataObject.GetData(DataFormats.Text) as string;
                foreach (char ch in str2.ToCharArray())
                {
                    this.inputChars.Enqueue(ch);
                }
                this.Insert();
            }
        }

        private bool PointInBlank(Document document, Point pointLocation, bool ignoreMouseState)
        {
            foreach (Blank blank in document.Blanks)
            {
                if (blank.Region.IsVisible(pointLocation))
                {
                    if (!ignoreMouseState)
                    {
                        this.mouseState = MouseState.InBlank;
                    }
                    return true;
                }
            }
            foreach (Element element in document.Elements)
            {
                if (element is TableInfo)
                {
                    foreach (Cell cell in (element as TableInfo).Items)
                    {
                        if (this.PointInBlank(cell, pointLocation, ignoreMouseState))
                        {
                            return true;
                        }
                    }
                }
                if (element is PictureInfo)
                {
                    foreach (Document document2 in (element as PictureInfo).Documents)
                    {
                        if (this.PointInBlank(document2, pointLocation, ignoreMouseState))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool PointInOperation(Document document, Point pointLocation, bool ignoreMouseState)
        {
            foreach (Element element in document.Elements)
            {
                if (element is OperationInfo)
                {
                    OperationInfo info = element as OperationInfo;
                    if (info.Region.IsVisible(pointLocation))
                    {
                        if (!ignoreMouseState)
                        {
                            this.mouseState = MouseState.InOperate;
                        }
                        return true;
                    }
                }
            }
            foreach (Element element in document.Elements)
            {
                if (element is TableInfo)
                {
                    foreach (Cell cell in (element as TableInfo).Items)
                    {
                        if (this.PointInOperation(cell, pointLocation, ignoreMouseState))
                        {
                            return true;
                        }
                    }
                }
                if (element is PictureInfo)
                {
                    foreach (Document document2 in (element as PictureInfo).Documents)
                    {
                        if (this.PointInOperation(document2, pointLocation, ignoreMouseState))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool PointInOptions(Document document, Point pointLocation, bool ignoreMouseState)
        {
            foreach (Options options in document.Optionss)
            {
                if (options.Region.IsVisible(pointLocation))
                {
                    if (!ignoreMouseState)
                    {
                        this.mouseState = MouseState.Choice;
                    }
                    return true;
                }
            }
            foreach (Element element in document.Elements)
            {
                if (element is TableInfo)
                {
                    foreach (Cell cell in (element as TableInfo).Items)
                    {
                        if (this.PointInOptions(cell, pointLocation, ignoreMouseState))
                        {
                            return true;
                        }
                    }
                }
                if (element is PictureInfo)
                {
                    foreach (Document document2 in (element as PictureInfo).Documents)
                    {
                        if (this.PointInOptions(document2, pointLocation, ignoreMouseState))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private Cell PointInTable(Point pointLocation, TableInfo table, bool ignoreMouseState = false)
        {
            if (!ignoreMouseState && ((((pointLocation.Y < ((table.Location.Y + (table.Margin.Top * 2)) + table.LineWidth)) && (pointLocation.Y >= table.Location.Y)) && (pointLocation.X < ((table.Location.X + (table.Margin.Left * 2)) + table.LineWidth))) && (pointLocation.X >= table.Location.X)))
            {
                this.mouseState = MouseState.AboveImage;
                this.mouseContainer = table;
                return null;
            }
            for (int i = 0; i < table.TableSize.X; i++)
            {
                for (int j = 0; j < table.TableSize.Y; j++)
                {
                    Cell cell = table.Rows[i].Cells[j];
                    if (!cell.ismerged)
                    {
                        if ((pointLocation.Y < (cell.DocLocation.Y + cell.Margin.Top)) && (pointLocation.Y >= ((cell.DocLocation.Y - cell.Margin.Bottom) - table.LineWidth)))
                        {
                            if (!ignoreMouseState)
                            {
                                this.mouseState = MouseState.DragBoundNS;
                                this.boundDragedObject = cell;
                                this.dragBound = Bound.Top;
                            }
                            return null;
                        }
                        if ((pointLocation.Y < (((cell.DocLocation.Y + table.Rows[i].Height) + cell.Margin.Top) + table.LineWidth)) && (pointLocation.Y >= ((cell.DocLocation.Y + table.Rows[i].Height) - cell.Margin.Bottom)))
                        {
                            if (!ignoreMouseState)
                            {
                                this.mouseState = MouseState.DragBoundNS;
                                this.boundDragedObject = cell;
                                this.dragBound = Bound.Bottom;
                            }
                            return null;
                        }
                        if ((pointLocation.X < (cell.DocLocation.X + cell.Margin.Left)) && (pointLocation.X >= ((cell.DocLocation.X - cell.Margin.Right) - table.LineWidth)))
                        {
                            if (!ignoreMouseState)
                            {
                                this.mouseState = MouseState.DragBoundWE;
                                this.boundDragedObject = cell;
                                this.dragBound = Bound.Left;
                            }
                            return null;
                        }
                        if ((pointLocation.X < (((cell.DocLocation.X + table.Columns[j].Width) + cell.Margin.Left) + table.LineWidth)) && (pointLocation.X >= ((cell.DocLocation.X + table.Columns[j].Width) - cell.Margin.Right)))
                        {
                            if (!ignoreMouseState)
                            {
                                this.mouseState = MouseState.DragBoundWE;
                                this.boundDragedObject = cell;
                                this.dragBound = Bound.Right;
                            }
                            return null;
                        }
                        if ((((pointLocation.Y < (cell.DocLocation.Y + table.Rows[i].Height)) && (pointLocation.Y >= cell.DocLocation.Y)) && (pointLocation.X < (cell.DocLocation.X + table.Columns[j].Width))) && (pointLocation.X >= cell.DocLocation.X))
                        {
                            this.boundDragedObject = null;
                            return cell;
                        }
                    }
                }
            }
            return null;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Document caretContainer;
            lineexpression parentExpression;
            Blank blank;
            if (keyData != Keys.Tab)
            {
                if (keyData != (Keys.Shift | Keys.Tab))
                {
                    long ticks;
                    bool flag;
                    bool flag2;
                    Blank blank5;
                    TableInfo parent;
                    int num4;
                    Document documentContainer;
                    PictureInfo info2;
                    lineexpression lineexpression2;
                    containerexpression containerexpression;
                    ExpressionInfo info;
                    int num5;
                    structexpression structexpression;
                    if (keyData == Keys.Delete)
                    {
                        this.inputChars.Enqueue('\x007f');
                        this.Insert();
                        return true;
                    }
                    if (keyData != Keys.Left)
                    {
                        if (keyData != Keys.Right)
                        {
                            Line lineContainer;
                            int index;
                            Line line2;
                            int num7;
                            float positiveInfinity;
                            int num9;
                            float num10;
                            float num11;
                            float num12;
                            if (keyData != Keys.Up)
                            {
                                Line line3;
                                if (keyData != Keys.Down)
                                {
                                    return base.ProcessCmdKey(ref msg, keyData);
                                }
                                if (this.caretContainer is Document)
                                {
                                    caretContainer = this.caretContainer as Document;
                                    lineContainer = ((caretContainer.Elements.Count == 0) || (this.caretIndex == caretContainer.Elements.Count)) ? caretContainer.LastLine : caretContainer.Elements[this.caretIndex].LineContainer;
                                    index = caretContainer.Lines.IndexOf(lineContainer);
                                    if (!this.ReadOnly)
                                    {
                                        if (index < (caretContainer.Lines.Count - 1))
                                        {
                                            line3 = caretContainer.Lines[index + 1];
                                            num7 = -1;
                                            positiveInfinity = float.PositiveInfinity;
                                            for (num9 = Math.Max(this.caretIndex + 1, line3.StartIndex); num9 < (line3.ElementCount + line3.StartIndex); num9++)
                                            {
                                                if (!caretContainer.Elements[num9].InBlank)
                                                {
                                                    num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                    num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                    num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                    if (num12 < positiveInfinity)
                                                    {
                                                        positiveInfinity = num12;
                                                        num7 = num9;
                                                    }
                                                }
                                            }
                                            if (num7 != -1)
                                            {
                                                this.findClosestElementDown(caretContainer, num7);
                                            }
                                        }
                                        else
                                        {
                                            this.findDownLine(caretContainer);
                                        }
                                    }
                                    else
                                    {
                                        blank = null;
                                        foreach (Blank blank2 in caretContainer.Blanks)
                                        {
                                            if ((blank2.StartIndex <= this.caretIndex) && ((blank2.StartIndex + blank2.Count) >= this.caretIndex))
                                            {
                                                blank = blank2;
                                                break;
                                            }
                                        }
                                        if ((blank != null) && (index < (caretContainer.Lines.Count - 1)))
                                        {
                                            line3 = caretContainer.Lines[index + 1];
                                            num7 = -1;
                                            positiveInfinity = float.PositiveInfinity;
                                            for (num9 = Math.Max(this.caretIndex + 1, line3.StartIndex); num9 < Math.Min((int) (blank.StartIndex + blank.Count), (int) (line3.StartIndex + line3.ElementCount)); num9++)
                                            {
                                                num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                if (num12 < positiveInfinity)
                                                {
                                                    positiveInfinity = num12;
                                                    num7 = num9;
                                                }
                                            }
                                            if (num7 != -1)
                                            {
                                                this.findClosestElementDown(caretContainer, num7);
                                            }
                                        }
                                    }
                                }
                                else if (this.caretContainer is lineexpression)
                                {
                                    parentExpression = this.caretContainer as lineexpression;
                                    if (parentExpression.DownLineExpression != null)
                                    {
                                        this.findClosestElementDown(parentExpression.DownLineExpression);
                                    }
                                    else
                                    {
                                        while (!(parentExpression is containerexpression) && (parentExpression != null))
                                        {
                                            parentExpression = parentExpression.ParentExpression.ParentExpression;
                                        }
                                        if (parentExpression != null)
                                        {
                                            info = (parentExpression as containerexpression).Info;
                                            caretContainer = info.DocumentContainer;
                                            lineContainer = info.LineContainer;
                                            index = caretContainer.Lines.IndexOf(lineContainer);
                                            if (!this.ReadOnly)
                                            {
                                                if (index < (caretContainer.Lines.Count - 1))
                                                {
                                                    line3 = caretContainer.Lines[index + 1];
                                                    num7 = -1;
                                                    positiveInfinity = float.PositiveInfinity;
                                                    for (num9 = Math.Max(this.caretIndex + 1, line3.StartIndex); num9 < (line3.ElementCount + line3.StartIndex); num9++)
                                                    {
                                                        if (!caretContainer.Elements[num9].InBlank)
                                                        {
                                                            num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                            num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                            num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                            if (num12 < positiveInfinity)
                                                            {
                                                                positiveInfinity = num12;
                                                                num7 = num9;
                                                            }
                                                        }
                                                    }
                                                    if (num7 != -1)
                                                    {
                                                        this.findClosestElementDown(caretContainer, num7);
                                                    }
                                                }
                                                else
                                                {
                                                    this.findDownLine(caretContainer);
                                                }
                                            }
                                            else
                                            {
                                                blank = null;
                                                foreach (Blank blank2 in caretContainer.Blanks)
                                                {
                                                    if ((blank2.StartIndex <= this.caretIndex) && ((blank2.StartIndex + blank2.Count) >= this.caretIndex))
                                                    {
                                                        blank = blank2;
                                                        break;
                                                    }
                                                }
                                                if ((blank != null) && (index < (caretContainer.Lines.Count - 1)))
                                                {
                                                    line3 = caretContainer.Lines[index + 1];
                                                    num7 = -1;
                                                    positiveInfinity = float.PositiveInfinity;
                                                    for (num9 = Math.Max(this.caretIndex + 1, line3.StartIndex); num9 < Math.Min((int) (blank.StartIndex + blank.Count), (int) (line3.StartIndex + line3.ElementCount)); num9++)
                                                    {
                                                        num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                        num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                        num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                        if (num12 < positiveInfinity)
                                                        {
                                                            positiveInfinity = num12;
                                                            num7 = num9;
                                                        }
                                                    }
                                                    if (num7 != -1)
                                                    {
                                                        this.findClosestElementDown(caretContainer, num7);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                return true;
                            }
                            if (this.caretContainer is Document)
                            {
                                caretContainer = this.caretContainer as Document;
                                lineContainer = ((caretContainer.Elements.Count == 0) || (this.caretIndex == caretContainer.Elements.Count)) ? caretContainer.LastLine : caretContainer.Elements[this.caretIndex].LineContainer;
                                index = caretContainer.Lines.IndexOf(lineContainer);
                                if (!this.ReadOnly)
                                {
                                    if (index > 0)
                                    {
                                        line2 = caretContainer.Lines[index - 1];
                                        num7 = -1;
                                        positiveInfinity = float.PositiveInfinity;
                                        for (num9 = Math.Min((int) (this.caretIndex - 1), (int) ((line2.ElementCount + line2.StartIndex) - 1)); num9 >= line2.StartIndex; num9--)
                                        {
                                            if (!caretContainer.Elements[num9].InBlank)
                                            {
                                                num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                if (num12 < positiveInfinity)
                                                {
                                                    positiveInfinity = num12;
                                                    num7 = num9;
                                                }
                                            }
                                        }
                                        if (num7 != -1)
                                        {
                                            this.findClosestElementUp(caretContainer, num7);
                                        }
                                    }
                                    else
                                    {
                                        this.findUpperLine(caretContainer);
                                    }
                                }
                                else
                                {
                                    blank = null;
                                    foreach (Blank blank2 in caretContainer.Blanks)
                                    {
                                        if ((blank2.StartIndex <= this.caretIndex) && ((blank2.StartIndex + blank2.Count) >= this.caretIndex))
                                        {
                                            blank = blank2;
                                            break;
                                        }
                                    }
                                    if ((blank != null) && (index > 0))
                                    {
                                        line2 = caretContainer.Lines[index - 1];
                                        num7 = -1;
                                        positiveInfinity = float.PositiveInfinity;
                                        for (num9 = Math.Min((int) (this.caretIndex - 1), (int) ((line2.ElementCount + line2.StartIndex) - 1)); num9 >= Math.Max(blank.StartIndex, line2.StartIndex); num9--)
                                        {
                                            num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                            num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                            num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                            if (num12 < positiveInfinity)
                                            {
                                                positiveInfinity = num12;
                                                num7 = num9;
                                            }
                                        }
                                        if (num7 != -1)
                                        {
                                            this.findClosestElementUp(caretContainer, num7);
                                        }
                                    }
                                }
                            }
                            else if (this.caretContainer is lineexpression)
                            {
                                parentExpression = this.caretContainer as lineexpression;
                                if (parentExpression.UpLineExpression != null)
                                {
                                    this.findClosestElementUp(parentExpression.UpLineExpression);
                                }
                                else
                                {
                                    while (!(parentExpression is containerexpression) && (parentExpression != null))
                                    {
                                        parentExpression = parentExpression.ParentExpression.ParentExpression;
                                    }
                                    if (parentExpression != null)
                                    {
                                        info = (parentExpression as containerexpression).Info;
                                        caretContainer = info.DocumentContainer;
                                        lineContainer = info.LineContainer;
                                        index = caretContainer.Lines.IndexOf(lineContainer);
                                        if (!this.ReadOnly)
                                        {
                                            if (index > 0)
                                            {
                                                line2 = caretContainer.Lines[index - 1];
                                                num7 = -1;
                                                positiveInfinity = float.PositiveInfinity;
                                                for (num9 = Math.Min((int) (this.caretIndex - 1), (int) ((line2.ElementCount + line2.StartIndex) - 1)); num9 >= line2.StartIndex; num9--)
                                                {
                                                    if (!caretContainer.Elements[num9].InBlank)
                                                    {
                                                        num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                        num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                        num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                        if (num12 < positiveInfinity)
                                                        {
                                                            positiveInfinity = num12;
                                                            num7 = num9;
                                                        }
                                                    }
                                                }
                                                if (num7 != -1)
                                                {
                                                    this.findClosestElementUp(caretContainer, num7);
                                                }
                                            }
                                            else
                                            {
                                                this.findUpperLine(caretContainer);
                                            }
                                        }
                                        else
                                        {
                                            blank = null;
                                            foreach (Blank blank2 in caretContainer.Blanks)
                                            {
                                                if ((blank2.StartIndex <= this.caretIndex) && ((blank2.StartIndex + blank2.Count) >= this.caretIndex))
                                                {
                                                    blank = blank2;
                                                    break;
                                                }
                                            }
                                            if ((blank != null) && (index > 0))
                                            {
                                                line2 = caretContainer.Lines[index - 1];
                                                num7 = -1;
                                                positiveInfinity = float.PositiveInfinity;
                                                for (num9 = Math.Min((int) (this.caretIndex - 1), (int) ((line2.ElementCount + line2.StartIndex) - 1)); num9 >= Math.Max(blank.StartIndex, line2.StartIndex); num9--)
                                                {
                                                    num10 = (this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X) - caretContainer.Elements[num9].OutSize.Width;
                                                    num11 = this.virtualCaretX - caretContainer.Elements[num9].OutLocation.X;
                                                    num12 = ((num10 * num11) > 0f) ? Math.Abs((float) (num10 + num11)) : 0f;
                                                    if (num12 < positiveInfinity)
                                                    {
                                                        positiveInfinity = num12;
                                                        num7 = num9;
                                                    }
                                                }
                                                if (num7 != -1)
                                                {
                                                    this.findClosestElementUp(caretContainer, num7);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return true;
                        }
                        ticks = DateTime.Now.Ticks;
                        if (this.caretContainer is Document)
                        {
                            caretContainer = this.caretContainer as Document;
                            if (!this.ReadOnly)
                            {
                                if (this.caretIndex >= caretContainer.Elements.Count)
                                {
                                    if (caretContainer.Parent != null)
                                    {
                                        if (caretContainer.Parent is TableInfo)
                                        {
                                            parent = caretContainer.Parent as TableInfo;
                                            num4 = parent.Items.IndexOf(caretContainer as Cell);
                                            if (num4 < (parent.Items.Count - 1))
                                            {
                                                this.caretContainer = parent.Items[num4 + 1];
                                                this.caretIndex = 0;
                                                this.UpdateCaretLocation();
                                            }
                                            else
                                            {
                                                documentContainer = parent.DocumentContainer;
                                                this.caretContainer = documentContainer;
                                                this.caretIndex = parent.Index + 1;
                                                this.UpdateCaretLocation();
                                            }
                                        }
                                        else if (caretContainer.Parent is PictureInfo)
                                        {
                                            info2 = caretContainer.Parent as PictureInfo;
                                            num4 = info2.Documents.IndexOf(caretContainer as Cell);
                                            if (num4 < (info2.Documents.Count - 1))
                                            {
                                                this.caretContainer = info2.Documents[num4 + 1];
                                                this.caretIndex = 0;
                                                this.UpdateCaretLocation();
                                            }
                                            else
                                            {
                                                documentContainer = info2.DocumentContainer;
                                                this.caretContainer = documentContainer;
                                                this.caretIndex = info2.Index + 1;
                                                this.UpdateCaretLocation();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    blank5 = null;
                                    foreach (Blank blank2 in caretContainer.Blanks)
                                    {
                                        if (blank2.StartIndex == this.caretIndex)
                                        {
                                            flag = true;
                                            blank5 = blank2;
                                            break;
                                        }
                                    }
                                    if (flag)
                                    {
                                        this.caretIndex = blank5.StartIndex + blank5.Count;
                                        this.UpdateCaretLocation();
                                    }
                                    else
                                    {
                                        this.moveCaretRight(caretContainer, this.caretIndex);
                                    }
                                }
                            }
                            else
                            {
                                flag2 = false;
                                foreach (Blank blank2 in caretContainer.Blanks)
                                {
                                    if ((blank2.StartIndex + blank2.Count) == this.caretIndex)
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (flag2)
                                {
                                    if ((ticks - this.rightArrowTime) < 0x16e360L)
                                    {
                                        this.getNextBlank(caretContainer, this.caretIndex, true);
                                        ticks = 0L;
                                    }
                                }
                                else
                                {
                                    this.moveCaretRight(caretContainer, this.caretIndex);
                                }
                            }
                        }
                        else if (this.caretContainer is lineexpression)
                        {
                            parentExpression = this.caretContainer as lineexpression;
                            if (this.caretIndex < parentExpression.Child.Count)
                            {
                                if (parentExpression.Child[this.caretIndex].Child != null)
                                {
                                    lineexpression2 = parentExpression.Child[this.caretIndex].Child[0];
                                    this.caretContainer = lineexpression2;
                                    this.caretIndex = 0;
                                }
                                else
                                {
                                    this.caretIndex++;
                                }
                                this.UpdateCaretLocation();
                            }
                            else
                            {
                                if (parentExpression is containerexpression)
                                {
                                    containerexpression = parentExpression as containerexpression;
                                    info = containerexpression.Info;
                                    this.caretContainer = info.DocumentContainer;
                                    this.caretIndex = info.Index + 1;
                                }
                                else
                                {
                                    num5 = parentExpression.ParentExpression.Child.IndexOf(parentExpression);
                                    if (num5 < (parentExpression.ParentExpression.Child.Count - 1))
                                    {
                                        this.caretContainer = parentExpression.ParentExpression.Child[num5 + 1];
                                        lineexpression2 = this.caretContainer as lineexpression;
                                        this.caretIndex = 0;
                                    }
                                    else
                                    {
                                        structexpression = parentExpression.ParentExpression;
                                        lineexpression2 = structexpression.ParentExpression;
                                        this.caretContainer = lineexpression2;
                                        this.caretIndex = lineexpression2.Child.IndexOf(structexpression) + 1;
                                    }
                                }
                                this.UpdateCaretLocation();
                            }
                        }
                        this.virtualCaretX = this.caretLocation.X;
                        this.rightArrowTime = ticks;
                        return true;
                    }
                    ticks = DateTime.Now.Ticks;
                    if (this.caretContainer is Document)
                    {
                        caretContainer = this.caretContainer as Document;
                        if (!this.ReadOnly)
                        {
                            if (this.caretIndex <= 0)
                            {
                                if (caretContainer.Parent != null)
                                {
                                    if (caretContainer.Parent is TableInfo)
                                    {
                                        parent = caretContainer.Parent as TableInfo;
                                        num4 = parent.Items.IndexOf(caretContainer as Cell);
                                        if (num4 > 0)
                                        {
                                            this.caretContainer = parent.Items[num4 - 1];
                                            this.caretIndex = parent.Items[num4 - 1].Elements.Count;
                                            this.UpdateCaretLocation();
                                        }
                                        else
                                        {
                                            documentContainer = parent.DocumentContainer;
                                            this.caretContainer = documentContainer;
                                            this.caretIndex = parent.Index;
                                            this.UpdateCaretLocation();
                                        }
                                    }
                                    else if (caretContainer.Parent is PictureInfo)
                                    {
                                        info2 = caretContainer.Parent as PictureInfo;
                                        num4 = info2.Documents.IndexOf(caretContainer as Cell);
                                        if (num4 > 0)
                                        {
                                            this.caretContainer = info2.Documents[num4 - 1];
                                            this.caretIndex = info2.Documents[num4 - 1].Elements.Count;
                                            this.UpdateCaretLocation();
                                        }
                                        else
                                        {
                                            documentContainer = info2.DocumentContainer;
                                            this.caretContainer = documentContainer;
                                            this.caretIndex = info2.Index;
                                            this.UpdateCaretLocation();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                flag2 = false;
                                blank5 = null;
                                foreach (Blank blank2 in caretContainer.Blanks)
                                {
                                    if ((blank2.StartIndex + blank2.Count) == this.caretIndex)
                                    {
                                        flag2 = true;
                                        blank5 = blank2;
                                        break;
                                    }
                                }
                                if (flag2)
                                {
                                    this.caretIndex = blank5.StartIndex;
                                    this.UpdateCaretLocation();
                                }
                                else
                                {
                                    this.moveCaretLeft(caretContainer, this.caretIndex);
                                }
                            }
                        }
                        else
                        {
                            flag = false;
                            foreach (Blank blank2 in caretContainer.Blanks)
                            {
                                if (blank2.StartIndex == this.caretIndex)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                if ((ticks - this.leftArrowTime) < 0x16e360L)
                                {
                                    this.getPrevBlank(caretContainer, this.caretIndex, true);
                                    ticks = 0L;
                                }
                            }
                            else
                            {
                                this.moveCaretLeft(caretContainer, this.caretIndex);
                            }
                        }
                    }
                    else if (this.caretContainer is lineexpression)
                    {
                        parentExpression = this.caretContainer as lineexpression;
                        if (this.caretIndex > 0)
                        {
                            if (parentExpression.Child[this.caretIndex - 1].Child != null)
                            {
                                lineexpression2 = parentExpression.Child[this.caretIndex - 1].Child[parentExpression.Child[this.caretIndex - 1].Child.Count - 1];
                                this.caretContainer = lineexpression2;
                                this.caretIndex = lineexpression2.Child.Count;
                            }
                            else
                            {
                                this.caretIndex--;
                            }
                            this.UpdateCaretLocation();
                        }
                        else
                        {
                            if (parentExpression is containerexpression)
                            {
                                containerexpression = parentExpression as containerexpression;
                                info = containerexpression.Info;
                                this.caretContainer = info.DocumentContainer;
                                this.caretIndex = info.Index;
                            }
                            else
                            {
                                num5 = parentExpression.ParentExpression.Child.IndexOf(parentExpression);
                                if (num5 > 0)
                                {
                                    this.caretContainer = parentExpression.ParentExpression.Child[num5 - 1];
                                    lineexpression2 = this.caretContainer as lineexpression;
                                    this.caretIndex = lineexpression2.Child.Count;
                                }
                                else
                                {
                                    structexpression = parentExpression.ParentExpression;
                                    lineexpression2 = structexpression.ParentExpression;
                                    this.caretContainer = lineexpression2;
                                    this.caretIndex = lineexpression2.Child.IndexOf(structexpression);
                                }
                            }
                            this.UpdateCaretLocation();
                        }
                        base.Invalidate();
                    }
                    this.virtualCaretX = this.caretLocation.X;
                    this.leftArrowTime = ticks;
                    return true;
                }
                if (!this.ReadOnly)
                {
                    goto Label_0406;
                }
                caretContainer = null;
                if (this.caretContainer is Document)
                {
                    caretContainer = this.caretContainer as Document;
                }
                else if (this.caretContainer is lineexpression)
                {
                    parentExpression = this.caretContainer as lineexpression;
                    while (!(parentExpression is containerexpression) && (parentExpression != null))
                    {
                        parentExpression = parentExpression.ParentExpression.ParentExpression;
                    }
                    if ((parentExpression != null) && (parentExpression is containerexpression))
                    {
                        caretContainer = (parentExpression as containerexpression).Info.DocumentContainer;
                    }
                }
                if (caretContainer == null)
                {
                    goto Label_0406;
                }
                blank = null;
                foreach (Blank blank2 in caretContainer.Blanks)
                {
                    if ((blank2.StartIndex <= this.caretIndex) && ((blank2.StartIndex + blank2.Count) >= this.caretIndex))
                    {
                        blank = blank2;
                        break;
                    }
                }
            }
            else
            {
                if (this.ReadOnly)
                {
                    caretContainer = null;
                    if (this.caretContainer is Document)
                    {
                        caretContainer = this.caretContainer as Document;
                    }
                    else if (this.caretContainer is lineexpression)
                    {
                        parentExpression = this.caretContainer as lineexpression;
                        while (!(parentExpression is containerexpression) && (parentExpression != null))
                        {
                            parentExpression = parentExpression.ParentExpression.ParentExpression;
                        }
                        if ((parentExpression != null) && (parentExpression is containerexpression))
                        {
                            caretContainer = (parentExpression as containerexpression).Info.DocumentContainer;
                        }
                    }
                    if (caretContainer != null)
                    {
                        blank = null;
                        foreach (Blank blank2 in caretContainer.Blanks)
                        {
                            if ((blank2.StartIndex <= this.caretIndex) && ((blank2.StartIndex + blank2.Count) >= this.caretIndex))
                            {
                                blank = blank2;
                                break;
                            }
                        }
                        if (blank != null)
                        {
                            Blank blank3 = null;
                            int startIndex = 0x7fffffff;
                            foreach (Blank blank2 in caretContainer.Blanks)
                            {
                                if ((blank2.StartIndex > (blank.StartIndex + blank.Count)) && (blank2.StartIndex < startIndex))
                                {
                                    blank3 = blank2;
                                    startIndex = blank2.StartIndex;
                                }
                            }
                            if (blank3 != null)
                            {
                                this.caretContainer = caretContainer;
                                this.caretIndex = blank3.StartIndex;
                                this.UpdateCaretLocation();
                            }
                        }
                    }
                }
                return true;
            }
            if (blank != null)
            {
                Blank blank4 = null;
                int num2 = -2147483648;
                foreach (Blank blank2 in caretContainer.Blanks)
                {
                    if (((blank2.StartIndex + blank2.Count) < blank.StartIndex) && ((blank2.StartIndex + blank2.Count) > num2))
                    {
                        blank4 = blank2;
                        num2 = blank2.StartIndex + blank2.Count;
                    }
                }
                if (blank4 != null)
                {
                    this.caretContainer = caretContainer;
                    this.caretIndex = blank4.StartIndex;
                    this.UpdateCaretLocation();
                }
            }
        Label_0406:
            return true;
        }

        public void Save(string PathtoSave)
        {
            string contents = this.mainDocument.toXml(PathtoSave);
            File.WriteAllText(Path.Combine(PathtoSave, "Question.Xml"), contents, Encoding.UTF8);
        }

        private void SelectablePaste(object sender, EventArgs e)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            List<string> list = new List<string>();
            List<string> strs = new List<string>();
            string fullName = typeof(Document).FullName;
            if (dataObject.GetDataPresent(fullName))
            {
                list.Add(fullName);
                strs.Add("启思文档编辑器格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Bitmap))
            {
                list.Add(DataFormats.Bitmap);
                strs.Add("Microsoft Windows 位图数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.CommaSeparatedValue))
            {
                list.Add(DataFormats.CommaSeparatedValue);
                strs.Add("逗号分隔值 (CSV)数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Dib))
            {
                list.Add(DataFormats.Dib);
                strs.Add("与设备无关的位图 (DIB) 数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Dif))
            {
                list.Add(DataFormats.Dif);
                strs.Add("Windows 数据交换 (DIF)格式数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.EnhancedMetafile))
            {
                list.Add(DataFormats.EnhancedMetafile);
                strs.Add("Windows 增强型图元文件格式");
            }
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                list.Add(DataFormats.FileDrop);
                strs.Add("Windows 文件放置格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Html))
            {
                list.Add(DataFormats.Html);
                strs.Add("HTML 数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Locale))
            {
                list.Add(DataFormats.Locale);
                strs.Add("Windows 区域设置 (区域性) 数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.MetafilePict))
            {
                list.Add(DataFormats.MetafilePict);
                strs.Add("Windows 图元文件图像数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.OemText))
            {
                list.Add(DataFormats.OemText);
                strs.Add("标准 Windows OEM 文本数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Palette))
            {
                list.Add(DataFormats.Palette);
                strs.Add("Windows 调色板数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.PenData))
            {
                list.Add(DataFormats.PenData);
                strs.Add("Windows 笔数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Riff))
            {
                list.Add(DataFormats.Riff);
                strs.Add("资源交换文件格式 (RIFF) 音频数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Rtf))
            {
                list.Add(DataFormats.Rtf);
                strs.Add("RTF 格式 (RTF) 数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Serializable))
            {
                list.Add(DataFormats.Serializable);
                strs.Add("封装任何类型可序列化数据对象的数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.StringFormat))
            {
                list.Add(DataFormats.StringFormat);
                strs.Add("公共语言运行时 (CLR) 字符串类数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.SymbolicLink))
            {
                list.Add(DataFormats.SymbolicLink);
                strs.Add("Windows 符号字符串数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Text))
            {
                list.Add(DataFormats.Text);
                strs.Add("ANSI 文本数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.Tiff))
            {
                list.Add(DataFormats.Tiff);
                strs.Add("标记图像文件格式 (TIFF) 数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                list.Add(DataFormats.UnicodeText);
                strs.Add("Unicode 文本数据格式");
            }
            if (dataObject.GetDataPresent(DataFormats.WaveAudio))
            {
                list.Add(DataFormats.WaveAudio);
                strs.Add("波形音频数据格式");
            }
            FormFormatSelect select = new FormFormatSelect(strs);
            if (select.ShowDialog(this) == DialogResult.OK)
            {
                int selectdindex = select.selectdindex;
            }
        }

        public void setFont(Font F)
        {
            this.Font = F;
        }

        private void SuperBox_Click(object sender, EventArgs e)
        {
            base.Focus();
        }

        private void SuperBox_KeyDown(object sender, KeyEventArgs e)
        {
            IntPtr hIMC = Qisi.Editor.NativeMethods.ImmGetContext(base.Handle);
            Qisi.Editor.NativeMethods.COMPOSITIONFORM lpCompositionForm = new Qisi.Editor.NativeMethods.COMPOSITIONFORM {
                dwStyle = 2,
                ptCurrentPos = new Point()
            };
            lpCompositionForm.ptCurrentPos.X = (int) this.caretLocation.X;
            lpCompositionForm.ptCurrentPos.Y = (int) this.caretLocation.Y;
            int num = Qisi.Editor.NativeMethods.ImmSetCompositionWindow(hIMC, ref lpCompositionForm);
        }

        private void SuperBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (this.InputLimited)
            {
                if (((((e.KeyChar < '一') || (e.KeyChar > 0x9fa5)) && ((e.KeyChar < ' ') || (e.KeyChar > '~'))) && ((!"，。“”‘’、…：；！？".Contains(e.KeyChar.ToString()) && (e.KeyChar != '\b')) && (e.KeyChar != '\n'))) && (e.KeyChar != '\r'))
                {
                    return;
                }
            }
            else if ((((e.KeyChar < ' ') && (e.KeyChar != '\b')) && (e.KeyChar != '\n')) && (e.KeyChar != '\r'))
            {
                return;
            }
            if (!((this.caretContainer is lineexpression) && (e.KeyChar == '\r')))
            {
                this.inputChars.Enqueue(e.KeyChar);
                this.Insert();
            }
        }

        private void SuperBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.FindMouse(this.mainDocument, e.Location, false);
            if (this.mouseState == MouseState.AboveImage)
            {
                this.CancelSelection();
                if (this.mouseContainer is Pic_Tab)
                {
                    Pic_Tab mouseContainer = this.mouseContainer as Pic_Tab;
                    this.caretContainer = mouseContainer.DocumentContainer;
                    this.caretIndex = mouseContainer.Index;
                    mouseContainer.AloneSelected = true;
                    this.selectedObject = mouseContainer;
                }
                this.hasCaret = false;
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
                base.Invalidate();
            }
            else if ((this.mouseState == MouseState.DragBoundNS) || (this.mouseState == MouseState.DragBoundWE))
            {
                this.hasCaret = false;
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
            }
            else if (this.mouseState == MouseState.DragElement)
            {
                if (this.selectedObject != null)
                {
                    this.selectedObject.AloneSelected = false;
                    this.selectedObject = null;
                }
                this.hasCaret = false;
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
            }
            else if (this.mouseState == MouseState.None)
            {
                if (this.selectedObject != null)
                {
                    this.selectedObject.AloneSelected = false;
                    this.selectedObject = null;
                }
                this.hasCaret = false;
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
            }
            else if (this.mouseState == MouseState.InBlank)
            {
                if (this.selectedObject != null)
                {
                    this.selectedObject.AloneSelected = false;
                    this.selectedObject = null;
                }
                this.hasCaret = false;
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
            }
            else if (this.mouseState == MouseState.Choice)
            {
                if (this.selectedObject != null)
                {
                    this.selectedObject.AloneSelected = false;
                    this.selectedObject = null;
                }
                this.hasCaret = false;
                this.mainDocument.Checked((PointF) e.Location);
                if (this.ContentChanged != null)
                {
                    this.ContentChanged(this, new EventArgs());
                }
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
                base.Invalidate();
            }
            else if (this.mouseState == MouseState.InOperate)
            {
                if (this.selectedObject != null)
                {
                    this.selectedObject.AloneSelected = false;
                    this.selectedObject = null;
                }
                this.hasCaret = false;
                this.mainDocument.Operate((PointF) e.Location);
                this.UpdateCaretLocation();
                this.virtualCaretX = this.caretLocation.X;
                base.Invalidate();
            }
            else
            {
                bool flag = false;
                for (int i = 0; i < this.selectedContainers.Count; i++)
                {
                    object obj2 = this.selectedContainers[i];
                    int num2 = Math.Min(this.selectedTextLast[i], this.selectedTextNew[i]);
                    int num3 = Math.Max(this.selectedTextLast[i], this.selectedTextNew[i]);
                    float x = 0f;
                    float num5 = 0f;
                    int index = 0;
                    int num7 = 0;
                    Region region = new Region(new RectangleF(0f, 0f, 0f, 0f));
                    if (this.selectedContainers[i] is Document)
                    {
                        Document document = this.selectedContainers[i] as Document;
                        if ((document.Elements.Count > 0) && (num2 != 0))
                        {
                            Element element = document.Elements[num2 - 1];
                            x = element.Location.X + element.OutSize.Width;
                            index = document.Lines.IndexOf(element.LineContainer);
                        }
                        else
                        {
                            x = document.DocLocation.X + document.Margin.Left;
                            index = 0;
                        }
                        if ((document.Elements.Count > 0) && (num3 != 0))
                        {
                            Element element2 = document.Elements[num3 - 1];
                            num5 = element2.Location.X + element2.OutSize.Width;
                            num7 = document.Lines.IndexOf(element2.LineContainer);
                        }
                        else
                        {
                            num5 = document.DocLocation.X + document.Margin.Left;
                            num7 = 0;
                        }
                        if (index == num7)
                        {
                            region.Union(new RectangleF(x, document.Lines[index].Top, num5 - x, document.Lines[index].Height));
                        }
                        else
                        {
                            region.Union(new RectangleF(x, document.Lines[index].Top, document.Lines[index].Right - x, document.Lines[index].Height));
                            for (int j = index + 1; j < num7; j++)
                            {
                                region.Union(new RectangleF(document.Lines[j].Left, document.Lines[j].Top, document.Lines[j].Right - document.Lines[j].Left, document.Lines[j].Height));
                            }
                            region.Union(new RectangleF(document.Lines[num7].Left, document.Lines[num7].Top, num5 - document.Lines[num7].Left, document.Lines[num7].Height));
                        }
                    }
                    else
                    {
                        int num9;
                        if (this.selectedContainers[i] is lineexpression)
                        {
                            lineexpression lineexpression = this.selectedContainers[i] as lineexpression;
                            num9 = num2;
                            while (num9 < Math.Min(num3, lineexpression.Child.Count))
                            {
                                if ((lineexpression.Child != null) && (lineexpression.Child.Count > num9))
                                {
                                    region.Union(new RectangleF(lineexpression.Child[num9].InputLocation, lineexpression.Child[num9].Region));
                                }
                                num9++;
                            }
                        }
                        else if (this.selectedContainers[i] is TableInfo)
                        {
                            TableInfo info = this.selectedContainers[i] as TableInfo;
                            num9 = num2;
                            while (num9 < Math.Min(num3, info.Items.Count))
                            {
                                Cell cell = info.Items[num9];
                                region.Union(new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
                                num9++;
                            }
                        }
                        else if (this.selectedContainers[i] is PictureInfo)
                        {
                            PictureInfo info2 = this.selectedContainers[i] as PictureInfo;
                            for (num9 = num2; num9 < Math.Min(num3, info2.Documents.Count); num9++)
                            {
                                Document document2 = info2.Documents[num9];
                                region.Union(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
                            }
                        }
                    }
                    if (region.IsVisible(e.Location))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    if (this.selectedObject != null)
                    {
                        this.selectedObject.AloneSelected = false;
                        this.selectedObject = null;
                    }
                    this.hasCaret = false;
                    this.UpdateCaretLocation();
                    this.virtualCaretX = this.caretLocation.X;
                    this.mouseState = MouseState.DragElement;
                    this.ToolStripState = ToolStripMenuType.General;
                }
                else
                {
                    this.ToolStripState = ToolStripMenuType.Insert | ToolStripMenuType.General;
                    if (((Control.ModifierKeys & Keys.Control) == Keys.Control) && (this.mouseState == MouseState.Select))
                    {
                        if (this.selectedObject != null)
                        {
                            this.selectedObject.AloneSelected = false;
                            this.selectedObject = null;
                        }
                    }
                    else
                    {
                        this.CancelSelection();
                    }
                    if ((this.mouseContainer is Document) && (((int) (this.mouseIndex + 0.5)) > 0))
                    {
                        Element element3 = ((Document) this.mouseContainer).Elements[((int) (this.mouseIndex + 0.5)) - 1];
                        if ((element3 is CharInfo) && ((element3 as CharInfo).Char == '\r'))
                        {
                            Region region2 = new Region(new RectangleF(((Document) this.mouseContainer).DocLocation.X, element3.LineContainer.Top, ((Document) this.mouseContainer).DocWidth, element3.LineContainer.Height));
                            if (region2.IsVisible(e.Location))
                            {
                                this.mouseIndex--;
                            }
                        }
                    }
                    this.selectedTextLast.Add((int) (this.mouseIndex + 0.5));
                    this.selectedTextNew.Add((int) (this.mouseIndex + 0.5));
                    this.selectedContainers.Add(this.mouseContainer);
                    this.caretContainer = this.mouseContainer;
                    this.caretIndex = (int) (this.mouseIndex + 0.5);
                    this.beginSelectContainer = this.mouseContainer;
                    this.beginSelectIndex = this.mouseIndex;
                    this.hasCaret = true;
                    this.UpdateCaretLocation();
                    this.virtualCaretX = this.caretLocation.X;
                    base.Invalidate();
                }
            }
        }

        private void SuperBox_MouseHover(object sender, EventArgs e)
        {
            Point pointLocation = base.PointToClient(Control.MousePosition);
            if (this.PointInOptions(this.mainDocument, pointLocation, true))
            {
                this.tip.Show("点击选项答题", this, pointLocation.X + Cursor.Current.Size.Width, pointLocation.Y, 0x7d0);
            }
        }

        private void SuperBox_MouseMove(object sender, MouseEventArgs e)
        {
            int num;
            int num2;
            int num3;
            float num4;
            float num5;
            int num6;
            int num7;
            Region region;
            Document document;
            Element element;
            Element element2;
            int num8;
            lineexpression lineexpression;
            int num9;
            TableInfo info;
            Cell cell;
            PictureInfo info2;
            Document document2;
            if (e.Button == MouseButtons.Left)
            {
                if (this.mouseState == MouseState.Select)
                {
                    this.FindMouse(this.mainDocument, e.Location, true);
                    int index = 0;
                    while (index < (this.selectedContainers.Count - 2))
                    {
                        object obj2 = this.selectedContainers[index];
                        num2 = Math.Min(this.selectedTextLast[index], this.selectedTextNew[index]);
                        num3 = Math.Max(this.selectedTextLast[index], this.selectedTextNew[index]);
                        num4 = 0f;
                        num5 = 0f;
                        num6 = 0;
                        num7 = 0;
                        region = new Region(new RectangleF(0f, 0f, 0f, 0f));
                        if (this.selectedContainers[index] is Document)
                        {
                            document = this.selectedContainers[index] as Document;
                            if ((document.Elements.Count > 0) && (num2 != 0))
                            {
                                element = document.Elements[num2 - 1];
                                num4 = element.Location.X + element.OutSize.Width;
                                num6 = document.Lines.IndexOf(element.LineContainer);
                            }
                            else
                            {
                                num4 = document.DocLocation.X + document.Margin.Left;
                                num6 = 0;
                            }
                            if ((document.Elements.Count > 0) && (num3 != 0))
                            {
                                element2 = document.Elements[num3 - 1];
                                num5 = element2.Location.X + element2.OutSize.Width;
                                num7 = document.Lines.IndexOf(element2.LineContainer);
                            }
                            else
                            {
                                num5 = document.DocLocation.X + document.Margin.Left;
                                num7 = 0;
                            }
                            if (num6 == num7)
                            {
                                region.Union(new RectangleF(num4, document.Lines[num6].Top, num5 - num4, document.Lines[num6].Height));
                            }
                            else
                            {
                                region.Union(new RectangleF(num4, document.Lines[num6].Top, document.Lines[num6].Right - num4, document.Lines[num6].Height));
                                for (num8 = num6 + 1; num8 < num7; num8++)
                                {
                                    region.Union(new RectangleF(document.Lines[num8].Left, document.Lines[num8].Top, document.Lines[num8].Right - document.Lines[num8].Left, document.Lines[num8].Height));
                                }
                                region.Union(new RectangleF(document.Lines[num7].Left, document.Lines[num7].Top, num5 - document.Lines[num7].Left, document.Lines[num7].Height));
                            }
                        }
                        else if (this.selectedContainers[index] is lineexpression)
                        {
                            lineexpression = this.selectedContainers[index] as lineexpression;
                            num9 = num2;
                            while (num9 < Math.Min(num3, lineexpression.Child.Count))
                            {
                                if ((lineexpression.Child != null) && (lineexpression.Child.Count > num9))
                                {
                                    region.Union(new RectangleF(lineexpression.Child[num9].InputLocation, lineexpression.Child[num9].Region));
                                }
                                num9++;
                            }
                        }
                        else if (this.selectedContainers[index] is TableInfo)
                        {
                            info = this.selectedContainers[index] as TableInfo;
                            num9 = num2;
                            while (num9 < Math.Min(num3, info.Items.Count))
                            {
                                cell = info.Items[num9];
                                region.Union(new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
                                num9++;
                            }
                        }
                        else if (this.selectedContainers[index] is PictureInfo)
                        {
                            info2 = this.selectedContainers[index] as PictureInfo;
                            for (num9 = num2; num9 < Math.Min(num3, info2.Documents.Count); num9++)
                            {
                                document2 = info2.Documents[num9];
                                region.Union(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
                            }
                        }
                        if (region.IsVisible(e.Location))
                        {
                            this.selectedContainers.RemoveAt(index);
                            this.selectedTextLast.RemoveAt(index);
                            this.selectedTextNew.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                    List<object> list = new List<object>();
                    List<object> list2 = new List<object>();
                    object mouseContainer = this.mouseContainer;
                    list.Insert(0, mouseContainer);
                    while ((mouseContainer != this.mainDocument) && (mouseContainer != null))
                    {
                        if (mouseContainer is Element)
                        {
                            mouseContainer = (mouseContainer as Element).DocumentContainer;
                        }
                        else if (mouseContainer is Document)
                        {
                            mouseContainer = (mouseContainer as Document).Parent;
                        }
                        else if (mouseContainer is expression)
                        {
                            if (mouseContainer is lineexpression)
                            {
                                if (mouseContainer is containerexpression)
                                {
                                    mouseContainer = (mouseContainer as containerexpression).Info;
                                }
                                else
                                {
                                    mouseContainer = (mouseContainer as lineexpression).ParentExpression;
                                }
                            }
                            else if (mouseContainer is structexpression)
                            {
                                mouseContainer = (mouseContainer as structexpression).ParentExpression;
                            }
                            else
                            {
                                mouseContainer = (mouseContainer as expression).ParentExpression;
                            }
                        }
                        list.Insert(0, mouseContainer);
                    }
                    mouseContainer = this.beginSelectContainer;
                    list2.Insert(0, mouseContainer);
                    while ((mouseContainer != this.mainDocument) && (mouseContainer != null))
                    {
                        if (mouseContainer is Element)
                        {
                            mouseContainer = (mouseContainer as Element).DocumentContainer;
                        }
                        else if (mouseContainer is Document)
                        {
                            mouseContainer = (mouseContainer as Document).Parent;
                        }
                        else if (mouseContainer is expression)
                        {
                            if (mouseContainer is lineexpression)
                            {
                                if (mouseContainer is containerexpression)
                                {
                                    mouseContainer = (mouseContainer as containerexpression).Info;
                                }
                                else
                                {
                                    mouseContainer = (mouseContainer as lineexpression).ParentExpression;
                                }
                            }
                            else if (mouseContainer is structexpression)
                            {
                                mouseContainer = (mouseContainer as structexpression).ParentExpression;
                            }
                            else
                            {
                                mouseContainer = (mouseContainer as expression).ParentExpression;
                            }
                        }
                        list2.Insert(0, mouseContainer);
                    }
                    if (list.Contains(null) || list2.Contains(null))
                    {
                        return;
                    }
                    object obj4 = null;
                    int num11 = 0;
                    for (num = 0; num < Math.Min(list.Count, list2.Count); num++)
                    {
                        if (list2[num] == list[num])
                        {
                            obj4 = list[num];
                            num11 = num;
                        }
                    }
                    if (obj4 == null)
                    {
                        return;
                    }
                    this.selectedContainers[this.selectedContainers.Count - 1] = obj4;
                    if ((obj4 == this.mouseContainer) && (obj4 == this.beginSelectContainer))
                    {
                        this.selectedTextNew[this.selectedTextNew.Count - 1] = (int) (0.5 + this.mouseIndex);
                        this.selectedTextLast[this.selectedTextLast.Count - 1] = (int) (0.5 + this.beginSelectIndex);
                    }
                    else
                    {
                        int num12;
                        int num13;
                        if ((obj4 != this.mouseContainer) && (obj4 == this.beginSelectContainer))
                        {
                            num12 = 0;
                            num13 = 0;
                            if (obj4 is Document)
                            {
                                num12 = (list[num11 + 1] as Element).Index;
                                num13 = (int) (0.5 + this.beginSelectIndex);
                                if (num12 > num13)
                                {
                                    num12++;
                                }
                            }
                            else if (obj4 is TableInfo)
                            {
                                num12 = (obj4 as TableInfo).Items.IndexOf(list[num11 + 1] as Cell);
                                num13 = (int) (0.5 + this.beginSelectIndex);
                                if (num12 > num13)
                                {
                                    num12++;
                                }
                            }
                            else if (obj4 is PictureInfo)
                            {
                                num12 = (obj4 as PictureInfo).Documents.IndexOf(list[num11 + 1] as Document);
                                num13 = (int) (0.5 + this.beginSelectIndex);
                                if (num12 > num13)
                                {
                                    num12++;
                                }
                            }
                            else if (obj4 is lineexpression)
                            {
                                num12 = (obj4 as lineexpression).Child.IndexOf(list[num11 + 1] as structexpression);
                                num13 = (int) (0.5 + this.beginSelectIndex);
                                if (num12 > num13)
                                {
                                    num12++;
                                }
                            }
                            this.selectedTextNew[this.selectedTextNew.Count - 1] = num12;
                            this.selectedTextLast[this.selectedTextLast.Count - 1] = num13;
                        }
                        else if ((obj4 == this.mouseContainer) && (obj4 != this.beginSelectContainer))
                        {
                            num12 = 0;
                            num13 = 0;
                            if (obj4 is Document)
                            {
                                num13 = (list2[num11 + 1] as Element).Index;
                                num12 = (int) (0.5 + this.mouseIndex);
                                if (num12 < num13)
                                {
                                    num13++;
                                }
                            }
                            else if (obj4 is TableInfo)
                            {
                                num13 = (obj4 as TableInfo).Items.IndexOf(list2[num11 + 1] as Cell);
                                num12 = (int) (0.5 + this.mouseIndex);
                                if (num12 < num13)
                                {
                                    num13++;
                                }
                            }
                            else if (obj4 is PictureInfo)
                            {
                                num13 = (obj4 as PictureInfo).Documents.IndexOf(list2[num11 + 1] as Document);
                                num12 = (int) (0.5 + this.mouseIndex);
                                if (num12 < num13)
                                {
                                    num13++;
                                }
                            }
                            else if (obj4 is lineexpression)
                            {
                                num13 = (obj4 as lineexpression).Child.IndexOf(list2[num11 + 1] as structexpression);
                                num12 = (int) (0.5 + this.mouseIndex);
                                if (num12 < num13)
                                {
                                    num13++;
                                }
                            }
                            this.selectedTextNew[this.selectedTextNew.Count - 1] = num12;
                            this.selectedTextLast[this.selectedTextLast.Count - 1] = num13;
                        }
                        else if ((obj4 != this.mouseContainer) && (obj4 != this.beginSelectContainer))
                        {
                            num12 = 0;
                            num13 = 0;
                            if (obj4 is Document)
                            {
                                num12 = (list[num11 + 1] as Element).Index;
                                num13 = (list2[num11 + 1] as Element).Index;
                                if (num12 >= num13)
                                {
                                    num12++;
                                }
                                else
                                {
                                    num13++;
                                }
                            }
                            else if (obj4 is TableInfo)
                            {
                                num12 = (obj4 as TableInfo).Items.IndexOf(list[num11 + 1] as Cell);
                                num13 = (obj4 as TableInfo).Items.IndexOf(list2[num11 + 1] as Cell);
                                if (num12 >= num13)
                                {
                                    num12++;
                                }
                                else
                                {
                                    num13++;
                                }
                            }
                            else if (obj4 is PictureInfo)
                            {
                                num12 = (obj4 as PictureInfo).Documents.IndexOf(list[num11 + 1] as Document);
                                num13 = (obj4 as PictureInfo).Documents.IndexOf(list2[num11 + 1] as Document);
                                if (num12 >= num13)
                                {
                                    num12++;
                                }
                                else
                                {
                                    num13++;
                                }
                            }
                            else if (obj4 is structexpression)
                            {
                                structexpression item = obj4 as structexpression;
                                num13 = item.Child.IndexOf(list2[num11 + 1] as lineexpression);
                                num12 = item.Child.IndexOf(list[num11 + 1] as lineexpression);
                                lineexpression parentExpression = item.ParentExpression;
                                this.selectedContainers[this.selectedContainers.Count - 1] = parentExpression;
                                if (num13 < num12)
                                {
                                    num13 = parentExpression.Child.IndexOf(item);
                                    num12 = num13 + 1;
                                }
                                else
                                {
                                    num12 = parentExpression.Child.IndexOf(item);
                                    num13 = num12 + 1;
                                }
                            }
                            else if (obj4 is lineexpression)
                            {
                                num13 = (obj4 as lineexpression).Child.IndexOf(list2[num11 + 1] as structexpression);
                                num12 = (obj4 as lineexpression).Child.IndexOf(list[num11 + 1] as structexpression);
                                if (num12 >= num13)
                                {
                                    num12++;
                                }
                                else
                                {
                                    num13++;
                                }
                            }
                            this.selectedTextLast[this.selectedTextLast.Count - 1] = num13;
                            this.selectedTextNew[this.selectedTextNew.Count - 1] = num12;
                        }
                    }
                    this.caretContainer = this.selectedContainers[this.selectedContainers.Count - 1];
                    this.caretIndex = this.selectedTextNew[this.selectedTextNew.Count - 1];
                    this.hasCaret = true;
                    this.UpdateCaretLocation();
                }
                else
                {
                    Cell boundDragedObject;
                    TableInfo parent;
                    Point rowColumn;
                    Cell cell3;
                    PictureInfo info4;
                    if (this.mouseState == MouseState.DragBoundWE)
                    {
                        float num14;
                        if (this.boundDragedObject is Cell)
                        {
                            boundDragedObject = this.boundDragedObject as Cell;
                            parent = boundDragedObject.Parent as TableInfo;
                            rowColumn = parent.GetRowColumn(boundDragedObject);
                            if (this.dragBound == Bound.Right)
                            {
                                for (num = 0; num < parent.TableSize.X; num++)
                                {
                                    cell3 = parent.Rows[num].Cells[rowColumn.Y];
                                    cell3.DocWidth = (e.Location.X - cell3.DocLocation.X) - cell3.Margin.Horizontal;
                                }
                            }
                            else if (this.dragBound == Bound.Left)
                            {
                                Cell cell4;
                                if (rowColumn.Y > 0)
                                {
                                    for (num = 0; num < parent.TableSize.X; num++)
                                    {
                                        cell4 = parent.Rows[num].Cells[rowColumn.Y - 1];
                                        cell4.DocWidth = (e.Location.X - cell4.DocLocation.X) - cell4.Margin.Horizontal;
                                    }
                                }
                                else
                                {
                                    num14 = ((parent.Location.X + parent.Margin.Left) + parent.LineWidth) + parent.Columns[0].Width;
                                    parent.Location = new PointF((float) (e.Location.X - parent.Margin.Left), parent.Location.Y);
                                    for (num = 0; num < parent.TableSize.X; num++)
                                    {
                                        cell4 = parent.Rows[num].Cells[rowColumn.Y];
                                        cell4.DocWidth = (num14 - ((parent.Location.X + parent.Margin.Left) + parent.LineWidth)) - cell4.Margin.Horizontal;
                                    }
                                }
                            }
                        }
                        else if (this.boundDragedObject is PictureInfo)
                        {
                            info4 = this.boundDragedObject as PictureInfo;
                            if (this.dragBound == Bound.Right)
                            {
                                info4.ImageShowSize = new SizeF((e.Location.X - info4.Location.X) - info4.Margin.Left, info4.ImageShowSize.Height);
                            }
                            else if (this.dragBound == Bound.Right)
                            {
                                num14 = (info4.Location.X + info4.ImageShowSize.Width) + info4.Margin.Left;
                                info4.Location = new PointF((float) (e.Location.X - info4.Margin.Left), info4.Location.Y);
                                info4.ImageShowSize = new SizeF(num14 - e.Location.X, info4.ImageShowSize.Height);
                            }
                        }
                        this.hasCaret = false;
                        this.UpdateCaretLocation();
                        this.virtualCaretX = this.caretLocation.X;
                    }
                    else if (this.mouseState == MouseState.DragBoundNS)
                    {
                        if (this.boundDragedObject is Cell)
                        {
                            boundDragedObject = this.boundDragedObject as Cell;
                            parent = boundDragedObject.Parent as TableInfo;
                            rowColumn = parent.GetRowColumn(boundDragedObject);
                            if (this.dragBound == Bound.Top)
                            {
                                if (rowColumn.X > 0)
                                {
                                    for (num = 0; num < parent.TableSize.Y; num++)
                                    {
                                        cell3 = parent.Columns[num].Cells[rowColumn.X];
                                        cell3.MinHeight = (e.Location.Y - cell3.DocLocation.Y) - cell3.Margin.Vertical;
                                    }
                                }
                            }
                            else if (this.dragBound == Bound.Bottom)
                            {
                                for (num = 0; num < parent.TableSize.Y; num++)
                                {
                                    cell3 = parent.Columns[num].Cells[rowColumn.X];
                                    cell3.MinHeight = (e.Location.Y - cell3.DocLocation.Y) - cell3.Margin.Vertical;
                                }
                            }
                        }
                        else if (this.boundDragedObject is PictureInfo)
                        {
                            info4 = this.boundDragedObject as PictureInfo;
                            if (this.dragBound == Bound.Top)
                            {
                                float num15 = (info4.Location.Y + info4.Margin.Top) + info4.ImageShowSize.Height;
                                info4.Location = new PointF(info4.Location.X, (float) (e.Location.Y - info4.Margin.Top));
                                info4.ImageShowSize = new SizeF(info4.ImageShowSize.Width, num15 - e.Location.Y);
                            }
                            else if (this.dragBound == Bound.Bottom)
                            {
                                info4.ImageShowSize = new SizeF(info4.ImageShowSize.Width, (e.Location.Y - info4.Location.Y) - info4.Margin.Top);
                            }
                        }
                        this.hasCaret = false;
                        this.UpdateCaretLocation();
                        this.virtualCaretX = this.caretLocation.X;
                    }
                    else if ((this.mouseState != MouseState.DragElement) && (((this.mouseState == MouseState.Choice) || (this.mouseState == MouseState.InBlank)) || (this.mouseState == MouseState.InOperate)))
                    {
                        this.Cursor = Cursors.Hand;
                        this.hasCaret = false;
                        this.UpdateCaretLocation();
                        this.virtualCaretX = this.caretLocation.X;
                    }
                }
                base.Invalidate();
            }
            else
            {
                this.FindMouse(this.mainDocument, e.Location, false);
                bool flag = false;
                for (num = 0; num < this.selectedContainers.Count; num++)
                {
                    num2 = Math.Min(this.selectedTextLast[num], this.selectedTextNew[num]);
                    num3 = Math.Max(this.selectedTextLast[num], this.selectedTextNew[num]);
                    num4 = 0f;
                    num5 = 0f;
                    num6 = 0;
                    num7 = 0;
                    region = new Region(new RectangleF(0f, 0f, 0f, 0f));
                    if (this.selectedContainers[num] is Document)
                    {
                        document = this.selectedContainers[num] as Document;
                        if ((document.Elements.Count > 0) && (num2 != 0))
                        {
                            element = document.Elements[num2 - 1];
                            num4 = element.Location.X + element.OutSize.Width;
                            num6 = document.Lines.IndexOf(element.LineContainer);
                        }
                        else
                        {
                            num4 = document.DocLocation.X + document.Margin.Left;
                            num6 = 0;
                        }
                        if ((document.Elements.Count > 0) && (num3 != 0))
                        {
                            element2 = document.Elements[num3 - 1];
                            num5 = element2.Location.X + element2.OutSize.Width;
                            num7 = document.Lines.IndexOf(element2.LineContainer);
                        }
                        else
                        {
                            num5 = document.DocLocation.X + document.Margin.Left;
                            num7 = 0;
                        }
                        if (num6 == num7)
                        {
                            region.Union(new RectangleF(num4, document.Lines[num6].Top, num5 - num4, document.Lines[num6].Height));
                        }
                        else
                        {
                            region.Union(new RectangleF(num4, document.Lines[num6].Top, document.Lines[num6].Right - num4, document.Lines[num6].Height));
                            for (num8 = num6 + 1; num8 < num7; num8++)
                            {
                                region.Union(new RectangleF(document.Lines[num8].Left, document.Lines[num8].Top, document.Lines[num8].Right - document.Lines[num8].Left, document.Lines[num8].Height));
                            }
                            region.Union(new RectangleF(document.Lines[num7].Left, document.Lines[num7].Top, num5 - document.Lines[num7].Left, document.Lines[num7].Height));
                        }
                    }
                    else if (this.selectedContainers[num] is lineexpression)
                    {
                        lineexpression = this.selectedContainers[num] as lineexpression;
                        num9 = num2;
                        while (num9 < Math.Min(num3, lineexpression.Child.Count))
                        {
                            if ((lineexpression.Child != null) && (lineexpression.Child.Count > num9))
                            {
                                region.Union(new RectangleF(lineexpression.Child[num9].InputLocation, lineexpression.Child[num9].Region));
                            }
                            num9++;
                        }
                    }
                    else if (this.selectedContainers[num] is TableInfo)
                    {
                        info = this.selectedContainers[num] as TableInfo;
                        num9 = num2;
                        while (num9 < Math.Min(num3, info.Items.Count))
                        {
                            cell = info.Items[num9];
                            region.Union(new RectangleF(cell.DocLocation, new SizeF(cell.OutWidth, cell.OutHeight)));
                            num9++;
                        }
                    }
                    else if (this.selectedContainers[num] is PictureInfo)
                    {
                        info2 = this.selectedContainers[num] as PictureInfo;
                        for (num9 = num2; num9 < Math.Min(num3, info2.Documents.Count); num9++)
                        {
                            document2 = info2.Documents[num9];
                            region.Union(new RectangleF(document2.DocLocation, new SizeF(document2.OutWidth, document2.OutHeight)));
                        }
                    }
                    if (region.IsVisible(e.Location))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    this.Cursor = Cursors.Arrow;
                }
                else
                {
                    switch (this.mouseState)
                    {
                        case MouseState.None:
                            this.Cursor = Cursors.Arrow;
                            break;

                        case MouseState.Select:
                            this.Cursor = Cursors.IBeam;
                            break;

                        case MouseState.DragElement:
                            this.Cursor = Cursors.Arrow;
                            break;

                        case MouseState.DragBoundNS:
                            this.Cursor = Cursors.SizeNS;
                            break;

                        case MouseState.DragBoundWE:
                            this.Cursor = Cursors.SizeWE;
                            break;

                        case MouseState.AboveImage:
                            this.Cursor = Cursors.SizeAll;
                            break;

                        case MouseState.Choice:
                            this.Cursor = Cursors.Hand;
                            break;

                        case MouseState.InBlank:
                            this.Cursor = Cursors.Hand;
                            break;

                        case MouseState.InOperate:
                            this.Cursor = Cursors.Hand;
                            break;
                    }
                }
            }
        }

        private void SuperBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.FindMouse(this.mainDocument, e.Location, false);
            if (this.mouseState == MouseState.None)
            {
                this.hasCaret = false;
            }
            else
            {
                this.hasCaret = true;
            }
            this.UpdateCaretLocation();
            this.virtualCaretX = this.caretLocation.X;
        }

        private void SuperBox_SizeChanged(object sender, EventArgs e)
        {
            if ((this.mainDocument != null) && (this.mainDocument.DocWidth != (base.Width - this.mainDocument.Margin.Horizontal)))
            {
                this.mainDocument.DocWidth = base.Width - this.mainDocument.Margin.Horizontal;
            }
        }

        private void UpdateCaretLocation()
        {
            if (!this.hasCaret)
            {
                Qisi.Editor.NativeMethods.HideCaret(base.Handle);
                Qisi.Editor.NativeMethods.DestroyCaret();
                this.caretHeight = 0f;
            }
            else
            {
                bool flag = true;
                bool flag2 = false;
                if (this.caretContainer is Document)
                {
                    Element element;
                    Document caretContainer = this.caretContainer as Document;
                    if ((caretContainer.Elements.Count > 0) && (this.caretIndex != 0))
                    {
                        element = caretContainer.Elements[this.caretIndex - 1];
                        this.Font = element.Font;
                        if (this.caretHeight != this.Font.Height)
                        {
                            this.caretHeight = this.Font.Height;
                            flag2 = true;
                        }
                        if ((element is CharInfo) && ((element as CharInfo).Char == '\r'))
                        {
                            Line lineContainer = element.LineContainer;
                            int index = caretContainer.Lines.IndexOf(lineContainer);
                            this.caretLocation.X = caretContainer.Lines[index + 1].Left;
                            FontFamily fontFamily = this.Font.FontFamily;
                            this.caretLocation.Y = (caretContainer.Lines[index + 1].BaseLine + caretContainer.Lines[index + 1].Top) - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
                        }
                        else
                        {
                            this.caretLocation.X = element.Location.X + element.OutSize.Width;
                            this.caretLocation.Y = (element.LineContainer.BaseLine + element.LineContainer.Top) - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
                        }
                    }
                    else if ((caretContainer.Elements.Count > 0) && (this.caretIndex == 0))
                    {
                        element = caretContainer.Elements[this.caretIndex];
                        this.Font = element.Font;
                        if (this.caretHeight != this.Font.Height)
                        {
                            this.caretHeight = this.Font.Height;
                            flag2 = true;
                        }
                        this.caretLocation.X = element.Location.X;
                        this.caretLocation.Y = (element.LineContainer.BaseLine + element.LineContainer.Top) - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
                    }
                    else
                    {
                        this.Font = caretContainer.DefaultFont;
                        if (this.caretHeight != this.Font.Height)
                        {
                            this.caretHeight = this.Font.Height;
                            flag2 = true;
                        }
                        this.caretLocation.X = caretContainer.DocLocation.X + caretContainer.Margin.Left;
                        this.caretLocation.Y = (caretContainer.Lines[0].BaseLine + caretContainer.Lines[0].Top) - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
                    }
                }
                else if (this.caretContainer is lineexpression)
                {
                    lineexpression parentExpression = this.caretContainer as lineexpression;
                    if ((parentExpression.Child != null) && (parentExpression.Child.Count > 0))
                    {
                        structexpression structexpression;
                        if (this.caretIndex != 0)
                        {
                            structexpression = parentExpression.Child[this.caretIndex - 1];
                            this.caretLocation.X = structexpression.InputLocation.X + structexpression.Region.Width;
                        }
                        else
                        {
                            structexpression = parentExpression.Child[this.caretIndex];
                            this.caretLocation.X = structexpression.InputLocation.X;
                        }
                    }
                    else
                    {
                        this.caretLocation.X = parentExpression.InputLocation.X;
                    }
                    this.Font = parentExpression.Font;
                    if (this.caretHeight != this.Font.Height)
                    {
                        this.caretHeight = this.Font.Height;
                        flag2 = true;
                    }
                    this.caretLocation.Y = (parentExpression.InputLocation.Y + parentExpression.BaseLine) - Qisi.Editor.CommonMethods.CalcAscentPixel(this.Font);
                    while (!(parentExpression is containerexpression))
                    {
                        parentExpression = parentExpression.ParentExpression.ParentExpression;
                    }
                    ExpressionInfo info = (parentExpression as containerexpression).Info;
                    if (info != null)
                    {
                        Document documentContainer = info.DocumentContainer;
                        while (true)
                        {
                            Region region = new Region(new RectangleF(documentContainer.DocLocation.X + documentContainer.Margin.Left, documentContainer.DocLocation.Y + documentContainer.Margin.Top, documentContainer.DocWidth, documentContainer.DocHeight));
                            if (!region.IsVisible(this.caretLocation))
                            {
                                flag = false;
                                break;
                            }
                            if (documentContainer == this.mainDocument)
                            {
                                break;
                            }
                            documentContainer = documentContainer.Parent.DocumentContainer;
                        }
                    }
                }
                if (flag)
                {
                    if (flag2)
                    {
                        Qisi.Editor.NativeMethods.HideCaret(base.Handle);
                        Qisi.Editor.NativeMethods.DestroyCaret();
                        Qisi.Editor.NativeMethods.CreateCaret(base.Handle, IntPtr.Zero, 1, (int) this.caretHeight);
                        Qisi.Editor.NativeMethods.ShowCaret(base.Handle);
                    }
                    Qisi.Editor.NativeMethods.SetCaretPos((int) this.caretLocation.X, (int) this.caretLocation.Y);
                }
                else
                {
                    Qisi.Editor.NativeMethods.HideCaret(base.Handle);
                    Qisi.Editor.NativeMethods.DestroyCaret();
                    this.caretHeight = 0f;
                }
            }
        }

        private void 删除行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 删除列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 在上方插入行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 在下方插入行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 在右侧插入列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 在左侧插入列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        protected override bool CanEnableIme
        {
            get
            {
                return true;
            }
        }

        internal List<Element> Elements
        {
            get
            {
                return this.mainDocument.Elements;
            }
        }

        [DefaultValue(true), Description("限制输入"), DisplayName("InputLimited"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), Category("Text"), Localizable(true)]
        public bool InputLimited { get; set; }

        [DefaultValue(false), Category("Text"), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Localizable(true), Description("只读"), DisplayName("ReadOnly")]
        public bool ReadOnly { get; set; }

        private ToolStripMenuType ToolStripState
        {
            get
            {
                return this.toolStripMenuType;
            }
            set
            {
                if (this.ReadOnly)
                {
                    this.toolStripMenuType = ToolStripMenuType.None;
                    this.contextMenuStrip1.Items.Clear();
                    this.ContextMenuStrip = null;
                }
                else
                {
                    this.toolStripMenuType = value;
                    this.contextMenuStrip1.Items.Clear();
                    if ((value & ToolStripMenuType.General) == ToolStripMenuType.General)
                    {
                        if (this.contextMenuStrip1.Items.Count != 0)
                        {
                            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        }
                        this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("剪切", Resources.CutHS, new EventHandler(this.Cut)));
                        this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("复制", Resources.CopyHS, new EventHandler(this.Copy)));
                        this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("粘贴", Resources.PasteHS, new EventHandler(this.Paste)));
                        this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("选择性粘贴", Resources.PasteHS, new EventHandler(this.SelectablePaste)));
                        this.ContextMenuStrip = this.contextMenuStrip1;
                    }
                    if ((value & ToolStripMenuType.Insert) == ToolStripMenuType.Insert)
                    {
                        if (this.contextMenuStrip1.Items.Count != 0)
                        {
                            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        }
                        ToolStripMenuItem item = new ToolStripMenuItem("插入填空格");
                        item.DropDownItems.Add(new ToolStripMenuItem("支持Enter的填空格", null, new EventHandler(this.InsertBlankEnterable)));
                        item.DropDownItems.Add(new ToolStripMenuItem("忽略Enter的填空格", null, new EventHandler(this.InsertBlankEnterless)));
                        this.contextMenuStrip1.Items.Add(item);
                        this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("插入选项", null, new EventHandler(this.InsertOptions)));
                        this.ContextMenuStrip = this.contextMenuStrip1;
                    }
                }
            }
        }

        private enum Bound
        {
            Top,
            Right,
            Bottom,
            Left
        }

        public delegate void MessageEventHandler(object sender, MessageEventArgs e);

        private enum MouseState
        {
            None,
            Select,
            DragElement,
            DragBoundNS,
            DragBoundWE,
            AboveImage,
            Choice,
            DrawPen,
            DrawLine,
            InBlank,
            InOperate
        }

        [Flags]
        private enum ToolStripMenuType
        {
            General = 2,
            Insert = 0x10,
            Matrix = 8,
            None = 1,
            Table = 4
        }
    }
}

