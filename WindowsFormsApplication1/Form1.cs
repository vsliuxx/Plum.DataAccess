using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Vic.Data;
using System.Data.Common;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private string sourceDbConn = ""; //源库链接串
        private string sourceDbProviderName = ""; //源库驱动
        private string targetDbconn = ""; //目标库链接串
        private string targetDbProviderName = ""; //目标库驱动

        public Form1()
        {
            InitializeComponent();

            this.sourceDbConn = ConfigurationManager.ConnectionStrings["sourceDbConn"].ConnectionString;
            this.sourceDbProviderName = ConfigurationManager.ConnectionStrings["sourceDbConn"].ProviderName;
            this.targetDbconn = ConfigurationManager.ConnectionStrings["targetDbconn"].ConnectionString;
            this.targetDbProviderName = ConfigurationManager.ConnectionStrings["targetDbconn"].ProviderName;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                DataAccess sourceDB = new DataAccess(this.sourceDbConn, this.sourceDbProviderName);
                DataAccess targetDB = new DataAccess(this.targetDbconn, this.targetDbProviderName);
                if (sourceDB != null && targetDB != null)
                {
                    #region 从中原物流系统数据库中提取数据，并导入中原单井预算系统的物资管理模块的物料主数据表中

                    this.textBox1.Text = System.DateTime.Now.ToString();

                    #region 提取物料类别
                    DbDataReader sourceWllbReader = null;
                    try
                    {
                        sourceWllbReader = sourceDB.QueryReader("select lb_no, lb_name, parent_id from wzlb");
                        if (sourceWllbReader != null)
                        {
                            List<string> targetWllbInsers = new List<string>();
                            while (sourceWllbReader.Read())
                            {
                                targetWllbInsers.Add(string.Format("INSERT INTO yw_base_wlzsj_imp_lb (lbbm, lbmc, plbbm) VALUES ('{0}', '{1}', '{2}')", sourceWllbReader["lb_no"], sourceWllbReader["lb_name"], sourceWllbReader["parent_id"]));
                            }

                            //清空目标库的YW_BASE_WLZSJ_IMP_LB表
                            targetDB.ExecuteNonQuery("TRUNCATE TABLE YW_BASE_WLZSJ_IMP_LB");

                            //执行插入数据操作到目标库的YW_BASE_WLZSJ_IMP_LB表中
                            targetDB.ExecuteSqlTran(0, targetWllbInsers.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (sourceWllbReader != null)
                            sourceWllbReader.Close();
                    }
                    #endregion

                    this.textBox2.Text = System.DateTime.Now.ToString();

                    this.textBox3.Text = System.DateTime.Now.ToString();

                    #region 提取物料明细
                    DbDataReader sourceWlReader = null;
                    string errWlbm = "";
                    try
                    {
                        sourceWlReader = sourceDB.QueryReader("select wz_no, wz_name, wz_type, dw1, avg_price from wzdm");
                        if (sourceWlReader != null)
                        {
                            List<string> targetWlInsers = new List<string>();

                            //**********************
                            targetDB.ExecuteNonQuery("TRUNCATE TABLE YW_BASE_WLZSJ_IMP");
                            //***********************

                            while (sourceWlReader.Read())
                            {
                                //targetWlInsers.Add(string.Format("INSERT INTO yw_base_wlzsj_imp (wlbm, wlmc, wlgg, jldw, dj) VALUES ('{0}', '{1}', '{2}', '{3}', {4})", sourceWlReader["wz_no"], sourceWlReader["wz_name"], sourceWlReader["wz_type"], sourceWlReader["dw1"], sourceWlReader["avg_price"]));

                                //**********************
                                errWlbm = sourceWlReader["wz_no"].ToString();

                                string wlmc = sourceWlReader["wz_name"].ToString().Replace(" ", "").Replace("'", "''");
                                string wlgg = sourceWlReader["wz_type"].ToString().Replace(" ", "").Replace("'", "''");
                                targetDB.ExecuteNonQuery(string.Format("INSERT INTO yw_base_wlzsj_imp (wlbm, wlmc, wlgg, jldw, dj) VALUES ('{0}', '{1}', '{2}', '{3}', {4})", sourceWlReader["wz_no"], wlmc, wlgg, sourceWlReader["dw1"], sourceWlReader["avg_price"]));
                                //**********************
                            }

                            //清空目标库的YW_BASE_WLZSJ_IMP表
                            //targetDB.ExecuteNonQuery("TRUNCATE TABLE YW_BASE_WLZSJ_IMP");

                            //执行插入数据操作到目标库的YW_BASE_WLZSJ_IMP表中
                            //targetDB.ExecuteSqlTran(5000, targetWlInsers.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                        MessageBox.Show(errWlbm + "------------" + ex.Message);
                    }
                    finally
                    {
                        if (sourceWlReader != null)
                            sourceWlReader.Close();
                    }
                    #endregion

                    this.textBox4.Text = System.DateTime.Now.ToString();

                    #region 处理目标库中导放的物料明细
                    try
                    {
                        targetDB.ExecProcedure("pro_imp_wlzsj");
                    }
                    catch (Exception ex)
                    { 

                    }

                    #endregion
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }
    }
}
