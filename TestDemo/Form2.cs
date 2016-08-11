using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vic.Data;

namespace TestDemo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("BM",typeof(string));
            dt1.Columns.Add("lbbm", typeof(string));
            dt1.Columns.Add("PXM", typeof(int));
            DataRow dr;
            dr = dt1.NewRow();
            dr["BM"] = "001";
            dr["lbbm"] = "A";
            dr["pxm"] = 1;
            dt1.Rows.Add(dr);

            dr = dt1.NewRow();
            dr["BM"] = "002";
            dr["lbbm"] = "B";
            dr["pxm"] = 2;
            dt1.Rows.Add(dr);

            dr = dt1.NewRow();
            dr["BM"] = "003";
            dr["lbbm"] = "C";
            dr["pxm"] = 3;
            dt1.Rows.Add(dr);
            List<mod_t> ms = new List<mod_t>();
            try
            {
                ms = dt1.ToList<mod_t>();
            }
            catch (Exception ex)
            {
 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Vic.Data.DataAccess dataAccess2 = new Vic.Data.DataAccess("Data Source = d:\\KSXT.db", Vic.Data.DbProviderType.SQLiteEF6);
            string sql = @"SELECT   kstkid, year, kstkmc, zy, gw, ktzs, zfs, dtsc, AUTHORID, kkcs, sjlx, jgfsx, yxfsx, pxm, sl AS sfysc, SUM(txgs) 
                AS tmlx_1
FROM      (SELECT   a.kstkid, a.year, a.kstkmc, a.zy, AUTHORID, a.gw, a.ktzs, a.zfs, a.dtsc, a.kkcs, a.sjlx, a.jgfsx, a.yxfsx, a.pxm, 
                                 b.tx, b.txgs, d.sl
                 FROM      KS_SJ_TKGL a LEFT OUTER JOIN
                                 KS_SJ_JCXX b ON a.kstkid = b.kstkid LEFT OUTER JOIN
                                     (SELECT   COUNT(*) AS sl, c.kstkid
                                      FROM      KS_SJ_STK c
                                      GROUP BY c.kstkid) d ON a.kstkid = d.kstkid) aa
GROUP BY kstkid, year, kstkmc, zy, gw, ktzs, zfs, dtsc, kkcs, sjlx, jgfsx, yxfsx, pxm, AUTHORID, sl
ORDER BY pxm";

            DataTable dt = dataAccess2.QueryTable(sql);

            
            //DataAccess2 d = new DataAccess2();


        }

        public DataTable QueryTable(string sql)
        {
            DataTable dt = new DataTable();
            return dt;
        }

        
        
    }

    public class mod_t
    {
        private string _bm;
        private string _lbbm;
        private int _pbm;
        private int _pxm;

        /// <summary>
        /// 上级编码
        /// </summary>
        public string LBBM
        {
            get { return _lbbm; }
            set { _lbbm = value; }
        }
        /// <summary>
        /// 编码
        /// </summary>
        public string BM
        {
            set { _bm = value; }
            get { return _bm; }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public int PBM
        {
            set { _pbm = value; }
            get { return _pbm; }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public int PXM
        {
            set { _pxm = value; }
            get { return _pxm; }
        }
    }
        //public static class Helper
        //{       

        //    public static List<T> ToList<T>(this DataTable dt,T a)
        //    {
        //        return new List<T>();
        //    }        
        //}
}
