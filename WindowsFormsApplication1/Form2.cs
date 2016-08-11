using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vic.Data;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        private System.Timers.Timer timer = null;
        private string applicationDirectory; //当前程序文件夹
        private string currDateTime = ""; //当前日期时间
        private string currTime = ""; //当前时间(时:分)
        private string sourceDbConn = ""; //源库链接串
        private string sourceDbProviderName = ""; //源库驱动
        private string targetDbconn = ""; //目标库链接串
        private string targetDbProviderName = ""; //目标库驱动

        public Form2()
        {
            InitializeComponent();

            this.timer = new System.Timers.Timer();
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            this.timer.Interval = 10000;
            this.timer.AutoReset = true;

            this.applicationDirectory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.sourceDbConn = ConfigurationManager.ConnectionStrings["sourceDbConn"].ConnectionString;
            this.sourceDbProviderName = ConfigurationManager.ConnectionStrings["sourceDbConn"].ProviderName;
            this.targetDbconn = ConfigurationManager.ConnectionStrings["targetDbconn"].ConnectionString;
            this.targetDbProviderName = ConfigurationManager.ConnectionStrings["targetDbconn"].ProviderName;

            this.timer.Start();
            WriteLog("服务启动.");
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                WriteLog("ssssss");
                int connCheckNum = 20; //数据库链接测试重试总次数
                this.currTime = System.DateTime.Now.ToString("HH:mm"); //当前时间
                WriteLog("开始执行时间：" + Properties.Settings.Default.SynTime);
                if (this.currTime.Equals(Properties.Settings.Default.SynTime)) //如果当前时间为设定提取时间则开始提取数据
                {
                    this.timer.Stop();
                }
                else
                {
                    return;
                }
                WriteLog("xxxxxx：" + Properties.Settings.Default.SynTime);
                bool connCheck = false;
                bool isOK_wllb = false;
                bool isOK_wlmx = false;
                bool isOk_wlzsj = false;
                DataAccess sourceDB = new DataAccess(this.sourceDbConn, this.sourceDbProviderName);
                DataAccess targetDB = new DataAccess(this.targetDbconn, this.targetDbProviderName);
                if (sourceDB != null && targetDB != null)
                {
                    string connErrMsg = "";
                    #region 数据库链接测试

                    for (int i = 1; i <= connCheckNum; i++)
                    {
                        WriteLog(string.Format("测试源数据库链接.{0}", sourceDB.ConnectionString));
                        if (sourceDB.IsConnState(out connErrMsg))
                        {
                            connCheck = true;
                            WriteLog(string.Format("测试源数据库链接正常."));
                        }
                        else
                        {
                            connCheck = false;
                            WriteLog(string.Format("测试源数据库链接失改！{0}", connErrMsg));
                        }

                        WriteLog(string.Format("测试目标数据库链接.{0}", targetDB.ConnectionString));
                        if (targetDB.IsConnState(out connErrMsg))
                        {
                            connCheck = true;
                            WriteLog(string.Format("测试目标数据库链接正常."));
                        }
                        else
                        {
                            connCheck = false;
                            WriteLog(string.Format("测试目标数据库链接失改！{0}", connErrMsg));
                        }

                        if (connCheck)
                        {
                            break;
                        }
                        else
                        {
                            if (i < 20)
                            {
                                WriteLog(string.Format("至少有一个数据库测试链接失败，5分钟后重试，共重试{0}次。", connCheckNum));
                                Thread.Sleep(300000);
                            }
                            else
                            {
                                WriteLog(string.Format("数据库链接测试{0}次均失败，结束测试！", connCheckNum));
                            }
                        }
                    }

                    #endregion

                    #region 从中原物流系统数据库中提取数据，并导入中原单井预算系统的物资管理模块的物料主数据表中

                    if (connCheck)
                    {
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
                            isOK_wllb = true;
                            WriteLog("提取物料类别成功！");
                        }
                        catch (Exception ex)
                        {
                            isOK_wllb = false;
                            WriteLog("提取物料类别：" + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            if (sourceWllbReader != null)
                                sourceWllbReader.Close();
                        }
                        #endregion

                        #region 提取物料明细
                        DbDataReader sourceWlReader = null;
                        try
                        {
                            sourceWlReader = sourceDB.QueryReader("select wz_no, wz_name, wz_type, dw1, avg_price from wzdm");
                            if (sourceWlReader != null)
                            {
                                List<string> targetWlInsers = new List<string>();
                                while (sourceWlReader.Read())
                                {
                                    string wlmc = sourceWlReader["wz_name"].ToString().Replace(" ", "").Replace("'", "''");
                                    string wlgg = sourceWlReader["wz_type"].ToString().Replace(" ", "").Replace("'", "''");

                                    targetWlInsers.Add(string.Format("INSERT INTO yw_base_wlzsj_imp (wlbm, wlmc, wlgg, jldw, dj, sj) VALUES ('{0}', '{1}', '{2}', '{3}', {4}, sysdate)", sourceWlReader["wz_no"], wlmc, wlgg, sourceWlReader["dw1"], sourceWlReader["avg_price"]));
                                }

                                //清空目标库的YW_BASE_WLZSJ_IMP表
                                targetDB.ExecuteNonQuery("TRUNCATE TABLE YW_BASE_WLZSJ_IMP");

                                //执行插入数据操作到目标库的YW_BASE_WLZSJ_IMP表中
                                targetDB.ExecuteSqlTran(5000, targetWlInsers.ToArray());
                            }

                            isOK_wlmx = true;
                            WriteLog("提取物料明细成功！");
                        }
                        catch (Exception ex)
                        {
                            isOK_wlmx = false;
                            WriteLog("提取物料明细：" + Environment.NewLine + ex.Message);
                        }
                        finally
                        {
                            if (sourceWlReader != null)
                                sourceWlReader.Close();
                        }
                        #endregion

                        #region 处理目标库中导入的物料明细
                        try
                        {
                            targetDB.ExecProcedure("pro_imp_wlzsj");
                            isOk_wlzsj = true;
                            WriteLog("处理目标库中导放的物料明细成功！");
                        }
                        catch (Exception ex)
                        {
                            isOk_wlzsj = false;
                            WriteLog("处理目标库中导放的物料明细：" + Environment.NewLine + ex.Message);
                        }

                        #endregion
                    }

                    #endregion
                }

                if (connCheck && isOK_wllb && isOK_wlmx && isOk_wlzsj)
                {
                    this.timer.Start();
                }
            }
            catch (Exception ex)
            {
                this.timer.Stop();
                this.timer.Close();
                WriteLog("服务停止." + ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        private void WriteLog(string message)
        {
            currDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //写LOG文件
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(this.applicationDirectory + "\\log.txt", true))
            {
                sw.WriteLine(string.Format(@"{0} {1}", currDateTime, message));
            }
        }
    }
}
