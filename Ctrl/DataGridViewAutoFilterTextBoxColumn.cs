namespace Qisi.General.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
	/// <summary>
	/// Data grid view auto filter text box, column.
	/// </summary>
    public class DataGridViewAutoFilterTextBoxColumn : DataGridViewTextBoxColumn
    {
        public DataGridViewAutoFilterTextBoxColumn()
        {
            base.DefaultHeaderCellType = typeof(DataGridViewAutoFilterColumnHeaderCell);
            base.SortMode = DataGridViewColumnSortMode.Programmatic;
        }
		/// <summary>
		/// Gets the filter status.
		/// </summary>
		/// <returns>The filter status.</returns>
		/// <param name="dataGridView">Data grid view.</param>
        public static string GetFilterStatus(DataGridView dataGridView)
        {
            return DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(dataGridView);
        }
		/// <summary>
		/// Removes the filter.
		/// </summary>
		/// <param name="dataGridView">Data grid view.</param>
        public static void RemoveFilter(DataGridView dataGridView)
        {
            DataGridViewAutoFilterColumnHeaderCell.RemoveFilter(dataGridView);
        }

        [DefaultValue(true)]
		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="Qisi.General.Controls.DataGridViewAutoFilterTextBoxColumn"/> automatic sorting is enabled.
		/// </summary>
		/// <value><c>true</c> if automatic sorting enabled (default); otherwise, <c>false</c>.</value>
        public bool AutomaticSortingEnabled
        {
            get
            {
                return ((DataGridViewAutoFilterColumnHeaderCell) base.HeaderCell).AutomaticSortingEnabled;
            }
            set
            {
                ((DataGridViewAutoFilterColumnHeaderCell) base.HeaderCell).AutomaticSortingEnabled = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// Gets the default type of the header cell.
		/// </summary>
		/// <value>The default type of the header cell.</value>
		public System.Type DefaultHeaderCellType
        {
            get
            {
                return typeof(DataGridViewAutoFilterColumnHeaderCell);
            }
        }

        [DefaultValue(20)]
		/// <summary>
		/// Gets or sets the max lines drop down list box.
		/// </summary>
		/// <value>The drop down list box max lines, default is 20.</value>
		public int DropDownListBoxMaxLines
        {
            get
            {
                return ((DataGridViewAutoFilterColumnHeaderCell) base.HeaderCell).DropDownListBoxMaxLines;
            }
            set
            {
                ((DataGridViewAutoFilterColumnHeaderCell) base.HeaderCell).DropDownListBoxMaxLines = value;
            }
        }

        [DefaultValue(true)]
        public bool FilteringEnabled
        {
            get
            {
                return ((DataGridViewAutoFilterColumnHeaderCell) base.HeaderCell).FilteringEnabled;
            }
            set
            {
                ((DataGridViewAutoFilterColumnHeaderCell) base.HeaderCell).FilteringEnabled = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), DefaultValue(2)]
		/// <summary>
		/// Gets or sets the sort mode.
		/// </summary>
		/// <value>The sort mode.</value>
        public DataGridViewColumnSortMode SortMode
        {
            get
            {
                return base.SortMode;
            }
            set
            {
                if (value == DataGridViewColumnSortMode.Automatic)
                {
                    throw new InvalidOperationException("A SortMode value of Automatic is incompatible with the DataGridViewAutoFilterColumnHeaderCell type. Use the AutomaticSortingEnabled property instead.");
                }
                base.SortMode = value;
            }
        }
    }
}

