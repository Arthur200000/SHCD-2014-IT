namespace Qisi.Editor.Controls
{
    using Qisi.Editor;
    using Qisi.Editor.Expression;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class FLabelList : Control
    {
        private int clickregion;
        private int clickregionfoot;
        private int flabelheight;
        private const int head = 20;
        private const int initlines = 3;
        private Size initsize;
        private bool isexpanded;
        private Keys[] keyslist;
        private int moveleft;
        private int moveright;
        private List<List<FLabel>> mylist;
        private List<bool> rightout;

        public FLabelList() : this("其他", 500, true)
        {
        }

        public FLabelList(string subject, int width, bool hashotkey = true)
        {
            int num;
            this.keyslist = new Keys[] { 
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, 
                Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
             };
            this.isexpanded = false;
            base.TabStop = true;
            this.Font = new Font("微软雅黑", 10f, FontStyle.Regular, GraphicsUnit.Pixel, 0x86);
            this.DoubleBuffered = true;
            this.mylist = new List<List<FLabel>>();
            this.rightout = new List<bool>();
            string[] strArray = CommonMethods.Groups(subject).Split(new char[] { ',' });
            foreach (string str in strArray)
            {
                this.rightout.Add(false);
                List<FLabel> item = new List<FLabel>();
                string[] strArray2 = CommonMethods.Exprs(str).Split(new char[] { ',' });
                num = 0;
                while (num < strArray2.Length)
                {
                    item.Add(new FLabel((FType) Enum.Parse(typeof(FType), strArray2[num], true), hashotkey));
                    num++;
                }
                this.mylist.Add(item);
            }
            if (hashotkey)
            {
                this.flabelheight = CommonMethods.height + (CommonMethods.height / 2);
            }
            else
            {
                this.flabelheight = CommonMethods.height;
            }
            int index = 0;
            for (num = 0; num < this.mylist.Count; num++)
            {
                index = num;
                for (int i = 0; i < this.mylist[num].Count; i++)
                {
                    base.Controls.Add(this.mylist[num][i]);
                    this.mylist[num][i].Location = new Point(CommonMethods.height * i, (this.flabelheight * num) + 20);
                    if ((CommonMethods.height * i) > width)
                    {
                        this.rightout[num] = true;
                    }
                    this.mylist[num][i].AppendExpression += new FLabel.AppendExpressionHandler(this.FLabelList_AppenExpression);
                    if (num >= 3)
                    {
                        this.mylist[num][i].Visible = false;
                    }
                    if (hashotkey && (index < this.keyslist.Length))
                    {
                        int id = Qisi.Editor.NativeMethods.GlobalAddAtom(Guid.NewGuid().ToString());
                        while (index < this.keyslist.Length)
                        {
                            if (!Qisi.Editor.NativeMethods.RegisterHotKey(base.Handle, id, Qisi.Editor.NativeMethods.KeyModifiers.Alt, this.keyslist[index]))
                            {
                                id = Qisi.Editor.NativeMethods.GlobalAddAtom(Guid.NewGuid().ToString());
                                index += this.mylist.Count;
                            }
                            else
                            {
                                this.mylist[num][i].HotKey = this.keyslist[index];
                                this.mylist[num][i].HotKeyId = id;
                                index += this.mylist.Count;
                                break;
                            }
                        }
                    }
                }
            }
            this.moveleft = Qisi.Editor.NativeMethods.GlobalAddAtom(Guid.NewGuid().ToString());
            Qisi.Editor.NativeMethods.RegisterHotKey(base.Handle, this.moveleft, Qisi.Editor.NativeMethods.KeyModifiers.Alt, Keys.Left);
            this.moveright = Qisi.Editor.NativeMethods.GlobalAddAtom(Guid.NewGuid().ToString());
            Qisi.Editor.NativeMethods.RegisterHotKey(base.Handle, this.moveright, Qisi.Editor.NativeMethods.KeyModifiers.Alt, Keys.Right);
            this.initsize = new Size(width, (Math.Min(this.mylist.Count, 3) * this.flabelheight) + 40);
            base.Size = new Size(width, (Math.Min(this.mylist.Count, 3) * this.flabelheight) + 40);
            base.Click += new EventHandler(this.FLabelList_Click);
            base.MouseMove += new MouseEventHandler(this.FLabelList_MouseMove);
        }

        private Control ActiveSuperBox(Control c)
        {
            if (c is ContainerControl)
            {
                if ((((ContainerControl) c).ActiveControl != null) && (((ContainerControl) c).ActiveControl.GetType().ToString() == "Qisi.Editor.Controls.SuperBox"))
                {
                    return ((ContainerControl) c).ActiveControl;
                }
                return this.ActiveSuperBox(((ContainerControl) c).ActiveControl);
            }
            return null;
        }

        private void FLabelList_AppenExpression(object sender, ExpressionEventArgs e)
        {
            Form form = base.FindForm();
            if ((form != null) && (form.ActiveControl != null))
            {
                if (form.ActiveControl.GetType().ToString() == "Qisi.Editor.Controls.SuperBox")
                {
                    ((SuperBox) form.ActiveControl).AppendF(e.Type);
                }
                else
                {
                    Control control = this.ActiveSuperBox(form.ActiveControl);
                    if (control != null)
                    {
                        ((SuperBox) control).AppendF(e.Type);
                    }
                }
            }
        }

        private void FLabelList_Click(object sender, EventArgs e)
        {
            Point point = base.PointToClient(Control.MousePosition);
            if (point.Y < 20)
            {
                if (point.X < this.clickregion)
                {
                    this.MoveLeft();
                }
                else if (point.X > (base.Width - this.clickregion))
                {
                    this.MoveRight();
                }
            }
            if ((point.Y > (base.Height - 20)) && (point.X < this.clickregionfoot))
            {
                for (int i = 3; i < this.mylist.Count; i++)
                {
                    for (int j = 0; j < this.mylist[i].Count; j++)
                    {
                        this.mylist[i][j].Visible = !this.isexpanded;
                    }
                }
                if (this.isexpanded)
                {
                    this.initsize = new Size(base.Width, (3 * this.flabelheight) + 40);
                    base.Size = new Size(base.Width, (3 * this.flabelheight) + 40);
                }
                else
                {
                    this.initsize = new Size(base.Width, (this.mylist.Count * this.flabelheight) + 40);
                    base.Size = new Size(base.Width, (this.mylist.Count * this.flabelheight) + 40);
                }
                this.isexpanded = !this.isexpanded;
                base.Invalidate();
            }
        }

        private void FLabelList_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Y < 20) && ((e.X < this.clickregion) || (e.X > (base.Width - this.clickregion))))
            {
                this.Cursor = Cursors.Hand;
            }
            else if ((e.Y > (base.Height - 20)) && (e.X < this.clickregionfoot))
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void MoveLeft()
        {
            for (int i = 0; i < this.mylist.Count; i++)
            {
                if (this.rightout[i])
                {
                    for (int j = 0; j < this.mylist[i].Count; j++)
                    {
                        this.mylist[i][j].Left -= CommonMethods.height;
                        if ((this.mylist[i][j].Left + this.mylist[i][j].Width) < 0)
                        {
                            if (j == 0)
                            {
                                this.mylist[i][j].Left = this.mylist[i][this.mylist[i].Count - 1].Right - CommonMethods.height;
                            }
                            else
                            {
                                this.mylist[i][j].Left = this.mylist[i][j - 1].Right;
                            }
                        }
                    }
                }
            }
        }

        private void MoveRight()
        {
            for (int i = 0; i < this.mylist.Count; i++)
            {
                if (this.rightout[i])
                {
                    for (int j = this.mylist[i].Count - 1; j >= 0; j--)
                    {
                        this.mylist[i][j].Left += CommonMethods.height;
                        if (this.mylist[i][j].Left > base.Width)
                        {
                            if (j == (this.mylist[i].Count - 1))
                            {
                                this.mylist[i][j].Left = (this.mylist[i][0].Left - this.mylist[i][j].Width) + CommonMethods.height;
                            }
                            else
                            {
                                this.mylist[i][j].Left = this.mylist[i][j + 1].Left - this.mylist[i][j].Width;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SizeF ef;
            this.clickregion = 0;
            using (List<bool>.Enumerator enumerator = this.rightout.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current)
                    {
                        ef = e.Graphics.MeasureString("点击此处或者按Alt+→向右滚动查看更多", this.Font, 0);
                        if ((ef.Width * 2f) < base.Width)
                        {
                            this.clickregion = (int) ef.Width;
                            e.Graphics.DrawString("点击此处或者按Alt+←向左滚动查看更多", this.Font, Brushes.Black, (PointF) new Point(0, 0));
                            e.Graphics.DrawString("点击此处或者按Alt+→向右滚动查看更多", this.Font, Brushes.Black, (PointF) new Point(base.Width - ((int) ef.Width), 0));
                        }
                        else
                        {
                            this.clickregion = base.Width / 2;
                        }
                        goto Label_0104;
                    }
                }
            }
        Label_0104:
            if (this.mylist.Count > 3)
            {
                ef = e.Graphics.MeasureString("点此展开", this.Font, 0);
                this.clickregionfoot = (int) ef.Width;
                if (this.isexpanded)
                {
                    e.Graphics.DrawString("点此收起", this.Font, Brushes.Black, (PointF) new Point(0, base.Height - 20));
                }
                else
                {
                    e.Graphics.DrawString("点此展开", this.Font, Brushes.Black, (PointF) new Point(0, base.Height - 20));
                }
            }
            else
            {
                this.clickregionfoot = 0;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (base.Size != this.initsize)
            {
                base.Size = this.initsize;
            }
            base.OnSizeChanged(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x312)
            {
                if (this.moveleft == ((int) m.WParam))
                {
                    this.MoveLeft();
                }
                else if (this.moveright == ((int) m.WParam))
                {
                    this.MoveRight();
                }
                else
                {
                    foreach (List<FLabel> list in this.mylist)
                    {
                        foreach (FLabel label in list)
                        {
                            if (label.HotKeyId == ((int) m.WParam))
                            {
                                Form form = base.FindForm();
                                if ((form != null) && (form.ActiveControl != null))
                                {
                                    if (form.ActiveControl.GetType().ToString() == "Qisi.Editor.Controls.SuperBox")
                                    {
                                        ((SuperBox) form.ActiveControl).AppendF(label.Ftype);
                                    }
                                    else
                                    {
                                        Control control = this.ActiveSuperBox(form.ActiveControl);
                                        if (control != null)
                                        {
                                            ((SuperBox) control).AppendF(label.Ftype);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}

