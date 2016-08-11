using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vic.Data
{
    /// <summary>
    /// 通用数据库访问公共类
    /// </summary>
    public class DataAccessComm
    {
        /// <summary>
        /// 通用数据库访问公共类
        /// </summary>
        public DataAccessComm()
        {

        }

        /// <summary>
        /// 用于数据库访问的提供程序名称
        /// </summary>
        /// <returns></returns>
        public static Dictionary<DbProviderType, string> DbProviderNames()
        {
            Dictionary<DbProviderType, string> dbProviderNames = new Dictionary<DbProviderType, string>();
            dbProviderNames.Add(DbProviderType.Access, "System.Data.OleDb");
            dbProviderNames.Add(DbProviderType.Excel, "System.Data.OleDb");
            dbProviderNames.Add(DbProviderType.MySql, "MySql.Data.MySqlClient");
            dbProviderNames.Add(DbProviderType.Oracle, "System.Data.OracleClient");
            dbProviderNames.Add(DbProviderType.OracleClient, "Oracle.DataAccess.Client");
            dbProviderNames.Add(DbProviderType.OracleManaged, "Oracle.ManagedDataAccess.Client");
            dbProviderNames.Add(DbProviderType.SQLite, "System.Data.SQLite");
            dbProviderNames.Add(DbProviderType.SQLiteEF6, "System.Data.SQLite.EF6");
            dbProviderNames.Add(DbProviderType.SQLiteLinq, "System.Data.SQLite.Linq");
            dbProviderNames.Add(DbProviderType.SqlServer, "System.Data.SqlClient");
            dbProviderNames.Add(DbProviderType.SqlServerCe_3_5, "System.Data.SqlServerCe.3.5");
            return dbProviderNames;
        }

        /// <summary>
        /// 返回访问数据库的提供程序类型对应的名称
        /// </summary>
        public static string GetProviderName(DbProviderType dbProviderType)
        {
            try
            {
                return DbProviderNames().First(t => t.Key == dbProviderType).Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + string.Format("DbProviderType 枚举项\"{0}\"没有从 DbProviderNames 列表中检索到值。", dbProviderType.ToString()));
            }
        }

        /// <summary>
        /// 返回访问数据库的提供程序名称对应的类型
        /// </summary>
        /// <param name="dbProviderName"></param>
        /// <returns></returns>
        public static DbProviderType GetProviderType(string dbProviderName)
        {
            try
            {
                return DbProviderNames().First(t => t.Value.ToLower() == dbProviderName.ToLower()).Key;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + string.Format("值\"{0}\"没有从 DbProviderNames 列表中检索到DbProviderType 枚举项。", dbProviderName));
            }
        }
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbProviderType : byte
    {
        /// <summary>
        /// 采用 System.Data.SqlClient
        /// </summary>
        SqlServer = 1,

        /// <summary>
        /// 采用 System.Data.SqlServerCe.3.5
        /// </summary>
        SqlServerCe_3_5 = 2,

        /// <summary>
        /// 采用 System.Data.OleDb
        /// </summary>
        Access = 3,

        /// <summary>
        /// 采用 System.Data.OleDb
        /// </summary>
        Excel = 4,

        /// <summary>
        /// 采用 System.Data.OracleClient
        /// </summary>
        Oracle = 5,

        /// <summary>
        /// 采用 Oracle.DataAccess.Client
        ///     可能需在应用程序的 .config 配置文件中的"system.data"的"DbProviderFactories"节点中添加如下项
        /// <para>＜add name="Oracle Data Provider for .NET" invariant="Oracle.DataAccess.Client" description="Oracle Data Provider for .NET" type="Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Version=2.111.6.20, Culture=neutral, PublicKeyToken=89b483f429c47342"＞</para>
        /// <para>其中Version、PublicKeyToken值视情况而定</para>
        /// </summary>
        OracleClient = 6,

        /// <summary>
        /// 采用 Oracle.ManagedDataAccess.Client
        ///     可能需在应用程序的 .config 配置文件中的"system.data"的"DbProviderFactories"节点中添加如下项
        /// <para>＜add name="ODP.NET, Managed Driver" invariant="Oracle.ManagedDataAccess.Client" description="Oracle Data Provider for .NET, Managed Driver" type="Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Version=4.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342"＞</para>
        /// <para>其中Version、PublicKeyToken值视情况而定</para>
        /// </summary>
        OracleManaged = 7,

        /// <summary>
        /// 采用 MySql.Data.MySqlClient
        /// </summary>
        MySql = 8,

        /// <summary>
        /// 采用 System.Data.SQLite
        /// </summary>
        SQLite = 9,

        /// <summary>
        /// 采用 System.Data.SQLite.EF6
        /// </summary>
        SQLiteEF6 = 10,

        /// <summary>
        /// 采用 System.Data.SQLite.Linq
        /// </summary>
        SQLiteLinq = 11
    }

    [Serializable]
    public class ProviderTypeNoneException : Exception
    {
        private string dbProviderName = "";

        //public ProviderTypeNoneException()
        //    : base()//调用基类的构造器
        //{
        //}

        //public ProviderTypeNoneException(string message)
        //    : base(message)//调用基类的构造器
        //{
        //}

        //public ProviderTypeNoneException(string message, Exception innerException)
        //    : base(message, innerException)//调用基类的构造器
        //{
        //}

        public ProviderTypeNoneException(string dbProviderName)
            : base()//调用基类的构造器
        {
            this.dbProviderName = dbProviderName;
        }

        public ProviderTypeNoneException(string dbProviderName, string message)
            : base(message)//调用基类的构造器
        {
            this.dbProviderName = dbProviderName;
        }

        public ProviderTypeNoneException(string dbProviderName, string message, Exception innerException)
            : base(message, innerException)//调用基类的构造器
        {
            this.dbProviderName = dbProviderName;
        }

        public override string Message
        {
            get
            {
                StringBuilder explain = new StringBuilder();
                explain.AppendFormat(@"原因：一般性问题，说明没有找到相关驱动""{0}""。" + Environment.NewLine, dbProviderName);
                explain.AppendFormat(@"解决方法：以Oracle.DataAccess.Client.dll驱动为例" + Environment.NewLine);
                explain.AppendFormat(@"步骤1：app.config或Web.config文件中可能需要添加如下节点，同时需要注意Version。" + Environment.NewLine);
                explain.AppendFormat(@"<system.data>" + Environment.NewLine);
                explain.AppendFormat(@"  <DbProviderFactories>" + Environment.NewLine);
                explain.AppendFormat(@"    <add name=""Oracle Data Provider for .NET"" invariant=""Oracle.DataAccess.Client"" description=""Oracle Data Provider for .NET"" type=""Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Version=2.111.6.20, Culture=neutral, PublicKeyToken=89b483f429c47342""/>" + Environment.NewLine);
                explain.AppendFormat(@"  </DbProviderFactories>" + Environment.NewLine);
                explain.AppendFormat(@"</system.data>" + Environment.NewLine);
                explain.AppendFormat(@"步骤2：注册相关驱动DLL到GAC（如果步骤1有效可以忽略该操作）。" + Environment.NewLine);
                explain.AppendFormat(@"(1)查看GAC：gacutil.exe /l Oracle.DataAccess" + Environment.NewLine);
                explain.AppendFormat(@"(2)注册GAC：gacutil.exe /i Oracle.DataAccess.dll" + Environment.NewLine);
                return base.Message + Environment.NewLine + explain.ToString();
            }
        }
    }
}
