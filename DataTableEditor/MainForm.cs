using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ADGV;
using XmlFileLib;

namespace DataTableEditor
{
    public partial class MainForm : Form
    {
        private Dictionary<int, String> filters = new Dictionary<int, String>();
        private Dictionary<int, String> sort = new Dictionary<int, String>();
        private DataTable dt;

        private string filePath;
        public MainForm()
        {
            InitializeComponent();
            this.dataGridView.AutoGenerateColumns = true;            
        }

        
        private void dataGridView_SortStringChanged(object sender, EventArgs e)
        {
            this.bindingSource.Sort = this.dataGridView.SortString;
        }

        private void dataGridView_FilterStringChanged(object sender, EventArgs e)
        {
            this.bindingSource.Filter = this.dataGridView.FilterString;
        }

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            this.dataGridView.ClearFilter(true);
        }

        private void clearSortButton_Click(object sender, EventArgs e)
        {
            this.dataGridView.ClearSort(true);
        }

        private void bindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.toolStripStatusLabel1.Text = Path.GetFileName(filePath) + " || Total rows - " + this.bindingSource.List.Count.ToString();
            this.searchToolBar.SetColumns(this.dataGridView.Columns);
        }

        private void searchToolBar_Search(object sender, SearchToolBarSearchEventArgs e)
        {
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {
                bool endcol = this.dataGridView.CurrentCell.ColumnIndex + 1 >= this.dataGridView.ColumnCount;
                bool endrow = this.dataGridView.CurrentCell.RowIndex + 1 >= this.dataGridView.RowCount;

                if (endcol && endrow)
                {
                    startColumn = this.dataGridView.CurrentCell.ColumnIndex;
                    startRow = this.dataGridView.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : this.dataGridView.CurrentCell.ColumnIndex + 1;
                    startRow = this.dataGridView.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = this.dataGridView.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);

            if (c != null)
                this.dataGridView.CurrentCell = c;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = filters.Count + 1;
            filters.Add(i, this.dataGridView.FilterString);
            sort.Add(i, this.dataGridView.SortString);

            ToolStripMenuItem itm = new ToolStripMenuItem(i.ToString());
            itm.Click += itm_Click;
            this.toolStripDropDownButton1.DropDownItems.Add(itm);
        }

        void itm_Click(object sender, EventArgs e)
        {
            int i = int.Parse((sender as ToolStripMenuItem).Text);
            this.dataGridView.LoadFilter(filters[i], sort[i]);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                LoadFileToGrid();
            }
        }

        private void LoadFileToGrid()
        {
            dt = XmlFile.GetFilledDataTable(filePath);

            dataSet.Tables.Clear();
            dataSet.Tables.Add(dt);

            this.bindingSource.DataMember = dt.TableName;

            dataGridView.DataSource = bindingSource;

            dataGridView.AutoResizeColumnHeadersHeight();
                        dataGridView.AutoResizeRows(
                DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            DataTable dataTable = dataSet.Tables[0];
            XmlFile.SaveDataTableToXMl(dataTable, filePath);
        }
    }
}
