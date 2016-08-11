using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Collections;

namespace Vic.Data
{
    public class DataAccess : MarshalByRefObject, IDataAccess
    {
        private DbProviderFactory providerFactory;
        private DbConnection conn;
        private DbCommand cmd;
        private DbDataAdapter adp;
        private DbTransaction trans;
        private bool isSucceed = false;
        private int errCode = 0;
        private string errMessage = "";
        private object result;

        private string _connectionString = "";
        /// <summary>
        /// 数据库链接串
        /// </summary>
        public string ConnectionString
        {
            get { return this._connectionString; }
            //set { this._connectionString = value; }
        }

        private DbProviderType _dbProviderType;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbProviderType DbProviderType
        {
            get { return this._dbProviderType; }
            //set { this._dbProviderType = value; }
        }

        /// <summary>
        /// 数据库操作类
        /// </summary>
        //public DataAccess()
        //{

        //}

        /// <summary>
        /// 数据库操作类。
        /// </summary>
        /// <param name="connectionString">数据库链接串</param>
        /// <param name="dbProviderType">数据库类型</param>
        public DataAccess(string connectionString, DbProviderType dbProviderType)
        {
            DataTable dt = DbProviderFactories.GetFactoryClasses();
            this._connectionString = connectionString;
            this._dbProviderType = dbProviderType;
            string providerName = DataAccessComm.GetProviderName(dbProviderType);
            try
            {
                providerFactory = DbProviderFactories.GetFactory(providerName);
                conn = providerFactory.CreateConnection();
                conn.ConnectionString = this._connectionString;
            }
            catch (DbException ex)
            {
                //throw new ArgumentException(ex.Message); 
            }
        }

        /// <summary>
        /// 检测数据库链接。
        /// </summary>
        /// <returns></returns>
        public bool IsConnState()
        {
            string exMessage = "";
            return IsConnState(out exMessage);
        }

        /// <summary>
        /// 检测数据库链接。
        /// </summary>
        /// <param name="exMessage">异常错误信息</param>
        /// <returns></returns>
        public bool IsConnState(out string exMessage)
        {
            bool isConnState = false;
            exMessage = "";
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    isConnState = true;
                }
            }
            catch (DbException ex)
            {
                exMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }
            return isConnState;
        }

        /// <summary>
        /// 打开数据库链接
        /// </summary>
        private void ConnOpen()
        {
            if (conn != null && conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
        }

        /// <summary>
        /// 关闭数据库链接，并释放资源。
        /// </summary>
        private void ConnClose()
        {
            if (cmd != null)
            {
                if (cmd.Connection.State != ConnectionState.Closed)
                {
                    cmd.Connection.Close();
                }
                cmd.Dispose();
                cmd = null;
            }
            else
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            if (adp != null)
            {
                adp.Dispose();
                adp = null;
            }

            if (trans != null)
            {
                adp.Dispose();
                adp = null;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        //~DataAccess()
        //{
        //    ConnClose();
        //}

        /// <summary>
        /// 创建一个DbParameter实例
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            if (providerFactory != null)
            {
                return providerFactory.CreateParameter();
            }
            return null;
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns></returns>
        private Dictionary<string, object> GetParametersValue(IList<DbParameter> parameters)
        {
            Dictionary<string, object> parms = new Dictionary<string, object>();
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    string name = "";
                    if (string.IsNullOrWhiteSpace(parameters[i].ParameterName))
                        name = "param" + i.ToString();
                    else
                        name = parameters[i].ParameterName;

                    object value = parameters[i].Value;
                    parms.Add(name, value);
                }
            }
            return parms; 
        }

        /// <summary>     
        /// 执行无返回数据集的SQL，返回受影响的行数。     
        /// </summary>     
        /// <param name="sql">SQL语句</param>
        /// <returns>Int</returns>  
        public DbResult ExecuteNonQuery(string sql)
        {
            try
            {
                ConnOpen();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                result = cmd.ExecuteNonQuery();
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            catch (DbException ex)
            {
                result = 0;
                isSucceed = false;
                errCode = ex.ErrorCode;
                errMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }
            return new DbResult(isSucceed, errCode, errMessage, result);
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqls">SQL语句</param>
        /// <returns>Null</returns>
        public DbResult ExecuteSqlTran(params string[] sqls)
        {
            try
            {
                ConnOpen();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                trans = conn.BeginTransaction();
                cmd.Transaction = trans;
                for (int i = 0; i < sqls.Length; i++)
                {
                    string strSql = sqls[i].ToString();
                    if (strSql.Trim().Length > 1)
                    {
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();
                    }
                }
                trans.Commit();
                result = null;
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            catch (DbException ex)
            {
                trans.Rollback();
                result = null;
                isSucceed = false;
                errCode = ex.ErrorCode;
                errMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }
            return new DbResult(isSucceed, errCode, errMessage, result);
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataSet</returns>
        public DbResult Query(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                ConnOpen();
                adp = providerFactory.CreateDataAdapter();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                adp.SelectCommand = cmd;
                adp.Fill(ds);
                result = ds;
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            catch (DbException ex)
            {
                result = null;
                isSucceed = false;
                errCode = ex.ErrorCode;
                errMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }
            return new DbResult(isSucceed, errCode, errMessage, result);
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqls">查询语句</param>
        /// <returns>DataSet</returns>
        public DbResult Query(params string[] sqls)
        {
            DataSet ds = new DataSet();
            try
            {
                ConnOpen();
                adp = providerFactory.CreateDataAdapter();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                for (int i = 0; i < sqls.Length; i++)
                {
                    string strSql = sqls[i].ToString();
                    if (strSql.Trim().Length > 1)
                    {
                        cmd.CommandText = strSql;
                        adp.SelectCommand = cmd;
                        adp.Fill(ds, "Table" + i.ToString());
                    }
                }
                result = ds;
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            catch (DbException ex)
            {
                result = null;
                isSucceed = false;
                errCode = ex.ErrorCode;
                errMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }
            return new DbResult(isSucceed, errCode, errMessage, result);
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataTable</returns>
        public DbResult QueryTable(string sql)
        {
            DataSet ds = new DataSet();
            DbResult re = Query(sql);
            ds = re.Result as DataSet;
            if (re.IsSucceed && ds != null && ds.Tables.Count > 0)
            {
                result = ((DataSet)re.Result).Tables[0];
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            else
            {
                result = null;
                isSucceed = false;
                errCode = re.ErrCode;
                errMessage = re.ErrMessage;
            }
            return new DbResult(isSucceed, errCode, errMessage, result);
        }

        /// <summary>
        /// 执行查询语句，返回DataReader，用完后要调用DbDataReader的Close()方法关闭实例
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DbDataReader</returns>
        public DbDataReader QueryReader(string sql)
        {

            DbDataReader dataReader = null;
            try
            {
                ConnOpen();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DbException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
            return dataReader;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <returns></returns>
        public DbResult ExecProcedure(string storedProcName)
        {
            return ExecProcedure(storedProcName, new List<DbParameter>());
        }

        /// <summary>
        /// 执行存储过程,带参数
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <returns></returns>
        public DbResult ExecProcedure(string storedProcName, IList<DbParameter> parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                ConnOpen();
                adp = providerFactory.CreateDataAdapter();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcName;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (DbParameter para in parameters)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                cmd.ExecuteNonQuery();
                adp.SelectCommand = cmd;
                adp.Fill(ds);
                if (ds != null && ds.Tables.Count == 0)
                    ds = null;
                result = ds;
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            catch (DbException ex)
            {
                result = null;
                isSucceed = false;
                errCode = ex.ErrorCode;
                errMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }

            return new DbResult(isSucceed, errCode, errMessage, result, GetParametersValue(parameters));
        }

        /// <summary>
        /// 执行存储过程,并返回指定表数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <param name="tableName">表名</param>
        /// <returns>DataSet</returns>
        public DbResult ExecProcedure(string storedProcName, params string[] tableNames)
        {
            return ExecProcedure(storedProcName, new List<DbParameter>(), tableNames);
        }

        /// <summary>
        /// 执行存储过程,带参数,并返回指定表数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <param name="tableName">表名</param>
        /// <returns>DataSet</returns>
        public DbResult ExecProcedure(string storedProcName, IList<DbParameter> parameters, params string[] tableNames)
        {
            DataSet ds = new DataSet();
            try
            {
                ConnOpen();
                adp = providerFactory.CreateDataAdapter();
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcName;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (DbParameter para in parameters)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                cmd.ExecuteNonQuery();

                #region 查询表数据
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                for (int i = 0; i < tableNames.Length; i++)
                {
                    string tableName = tableNames[i].ToString();
                    if (tableName.Trim().Length > 1)
                    {
                        cmd.CommandText = string.Format("select * from {0}", tableName);
                        adp.SelectCommand = cmd;
                        adp.Fill(ds, tableName);
                    }
                }
                #endregion

                if (ds != null && ds.Tables.Count == 0)
                    ds = null;
                result = ds;
                isSucceed = true;
                errCode = 0;
                errMessage = "";
            }
            catch (DbException ex)
            {
                result = null;
                isSucceed = false;
                errCode = ex.ErrorCode;
                errMessage = ex.Message;
            }
            finally
            {
                ConnClose();
            }

            return new DbResult(isSucceed, errCode, errMessage, result, GetParametersValue(parameters));
        }
    }
}
