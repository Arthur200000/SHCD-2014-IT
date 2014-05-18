namespace Qisi.General.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms;

    public class PopupHost : ToolStripDropDown
    {
        private PopupHost _childPopup;
        private ToolStripControlHost _controlHost;
        private PopupHost _ownerPopup;
        private Control _popupControl;
        private bool _resizableLeft;
        private bool _resizableTop;

        public PopupHost(Control c)
        {
            this.DoubleBuffered = true;
            base.ResizeRedraw = true;
            this.AutoSize = false;
            this.CanResize = false;
            this.BorderColor = Color.Black;
            base.Padding = Padding.Empty;
            base.Margin = Padding.Empty;
            this.CreateHost(c);
        }

        private void CreateHost(Control control)
        {
            if (control == null)
            {
                throw new ArgumentException("control");
            }
            this._popupControl = control;
            this._controlHost = new ToolStripControlHost(control, "PopupHost");
            this._controlHost.AutoSize = false;
            this._controlHost.Padding = Padding.Empty;
            this._controlHost.Margin = Padding.Empty;
            base.Size = new Size(control.Size.Width + base.Padding.Horizontal, control.Size.Height + base.Padding.Vertical);
            base.Items.Add(this._controlHost);
        }

        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            this._popupControl.RegionChanged -= new EventHandler(this.PopupControlRegionChanged);
            base.OnClosing(e);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            Control control = this._popupControl;
            if (control != null)
            {
                Qisi.General.Controls.NativeMethods.MINMAXINFO structure = (Qisi.General.Controls.NativeMethods.MINMAXINFO) Marshal.PtrToStructure(m.LParam, typeof(Qisi.General.Controls.NativeMethods.MINMAXINFO));
                if (control.MaximumSize.Width != 0)
                {
                    structure.maxTrackSize.Width = control.MaximumSize.Width;
                }
                if (control.MaximumSize.Height != 0)
                {
                    structure.maxTrackSize.Height = control.MaximumSize.Height;
                }
                structure.minTrackSize = new Size(100, 100);
                if (control.MinimumSize.Width > structure.minTrackSize.Width)
                {
                    structure.minTrackSize.Width = control.MinimumSize.Width + base.Padding.Horizontal;
                }
                if (control.MinimumSize.Height > structure.minTrackSize.Height)
                {
                    structure.minTrackSize.Height = control.MinimumSize.Height + base.Padding.Vertical;
                }
                Marshal.StructureToPtr(structure, m.LParam, false);
            }
            return true;
        }

        private bool OnNcHitTest(ref Message m)
        {
            Point pt = base.PointToClient(new Point(Qisi.General.Controls.NativeMethods.LOWORD(m.LParam), Qisi.General.Controls.NativeMethods.HIWORD(m.LParam)));
            Rectangle empty = Rectangle.Empty;
            if (this.CanResize && !this.ChangeRegion)
            {
                if (this._resizableLeft)
                {
                    if (this._resizableTop)
                    {
                        empty = new Rectangle(0, 0, 6, 6);
                    }
                    else
                    {
                        empty = new Rectangle(0, base.Height - 6, 6, 6);
                    }
                }
                else if (this._resizableTop)
                {
                    empty = new Rectangle(base.Width - 6, 0, 6, 6);
                }
                else
                {
                    empty = new Rectangle(base.Width - 6, base.Height - 6, 6, 6);
                }
            }
            if (empty.Contains(pt))
            {
                if (this._resizableLeft)
                {
                    if (this._resizableTop)
                    {
                        m.Result = (IntPtr) 13;
                        return true;
                    }
                    m.Result = (IntPtr) 0x10;
                    return true;
                }
                if (this._resizableTop)
                {
                    m.Result = (IntPtr) 14;
                    return true;
                }
                m.Result = (IntPtr) 0x11;
                return true;
            }
            Rectangle clientRectangle = base.ClientRectangle;
            if (((pt.X > (clientRectangle.Right - 3)) && (pt.X <= clientRectangle.Right)) && !this._resizableLeft)
            {
                m.Result = (IntPtr) 11;
                return true;
            }
            if (((pt.Y > (clientRectangle.Bottom - 3)) && (pt.Y <= clientRectangle.Bottom)) && !this._resizableTop)
            {
                m.Result = (IntPtr) 15;
                return true;
            }
            if (((pt.X > -1) && (pt.X < 3)) && this._resizableLeft)
            {
                m.Result = (IntPtr) 10;
                return true;
            }
            if (((pt.Y > -1) && (pt.Y < 3)) && this._resizableTop)
            {
                m.Result = (IntPtr) 12;
                return true;
            }
            return false;
        }

        protected override void OnOpened(EventArgs e)
        {
            if (this.OpenFocused)
            {
                this._popupControl.Focus();
            }
            base.OnOpened(e);
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            if (this._popupControl.IsDisposed || this._popupControl.Disposing)
            {
                e.Cancel = true;
            }
            else
            {
                this._popupControl.RegionChanged += new EventHandler(this.PopupControlRegionChanged);
                this.UpdateRegion();
            }
            base.OnOpening(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this.ChangeRegion)
            {
                ControlPaint.DrawBorder(e.Graphics, base.ClientRectangle, this.BorderColor, ButtonBorderStyle.Solid);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (this._controlHost != null)
            {
                this._controlHost.Size = new Size(base.Width - base.Padding.Horizontal, base.Height - base.Padding.Vertical);
            }
        }

        private void PopupControlRegionChanged(object sender, EventArgs e)
        {
            this.UpdateRegion();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private bool ProcessGrip(ref Message m)
        {
            if (this.CanResize && !this.ChangeRegion)
            {
                switch (m.Msg)
                {
                    case 0x24:
                        return this.OnGetMinMaxInfo(ref m);

                    case 0x84:
                        return this.OnNcHitTest(ref m);
                }
            }
            return false;
        }

        private void SetOwnerItem(Control control)
        {
            if (control != null)
            {
                if (control is PopupHost)
                {
                    PopupHost host = control as PopupHost;
                    this._ownerPopup = host;
                    this._ownerPopup._childPopup = this;
                    base.OwnerItem = host.Items[0];
                }
                else if (control.Parent != null)
                {
                    this.SetOwnerItem(control.Parent);
                }
            }
        }

        public void Show(Control control)
        {
            this.Show(control, control.ClientRectangle);
        }

        public void Show(Control control, bool center)
        {
            this.Show(control, control.ClientRectangle, center);
        }

        public void Show(Control control, Rectangle rect)
        {
            this.Show(control, rect, false);
        }

        public void Show(Control control, Rectangle rect, bool center)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            this.SetOwnerItem(control);
            if (this.CanResize && !this.ChangeRegion)
            {
                base.Padding = new Padding(3);
            }
            else if (!this.ChangeRegion)
            {
                base.Padding = new Padding(1);
            }
            else
            {
                base.Padding = Padding.Empty;
            }
            int horizontal = base.Padding.Horizontal;
            int vertical = base.Padding.Vertical;
            base.Size = new Size(this._popupControl.Width + horizontal, this._popupControl.Height + vertical);
            this._resizableTop = false;
            this._resizableLeft = false;
            Point p = control.PointToScreen(new Point(rect.Left, rect.Bottom));
            Rectangle workingArea = Screen.FromControl(control).WorkingArea;
            if (center)
            {
                if ((p.X + ((rect.Width + base.Size.Width) / 2)) > workingArea.Right)
                {
                    p.X = workingArea.Right - base.Size.Width;
                    this._resizableLeft = true;
                }
                else
                {
                    p.X -= (base.Size.Width - rect.Width) / 2;
                }
            }
            else if ((p.X + base.Size.Width) > (workingArea.Left + workingArea.Width))
            {
                this._resizableLeft = true;
                p.X = (workingArea.Left + workingArea.Width) - base.Size.Width;
            }
            if ((p.Y + base.Size.Height) > (workingArea.Top + workingArea.Height))
            {
                this._resizableTop = true;
                p.Y -= base.Size.Height + rect.Height;
            }
            p = control.PointToClient(p);
            base.Show(control, p, ToolStripDropDownDirection.BelowRight);
        }

        protected void UpdateRegion()
        {
            if (this.ChangeRegion)
            {
                if (base.Region != null)
                {
                    base.Region.Dispose();
                    base.Region = null;
                }
                if (this._popupControl.Region != null)
                {
                    base.Region = this._popupControl.Region.Clone();
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (!this.ProcessGrip(ref m))
            {
                base.WndProc(ref m);
            }
        }

        public Color BorderColor { get; set; }

        public bool CanResize { get; set; }

        public bool ChangeRegion { get; set; }

        public bool OpenFocused { get; set; }
    }
}

