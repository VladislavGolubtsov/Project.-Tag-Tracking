using System;
using System.Windows.Forms;

namespace L5
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            
            dataGridView1.Columns.Add("Эпоха", "Эпоха");
            for (int col = 1; col < MainForm.Kol_Marok_3_lvl + 1; col++)
            {
                dataGridView1.Columns.Add("H" + Convert.ToString(MainForm.Bloks_3[MainForm.SelectedIndex_3_lvl, 0, col - 1]), "H" + Convert.ToString(MainForm.Bloks_3[MainForm.SelectedIndex_3_lvl, 0, col - 1]));
                dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            for (int i = 0; i < MainForm.dTable.Rows.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = (Convert.ToString(i));

                for (int col = 0; col < MainForm.Kol_Marok_3_lvl; col++)
                {
                    dataGridView1.Rows[i].Cells[col + 1].Value = Convert.ToString(MainForm.Bloks_3[MainForm.SelectedIndex_3_lvl, i + 1, col]);
                    dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }
    }
}
