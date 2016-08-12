using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
//using Oracle.DataAccess.Client;

namespace TestDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = "Data Source=ORCL;User ID=test;Password=test;";
            this.textBox3.Text = "select * from dual";
            this.comboBox1.Items.Add(Vic.Data.DbProviderType.Oracle);
            this.comboBox1.Items.Add(Vic.Data.DbProviderType.OracleClient);
            this.comboBox1.Items.Add(Vic.Data.DbProviderType.OracleManaged);
            this.comboBox1.SelectedIndex = 0;

            DataTable factoryDt = System.Data.Common.DbProviderFactories.GetFactoryClasses();
            this.dataGridView2.DataSource = factoryDt;

            string name = Vic.Data.DataAccessComm.GetProviderName(Vic.Data.DbProviderType.OracleManaged);
            Vic.Data.DbProviderType type = Vic.Data.DataAccessComm.GetProviderType("System.Data.OleDb");

          //System.Configuration.ConfigurationManager.ConnectionStrings[].;
          //  System.Configuration.ConfigurationSettings.

            Vic.Data.IDataAccess idata = new Vic.Data.DataAccess("", "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Oracle.DataAccess.Client.OracleDbType t = new Oracle.DataAccess.Client.OracleDbType();
            //Oracle.DataAccess.Client.OracleDataReader dr=null;
            //foreach (string s in Enum.GetNames(typeof(Oracle.DataAccess.Client.OracleDbType)))
            //{
            //    this.textBox1.AppendText(s + "  --  " + "" + Environment.NewLine);
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //Vic.Data.DataAccess dataAccess = new Vic.Data.DataAccess("Data Source=ORCL;User ID=test;Password=test;", Vic.Data.DbProviderType.OracleClient);
            
            //string ex = "";
            //bool b = dataAccess.IsConnState(out ex);

            //object[] objs = new object[] { 1, "3", new DataTable() };

            //System.Data.Common.DbDataReader reader = dataAccess.QueryReader("select * from class_info");
            //while (reader.Read())
            //{
            //    string s = reader["class_id"].ToString();
            //}
            //reader.Close();

            //this.dataGridView1.DataSource = ((DataSet)db.Result).Tables[0];

            //Vic.Data.DbResult db1 = dataAccess.Query(new string[] { "select * from class_info", "select * from dual" });
            //this.dataGridView1.DataSource = ((DataSet)db1.Result).Tables[0];

            //Vic.Data.DbResult db2 = dataAccess.Query("select * from class_info", "select * from dual");
            //this.dataGridView1.DataSource = ((DataSet)db2.Result).Tables[0];

            //dataAccess.ExecProcedure("pro_c");
            //Vic.Data.DbResult db3 = dataAccess.QueryTable("select * from s_zbb");
            //this.dataGridView1.DataSource = (DataTable)db.Result;

            //int result = dataAccess.ExecuteNonQuery("update class_info set class_id='33' where class_id='666666'");

            #region MyRegion
            //List<System.Data.Common.DbParameter> parameters = new List<System.Data.Common.DbParameter>();
            //System.Data.Common.DbParameter para = dataAccess.CreateParameter();
            //para.ParameterName = "p1";
            ////para.DbType = Vic.Data.DbTypes.OracleClientParse(Vic.Data.OracleClientType.Varchar2);
            //para.DbType = DbType.String;
            //para.Direction = ParameterDirection.Input;
            //para.Value = "P1";
            //para.Size = 20;
            //parameters.Add(para);

            //para = dataAccess.CreateParameter();
            ////para.ParameterName = "p2";
            ////para.DbType = Vic.Data.DbTypes.OracleClientParse(Vic.Data.OracleClientType.Varchar2);
            //para.DbType = DbType.String;
            //para.Direction = ParameterDirection.Output;
            //para.Size = 20;
            //parameters.Add(para);

            //para = dataAccess.CreateParameter();
            //para.ParameterName = "p3";
            ////para.DbType = Vic.Data.DbTypes.OracleClientParse(Vic.Data.OracleClientType.Int32);
            //para.DbType = DbType.Int32;
            //para.Direction = ParameterDirection.Output;
            //para.Value = 3333;
            //para.Size = 8;
            //parameters.Add(para);

            //dataAccess.ExecProcedure("pro_a", parameters, "score_info", "student_info"); 
            #endregion

            //object obj = dr.GetParamValue("p3");
            //object obj2 = dr.GetParamValue(2);

            Vic.Data.DataAccess dataAccess2 = new Vic.Data.DataAccess(this.textBox2.Text, (Vic.Data.DbProviderType)this.comboBox1.SelectedItem);
            string conn = ConfigurationManager.ConnectionStrings["oracle"].ConnectionString;
            string prname = ConfigurationManager.ConnectionStrings["oracle"].ProviderName;
            dataAccess2 = new Vic.Data.DataAccess(conn, prname);
            DataTable dt = dataAccess2.QueryPage(this.textBox3.Text, 2, 2);
            this.dataGridView1.DataSource = dt;
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            #region OracleParameters
            //OracleConnection connection = new OracleConnection("Data Source=ORCL;User ID=test;Password=test;");
            //OracleCommand oc = connection.CreateCommand();
            //oc.Parameters.Add("a", OracleDbType.Array);
            //oc.Parameters.Add("a", OracleDbType.BFile);
            //oc.Parameters.Add("a", OracleDbType.BinaryDouble);
            //oc.Parameters.Add("a", OracleDbType.BinaryFloat);
            //oc.Parameters.Add("a", OracleDbType.Blob);
            //oc.Parameters.Add("a", OracleDbType.Byte);
            //oc.Parameters.Add("a", OracleDbType.Char);
            //oc.Parameters.Add("a", OracleDbType.Clob);
            //oc.Parameters.Add("a", OracleDbType.Date);
            //oc.Parameters.Add("a", OracleDbType.Decimal);
            //oc.Parameters.Add("a", OracleDbType.Double);
            //oc.Parameters.Add("a", OracleDbType.Int16);
            //oc.Parameters.Add("a", OracleDbType.Int32);
            //oc.Parameters.Add("a", OracleDbType.Int64);
            //oc.Parameters.Add("a", OracleDbType.IntervalDS);
            //oc.Parameters.Add("a", OracleDbType.IntervalYM);
            //oc.Parameters.Add("a", OracleDbType.Long);
            //oc.Parameters.Add("a", OracleDbType.LongRaw);
            //oc.Parameters.Add("a", OracleDbType.NChar);
            //oc.Parameters.Add("a", OracleDbType.NClob);
            //oc.Parameters.Add("a", OracleDbType.NVarchar2);
            //oc.Parameters.Add("a", OracleDbType.Object);
            //oc.Parameters.Add("a", OracleDbType.Raw);
            //oc.Parameters.Add("a", OracleDbType.Ref);
            //oc.Parameters.Add("a", OracleDbType.RefCursor);
            //oc.Parameters.Add("a", OracleDbType.Single);
            //oc.Parameters.Add("a", OracleDbType.TimeStamp);
            //oc.Parameters.Add("a", OracleDbType.TimeStampLTZ);
            //oc.Parameters.Add("a", OracleDbType.TimeStampTZ);
            //oc.Parameters.Add("a", OracleDbType.Varchar2);
            //oc.Parameters.Add("a", OracleDbType.XmlType);

            //foreach (OracleParameter o in oc.Parameters)
            //{
            //    this.textBox1.AppendText(o.OracleDbType.ToString() + "----" + o.DbType.ToString() + Environment.NewLine);
            //}           
            #endregion

            #region 更新数据库
            Vic.Data.DataAccess dataAccess = new Vic.Data.DataAccess("Data Source=ORCL;User ID=test;Password=test;", Vic.Data.DbProviderType.OracleClient);
            DataTable dt = dataAccess.QueryTable("select col1,col2 from PRO_TEST");
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                dr["col2"] = "bbbb";

                //dr = dt.NewRow();
                //dr["col1"] = "3333";
                //dr["col2"] = "bdsfd";
                //dt.Rows.Add(dr);

                //dr = dt.Rows[1];
                //dr.Delete();

            }

            DataTable dt2 = new DataTable();
            dt2 = dataAccess.QueryTable("select * from class_info");
            dt2.PrimaryKey = new DataColumn[] { dt2.Columns[0] };
            if (dt2 != null)
            {
                DataRow dr = dt2.NewRow();
                dr["class_id"] = "012";
                dr["class_name"] = "1212";
                dt2.Rows.Add(dr);
            }
            dt2.TableName = "class_info";

            DataTable dt3 = new DataTable();
            dt3 = dataAccess.QueryTable("select stu_id, class_id, stu_name FROM student_info");
            dt3.PrimaryKey = new DataColumn[] { dt3.Columns[0], dt3.Columns[1] };
            if (dt3 != null)
            {
                DataRow dr = dt3.NewRow();
                dr["stu_id"] = "0000";
                dr["class_id"] = "1111";
                dr["stu_name"] = "aaaaaa";
                dt3.Rows.Add(dr);
            }
            dt3.TableName = "student_info";

            DataSet ds = new DataSet();
            ds.Tables.Add(dt2);
            ds.Tables.Add(dt3);
            dataAccess.Update(ds, "select * from class_info", "select * from student_info");


            DataTable cuDt = new DataTable();
            cuDt.Columns.Add("class_id", typeof(string));
            cuDt.Columns.Add("class_name", typeof(string));
            cuDt.PrimaryKey = new DataColumn[] { cuDt.Columns[1] };
            DataRow cuDr = cuDt.NewRow();
            cuDr["class_id"] = "010";
            cuDr["class_name"] = "aa1";
            cuDt.Rows.Add(cuDr);
            cuDt.AcceptChanges();

            cuDt.Rows[0]["class_name"] = "aa123";
            //cuDt.Rows[0].Delete();

            //dataAccess.Update(cuDt, "select * from class_info");
             
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Vic.Data.DataAccess dbAccess = new Vic.Data.DataAccess("Data Source=ora24;User ID=test;Password=test;", Vic.Data.DbProviderType.OracleManaged);
            List<Product> lstProduct = dbAccess.Query<Product>("select * from product");
            this.dataGridView1.DataSource = lstProduct;
        }
    }

    public class Product
    {
        public Decimal ProductId { get; set; }
        public string Name { get; set; }
        public Decimal CategoryId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
