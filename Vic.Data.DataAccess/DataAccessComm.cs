using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vic.Data
{
    /// <summary>
    /// 通用数据库访问公共类
    /// </summary>
    [Serializable]
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
    [Serializable]
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

    /// <summary>
    /// SQL字符串带参数
    /// </summary>
    [Serializable]
    public struct DbSQL
    {
        /// <summary>
        /// SQL字符串
        /// </summary>
        public string SQLString;

        /// <summary>
        /// SQL字符串中对应的 DbParameter 参数
        /// </summary>
        public System.Data.Common.DbParameter[] DbParameters;

        /// <summary>
        /// 带DbParameters参数的SQL实例
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="dbParameters"></param>
        public DbSQL(string sqlString, params System.Data.Common.DbParameter[] dbParameters)
        {
            this.SQLString = sqlString;
            this.DbParameters = dbParameters;
        }
    }
}
