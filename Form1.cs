using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace L5
{
    public partial class MainForm : Form
    {
        private SQLiteConnection SQLiteConn;
        public static DataTable dTable;
        public double T, A;
        public double[,] MAS;
        public double[,] firstlvl;
        public double[,,] Secondlvl;
        public double[,,] Bloks;
        public static double[,,] Bloks_3;
        public static double[,,] Bloks_3_lvl;
        public static double[,,] Thirdlvl;
        public int Kol_Blocks, Data;
        public double ogr_x_min, ogr_y_min, ogr_y_max, ogr_x_max;
        public double ogr_x_min_2_lvl, ogr_y_min_2_lvl, ogr_y_max_2_lvl, ogr_x_max_2_lvl;
        public double ogr_x_min_3_lvl, ogr_y_min_3_lvl, ogr_y_max_3_lvl, ogr_x_max_3_lvl;
        public static int SelectedIndex_3_lvl, Kol_Marok_3_lvl;
        public double[,] third_lvl_abs;
        public double[,] delta_third_lvl;
        public int Kol_Blocks_3_lvl, Data_3_lvl;

        private int MessageInfo = 0;

        public int BlocksCount;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            label5.Enabled = false;
            comboBox1.Enabled = false;
            groupBox2.Enabled = false;
            button4.Enabled = false;
            button8.Enabled = false;
            groupBox3.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            label1.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            groupBox2.Enabled = false;
            splitContainer9.Enabled = false;
            splitContainer12.Enabled = false;
            groupBox4.Enabled = false;
            splitContainer7.Enabled = false;
            splitContainer2.Enabled = false;
            splitContainer13.Enabled = false;
            splitContainer1.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (OpenDBFile() == true)
            {
                LoadData();
            }
        }
        private void LoadData()
        {
            GetTableNames();
            ShowTable(SQL_Start_Table());
            LoadImage();
            Load_A();
            Load_T();
            MASIV();
            button2.Enabled = true;
            button3.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button8.Enabled = true;
            checkBox1.Enabled = true;
            checkBox2.Enabled = true;
            checkBox3.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
        }
        private void MASIV()
        {
            if (MAS != null) MAS = null;
            MAS = new double[dTable.Rows.Count, dTable.Columns.Count - 1];

            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                for (int j = 0; j < dTable.Columns.Count - 1; j++)
                {
                    MAS[i, j] = Convert.ToDouble(dTable.Rows[i].ItemArray[j + 1]);
                }
            }
        }
        private void Load_A()//(авто загрузка из файла bd)
        {
            string PQuery = "SELECT [A] FROM [Доп_данные]";
            SQLiteCommand Pcommand = new SQLiteCommand(PQuery, SQLiteConn);
            IDataReader Preader = Pcommand.ExecuteReader();
            while (Preader.Read())
            {
                textBox2.Text = Convert.ToString(Preader[0]);
            }
        }
        private void Load_T()//(авто загрузка из файла bd)
        {
            string PQuery = "SELECT [T] FROM [Доп_данные]";
            SQLiteCommand Pcommand = new SQLiteCommand(PQuery, SQLiteConn);
            IDataReader Preader = Pcommand.ExecuteReader();
            while (Preader.Read())
            {
                textBox1.Text = Convert.ToString(Preader[0]);
            }
        }
        private bool OpenDBFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Текстовые файлы (*.db|*.db|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SQLiteConn = new SQLiteConnection("Data Source=" + openFileDialog.FileName + ";Version=3");
                SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteConn;
                return true;
            }
            else return false;
        }
        private void GetTableNames()
        {
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();
            dTable.Clear();
        }
        private string SQL_Start_Table()
        {
            return "SELECT * FROM [Данные] order by 1";
        }

        private void ShowTable(string SQLQuery)
        {
            dTable.Clear();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            adapter.Fill(dTable);

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            for (int col = 0; col < dTable.Columns.Count; col++)
            {
                string ColName = dTable.Columns[col].ColumnName;
                dataGridView1.Columns.Add(ColName, ColName);
                dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                dataGridView1.Rows.Add(dTable.Rows[row].ItemArray);
            }
            groupBox2.Enabled = true;
        }

        private void LoadImage()
        {
            string PQuery = "SELECT [Картинка] FROM [Доп_данные];";
            SQLiteCommand Pcommand = new SQLiteCommand(PQuery, SQLiteConn);
            try
            {
                IDataReader Preader = Pcommand.ExecuteReader();
                try
                {
                    while (Preader.Read())
                    {
                        byte[] a = (byte[])Preader[0];
                        pictureBox1.Image = ByteToImage(a);
                        pictureBox2.Image = ByteToImage(a);
                    }
                }
                catch (Exception exc) { MessageBox.Show(exc.Message); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Image ByteToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }

        private void button3_Click(object sender, EventArgs e) 
        {
            string S = "", S1 = "", S2;
            double delta_max, delta;
            Random random = new Random();

            S = Convert.ToString(dataGridView1.Rows.Count);

            for (int i = 1; i < dataGridView1.Columns.Count; i++)
            {
                delta = 0;
                delta_max = 0;
                for (int b = 0; b < dataGridView1.Rows.Count - 2; b++)
                {
                    delta = (Math.Abs(MAS[b, i - 1] - MAS[b + 1, i - 1]));
                    if (delta > delta_max) delta_max = delta;
                }

                delta = Math.Round((random.NextDouble() * (2 * delta_max)) - delta_max + (Convert.ToDouble(dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[i].Value)), 4);
                S1 = Convert.ToString(delta);
                S2 = S1.Replace(',', '.');
                S = S + ", " + S2;
            }
            string SQL = "INSERT INTO Данные VALUES( " + S + " )";
            SQLiteCommand cmd = new SQLiteCommand(SQL, SQLiteConn);
            cmd.ExecuteNonQuery();
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox2.Text == "")
            {
                MessageBox.Show("Введите значения T и A", "Ошибка");
                return;
            }
            else
            {
                label1.Enabled = true;
                groupBox2.Enabled = true;
                splitContainer12.Enabled = true;
                splitContainer7.Enabled = true;
                splitContainer1.Enabled = true;
                First(); 
                checkedListBox1.Items.Clear();
                for (int i = 1; i < dTable.Columns.Count; i++) 
                    checkedListBox1.Items.Add(i);
                Prover();
            }
        }

        private void First()
        { 
            dataGridView2.Rows.Clear();
            A = Convert.ToDouble(textBox2.Text);
            T = Convert.ToDouble(textBox1.Text);
            MASIV();
            //расчет 1ого ур декомпазиции            
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            chart1.Series[3].Points.Clear();
            chart6.Series[0].Points.Clear();
            chart6.Series[1].Points.Clear();
            chart6.Series[2].Points.Clear();
            chart6.Series[3].Points.Clear();
            ogr_x_min = 100000;
            ogr_y_min = 100000;
            ogr_y_max = -100000;
            ogr_x_max = -100000;

            if (firstlvl != null) firstlvl = null;
            firstlvl = new double[dTable.Rows.Count + 1, 14];

            double dip;
            int col_a, col_m, col_m_pr, col_a_pr, series;
            
            dip = 0; col_m = 1; col_a = 2; col_m_pr = 8; col_a_pr = 9; series = 0;
            first_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr);
            Print_first_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series);

            dip = T; col_m = 3; col_a = 4; col_m_pr = 10; col_a_pr = 11; series = 1;
            first_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr);
            Print_first_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series);

            dip = (-1) * T; col_m = 5; col_a = 6; col_m_pr = 12; col_a_pr = 13; series = 2;
            first_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr);
            Print_first_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series);
            PrintPP();
        }
        private void first_lvl_and_td(double dip, int col_m, int col_a, int col_m_pr, int col_a_pr)
        {
            double sum_m = 0, sum_a = 0, avg_m = 0, avg_a = 0, avg_pr_m = 0, avg_pr_a = 0;
            int N = dTable.Rows.Count - 1, m;

            for (int n = 0; n < dTable.Rows.Count; n++)
            {
                sum_m = 0;
                sum_a = 0;

                for (int j = 0; j < dTable.Columns.Count - 1; j++)
                {
                    sum_m = sum_m + Math.Pow((MAS[n, j] + dip), 2);
                    sum_a = sum_a + (MAS[0, j] + dip) * (MAS[n, j] + dip);
                }
                firstlvl[n, col_m] = Math.Sqrt(sum_m);
                avg_m = avg_m + firstlvl[n, col_m];
                if ((ogr_y_min > firstlvl[n, col_m])) ogr_y_min = firstlvl[n, col_m];
                if (ogr_y_max < firstlvl[n, col_m]) ogr_y_max = firstlvl[n, col_m];

                if (n == 0)
                {
                    firstlvl[n, col_a] = 0;
                }
                else
                {
                    sum_a = sum_a / (firstlvl[n, col_m] * firstlvl[0, col_m]);
                    if (sum_a > 1) sum_a = 1;

                    m = (int)(Math.Acos(sum_a) * Math.Pow(10, 6));
                    sum_a = m / Math.Pow(10, 6);
                    avg_a = avg_a + (206265 * sum_a);
                    firstlvl[n, col_a] = 206265 * sum_a;
                    if ((ogr_x_min > firstlvl[n, col_a])) ogr_x_min = firstlvl[n, col_a];
                    if (ogr_x_max < firstlvl[n, col_a]) ogr_x_max = firstlvl[n, col_a];
                }
            }
            for (int n = 0; n < dTable.Rows.Count; n++)
            {


                if (n == 0)
                {
                    firstlvl[n, col_m_pr] = A * firstlvl[n, col_m] + (1 - A) * (avg_m / (dTable.Rows.Count));
                }
                else
                {
                    firstlvl[n, col_m_pr] = A * firstlvl[n, col_m] + (1 - A) * firstlvl[n - 1, col_m_pr];
                }

                if (n != 0)
                {
                    if (n == 1)
                    {
                        firstlvl[n, col_a_pr] = A * firstlvl[n, col_a] + (1 - A) * (avg_a / (dTable.Rows.Count));                    
                    }
                    else
                    {
                        firstlvl[n, col_a_pr] = A * firstlvl[n, col_a] + (1 - A) * firstlvl[n - 1, col_a];                       
                    }
                }
                avg_pr_m = avg_pr_m + firstlvl[n, col_m_pr];
                avg_pr_a = avg_pr_a + firstlvl[n, col_a_pr];
            }
            firstlvl[N + 1, col_m_pr] = A * (avg_pr_m / (N + 1)) + (1 - A) * firstlvl[N, col_m_pr];
            firstlvl[N + 1, col_a_pr] = A * (avg_pr_a / (N - 1)) + (1 - A) * firstlvl[N, col_a_pr];
            firstlvl[N + 1, col_m] = firstlvl[N + 1, col_m_pr];
            firstlvl[N + 1, col_a] = firstlvl[N + 1, col_a_pr];


        }

        private void PrintPP()
        {

            for (int n = 0; n < dTable.Rows.Count + 1; n++)
            {
                double L = Math.Abs(firstlvl[0, 1] - firstlvl[n, 1]);
                double R = (Math.Abs(firstlvl[n, 3] - firstlvl[n, 5])) / 2.0;
                if (L != R)
                {
                    if (L>R)
                    {
                        MessageInfo++;
                        dataGridView2.Rows[n].Cells[7].Value = "Аварийное";
                        dataGridView2.Rows[n].Cells[7].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dataGridView2.Rows[n].Cells[7].Value = "Не аварийное";
                        dataGridView2.Rows[n].Cells[7].Style.BackColor = Color.Green;
                        dataGridView2.Rows[n].Cells[7].Style.ForeColor = Color.White;
                    }
                }
                else
                {
                    MessageInfo++;
                    dataGridView2.Rows[n].Cells[7].Value = "Предаварийное";
                    dataGridView2.Rows[n].Cells[7].Style.BackColor = Color.Yellow;
                }
            }
        }

        private void Print_first_lvl(double dip, int col_m, int col_a, int col_m_pr, int col_a_pr, int series)
        {
            int N = dTable.Rows.Count - 1;
            for (int n = 0; n < dTable.Rows.Count; n++)
            {
                if (dip == 0)
                {
                    dataGridView2.Rows.Add(); 
                    dataGridView2.Rows[n].Cells[0].Value = n;
                }
                dataGridView2.Rows[n].Cells[col_m].Value = Math.Round(firstlvl[n, col_m], 5);
                dataGridView2.Rows[n].Cells[col_a].Value = Math.Round(firstlvl[n, col_a], 5);
                dataGridView2.Rows[n].Cells[col_m_pr].Value = Math.Round(firstlvl[n, col_m_pr], 5);
                dataGridView2.Rows[n].Cells[col_a_pr].Value = Math.Round(firstlvl[n, col_a_pr], 5);

                chart1.Series[series].Points.AddXY(firstlvl[n, col_m], firstlvl[n, col_a]);
                chart1.Series[series].Points[n].Label = Convert.ToString(n);
                chart1.Series[series].Points[n].MarkerStyle = MarkerStyle.Circle;
                chart1.Series[series].Points[n].MarkerSize = 5;
                if (dip == 0)
                {
                    chart1.Series[3].Points.AddXY(firstlvl[n, col_m_pr], firstlvl[n, col_a_pr]);
                    chart1.Series[3].Points[n].Label = Convert.ToString(n);
                    chart1.Series[3].Points[n].MarkerStyle = MarkerStyle.Circle;
                    chart1.Series[3].Points[n].MarkerSize = 5;
                }

            }
            dataGridView2.Rows[dTable.Rows.Count].Cells[0].Value = "Прогноз";
            dataGridView2.Rows[dTable.Rows.Count].Cells[col_m_pr].Value = Math.Round(firstlvl[dTable.Rows.Count, col_m_pr], 5);
            dataGridView2.Rows[dTable.Rows.Count].Cells[col_a_pr].Value = Math.Round(firstlvl[dTable.Rows.Count, col_a_pr], 5);
            dataGridView2.Rows[dTable.Rows.Count].Cells[col_m].Value = Math.Round(firstlvl[dTable.Rows.Count, col_m], 5);
            dataGridView2.Rows[dTable.Rows.Count].Cells[col_a].Value = Math.Round(firstlvl[dTable.Rows.Count, col_a], 5);
            chart1.Series[series].Points.AddXY(firstlvl[dTable.Rows.Count, col_m], firstlvl[dTable.Rows.Count, col_a]);
            chart1.Series[series].Points[dTable.Rows.Count].Label = Convert.ToString(dTable.Rows.Count);
            chart1.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Star5;
            chart1.Series[series].Points[dTable.Rows.Count].MarkerSize = 15;
            if (dip == 0)
            {
                chart1.Series[3].Points.AddXY(firstlvl[dTable.Rows.Count, col_m_pr], firstlvl[dTable.Rows.Count, col_a_pr]);
                chart1.Series[3].Points[dTable.Rows.Count].Label = Convert.ToString(dTable.Rows.Count);
                chart1.Series[3].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Star5;
                chart1.Series[3].Points[dTable.Rows.Count].MarkerSize = 15;
            }
            double sr_y = (ogr_y_max - ogr_y_min) / 10;
            double sr_x = ((ogr_x_max - ogr_x_min) / 10);
            chart1.ChartAreas[0].AxisX.Minimum = ogr_y_min - sr_y;
            chart1.ChartAreas[0].AxisX.Maximum = ogr_y_max + sr_y;
            if (ogr_x_min == ogr_x_max)
            {
                chart1.ChartAreas[0].AxisY.Minimum = ogr_x_min - 5;
                chart1.ChartAreas[0].AxisY.Maximum = ogr_x_max + 5;
            }
            else
            {
                chart1.ChartAreas[0].AxisY.Minimum = ogr_x_min - sr_x;
                chart1.ChartAreas[0].AxisY.Maximum = ogr_x_max + sr_x;
            }
            


            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                chart6.Series[series].Points.AddXY(i, firstlvl[i, col_m]);
                chart6.Series[series].Points[i].MarkerStyle = MarkerStyle.Circle;
                chart6.Series[series].Points[i].MarkerSize = 5;

                if (dip == 0)
                {
                    chart6.Series[3].Points.AddXY(i, firstlvl[i, col_m_pr]);
                    chart6.Series[3].Points[i].MarkerStyle = MarkerStyle.Circle;
                    chart6.Series[3].Points[i].MarkerSize = 5;
                }
            }
            chart6.Series[series].Points.AddXY(dTable.Rows.Count, firstlvl[dTable.Rows.Count, col_m]);
            chart6.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Circle;
            chart6.Series[series].Points[dTable.Rows.Count].MarkerSize = 5;
            if (dip == 0)
            {
                chart6.Series[3].Points.AddXY(dTable.Rows.Count, firstlvl[dTable.Rows.Count, col_m_pr]);
                chart6.Series[3].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Circle;
                chart6.Series[3].Points[dTable.Rows.Count].MarkerSize = 5;
            }
            chart6.ChartAreas[0].AxisY.Minimum = ogr_y_min - sr_y;
            chart6.ChartAreas[0].AxisY.Maximum = ogr_y_max + sr_y;
        }


        private void Prover()
        {
            chart1.Series[0].Enabled = checkBox1.Checked;
            chart1.Series[1].Enabled = checkBox3.Checked;
            chart1.Series[2].Enabled = checkBox3.Checked;
            chart1.Series[3].Enabled = checkBox2.Checked;
        }




        private void checkBox_Click(object sender, EventArgs e)
        {
            Prover();
        }
        private void checkBox_Click_2_lvl(object sender, EventArgs e)
        {
            Prover_2_lvl();
        }
        private void checkBox_Click_3_lvl(object sender, EventArgs e)
        {
            Prover_3_lvl();
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            int lim_metok;
            if ((textBox3.Text != "") && (textBox3.Text != "0") && (textBox3.Text != "1"))
            {
                Kol_Blocks = Convert.ToInt32(textBox3.Text);

                if (Kol_Blocks * 2 <= dTable.Columns.Count - 1)
                {
                    label5.Enabled = true;
                    comboBox1.Enabled = true;


                    lim_metok = (dTable.Columns.Count - 1) / Kol_Blocks;
                    comboBox1.Items.Clear();
                    for (int i = 1; i < lim_metok; i++)
                        comboBox1.Items.Add(i + 1);
                }
                else
                {
                    groupBox3.Enabled = false;
                    label5.Enabled = false;
                    comboBox1.Enabled = false;
                    MessageBox.Show("Кол-во марок на блоке должно быть больше или равно 2", "Ошибка");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox3.Enabled = true;
            Data = Convert.ToInt32(comboBox1.Text);
            Kol_Blocks = Convert.ToInt32(textBox3.Text);
            if (Bloks != null) Bloks = null;
            if (Bloks_3 != null) Bloks_3 = null;
            comboBox2.Items.Clear();
            comboBox4.Items.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();

            Bloks = new double[Kol_Blocks, dTable.Rows.Count + 1, Data];
            Bloks_3 = new double[Kol_Blocks, dTable.Rows.Count + 1, dTable.Columns.Count];
            splitContainer9.Enabled = true;
            for (int i = 65; i - 65 < Kol_Blocks; i++)
            {
                comboBox2.Items.Add((char)i);
                comboBox4.Items.Add((char)i);
            }
            comboBox2.SelectedIndex = 0;

            for (int i = 0; i < dTable.Columns.Count - 1; i++)
            {
                listBox1.Items.Add(i + 1);
                listBox3.Items.Add(i + 1);
            }
        }
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
           
            listBox2.Items.Clear();
            for (int i = 0; i < Data; i++)
                if (Bloks[comboBox2.SelectedIndex, 0, i] != 0)
                    listBox2.Items.Add(Bloks[comboBox2.SelectedIndex, 0, i]);
        }

        private void listBox1_Click(object sender, EventArgs e)
        {

            if (listBox2.Items.Count < Data)
            {
                listBox2.Items.Add(listBox1.SelectedItem);
                listBox4.Items.Add(listBox1.SelectedItem);
                listBox1.Items.Remove(listBox1.SelectedItem);
                listBox3.Items.Remove(listBox1.SelectedItem);
                FillBlocks();
            }

        }
        private void listBox6_Click(object sender, EventArgs e)
        {
            if (listBox5.Items.Count < Data_3_lvl)
            {
                listBox5.Items.Add(listBox6.SelectedItem);
                listBox6.Items.Remove(listBox6.SelectedItem);
                FillBlocks_3_lvl_calculation();
            }
        }
        private void FillBlocks_3_lvl_calculation()
        {
            ClearBlock_3_lvl_calculation(comboBox6.SelectedIndex);
            for (int i = 0; i < listBox5.Items.Count; i++)
            {
                for (int j = 0; j < dTable.Rows.Count + 1; j++)
                {
                    if (j == 0)
                        Bloks_3_lvl[comboBox6.SelectedIndex, 0, i] = Convert.ToInt32(listBox5.Items[i]);
                    else
                        Bloks_3_lvl[comboBox6.SelectedIndex, j, i] = MAS[j - 1, Convert.ToInt32(listBox5.Items[i]) - 1];
                }
            }
        }
        private void ClearBlock_3_lvl_calculation(int b)
        {
            for (int i = 0; i < Data_3_lvl; i++)
                for (int j = 0; j <= dTable.Rows.Count - 1; j++)
                {
                    Bloks_3_lvl[b, j, i] = 0;
                }
        }
        private void FillBlocks()
        {
            ClearBlock(comboBox2.SelectedIndex);
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                for (int j = 0; j < dTable.Rows.Count + 1; j++)
                {
                    if (j == 0)
                    {
                        Bloks[comboBox2.SelectedIndex, 0, i] = Convert.ToInt32(listBox2.Items[i]);
                        Bloks_3[comboBox2.SelectedIndex, 0, i] = Convert.ToInt32(listBox2.Items[i]);
                    }
                    else
                    {
                        Bloks[comboBox2.SelectedIndex, j, i] = MAS[j - 1, Convert.ToInt32(listBox2.Items[i]) - 1];
                        Bloks_3[comboBox2.SelectedIndex, j, i] = MAS[j - 1, Convert.ToInt32(listBox2.Items[i]) - 1];
                    }
                }
            }
        }
        private void ClearBlock(int b)//нужно исправить 3х мерн массив
        {
            for (int i = 0; i < Data; i++)
                for (int j = 0; j <= dTable.Rows.Count - 1; j++)
                {
                    Bloks[b, j, i] = 0;
                    Bloks_3[b, j, i] = 0;
                }


        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(listBox2.SelectedItem);
            listBox3.Items.Add(listBox2.SelectedItem);
            listBox2.Items.Remove(listBox2.SelectedItem);
            listBox4.Items.Remove(listBox2.SelectedItem);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int s = 0;
            double ogr_y_min = 100000, ogr_y_max = -10000000, sr = 0;
            chart3.Series.Clear();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    CreateSerie(i, ref ogr_y_min, ref ogr_y_max);
                    s++;
                }
            }
            sr = (ogr_y_max - ogr_y_min);
            chart3.ChartAreas[0].AxisY.Minimum = ogr_y_min - sr / s;
            chart3.ChartAreas[0].AxisY.Maximum = ogr_y_max + sr / s;
        }

        private void CreateSerie(int numserie, ref double ogr_y_min, ref double ogr_y_max)
        {
            double x, y;
            string nameserie = Convert.ToString(numserie + 1);
            chart3.Series.Add(new Series(nameserie));
            chart3.Series[nameserie].ChartType = (SeriesChartType)4;
            chart3.Series[nameserie].Enabled = true;
            


            for (int p = 0; p < dTable.Rows.Count; p++)
            {
                x = p;
                y = MAS[p, numserie];
                if (y > ogr_y_max) ogr_y_max = y;
                if (y < ogr_y_min) ogr_y_min = y;
                chart3.Series[nameserie].Points.AddXY(x, y);
                chart3.Series[nameserie].Points[p].MarkerStyle = MarkerStyle.Circle;
                chart3.Series[nameserie].Points[p].MarkerSize = 7;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            for (int i = 0; i < Data; i++)
                if (Bloks[comboBox2.SelectedIndex, 0, i] == 0)
                {
                    MessageBox.Show("Ошибка. Проверьте выбранные метки.\nВыбраны не все метки", "Ошибка");
                    splitContainer2.Enabled = false;
                    return;
                }
            splitContainer2.Enabled = true;
            for (int i = 65; i - 65 < Kol_Blocks; i++)
                comboBox3.Items.Add((char)i);
            two_lvl();
            comboBox3.SelectedIndex = 0;
            groupBox4.Enabled = true;
            listBox3.Items.Clear();
            for (int i = 0; i < listBox1.Items.Count; i++)
                listBox3.Items.Add((listBox1.Items[i]));
            comboBox4.SelectedIndex = 0;

        }
        private void three_lvl()
        {

            A = Convert.ToDouble(textBox2.Text);
            T = Convert.ToDouble(textBox1.Text);

            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            chart4.Series[2].Points.Clear();
            chart4.Series[3].Points.Clear();
            chart7.Series[0].Points.Clear();
            chart7.Series[1].Points.Clear();
            chart7.Series[2].Points.Clear();
            chart7.Series[3].Points.Clear();
            if (Thirdlvl != null) Thirdlvl = null;
            Thirdlvl = new double[Kol_Blocks_3_lvl, dTable.Rows.Count + 1, 14];

            double dip;
            int col_a, col_m, col_m_pr, col_a_pr, era;


            for (int i = 0; i < Kol_Blocks_3_lvl; i++)
            {
                era = i;
                dip = 0; col_m = 1; col_a = 2; col_m_pr = 8; col_a_pr = 9;
                test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
                dip = T; col_m = 3; col_a = 4; col_m_pr = 10; col_a_pr = 11;
                test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
                dip = (-1) * T; col_m = 5; col_a = 6; col_m_pr = 12; col_a_pr = 13;
                test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            }
        }
        private void test_3_lvl_and_td(double dip, int col_m, int col_a, int col_m_pr, int col_a_pr, int era) 
        {

            double sum_m = 0, sum_a = 0, avg_m = 0, avg_a = 0, avg_pr_m = 0, avg_pr_a = 0;
            int N = dTable.Rows.Count - 1, m;

            for (int n = 1; n < dTable.Rows.Count + 1; n++)
            {
                sum_m = 0;
                sum_a = 0;

                for (int j = 0; j < Data_3_lvl; j++)
                {
                    sum_m = sum_m + Math.Pow((Bloks_3_lvl[era, n, j] + dip), 2);
                    sum_a = sum_a + (Bloks_3_lvl[era, 1, j] + dip) * (Bloks_3_lvl[era, n, j] + dip);
                }
                double gg = Math.Sqrt(sum_m);
                Thirdlvl[era, n - 1, col_m] = Math.Sqrt(sum_m);
                avg_m = avg_m + Thirdlvl[era, n - 1, col_m];
                if ((ogr_y_min_3_lvl > Thirdlvl[era, n - 1, col_m])) ogr_y_min_3_lvl = Thirdlvl[era, n - 1, col_m];
                if (ogr_y_max_3_lvl < Thirdlvl[era, n - 1, col_m]) ogr_y_max_3_lvl = Thirdlvl[era, n - 1, col_m];

                if (n == 1)
                {
                    Thirdlvl[era, n - 1, col_a] = 0;
                }
                else
                {
                    sum_a = sum_a / (Thirdlvl[era, n - 1, col_m] * Thirdlvl[era, 0, col_m]);
                    if (sum_a > 1) sum_a = 1;
                    m = (int)(Math.Acos(sum_a) * Math.Pow(10, 6));
                    sum_a = m / Math.Pow(10, 6);
                    avg_a = avg_a + (206265 * sum_a);                    
                    Thirdlvl[era, n - 1, col_a] = 206265 * sum_a;
                    if ((ogr_x_min_3_lvl > Thirdlvl[era, n - 1, col_a])) ogr_x_min_3_lvl = Thirdlvl[era, n - 1, col_a];
                    if (ogr_x_max_3_lvl < Thirdlvl[era, n - 1, col_a]) ogr_x_max_3_lvl = Thirdlvl[era, n - 1, col_a];
                }
            }
            for (int n = 0; n < dTable.Rows.Count; n++)
            {


                if (n == 0)
                {
                    Thirdlvl[era, n, col_m_pr] = A * Thirdlvl[era, n, col_m] + (1 - A) * (avg_m / (dTable.Rows.Count));
                }
                else
                {
                    Thirdlvl[era, n, col_m_pr] = A * Thirdlvl[era, n, col_m] + (1 - A) * Thirdlvl[era, n - 1, col_m_pr];
                }

                if (n != 0)
                {
                    if (n == 1)
                    {
                        Thirdlvl[era, n, col_a_pr] = A * Thirdlvl[era, n, col_a] + (1 - A) * (avg_a / (dTable.Rows.Count));                  
                    }
                    else
                    {
                        Thirdlvl[era, n, col_a_pr] = A * Thirdlvl[era, n, col_a] + (1 - A) * Thirdlvl[era, n - 1, col_a];                     
                    }
                }
                avg_pr_m = avg_pr_m + Thirdlvl[era, n, col_m_pr];
                avg_pr_a = avg_pr_a + Thirdlvl[era, n, col_a_pr];
            }
            Thirdlvl[era, N + 1, col_m_pr] = A * (avg_pr_m / (N + 1)) + (1 - A) * Thirdlvl[era, N, col_m_pr];
            Thirdlvl[era, N + 1, col_a_pr] = A * (avg_pr_a / (N - 1)) + (1 - A) * Thirdlvl[era, N, col_a_pr]; 
            Thirdlvl[era, N + 1, col_m] = Thirdlvl[era, N + 1, col_m_pr];
            Thirdlvl[era, N + 1, col_a] = Thirdlvl[era, N + 1, col_a_pr]; 
        }

        private void PrintPP_3_lvl(int era)
        {
            
            for (int n = 0; n < dTable.Rows.Count + 1; n++)
            {
                double L = Math.Abs(Thirdlvl[era, 0, 1] - Thirdlvl[era, n, 1]);
                double R = (Math.Abs(Thirdlvl[era, n, 3] - Thirdlvl[era, n, 5])) / 2.0;

                if (L != R)
                {
                    if (L > R)
                    {
                        MessageInfo++;
                        dataGridView6.Rows[n].Cells[7].Value = "Аварийное";
                        dataGridView6.Rows[n].Cells[7].Style.BackColor = Color.Red;
                    }

                    else
                    {
                        dataGridView6.Rows[n].Cells[7].Value = "Не аварийное";
                        dataGridView6.Rows[n].Cells[7].Style.BackColor = Color.Green;
                        dataGridView6.Rows[n].Cells[7].Style.ForeColor = Color.White;
                    }
                }
                else
                {
                    MessageInfo++;
                    dataGridView6.Rows[n].Cells[7].Value = "Предаварийное";
                    dataGridView6.Rows[n].Cells[7].Style.BackColor = Color.Yellow;
                }
            }

        }

        private void two_lvl()
        {

            A = Convert.ToDouble(textBox2.Text);
            T = Convert.ToDouble(textBox1.Text);
           

            if (Secondlvl != null) Secondlvl = null;
            Secondlvl = new double[Kol_Blocks, dTable.Rows.Count + 1, 14];

            double dip;
            int col_a, col_m, col_m_pr, col_a_pr, era;
          
            for (int i = 0; i < Kol_Blocks; i++)
            {
                era = i;
                dip = 0; col_m = 1; col_a = 2; col_m_pr = 8; col_a_pr = 9;
                test_2_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
                dip = T; col_m = 3; col_a = 4; col_m_pr = 10; col_a_pr = 11;
                test_2_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
                dip = (-1) * T; col_m = 5; col_a = 6; col_m_pr = 12; col_a_pr = 13;
                test_2_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            }
        }
        private void test_2_lvl_and_td(double dip, int col_m, int col_a, int col_m_pr, int col_a_pr, int era) 
        {

            double sum_m = 0, sum_a = 0, avg_m = 0, avg_a = 0, avg_pr_m = 0, avg_pr_a = 0;
            int N = dTable.Rows.Count - 1, m;

            for (int n = 1; n < dTable.Rows.Count + 1; n++)
            {
                sum_m = 0;
                sum_a = 0;

                for (int j = 0; j < Data; j++)
                {
                    sum_m = sum_m + Math.Pow((Bloks[era, n, j] + dip), 2);
                    sum_a = sum_a + (Bloks[era, 1, j] + dip) * (Bloks[era, n, j] + dip);
                }
                double gg = Math.Sqrt(sum_m);
                Secondlvl[era, n - 1, col_m] = Math.Sqrt(sum_m);
                avg_m = avg_m + Secondlvl[era, n - 1, col_m];
                if ((ogr_y_min_2_lvl > Secondlvl[era, n - 1, col_m])) ogr_y_min_2_lvl = Secondlvl[era, n - 1, col_m];
                if (ogr_y_max_2_lvl < Secondlvl[era, n - 1, col_m]) ogr_y_max_2_lvl = Secondlvl[era, n - 1, col_m];

                if (n == 1)
                {
                    Secondlvl[era, n - 1, col_a] = 0; //alfa
                }
                else
                {
                    sum_a = sum_a / (Secondlvl[era, n - 1, col_m] * Secondlvl[era, 0, col_m]);
                    if (sum_a > 1) sum_a = 1;
                    m = (int)(Math.Acos(sum_a) * Math.Pow(10, 6));
                    sum_a = m / Math.Pow(10, 6);
                    avg_a = avg_a + (206265 * sum_a);            
                    
                    Secondlvl[era, n - 1, col_a] = 206265 * sum_a;
                    if ((ogr_x_min_2_lvl > Secondlvl[era, n - 1, col_a])) ogr_x_min_2_lvl = Secondlvl[era, n - 1, col_a];
                    if (ogr_x_max_2_lvl < Secondlvl[era, n - 1, col_a]) ogr_x_max_2_lvl = Secondlvl[era, n - 1, col_a];
                }
            }
            for (int n = 0; n < dTable.Rows.Count; n++)
            {


                if (n == 0)
                {
                    Secondlvl[era, n, col_m_pr] = A * Secondlvl[era, n, col_m] + (1 - A) * (avg_m / (dTable.Rows.Count));
                }
                else
                {
                    Secondlvl[era, n, col_m_pr] = A * Secondlvl[era, n, col_m] + (1 - A) * Secondlvl[era, n - 1, col_m_pr];
                }

                if (n != 0)
                {
                    if (n == 1) // расчет alfa прогнозные
                    {
                        Secondlvl[era, n, col_a_pr] = A * Secondlvl[era, n, col_a] + (1 - A) * (avg_a / (dTable.Rows.Count)); // alfa прогнозное                        
                    }
                    else
                    {
                        Secondlvl[era, n, col_a_pr] = A * Secondlvl[era, n, col_a] + (1 - A) * Secondlvl[era, n - 1, col_a]; // alfa прогнозное                        
                    }
                }
                avg_pr_m = avg_pr_m + Secondlvl[era, n, col_m_pr];
                avg_pr_a = avg_pr_a + Secondlvl[era, n, col_a_pr];
            }
            Secondlvl[era, N + 1, col_m_pr] = A * (avg_pr_m / (N + 1)) + (1 - A) * Secondlvl[era, N, col_m_pr]; //прогнозые M
            Secondlvl[era, N + 1, col_a_pr] = A * (avg_pr_a / (N - 1)) + (1 - A) * Secondlvl[era, N, col_a_pr]; //прогнозые альфа
            Secondlvl[era, N + 1, col_m] = Secondlvl[era, N + 1, col_m_pr]; 
            Secondlvl[era, N + 1, col_a] = Secondlvl[era, N + 1, col_a_pr]; 
        }

        private void PrintPP_2_lvl(int era)
        {

            for (int n = 0; n < dTable.Rows.Count + 1; n++)
            {
                double L = Math.Abs(Secondlvl[era, 0, 1] - Secondlvl[era, n, 1]);
                double R = (Math.Abs(Secondlvl[era, n, 3] - Secondlvl[era, n, 5])) / 2.0;

                if (L != R)
                {
                    if (L > R)
                    {
                        MessageInfo++;
                        dataGridView3.Rows[n].Cells[7].Value = "Аварийное";
                        dataGridView3.Rows[n].Cells[7].Style.BackColor = Color.Red;
                    }

                    else
                    {
                        dataGridView3.Rows[n].Cells[7].Value = "Не аварийное";
                        dataGridView3.Rows[n].Cells[7].Style.BackColor = Color.Green;
                        dataGridView3.Rows[n].Cells[7].Style.ForeColor = Color.White;
                    }
                }
                else
                {
                    MessageInfo++;
                    dataGridView3.Rows[n].Cells[7].Value = "Предаварийное";
                    dataGridView3.Rows[n].Cells[7].Style.BackColor = Color.Yellow;
                }
            }

        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            { Form2 form2 = new Form2(pictureBox1.Image); form2.Show(); }
        }
        private void listBox3_Click(object sender, EventArgs e)
        {
            if (listBox3.Items.Count != 0)
            {
                listBox4.Items.Add(listBox3.SelectedItem);
                listBox3.Items.Remove(listBox3.SelectedItem);
                FillBlocks_3_lvl();
            }
        }
        private void FillBlocks_3_lvl()
        {
            ClearBlock_3_lvl(comboBox4.SelectedIndex);
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                for (int j = 0; j < dTable.Rows.Count + 1; j++)
                {
                    if (j == 0)
                        Bloks_3[comboBox4.SelectedIndex, 0, i] = Convert.ToInt32(listBox4.Items[i]);
                    else
                        Bloks_3[comboBox4.SelectedIndex, j, i] = MAS[j - 1, Convert.ToInt32(listBox4.Items[i]) - 1];
                }
            }
        }
        private void ClearBlock_3_lvl(int b)
        {
            for (int i = 0; i < Kol_Marok_3_lvl; i++)
                for (int j = 0; j <= dTable.Rows.Count - 1; j++)
                {
                    Bloks_3[b, j, i] = 0;
                }
        }

        private void listBox4_Click(object sender, EventArgs e) 
        {
            listBox3.Items.Add(listBox4.SelectedItem);
            listBox4.Items.Remove(listBox4.SelectedItem);
            FillBlocks_3_lvl();
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int lim_metok;
            if ((textBox4.Text != "") && (Convert.ToInt32(textBox4.Text)>=2))
            {
                Kol_Blocks_3_lvl = Convert.ToInt32(textBox4.Text);

                if (Kol_Blocks_3_lvl * 2 <= listBox4.Items.Count)
                {
                    lim_metok = (listBox4.Items.Count) / Kol_Blocks_3_lvl;
                    comboBox7.Items.Clear();
                    for (int i = 1; i < lim_metok; i++)
                        comboBox7.Items.Add(i + 1);
                }
            }
            else
            {
                MessageBox.Show("Кол-во марок на подблоке должно быть больше либо равно 2", "Ошибка");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            groupBox3.Enabled = true;
            Data_3_lvl = Convert.ToInt32(comboBox7.Text);
            Kol_Blocks_3_lvl = Convert.ToInt32(textBox4.Text);
            if (Bloks_3_lvl != null) Bloks_3_lvl = null;
            comboBox2.Items.Clear();
            listBox6.Items.Clear();
            listBox5.Items.Clear();
            comboBox6.Items.Clear();
            Bloks_3_lvl = new double[Kol_Blocks_3_lvl, dTable.Rows.Count + 1, Data_3_lvl];

            for (int i = 0; i < Kol_Blocks_3_lvl; i++)
            {
                comboBox6.Items.Add(Convert.ToString(i + 1) + " подблок");
            }
            comboBox6.SelectedIndex = 0;

            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox6.Items.Add(listBox4.Items[i]);
            }
        }

        private void listBox5_Click(object sender, EventArgs e)
        {
            if (listBox5.Items.Count != 0)
            {
                listBox6.Items.Add(listBox5.SelectedItem);
                listBox5.Items.Remove(listBox5.SelectedItem);
                FillBlocks_3_lvl_calculation();
            }
        }

        private void comboBox6_SelectedValueChanged(object sender, EventArgs e)
        {
            listBox5.Items.Clear();
            for (int i = 0; i < Data_3_lvl; i++)
                if (Bloks_3_lvl[comboBox6.SelectedIndex, 0, i] != 0)
                    listBox5.Items.Add(Bloks_3_lvl[comboBox6.SelectedIndex, 0, i]);
        }

        private void comboBox5_SelectedValueChanged(object sender, EventArgs e)
        {
            dataGridView6.Rows.Clear();
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            chart4.Series[2].Points.Clear();
            chart4.Series[3].Points.Clear();
            chart7.Series[0].Points.Clear();
            chart7.Series[1].Points.Clear();
            chart7.Series[2].Points.Clear();
            chart7.Series[3].Points.Clear();
            ogr_x_min_3_lvl = 100000;
            ogr_y_min_3_lvl = 100000;
            ogr_y_max_3_lvl = -100000;
            ogr_x_max_3_lvl = -100000;

            double dip;
            int col_a, col_m, col_m_pr, col_a_pr, series, era;

            era = comboBox5.SelectedIndex; 
            dip = 0; col_m = 1; col_a = 2; col_m_pr = 8; col_a_pr = 9; series = 0;
            test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_3_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            dip = T; col_m = 3; col_a = 4; col_m_pr = 10; col_a_pr = 11; series = 1;
            test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_3_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            dip = (-1) * T; col_m = 5; col_a = 6; col_m_pr = 12; col_a_pr = 13; series = 2;
            test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_3_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            PrintPP_3_lvl(era);
        }
        private void button12_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < Data_3_lvl; i++)
                if (Bloks_3_lvl[comboBox6.SelectedIndex, 0, i] == 0)
                {
                    MessageBox.Show("Ошибка. Проверьте выбранные метки.\nВыбраны не все метки", "Ошибка");
                    splitContainer13.Enabled = false;
                    return;
                }
            splitContainer13.Enabled = true;
            comboBox5.Items.Clear();
            for (int i = 0; i < Kol_Blocks_3_lvl; i++)
                comboBox5.Items.Add(Convert.ToString(i + 1) + " подблок");
            three_lvl();
            comboBox5.SelectedIndex = 0;
        }


        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView6.Rows.Clear();
            chart4.Series[0].Points.Clear();
            chart4.Series[1].Points.Clear();
            chart4.Series[2].Points.Clear();
            chart4.Series[3].Points.Clear();
            ogr_x_min_3_lvl = 100000;
            ogr_y_min_3_lvl = 100000;
            ogr_y_max_3_lvl = -100000;
            ogr_x_max_3_lvl = -100000;

            double dip;
            int col_a, col_m, col_m_pr, col_a_pr, series, era;

            era = comboBox5.SelectedIndex;

            dip = 0; col_m = 1; col_a = 2; col_m_pr = 8; col_a_pr = 9; series = 0;
            test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_3_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            dip = T; col_m = 3; col_a = 4; col_m_pr = 10; col_a_pr = 11; series = 1;
            test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_3_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            dip = (-1) * T; col_m = 5; col_a = 6; col_m_pr = 12; col_a_pr = 13; series = 2;
            test_3_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_3_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            PrintPP_3_lvl(era);
        }

        private void checkBox15_Click(object sender, EventArgs e)
        {
            first_lvl_chart_m_t();
        }

        private void first_lvl_chart_m_t()
        {
            chart6.Series[0].Enabled = checkBox15.Checked;
            chart6.Series[1].Enabled = checkBox16.Checked;
            chart6.Series[2].Enabled = checkBox17.Checked;
            chart6.Series[3].Enabled = checkBox14.Checked;
        }
        private void checkBox10_Click(object sender, EventArgs e)
        {
            two_lvl_chart_m_t();
        }

        private void two_lvl_chart_m_t()
        {
            chart5.Series[0].Enabled = checkBox10.Checked;
            chart5.Series[1].Enabled = checkBox11.Checked;
            chart5.Series[2].Enabled = checkBox12.Checked;
            chart5.Series[3].Enabled = checkBox13.Checked;
        }
        private void checkBox19_Click(object sender, EventArgs e)
        {
            three_lvl_chart_m_t();
        }

        private void three_lvl_chart_m_t()
        {
            chart7.Series[0].Enabled = checkBox19.Checked;
            chart7.Series[1].Enabled = checkBox20.Checked;
            chart7.Series[2].Enabled = checkBox21.Checked;
            chart7.Series[3].Enabled = checkBox18.Checked;
        }
        private void Print_3_lvl(double dip, int col_m, int col_a, int col_m_pr, int col_a_pr, int series, int era)
        {
            int N = dTable.Rows.Count - 1;
            for (int n = 0; n < dTable.Rows.Count; n++)
            {
                if (dip == 0)
                {
                    dataGridView6.Rows.Add(); 
                    dataGridView6.Rows[n].Cells[0].Value = n;
                }

                dataGridView6.Rows[n].Cells[col_m].Value = Math.Round(Thirdlvl[era, n, col_m], 5); // M
                dataGridView6.Rows[n].Cells[col_a].Value = Math.Round(Thirdlvl[era, n, col_a], 5); // alfa
                dataGridView6.Rows[n].Cells[col_m_pr].Value = Math.Round(Thirdlvl[era, n, col_m_pr], 5); //прогнозые M
                dataGridView6.Rows[n].Cells[col_a_pr].Value = Math.Round(Thirdlvl[era, n, col_a_pr], 5); //прогнозые альфа

                chart4.Series[series].Points.AddXY(Thirdlvl[era, n, col_m], Thirdlvl[era, n, col_a]);
                chart4.Series[series].Points[n].Label = Convert.ToString(n);
                chart4.Series[series].Points[n].MarkerStyle = MarkerStyle.Circle;
                chart4.Series[series].Points[n].MarkerSize = 5;
                if (dip == 0)
                {
                    chart4.Series[3].Points.AddXY(Thirdlvl[era, n, col_m_pr], Thirdlvl[era, n, col_a_pr]);
                    chart4.Series[3].Points[n].Label = Convert.ToString(n);
                    chart4.Series[3].Points[n].MarkerStyle = MarkerStyle.Circle;
                    chart4.Series[3].Points[n].MarkerSize = 5;
                }

            }
            dataGridView6.Rows[dTable.Rows.Count].Cells[0].Value = "Прогноз";
            dataGridView6.Rows[dTable.Rows.Count].Cells[col_m_pr].Value = Math.Round(Thirdlvl[era, dTable.Rows.Count, col_m_pr], 5); //прогнозые M
            dataGridView6.Rows[dTable.Rows.Count].Cells[col_a_pr].Value = Math.Round(Thirdlvl[era, dTable.Rows.Count, col_a_pr], 5); //прогнозые альфа
            dataGridView6.Rows[dTable.Rows.Count].Cells[col_m].Value = Math.Round(Thirdlvl[era, dTable.Rows.Count, col_m], 5); //прогноз M
            dataGridView6.Rows[dTable.Rows.Count].Cells[col_a].Value = Math.Round(Thirdlvl[era, dTable.Rows.Count, col_a], 5); //прогноз alfa
            chart4.Series[series].Points.AddXY(Thirdlvl[era, dTable.Rows.Count, col_m], Thirdlvl[era, dTable.Rows.Count, col_a]);
            chart4.Series[series].Points[dTable.Rows.Count].Label = Convert.ToString(dTable.Rows.Count);
            chart4.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Star5;
            chart4.Series[series].Points[dTable.Rows.Count].MarkerSize = 15;

            if (dip == 0)
            {
                chart4.Series[3].Points.AddXY(Thirdlvl[era, dTable.Rows.Count, col_m_pr], Thirdlvl[era, dTable.Rows.Count, col_a_pr]);
                chart4.Series[3].Points[dTable.Rows.Count].Label = Convert.ToString(dTable.Rows.Count);
                chart4.Series[3].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Star5;
                chart4.Series[3].Points[dTable.Rows.Count].MarkerSize = 15;
            }
            double sr_y = (ogr_y_max - ogr_y_min) / 10;
            double sr_x = (ogr_x_max - ogr_x_min) / 10;
            chart4.ChartAreas[0].AxisX.Minimum = ogr_y_min_3_lvl - sr_y;
            chart4.ChartAreas[0].AxisX.Maximum = ogr_y_max_3_lvl + sr_y;
            chart4.ChartAreas[0].AxisY.Minimum = ogr_x_min_3_lvl - sr_x;
            chart4.ChartAreas[0].AxisY.Maximum = ogr_x_max_3_lvl + sr_x;


            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                chart7.Series[series].Points.AddXY(i, Thirdlvl[era, i, col_m]);
                chart7.Series[series].Points[i].MarkerStyle = MarkerStyle.Circle;
                chart7.Series[series].Points[i].MarkerSize = 5;

                if (dip == 0)
                {
                    chart7.Series[3].Points.AddXY(i, Thirdlvl[era, i, col_m_pr]);
                    chart7.Series[series].Points[i].MarkerStyle = MarkerStyle.Circle;
                    chart7.Series[series].Points[i].MarkerSize = 5;
                }
            }
            chart7.Series[series].Points.AddXY(dTable.Rows.Count, Thirdlvl[era, dTable.Rows.Count, col_m]);
            chart7.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Circle;
            chart7.Series[series].Points[dTable.Rows.Count].MarkerSize = 5;
            if (dip == 0)
            {
                chart7.Series[3].Points.AddXY(dTable.Rows.Count, Thirdlvl[era, dTable.Rows.Count, col_m_pr]);
                chart7.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Circle;
                chart7.Series[series].Points[dTable.Rows.Count].MarkerSize = 5;
            }
            chart7.ChartAreas[0].AxisY.Minimum = ogr_y_min_3_lvl - sr_y;
            chart7.ChartAreas[0].AxisY.Maximum = ogr_y_max_3_lvl + sr_y;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            dataGridView4.Columns.Clear();
            Kol_Marok_3_lvl = 0;
            if (listBox3.Items.Count != 0)
            {
                MessageBox.Show("Ошибка. Не все маркеры распределены", "Ошибка");
                return;
            }
            else
            {
                dataGridView4.Rows.Clear();
                dataGridView4.Columns.Clear();
                dataGridView5.Rows.Clear();
                dataGridView5.Columns.Clear();
                SelectedIndex_3_lvl = comboBox4.SelectedIndex;
                for (int i = 0; i < dTable.Columns.Count; i++)
                    if (Bloks_3[comboBox4.SelectedIndex, 0, i] != 0)
                        Kol_Marok_3_lvl++;
                dataGridView4.Columns.Add("Эпоха", "Эпоха");
                dataGridView5.Columns.Add("Эпоха", "Эпоха");
                int kol_Cloums_abs_3_lvl = 0;
                for (int i = 0; i < Kol_Marok_3_lvl - 1; i++)
                {
                    for (int j = i + 1; j < Kol_Marok_3_lvl; j++)
                    {
                        string ff = "H" + (Convert.ToString(Bloks_3[SelectedIndex_3_lvl, 0, i])) + " - H" + (Convert.ToString(Bloks_3[SelectedIndex_3_lvl, 0, j]));
                        string gg = "Delta |H" + (Convert.ToString(Bloks_3[SelectedIndex_3_lvl, 0, i])) + " - H" + (Convert.ToString(Bloks_3[SelectedIndex_3_lvl, 0, j])) + "|";
                        dataGridView4.Columns.Add(ff, ff);
                        dataGridView5.Columns.Add(gg, gg);
                        kol_Cloums_abs_3_lvl++;
                    }
                }
                third_lvl_abs = new double[dTable.Rows.Count, kol_Cloums_abs_3_lvl];
                int column_counter = 0;
                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    dataGridView4.Rows.Add();
                    dataGridView5.Rows.Add();
                    dataGridView4.Rows[i].Cells[0].Value = Convert.ToString(i);
                    dataGridView5.Rows[i].Cells[0].Value = Convert.ToString(i);
                }
                dataGridView5.Rows.Add();
                for (int i = 0; i < Kol_Marok_3_lvl - 1; i++)
                {
                    for (int j = i + 1; j < Kol_Marok_3_lvl; j++)
                    {
                        for (int k = 1; k < dTable.Rows.Count + 1; k++)
                        {
                            third_lvl_abs[k - 1, column_counter] = Math.Abs(Bloks_3[SelectedIndex_3_lvl, k, i] - Bloks_3[SelectedIndex_3_lvl, k, j]);
                            dataGridView4.Rows[k - 1].Cells[column_counter + 1].Value = Convert.ToString(Math.Round(third_lvl_abs[k - 1, column_counter], 4));
                            dataGridView4.Columns[column_counter + 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        column_counter++;
                    }
                }
                int plus_or_minus;
                delta_third_lvl = new double[dTable.Rows.Count, kol_Cloums_abs_3_lvl];
                for (int j = 0; j < kol_Cloums_abs_3_lvl; j++)
                {
                    plus_or_minus = 0;
                    for (int i = 0; i < dTable.Rows.Count; i++)
                    {
                        delta_third_lvl[i, j] = Math.Abs(third_lvl_abs[0, j] - third_lvl_abs[i, j]);
                        if (delta_third_lvl[i, j] > T)
                        {
                            dataGridView5.Rows[i].Cells[j + 1].Style.BackColor = Color.Red;
                            plus_or_minus += 1;
                        }
                        else
                        {
                            dataGridView5.Rows[i].Cells[j + 1].Style.BackColor = Color.Green;
                        }                        
                        dataGridView5.Rows[i].Cells[j + 1].Value = Convert.ToString(Math.Round(delta_third_lvl[i, j], 4));
                        dataGridView5.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    if (plus_or_minus == 0)
                    {
                        dataGridView5.Rows[dTable.Rows.Count].Cells[j + 1].Value = "+";
                        dataGridView5.Rows[dTable.Rows.Count].Cells[j + 1].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        dataGridView5.Rows[dTable.Rows.Count].Cells[j + 1].Value = "-";
                        dataGridView5.Rows[dTable.Rows.Count].Cells[j + 1].Style.BackColor = Color.Red;
                    }
                }


            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tabControl1.SelectedIndex == 1) && (MessageInfo != 0)) MessageBox.Show("Перейдите на следующий уровень декомпозиции", "Рекомендации");
            MessageInfo = 0;
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tabControl2.SelectedIndex == 1) && (MessageInfo != 0)) MessageBox.Show("Перейдите на следующий уровень декомпозиции", "Рекомендации");
            MessageInfo = 0;
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tabControl3.SelectedIndex == 1) && (MessageInfo != 0)) MessageBox.Show("Перейдите на следующий уровень декомпозиции", "Рекомендации");
            MessageInfo = 0;
        }

        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            Form4 gg = new Form4();
            gg.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Form5 gg = new Form5();
            gg.Show();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {                
                T = Convert.ToDouble(textBox1.Text);
                A = Convert.ToDouble(textBox2.Text);
            }
            catch (Exception ex)
            {             
                MessageBox.Show(ex.Message);
                return;
            }
            
            SQLiteCommand command_line = new SQLiteCommand("UPDATE Доп_данные SET T=" + Convert.ToString(T).Replace(',', '.') + ";", SQLiteConn);
            command_line.ExecuteNonQuery();
            
            command_line = new SQLiteCommand("UPDATE Доп_данные SET A=" + Convert.ToString(A).Replace(',', '.') + ";", SQLiteConn);
            command_line.ExecuteNonQuery();
            
            MessageBox.Show("Параметры Т и А сохранены");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form3 gg = new Form3();
            gg.Show();
        }

        private void comboBox4_SelectedValueChanged(object sender, EventArgs e)
        {
            listBox4.Items.Clear();
            for (int i = 0; i < dTable.Columns.Count; i++)
                if (Bloks_3[comboBox4.SelectedIndex, 0, i] != 0)
                    listBox4.Items.Add(Bloks_3[comboBox4.SelectedIndex, 0, i]);
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

            button4.Enabled = comboBox1.Text != "" ? true : false;

        }

        private void Print_2_lvl(double dip, int col_m, int col_a, int col_m_pr, int col_a_pr, int series, int era)
        {
            int N = dTable.Rows.Count - 1;
            for (int n = 0; n < dTable.Rows.Count; n++)
            {
                if (dip == 0)
                {
                    dataGridView3.Rows.Add(); 
                    dataGridView3.Rows[n].Cells[0].Value = n;
                }
                dataGridView3.Rows[n].Cells[col_m].Value = Math.Round(Secondlvl[era, n, col_m], 5); // M
                dataGridView3.Rows[n].Cells[col_a].Value = Math.Round(Secondlvl[era, n, col_a], 5); // alfa
                dataGridView3.Rows[n].Cells[col_m_pr].Value = Math.Round(Secondlvl[era, n, col_m_pr], 5); //прогнозые M
                dataGridView3.Rows[n].Cells[col_a_pr].Value = Math.Round(Secondlvl[era, n, col_a_pr], 5); //прогнозые альфа

                chart2.Series[series].Points.AddXY(Secondlvl[era, n, col_m], Secondlvl[era, n, col_a]);
                chart2.Series[series].Points[n].Label = Convert.ToString(n);
                chart2.Series[series].Points[n].MarkerStyle = MarkerStyle.Circle;
                chart2.Series[series].Points[n].MarkerSize = 5;
                if (dip == 0)
                {
                    chart2.Series[3].Points.AddXY(Secondlvl[era, n, col_m_pr], Secondlvl[era, n, col_a_pr]);
                    chart2.Series[3].Points[n].Label = Convert.ToString(n);
                    chart2.Series[3].Points[n].MarkerStyle = MarkerStyle.Circle;
                    chart2.Series[3].Points[n].MarkerSize = 5;
                }
            }
            dataGridView3.Rows[dTable.Rows.Count].Cells[0].Value = "Прогноз";
            dataGridView3.Rows[dTable.Rows.Count].Cells[col_m_pr].Value = Math.Round(Secondlvl[era, dTable.Rows.Count, col_m_pr], 5); //прогнозые M
            dataGridView3.Rows[dTable.Rows.Count].Cells[col_a_pr].Value = Math.Round(Secondlvl[era, dTable.Rows.Count, col_a_pr], 5); //прогнозые альфа
            dataGridView3.Rows[dTable.Rows.Count].Cells[col_m].Value = Math.Round(Secondlvl[era, dTable.Rows.Count, col_m], 5); //прогноз M
            dataGridView3.Rows[dTable.Rows.Count].Cells[col_a].Value = Math.Round(Secondlvl[era, dTable.Rows.Count, col_a], 5); //прогноз alfa
            chart2.Series[series].Points.AddXY(Secondlvl[era, dTable.Rows.Count, col_m], Secondlvl[era, dTable.Rows.Count, col_a]);
            chart2.Series[series].Points[dTable.Rows.Count].Label = Convert.ToString(dTable.Rows.Count);
            chart2.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Star5;
            chart2.Series[series].Points[dTable.Rows.Count].MarkerSize = 15;
            if (dip == 0)
            {
                chart2.Series[3].Points.AddXY(Secondlvl[era, dTable.Rows.Count, col_m_pr], Secondlvl[era, dTable.Rows.Count, col_a_pr]);
                chart2.Series[3].Points[dTable.Rows.Count].Label = Convert.ToString(dTable.Rows.Count);
                chart2.Series[3].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Star5;
                chart2.Series[3].Points[dTable.Rows.Count].MarkerSize = 15;
            }
            double sr_y = (ogr_y_max - ogr_y_min) / 10;
            double sr_x = (ogr_x_max - ogr_x_min) / 10;
            chart2.ChartAreas[0].AxisX.Minimum = ogr_y_min_2_lvl - sr_y;
            chart2.ChartAreas[0].AxisX.Maximum = ogr_y_max_2_lvl + sr_y;
            chart2.ChartAreas[0].AxisY.Minimum = ogr_x_min_2_lvl - sr_x;
            chart2.ChartAreas[0].AxisY.Maximum = ogr_x_max_2_lvl + sr_x;

            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                chart5.Series[series].Points.AddXY(i, Secondlvl[era, i, col_m]);
                chart5.Series[series].Points[i].MarkerStyle = MarkerStyle.Circle;
                chart5.Series[series].Points[i].MarkerSize = 5;

                if (dip == 0)
                {
                    chart5.Series[3].Points.AddXY(i, Secondlvl[era, i, col_m_pr]);
                    chart5.Series[3].Points[i].MarkerStyle = MarkerStyle.Circle;
                    chart5.Series[3].Points[i].MarkerSize = 5;
                }
            }
            chart5.Series[series].Points.AddXY(dTable.Rows.Count, Secondlvl[era, dTable.Rows.Count, col_m]);
            chart5.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Circle;
            chart5.Series[series].Points[dTable.Rows.Count].MarkerSize = 5;
            if (dip == 0)
            {
                chart5.Series[3].Points.AddXY(dTable.Rows.Count, Secondlvl[era, dTable.Rows.Count, col_m_pr]);
                chart5.Series[series].Points[dTable.Rows.Count].MarkerStyle = MarkerStyle.Circle;
                chart5.Series[series].Points[dTable.Rows.Count].MarkerSize = 5;
            }
            chart5.ChartAreas[0].AxisY.Minimum = ogr_y_min_2_lvl - sr_y;
            chart5.ChartAreas[0].AxisY.Maximum = ogr_y_max_2_lvl + sr_y;
        }


        private void Prover_2_lvl()
        {
            chart2.Series[0].Enabled = checkBox6.Checked;
            chart2.Series[1].Enabled = checkBox4.Checked;
            chart2.Series[2].Enabled = checkBox4.Checked;
            chart2.Series[3].Enabled = checkBox5.Checked;
        }
        private void Prover_3_lvl()
        {
            chart4.Series[0].Enabled = checkBox9.Checked;
            chart4.Series[1].Enabled = checkBox7.Checked;
            chart4.Series[2].Enabled = checkBox7.Checked;
            chart4.Series[3].Enabled = checkBox8.Checked;
        }



        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();
            chart2.Series[2].Points.Clear();
            chart2.Series[3].Points.Clear();
            chart5.Series[0].Points.Clear();
            chart5.Series[1].Points.Clear();
            chart5.Series[2].Points.Clear();
            chart5.Series[3].Points.Clear();
            ogr_x_min_2_lvl = 100000;
            ogr_y_min_2_lvl = 100000;
            ogr_y_max_2_lvl = -100000;
            ogr_x_max_2_lvl = -100000;

            double dip;
            int col_a, col_m, col_m_pr, col_a_pr, series, era;

            era = comboBox3.SelectedIndex;
            dip = 0; col_m = 1; col_a = 2; col_m_pr = 8; col_a_pr = 9; series = 0;
            test_2_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_2_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            dip = T; col_m = 3; col_a = 4; col_m_pr = 10; col_a_pr = 11; series = 1;
            test_2_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_2_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            dip = (-1) * T; col_m = 5; col_a = 6; col_m_pr = 12; col_a_pr = 13; series = 2;
            test_2_lvl_and_td(dip, col_m, col_a, col_m_pr, col_a_pr, era);
            Print_2_lvl(dip, col_m, col_a, col_m_pr, col_a_pr, series, era);
            PrintPP_2_lvl(era);

        }

        private void button8_Click(object sender, EventArgs e)
        {
            int num = dTable.Rows.Count - 1;
            string last_era = "";
            last_era = "DELETE FROM Данные WHERE Эпоха = " + num;
            SQLiteCommand cmd = new SQLiteCommand(last_era, SQLiteConn);
            cmd.ExecuteNonQuery();
            LoadData();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            if (button14.Text == "График Alfa(M)")
            {
                button14.Text = "График M(t)";
                splitContainer17.Panel1Collapsed = false;
                splitContainer17.Panel2Collapsed = true;
            }
            else
            {
                button14.Text = "График Alfa(M)";
                splitContainer17.Panel1Collapsed = true;
                splitContainer17.Panel2Collapsed = false;
            }
        }
        private void button15_Click(object sender, EventArgs e)
        {
            if (button15.Text == "График Alfa(M)")
            {
                button15.Text = "График M(t)";
                splitContainer8.Panel1Collapsed = false;
                splitContainer8.Panel2Collapsed = true;
            }
            else
            {
                button15.Text = "График Alfa(M)";
                splitContainer8.Panel1Collapsed = true;
                splitContainer8.Panel2Collapsed = false;
            }
        }
        private void button16_Click(object sender, EventArgs e)
        {
            if (button16.Text == "График Alfa(M)")
            {
                button16.Text = "График M(t)";
                splitContainer15.Panel1Collapsed = false;
                splitContainer15.Panel2Collapsed = true;
            }
            else
            {
                button16.Text = "График Alfa(M)";
                splitContainer15.Panel1Collapsed = true;
                splitContainer15.Panel2Collapsed = false;
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            {
                if (button9.Text == "Выбрать все")
                {
                    button9.Text = "Удалить все";
                    SelectDeselectAll(true);
                }
                else
                {
                    button9.Text = "Выбрать все";
                    SelectDeselectAll(false);
                }
            }
            void SelectDeselectAll(bool Selected)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, Selected);
                }
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' &&
                e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
    }
}
