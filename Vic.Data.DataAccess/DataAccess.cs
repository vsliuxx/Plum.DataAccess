using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Collections;

namespace Vic.Data
{
    /// <summary>
    /// 通用数据库访问类
    /// </summary>
    public class DataAccess : MarshalByRefObject, IDataAccess, IDisposable
    {
        private Dictionary<string, object> parms;

        private string _connectionString = "";
        /// <summary>
        /// 数据库链接串
        /// </summary>
        public string ConnectionString
        {
            get { return this._connectionString; }
            //set { this._connectionString = value; }
        }

        private string _dbProviderName = "";
        /// <summary>
        /// 数据库驱动名称
        /// </summary>
        public string DbProviderName
        {
            get { return this._dbProviderName; }
            //set { this._dbProviderName = value; }
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

        private DataTable _factoryClasses;
        /// <summary>
        /// 包含有关实现 System.Data.Common.DbProviderFactory 的所有已安装提供程序的信息
        /// </summary>
        public DataTable FactoryClasses
        {
            get { return this._factoryClasses; }
        }

        private bool isDisposed = false;
        /// <summary>
        /// 资源是否已被释放过
        /// </summary>
        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        /// <summary>
        /// 通用数据库访问类
        /// </summary>
        //public DataAccess()
        //{

        //}

        /// <summary>
        /// 通用数据库访问类。
        /// </summary>
        /// <param name="connectionString">数据库链接串</param>
        /// <param name="dbProviderName">数据库程序集名称</param>
        public DataAccess(string connectionString, string dbProviderName)
        {
            this._factoryClasses = DbProviderFactories.GetFactoryClasses();
            this._connectionString = connectionString;
            this._dbProviderName = dbProviderName;
            try
            {
                this._dbProviderType = DataAccessComm.GetProviderType(dbProviderName);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 通用数据库访问类
        /// </summary>
        /// <param name="connectionString">数据库链接串</param>
        /// <param name="dbProviderType">数据库程序集类型</param>
        public DataAccess(string connectionString, DbProviderType dbProviderType)
            : this(connectionString, DataAccessComm.GetProviderName(dbProviderType))
        {
        }

        /// <summary>
        /// 获取一个数据工厂实例
        /// </summary>
        /// <returns></returns>
        public DbProviderFactory GetDbProviderFactory()
        {
            try
            {
                return DbProviderFactories.GetFactory(this._dbProviderName);
            }
            catch (Exception ex)
            {
                throw new ProviderTypeNoneException(this._dbProviderName);
            }
        }

        /// <summary>
        /// 创建一个DbConnection实例
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            DbConnection conn = GetDbProviderFactory().CreateConnection();
            conn.ConnectionString = this._connectionString;
            return conn;
        }

        /// <summary>
        /// 创建一个DbConnection对象的DbCommand实例
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public DbCommand CreateCommand(DbConnection dbConnection)
        {
            return dbConnection.CreateCommand();
        }

        /// <summary>
        /// 创建一个DbParameter实例
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return GetDbProviderFactory().CreateParameter();
        }

        /// <summary>
        /// 创建一个DbDataAdapter实例
        /// </summary>
        /// <returns></returns>
        public DbDataAdapter CreateDataAdapter()
        {
            return GetDbProviderFactory().CreateDataAdapter();
        }

        /// <summary>
        /// 创建一个CreateCommandBuilder实例
        /// </summary>
        /// <returns></returns>
        public DbCommandBuilder CreateCommandBuilder()
        {
            return GetDbProviderFactory().CreateCommandBuilder();
        }

        /// <summary>
        /// 检测数据库链接。
        /// </summary>
        /// <returns></returns>
        public bool CheckConn()
        {
            string exMessage = "";
            return CheckConn(out exMessage);
        }

        /// <summary>
        /// 检测数据库链接。
        /// </summary>
        /// <param name="exMessage">异常错误信息</param>
        /// <returns></returns>
        public bool CheckConn(out string exMessage)
        {
            bool checkConn = false;
            exMessage = "";

            using (DbConnection conn = CreateConnection())
            {
                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        checkConn = true;
                    }
                }
                catch (DbException dbEx)
                {
                    exMessage = dbEx.Message;
                }
            }
            return checkConn;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                //释放托管资源
                if (_factoryClasses != null)
                {
                    _factoryClasses.Dispose();
                    //_factoryClasses = null;
                }
            }

            if (parms != null)
            {
                parms.Clear();
                parms = null;
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~DataAccess()
        {
            this.Dispose(false);
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
        /// 获取最后一次执行带DbParameter参数的方法的一个参数值
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns>object</returns>
        public object GetParamValue(string parameterName)
        {
            object obj = null;
            if (this.parms != null && this.parms.Count > 0)
            {
                obj = this.parms.FirstOrDefault(p => p.Key.ToUpper() == parameterName.ToUpper()).Value;
            }
            return obj;
        }

        /// <summary>
        /// 获取最后一次执行带DbParameter参数的方法的一个参数值
        /// </summary>
        /// <param name="index">参数索引,从0开始</param>
        /// <returns>object</returns>
        public object GetParamValue(int index)
        {
            object obj = null;
            if (this.parms != null && this.parms.Count > 0)
            {
                if (index > -1 && index < this.parms.Count)
                    obj = this.parms.Values.ToList()[index];
            }
            return obj;
        }

        /// <summary>     
        /// 执行无返回数据集的SQL，返回受影响的行数。     
        /// </summary>     
        /// <param name="sql">SQL语句</param>
        /// <returns>Int</returns>  
        public int ExecuteNonQuery(string sql)
        {
            int result = 0;
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                try
                {
                    conn.Open();
                    cmd = CreateCommand(conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    result = cmd.ExecuteNonQuery();
                }
                catch (DbException dbEx)
                {
                    throw dbEx;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="commits">指定执行多少条SQL后提交一次，小于或等于0为不指定即执行所有SQL后再提交。</param>
        /// <param name="sqls">SQL语句</param>
        /// <returns></returns>
        public void ExecuteSqlTran(int commits, params string[] sqls)
        {
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbTransaction trans = null;
                try
                {
                    conn.Open();
                    cmd = CreateCommand(conn);
                    trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    cmd.CommandType = CommandType.Text;
                    for (int i = 0; i < sqls.Length; i++)
                    {
                        string strSql = sqls[i].ToString();
                        if (strSql.Trim().Length > 1)
                        {
                            cmd.CommandText = strSql;
                            cmd.ExecuteNonQuery();
                        }

                        //如果commits > 0 ，则按该数设定的值进行一次提交操作，i+1 是因为当i=0时是第一条记录。
                        if (commits > 0 && (i + 1) % commits == 0)
                        {
                            trans.Commit();
                            trans = conn.BeginTransaction();
                            cmd.Transaction = trans;
                        }
                    }
                    trans.Commit();
                }
                catch (DbException dbEx)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw dbEx;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                    if (trans != null)
                        trans.Dispose();
                }
            }
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string sql)
        {
            DataSet result = new DataSet();
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                try
                {
                    conn.Open();
                    cmd = CreateCommand(conn);
                    adp = CreateDataAdapter();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    adp.SelectCommand = cmd;
                    adp.Fill(result);
                }
                catch (DbException dbEx)
                {
                    result = null;
                    throw dbEx;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sqls">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(params string[] sqls)
        {
            DataSet result = new DataSet();
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                try
                {
                    conn.Open();
                    cmd = CreateCommand(conn);
                    adp = CreateDataAdapter();
                    cmd.CommandType = CommandType.Text;
                    for (int i = 0; i < sqls.Length; i++)
                    {
                        string strSql = sqls[i].ToString();
                        if (strSql.Trim().Length > 1)
                        {
                            cmd.CommandText = strSql;
                            adp.SelectCommand = cmd;
                            adp.Fill(result, "Table" + i.ToString());
                        }
                    }
                }
                catch (DbException dbEx)
                {
                    result = null;
                    throw dbEx;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataTable</returns>
        public DataTable QueryTable(string sql)
        {
            DataTable result = new DataTable();
            DataSet ds = Query(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                result = ds.Tables[0].Copy();
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 执行查询语句，返回DataReader，用完后要调用DbDataReader的Close()方法关闭实例
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DbDataReader</returns>
        public DbDataReader QueryReader(string sql)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader dataReader = null;
            try
            {
                conn = CreateConnection();
                conn.StateChange += new StateChangeEventHandler(conn_StateChange);
                conn.Open();
                cmd = CreateCommand(conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DbException dbEx)
            {
                if (dataReader != null)
                {
                    if (!dataReader.IsClosed)
                        dataReader.Close();
                    dataReader.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.StateChange -= conn_StateChange;
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                    conn.Dispose();
                }
                throw dbEx;
            }
            finally
            {

            }
            return dataReader;
        }

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currPageIndex">当前页索引</param>
        /// <returns>DataTable</returns>
        public DataTable QueryPage(string sql, int pageSize, int currPageIndex)
        {
            DataTable result = new DataTable();
            result.TableName = "Table";
            int startIndex = (currPageIndex - 1) * pageSize; //读取数据的开始索引
            int endIndex = currPageIndex * pageSize - 1; //读取数据的结束索引
            int readCurrIndex = -1;  //DataReader读取的当前数据行的索引 
            DbDataReader dataReader = null;
            try
            {
                dataReader = QueryReader(sql);

                #region 构造表结构
                DataTable schemaDt = dataReader.GetSchemaTable();
                int cols = dataReader.VisibleFieldCount;
                for (int i = 0; i < cols; i++)
                {
                    result.Columns.Add(new DataColumn(dataReader.GetName(i), dataReader.GetFieldType(i)));
                }
                #endregion

                #region 填充数据
                while (dataReader.Read())
                {
                    readCurrIndex++;

                    //如果当前行Index小于开始行Index，则下一个循环
                    if (readCurrIndex < startIndex)
                        continue;

                    //如果当前行Index大于结束行Index，则跳出循环
                    if (readCurrIndex > endIndex)
                        break;

                    DataRow dr = result.NewRow();
                    for (int i = 0; i < cols; i++)
                    {
                        string colName = result.Columns[i].ColumnName;
                        dr[colName] = dataReader[colName];
                    }
                    result.Rows.Add(dr);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            finally
            {
                if (dataReader != null)
                {
                    if (!dataReader.IsClosed)
                        dataReader.Close();
                    dataReader.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <returns></returns>
        public DataSet ExecProcedure(string storedProcName)
        {
            return ExecProcedure(storedProcName, new List<DbParameter>());
        }

        /// <summary>
        /// 执行存储过程,带参数
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <returns></returns>
        public DataSet ExecProcedure(string storedProcName, IList<DbParameter> parameters)
        {
            DataSet result = new DataSet();
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                try
                {
                    conn.Open();
                    adp = CreateDataAdapter();
                    cmd = CreateCommand(conn);
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
                    adp.Fill(result);
                    if (result != null && result.Tables.Count == 0)
                        result = null;
                }
                catch (DbException dbEx)
                {
                    result = null;
                    throw dbEx;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }

            this.parms = GetParametersValue(parameters);
            return result;
        }

        /// <summary>
        /// 执行存储过程,并返回指定表数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="tableNames">表名</param>
        /// <returns>DataSet</returns>
        public DataSet ExecProcedure(string storedProcName, params string[] tableNames)
        {
            return ExecProcedure(storedProcName, new List<DbParameter>(), tableNames);
        }

        /// <summary>
        /// 执行存储过程,带参数,并返回指定表数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <param name="tableNames">表名</param>
        /// <returns>DataSet</returns>
        public DataSet ExecProcedure(string storedProcName, IList<DbParameter> parameters, params string[] tableNames)
        {
            DataSet result = new DataSet();
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                try
                {
                    conn.Open();
                    adp = CreateDataAdapter();
                    cmd = CreateCommand(conn);
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
                    cmd = CreateCommand(conn);
                    cmd.CommandType = CommandType.Text;
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string tableName = tableNames[i].ToString();
                        if (tableName.Trim().Length > 1)
                        {
                            cmd.CommandText = string.Format("select * from {0}", tableName);
                            adp.SelectCommand = cmd;
                            adp.Fill(result, tableName);
                        }
                    }
                    #endregion

                    if (result != null && result.Tables.Count == 0)
                        result = null;
                }
                catch (DbException dbEx)
                {
                    result = null;
                    throw dbEx;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }

            this.parms = GetParametersValue(parameters);
            return result;
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="dataTable">DataTable，必须设置主键。</param>
        /// <param name="sql">Table对应的SQL，必须包含主键列。</param>
        /// <returns></returns>
        public void Update(DataTable dataTable, string sql)
        {
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                DbCommandBuilder cmbBuilder = null;
                try
                {
                    conn.Open();
                    adp = CreateDataAdapter();
                    cmd = CreateCommand(conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    adp.SelectCommand = cmd;
                    cmbBuilder = CreateCommandBuilder();
                    cmbBuilder.DataAdapter = adp;

                    adp.InsertCommand = cmbBuilder.GetInsertCommand();
                    adp.UpdateCommand = cmbBuilder.GetUpdateCommand();
                    adp.DeleteCommand = cmbBuilder.GetDeleteCommand();

                    //考虑并发
                    lock (typeof(DataAccess))
                    {
                        adp.Update(dataTable);
                    }
                }
                catch (DbException dbEx)
                {
                    throw dbEx;
                }
                finally
                {
                    if (cmbBuilder != null)
                        cmbBuilder.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="dataSet">DataSet，必须设置主键，多表时需要设置每个的TableName。</param>
        /// <param name="sql">每个Table对应的SQL，必须包含主键列。</param>
        /// <returns></returns>
        public void Update(DataSet dataSet, params string[] sql)
        {
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                DbCommandBuilder cmbBuilder = null;

                try
                {
                    conn.Open();
                    adp = CreateDataAdapter();
                    for (int i = 0; i < dataSet.Tables.Count; i++)
                    {
                        cmd = CreateCommand(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql[i];
                        adp.SelectCommand = cmd;
                        cmbBuilder = CreateCommandBuilder();
                        cmbBuilder.DataAdapter = adp;

                        DbCommand inCmd = cmbBuilder.GetInsertCommand();
                        DbCommand upCmd = cmbBuilder.GetUpdateCommand();
                        DbCommand delCmd = cmbBuilder.GetDeleteCommand();

                        //考虑并发
                        lock (typeof(DataAccess))
                        {
                            adp.Update(dataSet.Tables[i]);
                        }
                    }
                }
                catch (DbException dbEx)
                {
                    throw dbEx;
                }
                finally
                {
                    if (cmbBuilder != null)
                        cmbBuilder.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }
        }

        /// <summary>
        /// 更新数据库(事务)
        /// </summary>
        /// <param name="dataSet">DataSet，必须设置主键，多表时需要设置每个的TableName。</param>
        /// <param name="sql">每个Table对应的SQL，必须包含主键列。</param>
        /// <returns></returns>
        public void UpdateTran(DataSet dataSet, params string[] sql)
        {
            using (DbConnection conn = CreateConnection())
            {
                DbCommand cmd = null;
                DbDataAdapter adp = null;
                DbCommandBuilder cmbBuilder = null;
                DbTransaction trans = null;
                try
                {
                    conn.Open();
                    adp = CreateDataAdapter();
                    trans = conn.BeginTransaction();
                    for (int i = 0; i < dataSet.Tables.Count; i++)
                    {
                        cmd = conn.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql[i];
                        adp.SelectCommand = cmd;
                        cmbBuilder = CreateCommandBuilder();
                        cmbBuilder.DataAdapter = adp;

                        DbCommand inCmd = cmbBuilder.GetInsertCommand();
                        DbCommand upCmd = cmbBuilder.GetUpdateCommand();
                        DbCommand delCmd = cmbBuilder.GetDeleteCommand();

                        //考虑并发
                        lock (typeof(DataAccess))
                        {
                            adp.Update(dataSet.Tables[i]);
                        }
                    }
                    trans.Commit();
                }
                catch (DbException dbEx)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw dbEx;
                }
                finally
                {
                    if (trans != null)
                        trans.Dispose();
                    if (cmbBuilder != null)
                        cmbBuilder.Dispose();
                    if (cmd != null)
                        cmd.Dispose();
                    if (adp != null)
                        adp.Dispose();
                }
            }
        }

        private void conn_StateChange(object sender, StateChangeEventArgs e)
        {
            DbConnection conn = sender as DbConnection;
            if (e.CurrentState == ConnectionState.Closed)
            {
                if (conn != null)
                    conn.Dispose();
            }
        }
    }
}