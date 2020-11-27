using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

using NiceApiLibrary;
using NiceApiLibrary_low;

//namespace NiceDesktopSupportApp
//{
//    public partial class SqlTesterForm : Form
//    {
//        public static void Show(IMyLog log, QuestionOption it)
//        {
//            Form f = new SqlTesterForm();
//            f.ShowDialog();
//            f = null; 
//        }

//        private readonly string rLog = "DBFormLog.txt";
//        private string rDbConStr
//        {
//            get
//            {
//                switch (comboBox1.SelectedIndex)
//                {
//                    default:
//                        return "Data Source=C:\\HostingSpaces\\mgillman\\_NiceSolution\\ASP_Zap\\ServerState\\lite_All.sqlite;";
//                }
//            }
//        }

//        public SqlTesterForm()
//        {
//            InitializeComponent();
//            refreshLog();
//        }

//        private void refreshLog()
//        {
//            try
//            {
//                this.textBox2.Text = File.ReadAllText(rLog);
//            }
//            catch
//            {

//            }
//        }

//        private void SqlTesterForm_Load(object sender, EventArgs e)
//        {

//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                string sql = this.textBox1.Text;
//                object o = DSSwitch.sql().ProcessSql(textBox1.Text);
//                System.Data.DataTable r1 = (System.Data.DataTable)o;
//                WebControlsTableResult r2 = r1.DataTable2WebControlsTable(99);

//                this.dataGridView1.DataSource = o;
//                File.AppendAllText(rLog, sql + "\r\n");
//                refreshLog();

//                //using (SQLiteConnection db = new SQLiteConnection(rDbConStr))
//                //{
//                //    db.Open();
//                //    SQLiteCommand cmd = new SQLiteCommand(sql, db);
//                //    SQLiteDataReader reader = cmd.ExecuteReader();
//                //    DataTable dt = new DataTable();
//                //    dt.Load(reader);
//                //    this.dataGridView1.DataSource = dt;
//                //    File.AppendAllText(rLog, sql + "\r\n");
//                //    refreshLog();
//                //}
//            }
//            catch (SystemException se)
//            {
//                MessageBox.Show(se.Message);
//            }
//        }

//    }
//}
