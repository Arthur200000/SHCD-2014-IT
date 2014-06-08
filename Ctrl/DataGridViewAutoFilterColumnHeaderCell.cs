namespace Qisi.General.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
	/// <summary>
	/// Data grid view auto filter column header cell.
	/// </summary>
    public class DataGridViewAutoFilterColumnHeaderCell : DataGridViewColumnHeaderCell
    {
        private bool automaticSortingEnabledValue;
        private string currentColumnFilter;
        private int currentDropDownButtonPaddingOffset;
        private Rectangle dropDownButtonBoundsValue;
        private static FilterListBox dropDownListBox = new FilterListBox();
        private int dropDownListBoxMaxLinesValue;
        private bool dropDownListBoxShowing;
        private bool filtered;
        private bool filteringEnabledValue;
        private OrderedDictionary filters;
        private bool lostFocusOnDropDownButtonClick;
        private string selectedFilterValue;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.Controls.DataGridViewAutoFilterColumnHeaderCell"/> class.
		/// </summary>
        public DataGridViewAutoFilterColumnHeaderCell()
        {
            this.filters = new OrderedDictionary();
            this.selectedFilterValue = string.Empty;
            this.currentColumnFilter = string.Empty;
            this.dropDownButtonBoundsValue = Rectangle.Empty;
            this.filteringEnabledValue = true;
            this.automaticSortingEnabledValue = true;
            this.dropDownListBoxMaxLinesValue = 20;
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.Controls.DataGridViewAutoFilterColumnHeaderCell"/> class.
		/// </summary>
		/// <param name="oldHeaderCell">Old header cell.</param>
        public DataGridViewAutoFilterColumnHeaderCell(DataGridViewColumnHeaderCell oldHeaderCell)
        {
            this.filters = new OrderedDictionary();
            this.selectedFilterValue = string.Empty;
            this.currentColumnFilter = string.Empty;
            this.dropDownButtonBoundsValue = Rectangle.Empty;
            this.filteringEnabledValue = true;
            this.automaticSortingEnabledValue = true;
            this.dropDownListBoxMaxLinesValue = 20;
            this.ContextMenuStrip = oldHeaderCell.ContextMenuStrip;
            base.ErrorText = oldHeaderCell.ErrorText;
            base.Tag = oldHeaderCell.Tag;
            base.ToolTipText = oldHeaderCell.ToolTipText;
            base.Value = oldHeaderCell.Value;
            this.ValueType = oldHeaderCell.ValueType;
            if (oldHeaderCell.HasStyle)
            {
                base.Style = oldHeaderCell.Style;
            }
            DataGridViewAutoFilterColumnHeaderCell cell = oldHeaderCell as DataGridViewAutoFilterColumnHeaderCell;
            if (cell != null)
            {
                this.FilteringEnabled = cell.FilteringEnabled;
                this.AutomaticSortingEnabled = cell.AutomaticSortingEnabled;
                this.DropDownListBoxMaxLines = cell.DropDownListBoxMaxLines;
                this.currentDropDownButtonPaddingOffset = cell.currentDropDownButtonPaddingOffset;
            }
        }

        private void AdjustPadding(int newDropDownButtonPaddingOffset)
        {
            int right = newDropDownButtonPaddingOffset - this.currentDropDownButtonPaddingOffset;
            if (right != 0)
            {
                this.currentDropDownButtonPaddingOffset = newDropDownButtonPaddingOffset;
                Padding padding = new Padding(0, 0, right, 0);
                base.Style.Padding = Padding.Add(base.InheritedStyle.Padding, padding);
            }
        }
		/// <summary>
		/// Clone this instance.
		/// </summary>
        public override object Clone()
        {
            return new DataGridViewAutoFilterColumnHeaderCell(this);
        }

        private void DataGridView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            this.ResetDropDown();
        }

        private void DataGridView_ColumnHeadersHeightChanged(object sender, EventArgs e)
        {
            this.ResetDropDown();
        }

        private void DataGridView_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if ((e.Column == base.OwningColumn) && (e.Column.SortMode == DataGridViewColumnSortMode.Automatic))
            {
                throw new InvalidOperationException("A SortMode value of Automatic is incompatible with the DataGridViewAutoFilterColumnHeaderCell type. Use the AutomaticSortingEnabled property instead.");
            }
        }

        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            this.ResetDropDown();
        }

        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                this.ResetDropDown();
                this.ResetFilter();
            }
        }

        private void DataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            this.VerifyDataSource();
            this.ResetDropDown();
            this.ResetFilter();
        }

        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                this.ResetDropDown();
            }
        }

        private void DataGridView_SizeChanged(object sender, EventArgs e)
        {
            this.ResetDropDown();
        }

        private void DropDownListBox_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            if (keyCode != Keys.Enter)
            {
                if (keyCode != Keys.Escape)
                {
                    return;
                }
            }
            else
            {
                this.UpdateFilter();
                this.HideDropDownList();
                return;
            }
            this.HideDropDownList();
        }

        private void DropDownListBox_LostFocus(object sender, EventArgs e)
        {
            if (this.DropDownButtonBounds.Contains(base.DataGridView.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y))))
            {
                this.lostFocusOnDropDownButtonClick = true;
            }
            this.HideDropDownList();
        }

        private void DropDownListBox_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.Assert(base.DataGridView != null, "DataGridView is null");
            if (dropDownListBox.DisplayRectangle.Contains(e.X, e.Y))
            {
                this.UpdateFilter();
                this.HideDropDownList();
            }
        }

        private string FilterWithoutCurrentColumn(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                if (!this.filtered)
                {
                    return filter;
                }
                if (filter.IndexOf(this.currentColumnFilter) > 0)
                {
                    return filter.Replace(" AND " + this.currentColumnFilter, string.Empty);
                }
                if (filter.Length > this.currentColumnFilter.Length)
                {
                    return filter.Replace(this.currentColumnFilter + " AND ", string.Empty);
                }
            }
            return string.Empty;
        }
		/// <summary>
		/// Gets the filter status.
		/// </summary>
		/// <returns>The filter status.</returns>
		/// <param name="dataGridView">Data grid view.</param>
        public static string GetFilterStatus(DataGridView dataGridView)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException("dataGridView");
            }
            BindingSource dataSource = dataGridView.DataSource as BindingSource;
            if ((string.IsNullOrEmpty(dataSource.Filter) || (dataSource == null)) || ((dataSource.DataSource == null) || !dataSource.SupportsFiltering))
            {
                return string.Empty;
            }
            int count = dataSource.Count;
            dataSource.RaiseListChangedEvents = false;
            string filter = dataSource.Filter;
            dataSource.Filter = null;
            int num2 = dataSource.Count;
            dataSource.Filter = filter;
            dataSource.RaiseListChangedEvents = true;
            Debug.Assert(count <= num2, "current count is greater than unfiltered count");
            if (count == num2)
            {
                return string.Empty;
            }
            return string.Format("{0} of {1} records found", count, num2);
        }

        private void HandleDataGridViewEvents()
        {
            base.DataGridView.Scroll += new ScrollEventHandler(this.DataGridView_Scroll);
            base.DataGridView.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(this.DataGridView_ColumnDisplayIndexChanged);
            base.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(this.DataGridView_ColumnWidthChanged);
            base.DataGridView.ColumnHeadersHeightChanged += new EventHandler(this.DataGridView_ColumnHeadersHeightChanged);
            base.DataGridView.SizeChanged += new EventHandler(this.DataGridView_SizeChanged);
            base.DataGridView.DataSourceChanged += new EventHandler(this.DataGridView_DataSourceChanged);
            base.DataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(this.DataGridView_DataBindingComplete);
            base.DataGridView.ColumnSortModeChanged += new DataGridViewColumnEventHandler(this.DataGridView_ColumnSortModeChanged);
        }

        private void HandleDropDownListBoxEvents()
        {
            dropDownListBox.MouseClick += new MouseEventHandler(this.DropDownListBox_MouseClick);
            dropDownListBox.LostFocus += new EventHandler(this.DropDownListBox_LostFocus);
            dropDownListBox.KeyDown += new KeyEventHandler(this.DropDownListBox_KeyDown);
        }
		/// <summary>
		/// Hides the drop down list.
		/// </summary>
        public void HideDropDownList()
        {
            Debug.Assert(base.DataGridView != null, "DataGridView is null");
            this.dropDownListBoxShowing = false;
            dropDownListBox.Visible = false;
            this.UnhandleDropDownListBoxEvents();
            base.DataGridView.Controls.Remove(dropDownListBox);
            base.DataGridView.InvalidateCell(this);
        }

        private void InvalidateDropDownButtonBounds()
        {
            if (!this.dropDownButtonBoundsValue.IsEmpty)
            {
                this.dropDownButtonBoundsValue = Rectangle.Empty;
            }
        }

        protected override void OnDataGridViewChanged()
        {
            if (base.DataGridView != null)
            {
                if (base.OwningColumn != null)
                {
                    if (((base.OwningColumn is DataGridViewImageColumn) || ((base.OwningColumn is DataGridViewButtonColumn) && ((DataGridViewButtonColumn) base.OwningColumn).UseColumnTextForButtonValue)) || ((base.OwningColumn is DataGridViewLinkColumn) && ((DataGridViewLinkColumn) base.OwningColumn).UseColumnTextForLinkValue))
                    {
                        this.AutomaticSortingEnabled = false;
                        this.FilteringEnabled = false;
                    }
                    if (base.OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                    {
                        base.OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
                    }
                }
                this.VerifyDataSource();
                this.HandleDataGridViewEvents();
                this.SetDropDownButtonBounds();
                base.OnDataGridViewChanged();
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            Debug.Assert(base.DataGridView != null, "DataGridView is null");
            if (this.lostFocusOnDropDownButtonClick)
            {
                this.lostFocusOnDropDownButtonClick = false;
            }
            else
            {
                Rectangle rectangle = base.DataGridView.GetCellDisplayRectangle(e.ColumnIndex, -1, false);
                if ((base.OwningColumn.Resizable != DataGridViewTriState.True) || (((base.DataGridView.RightToLeft != RightToLeft.No) || ((rectangle.Width - e.X) >= 6)) && (e.X >= 6)))
                {
                    int firstDisplayedScrollingColumnHiddenWidth = 0;
                    if ((base.DataGridView.RightToLeft == RightToLeft.No) && (base.DataGridView.FirstDisplayedScrollingColumnIndex == base.ColumnIndex))
                    {
                        firstDisplayedScrollingColumnHiddenWidth = base.DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
                    }
                    if (this.FilteringEnabled && this.DropDownButtonBounds.Contains((e.X + rectangle.Left) - firstDisplayedScrollingColumnHiddenWidth, e.Y + rectangle.Top))
                    {
                        if (base.DataGridView.IsCurrentCellInEditMode)
                        {
                            base.DataGridView.EndEdit();
                            BindingSource dataSource = base.DataGridView.DataSource as BindingSource;
                            if (dataSource != null)
                            {
                                dataSource.EndEdit();
                            }
                        }
                        this.ShowDropDownList();
                    }
                    else if (this.AutomaticSortingEnabled && (base.DataGridView.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect))
                    {
                        this.SortByColumn();
                    }
                    base.OnMouseDown(e);
                }
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            if (this.FilteringEnabled && ((paintParts & DataGridViewPaintParts.ContentBackground) != DataGridViewPaintParts.None))
            {
                Rectangle dropDownButtonBounds = this.DropDownButtonBounds;
                if ((dropDownButtonBounds.Width >= 1) && (dropDownButtonBounds.Height >= 1))
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        ComboBoxState normal = ComboBoxState.Normal;
                        if (this.dropDownListBoxShowing)
                        {
                            normal = ComboBoxState.Pressed;
                        }
                        else if (this.filtered)
                        {
                            normal = ComboBoxState.Hot;
                        }
                        ComboBoxRenderer.DrawDropDownButton(graphics, dropDownButtonBounds, normal);
                    }
                    else
                    {
                        int num = 0;
                        PushButtonState state = PushButtonState.Normal;
                        if (this.dropDownListBoxShowing)
                        {
                            state = PushButtonState.Pressed;
                            num = 1;
                        }
                        ButtonRenderer.DrawButton(graphics, dropDownButtonBounds, state);
                        if (this.filtered)
                        {
                            Point[] points = new Point[] { new Point((((dropDownButtonBounds.Width / 2) + dropDownButtonBounds.Left) - 1) + num, ((((dropDownButtonBounds.Height * 3) / 4) + dropDownButtonBounds.Top) - 1) + num), new Point(((dropDownButtonBounds.Width / 4) + dropDownButtonBounds.Left) + num, (((dropDownButtonBounds.Height / 2) + dropDownButtonBounds.Top) - 1) + num), new Point(((((dropDownButtonBounds.Width * 3) / 4) + dropDownButtonBounds.Left) - 1) + num, (((dropDownButtonBounds.Height / 2) + dropDownButtonBounds.Top) - 1) + num) };
                            graphics.DrawPolygon(SystemPens.ControlText, points);
                        }
                        else
                        {
                            Point[] pointArray2 = new Point[] { new Point((((dropDownButtonBounds.Width / 2) + dropDownButtonBounds.Left) - 1) + num, ((((dropDownButtonBounds.Height * 3) / 4) + dropDownButtonBounds.Top) - 1) + num), new Point(((dropDownButtonBounds.Width / 4) + dropDownButtonBounds.Left) + num, (((dropDownButtonBounds.Height / 2) + dropDownButtonBounds.Top) - 1) + num), new Point(((((dropDownButtonBounds.Width * 3) / 4) + dropDownButtonBounds.Left) - 1) + num, (((dropDownButtonBounds.Height / 2) + dropDownButtonBounds.Top) - 1) + num) };
                            graphics.FillPolygon(SystemBrushes.ControlText, pointArray2);
                        }
                    }
                }
            }
        }

        private void PopulateFilters()
        {
            if (base.DataGridView != null)
            {
                BindingSource dataSource = base.DataGridView.DataSource as BindingSource;
                Debug.Assert(((dataSource != null) && dataSource.SupportsFiltering) && (base.OwningColumn != null), "DataSource is not a BindingSource, or does not support filtering, or OwningColumn is null");
                dataSource.RaiseListChangedEvents = false;
                string filter = dataSource.Filter;
                dataSource.Filter = this.FilterWithoutCurrentColumn(filter);
                this.filters.Clear();
                bool flag = false;
                bool flag2 = false;
                ArrayList list = new ArrayList(dataSource.Count);
                foreach (object obj2 in dataSource)
                {
                    object item = null;
                    ICustomTypeDescriptor descriptor = obj2 as ICustomTypeDescriptor;
                    if (descriptor != null)
                    {
                        foreach (PropertyDescriptor descriptor2 in descriptor.GetProperties())
                        {
                            if (string.Compare(base.OwningColumn.DataPropertyName, descriptor2.Name, true, CultureInfo.InvariantCulture) == 0)
                            {
                                item = descriptor2.GetValue(obj2);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (PropertyInfo info in obj2.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (string.Compare(base.OwningColumn.DataPropertyName, info.Name, true, CultureInfo.InvariantCulture) == 0)
                            {
                                item = info.GetValue(obj2, null);
                                break;
                            }
                        }
                    }
                    if ((item == null) || (item == DBNull.Value))
                    {
                        flag = true;
                    }
                    else if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
                list.Sort();
                foreach (object obj4 in list)
                {
                    string str2 = null;
                    DataGridViewCellStyle inheritedStyle = base.OwningColumn.InheritedStyle;
                    str2 = (string) this.GetFormattedValue(obj4, -1, ref inheritedStyle, null, null, DataGridViewDataErrorContexts.Formatting);
                    if (string.IsNullOrEmpty(str2))
                    {
                        flag = true;
                    }
                    else if (!this.filters.Contains(str2))
                    {
                        flag2 = true;
                        this.filters.Add(str2, obj4.ToString());
                    }
                }
                if (filter != null)
                {
                    dataSource.Filter = filter;
                }
                dataSource.RaiseListChangedEvents = true;
                this.filters.Insert(0, "(全部)", null);
                if (flag && flag2)
                {
                    this.filters.Add("(空白值)", null);
                    this.filters.Add("(非空白值)", null);
                }
            }
        }
		/// <summary>
		/// Removes the filter.
		/// </summary>
		/// <param name="dataGridView">Data grid view.</param>
        public static void RemoveFilter(DataGridView dataGridView)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException("dataGridView");
            }
            BindingSource dataSource = dataGridView.DataSource as BindingSource;
            if (((dataSource == null) || (dataSource.DataSource == null)) || !dataSource.SupportsFiltering)
            {
                throw new ArgumentException("The DataSource property of the specified DataGridView is not set to a BindingSource with a SupportsFiltering property value of true.");
            }
            if ((dataGridView.CurrentRow != null) && dataGridView.CurrentRow.IsNewRow)
            {
                dataGridView.CurrentCell = null;
            }
            dataSource.Filter = null;
        }

        private void ResetDropDown()
        {
            this.InvalidateDropDownButtonBounds();
            if (this.dropDownListBoxShowing)
            {
                this.HideDropDownList();
            }
        }

        private void ResetFilter()
        {
            if (base.DataGridView != null)
            {
                BindingSource dataSource = base.DataGridView.DataSource as BindingSource;
                if ((dataSource == null) || string.IsNullOrEmpty(dataSource.Filter))
                {
                    this.filtered = false;
                    this.selectedFilterValue = "(全部)";
                    this.currentColumnFilter = string.Empty;
                }
            }
        }

        private void SetDropDownButtonBounds()
        {
            Rectangle rectangle = base.DataGridView.GetCellDisplayRectangle(base.ColumnIndex, -1, false);
            int width = base.InheritedStyle.Font.Height + 5;
            Rectangle rectangle2 = this.BorderWidths(base.DataGridView.AdjustColumnHeaderBorderStyle(base.DataGridView.AdvancedColumnHeadersBorderStyle, new DataGridViewAdvancedBorderStyle(), false, false));
            int num2 = ((2 + rectangle2.Top) + rectangle2.Height) + base.InheritedStyle.Padding.Vertical;
            bool flag = Application.RenderWithVisualStyles && base.DataGridView.EnableHeadersVisualStyles;
            if (flag)
            {
                num2 += 3;
            }
            if (width > (base.DataGridView.ColumnHeadersHeight - num2))
            {
                width = base.DataGridView.ColumnHeadersHeight - num2;
            }
            if (width > (rectangle.Width - 3))
            {
                width = rectangle.Width - 3;
            }
            int num3 = flag ? 4 : 1;
            int y = (rectangle.Bottom - width) - num3;
            int num5 = flag ? 3 : 1;
            int x = 0;
            if (base.DataGridView.RightToLeft == RightToLeft.No)
            {
                x = (rectangle.Right - width) - num5;
            }
            else
            {
                x = rectangle.Left + num5;
            }
            this.dropDownButtonBoundsValue = new Rectangle(x, y, width, width);
            this.AdjustPadding(width + num5);
        }

        private void SetDropDownListBoxBounds()
        {
            Debug.Assert(this.filters.Count > 0, "filters.Count <= 0");
            int height = 2;
            int num2 = 0;
            int width = 0;
            int x = 0;
            using (Graphics graphics = dropDownListBox.CreateGraphics())
            {
                foreach (string str in this.filters.Keys)
                {
                    SizeF ef = graphics.MeasureString(str, dropDownListBox.Font);
                    height += (int) ef.Height;
                    num2 = (int) ef.Width;
                    if (width < num2)
                    {
                        width = num2;
                    }
                }
            }
            width += 6;
            if (height > this.DropDownListBoxMaxHeightInternal)
            {
                height = this.DropDownListBoxMaxHeightInternal;
                width += SystemInformation.VerticalScrollBarWidth;
            }
            if (base.DataGridView.RightToLeft == RightToLeft.No)
            {
                x = (this.DropDownButtonBounds.Right - width) + 1;
            }
            else
            {
                x = this.DropDownButtonBounds.Left - 1;
            }
            int num5 = 1;
            int right = base.DataGridView.ClientRectangle.Right;
            if (base.DataGridView.DisplayedRowCount(false) < base.DataGridView.RowCount)
            {
                if (base.DataGridView.RightToLeft == RightToLeft.Yes)
                {
                    num5 += SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    right -= SystemInformation.VerticalScrollBarWidth;
                }
            }
            if (x < num5)
            {
                x = num5;
            }
            int num7 = (x + width) + 1;
            if (num7 > right)
            {
                if (x == num5)
                {
                    width -= num7 - right;
                }
                else
                {
                    x -= num7 - right;
                    if (x < num5)
                    {
                        width -= num5 - x;
                        x = num5;
                    }
                }
            }
            dropDownListBox.Bounds = new Rectangle(x, this.DropDownButtonBounds.Bottom, width, height);
        }
		/// <summary>
		/// Shows the drop down list.
		/// </summary>
        public void ShowDropDownList()
        {
            Debug.Assert(base.DataGridView != null, "DataGridView is null");
            if ((base.DataGridView.CurrentRow != null) && base.DataGridView.CurrentRow.IsNewRow)
            {
                base.DataGridView.CurrentCell = null;
            }
            this.PopulateFilters();
            string[] array = new string[this.filters.Count];
            this.filters.Keys.CopyTo(array, 0);
            dropDownListBox.Items.Clear();
            dropDownListBox.Items.AddRange(array);
            dropDownListBox.SelectedItem = this.selectedFilterValue;
            this.HandleDropDownListBoxEvents();
            this.SetDropDownListBoxBounds();
            dropDownListBox.Visible = true;
            this.dropDownListBoxShowing = true;
            Debug.Assert(dropDownListBox.Parent == null, "ShowDropDownListBox has been called multiple times before HideDropDownListBox");
            base.DataGridView.Controls.Add(dropDownListBox);
            dropDownListBox.Focus();
            base.DataGridView.InvalidateCell(this);
        }

        private void SortByColumn()
        {
            Debug.Assert((base.DataGridView != null) && (base.OwningColumn != null), "DataGridView or OwningColumn is null");
            IBindingList dataSource = base.DataGridView.DataSource as IBindingList;
            if (((dataSource != null) && dataSource.SupportsSorting) && this.AutomaticSortingEnabled)
            {
                ListSortDirection ascending = ListSortDirection.Ascending;
                if ((base.DataGridView.SortedColumn == base.OwningColumn) && (base.DataGridView.SortOrder == SortOrder.Ascending))
                {
                    ascending = ListSortDirection.Descending;
                }
                base.DataGridView.Sort(base.OwningColumn, ascending);
            }
        }

        private void UnhandleDropDownListBoxEvents()
        {
            dropDownListBox.MouseClick -= new MouseEventHandler(this.DropDownListBox_MouseClick);
            dropDownListBox.LostFocus -= new EventHandler(this.DropDownListBox_LostFocus);
            dropDownListBox.KeyDown -= new KeyEventHandler(this.DropDownListBox_KeyDown);
        }

        private void UpdateFilter()
        {
            string str3;
            if (dropDownListBox.SelectedItem.ToString().Equals(this.selectedFilterValue))
            {
                return;
            }
            this.selectedFilterValue = dropDownListBox.SelectedItem.ToString();
            IBindingListView dataSource = base.DataGridView.DataSource as IBindingListView;
            Debug.Assert((dataSource != null) && dataSource.SupportsFiltering, "DataSource is not an IBindingListView or does not support filtering");
            if (this.selectedFilterValue.Equals("(全部)"))
            {
                dataSource.Filter = this.FilterWithoutCurrentColumn(dataSource.Filter);
                this.filtered = false;
                this.currentColumnFilter = string.Empty;
                return;
            }
            string str = null;
            string str2 = base.OwningColumn.DataPropertyName.Replace("]", @"\]");
            string selectedFilterValue = this.selectedFilterValue;
            if (selectedFilterValue != null)
            {
                if (!(selectedFilterValue == "(空白值)"))
                {
                    if (selectedFilterValue == "(非空白值)")
                    {
                        str = string.Format("LEN(ISNULL(CONVERT([{0}],'System.String'),''))>0", str2);
                        goto Label_0123;
                    }
                }
                else
                {
                    str = string.Format("LEN(ISNULL(CONVERT([{0}],'System.String'),''))=0", str2);
                    goto Label_0123;
                }
            }
            str = string.Format("[{0}]='{1}'", str2, ((string) this.filters[this.selectedFilterValue]).Replace("'", "''"));
        Label_0123:
            str3 = this.FilterWithoutCurrentColumn(dataSource.Filter);
            if (string.IsNullOrEmpty(str3))
            {
                str3 = str3 + str;
            }
            else
            {
                str3 = str3 + " AND " + str;
            }
            try
            {
                dataSource.Filter = str3;
            }
            catch (InvalidExpressionException exception)
            {
                throw new NotSupportedException("Invalid expression: " + str3, exception);
            }
            this.filtered = true;
            this.currentColumnFilter = str;
        }

        private void VerifyDataSource()
        {
            if (((base.DataGridView != null) && (base.DataGridView.DataSource != null)) && !(base.DataGridView.DataSource is BindingSource))
            {
                throw new NotSupportedException("The DataSource property of the containing DataGridView control must be set to a BindingSource.");
            }
        }

        [DefaultValue(true)]
		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="Qisi.General.Controls.DataGridViewAutoFilterColumnHeaderCell"/> automatic sorting is enabled.
		/// </summary>
		/// <value><c>true</c> if automatic sorting enabled; otherwise, <c>false</c>.</value>
        public bool AutomaticSortingEnabled
        {
            get
            {
                return this.automaticSortingEnabledValue;
            }
            set
            {
                this.automaticSortingEnabledValue = value;
                if (base.OwningColumn != null)
                {
                    if (value)
                    {
                        base.OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
                    }
                    else
                    {
                        base.OwningColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
            }
        }

        protected Rectangle DropDownButtonBounds
        {
            get
            {
                if (!this.FilteringEnabled)
                {
                    return Rectangle.Empty;
                }
                if (this.dropDownButtonBoundsValue == Rectangle.Empty)
                {
                    this.SetDropDownButtonBounds();
                }
                return this.dropDownButtonBoundsValue;
            }
        }

        protected int DropDownListBoxMaxHeightInternal
        {
            get
            {
                int num = (base.DataGridView.Height - base.DataGridView.ColumnHeadersHeight) - 1;
                if (base.DataGridView.DisplayedColumnCount(false) < base.DataGridView.ColumnCount)
                {
                    num -= SystemInformation.HorizontalScrollBarHeight;
                }
                int num2 = (this.dropDownListBoxMaxLinesValue * dropDownListBox.ItemHeight) + 2;
                if (num2 < num)
                {
                    return num2;
                }
                return num;
            }
        }

        [DefaultValue(20)]
		/// <summary>
		/// Gets or sets the drop down list box max lines, default is 20.
		/// </summary>
		/// <value>The drop down list box max lines, default is 20.</value>
        public int DropDownListBoxMaxLines
        {
            get
            {
                return this.dropDownListBoxMaxLinesValue;
            }
            set
            {
                this.dropDownListBoxMaxLinesValue = value;
            }
        }

        [DefaultValue(true)]
		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="Qisi.General.Controls.DataGridViewAutoFilterColumnHeaderCell"/> filtering enabled.
		/// </summary>
		/// <value><c>true</c> if filtering enabled (default); otherwise, <c>false</c>.</value>
        public bool FilteringEnabled
        {
            get
            {
                if ((base.DataGridView == null) || (base.DataGridView.DataSource == null))
                {
                    return this.filteringEnabledValue;
                }
                BindingSource dataSource = base.DataGridView.DataSource as BindingSource;
                Debug.Assert(dataSource != null);
                return (this.filteringEnabledValue && dataSource.SupportsFiltering);
            }
            set
            {
                if (!value)
                {
                    this.AdjustPadding(0);
                    this.InvalidateDropDownButtonBounds();
                }
                this.filteringEnabledValue = value;
            }
        }

        private class FilterListBox : ListBox
		{
			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="Qisi.General.Controls.DataGridViewAutoFilterColumnHeaderCell+FilterListBox"/> class.
			/// </summary>
            public FilterListBox()
            {
                base.Visible = false;
                base.IntegralHeight = true;
                base.BorderStyle = BorderStyle.FixedSingle;
                base.TabStop = false;
            }

            protected override bool IsInputKey(Keys keyData)
            {
                return true;
            }

            protected override bool ProcessKeyMessage(ref Message m)
            {
                return this.ProcessKeyEventArgs(ref m);
            }
        }
    }
}

