using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BindTextFileToGrid
{
    public partial class Form1 : Form
    {
        int rowIX = -1;
        bool isNewRow=false, isDelete=false;
        string filePath = "txtFile.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createTxtFile();
            newRow();
            
            DataTable dataTable = ConvertToDataTable(6);
              dataGridView1.DataSource = dataTable;
            groupBox1.Enabled= false;
        }
        private void createTxtFile()
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter w = File.AppendText(filePath))
                {
                    w.WriteLine("S.No,Status,First Name,Last Name,Age,Gender");
                    w.Flush();
                    w.Close();
                }
            }
        }

        private void clearForm()
        {
            txtSNO.Text = "";
            statusCheckBox.Checked = false;
            txtFN.Text = "";
            txtLN.Text = "";
            txtAge.Text = "";
            comboBoxG.SelectedIndex=-1;
        }
        private void newRow()
        {
            addRowToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
            
        }

        private void saveRow()
        {
            addRowToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = true;
            deleteToolStripMenuItem.Enabled = false;
           
        }

        private void modieRow()
        {
            addRowToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = true;
            deleteToolStripMenuItem.Enabled = true;
           
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            isNewRow= true;
            txtSNO.Focus();
          saveRow(); 
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
           
            lineChanger(txtSNO.Text.Trim() + "," + statusCheckBox.Checked + "," +txtFN.Text + "," +txtLN.Text + "," +txtAge.Text + "," +comboBoxG.SelectedItem);
            dataGridView1.DataSource = ConvertToDataTable(6);
            dataGridView1.Update();
            dataGridView1.Refresh();
            clearForm();
            newRow();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false; isDelete = true;
            lineChanger("", isDelete);
            newRow();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex>=0)
            {
                rowIX=e.RowIndex;
                loadValues();
                modieRow();
            }
        }

        private void loadValues()
        {
            txtSNO.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            statusCheckBox.Checked =Convert.ToBoolean( dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
            txtFN.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            txtLN.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            txtAge.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            comboBoxG.SelectedItem = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
        }


        private DataTable ConvertToDataTable(int numberOfColumns)
        {
            DataTable tbl = new DataTable();
            string[] lines = System.IO.File.ReadAllLines(filePath);

            for (int col = 0; col < numberOfColumns; col++)
            {
                var cols = lines[0].Split(',');
                tbl.Columns.Add(new DataColumn(cols[col].ToString()));
            }

            lines = lines.Skip(1).ToArray();
            foreach (string line in lines)
            {

                var cols = line.Split(',');
                DataRow dr = tbl.NewRow();
                for (int cIndex = 0; cIndex < cols.Length; cIndex++)
                {
                    dr[cIndex] = cols[cIndex];
                }

                tbl.Rows.Add(dr);
            }

            return tbl;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        void lineChanger(string newText, bool isDelete = false)
        {
            string[] arrLine = File.ReadAllLines(filePath);
            if (isNewRow)
            {
                Array.Resize<string>(ref arrLine, arrLine.Length + 1);
                arrLine[arrLine.Length - 1] = newText;
            }
            else if (isDelete)
            {
                string fileIN = filePath;
                string fileOUT = "temp.txt";

                for (int i = 0; i < arrLine.Length; i++)
                    if (!(rowIX + 1 == i))
                    {
                        File.AppendAllText(fileOUT, arrLine[i]);
                        File.AppendAllText(fileOUT, Environment.NewLine);
                    }
                File.Delete(fileIN);
                File.Move(fileOUT, fileIN);

            }
            else
            {
                arrLine[rowIX + 1] = newText;
            }
            if (!isDelete)
                File.WriteAllLines(filePath, arrLine);
        }
    }
}
