using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Vic.Data
{
    /// <summary>
    /// 通用数据库访问接口类
    /// </summary>
    public interface IDataAccess2
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        string ConnectionString
        {
            get;
            //set;
        }

        /// <summary>
        /// 数据库驱动名称
        /// </summary>
        string DbProviderName
        {
            get;
            //set;
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DbProviderType DbProviderType
        {
            get;
            //set;
        } 

        /// <summary>
        /// 包含有关实现 System.Data.Common.DbProviderFactory 的所有已安装提供程序的信息
        /// </summary>
        DataTable FactoryClasses
        {
            get;
        }
 
        /// <summary>
        /// 表示连接状态的字符串
        /// </summary>
        string ConnState
        {
            get;
        }

        /// <summary>
        /// 检测数据库链接。
        /// </summary>
        /// <returns></returns>
        bool CheckConn();

        /// <summary>
        /// 检测数据库链接。
        /// </summary>
        /// <param name="exMessage">异常错误信息</param>
        /// <returns></returns>
        bool CheckConn(out string exMessage);

        /// <summary>
        /// 创建一个DbParameter实例
        /// </summary>
        /// <returns></returns>
        DbParameter CreateParameter();

        /// <summary>
        /// 获取最后一次执行带DbParameter参数的方法的一个参数值
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns>object</returns>
        object GetParamValue(string parameterName);

        /// <summary>
        /// 获取最后一次执行带DbParameter参数的方法的一个参数值
        /// </summary>
        /// <param name="index">参数索引,从0开始</param>
        /// <returns>object</returns>
        object GetParamValue(int index);

        /// <summary>     
        /// 执行无返回数据集的SQL，返回受影响的行数。     
        /// </summary>     
        /// <param name="sql">SQL语句</param>
        /// <returns>Int</returns>  
        int ExecuteNonQuery(string sql);

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="commits">指定执行多少条SQL后提交一次，小于或等于0为不指定即执行所有SQL后再提交。</param>
        /// <param name="sqls">SQL语句</param>
        /// <returns></returns>
        void ExecuteSqlTran(int commits, params string[] sqls);

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sqls">查询语句</param>
        /// <returns>DataSet</returns>
        DataSet Query(string sql);

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sqls">查询语句</param>
        /// <returns>DataSet</returns>
        DataSet Query(params string[] sqls);

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DataTable</returns>
        DataTable QueryTable(string sql);

        /// <summary>
        /// 执行查询语句，返回DataReader，用完后要调用DbDataReader的Close()方法关闭实例
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>DbDataReader</returns>
        DbDataReader QueryReader(string sql);

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="currPageIndex">当前页索引</param>
        /// <returns>DataTable</returns>
        DataTable QueryPage(string sql, int pageSize, int currPageIndex);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <returns></returns>
        DataSet ExecProcedure(string storedProcName);

        /// <summary>
        /// 执行存储过程,带参数
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <returns></returns>
        DataSet ExecProcedure(string storedProcName, IList<DbParameter> parameters);

        /// <summary>
        /// 执行存储过程,并返回指定表数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="tableNames">表名</param>
        /// <returns>DataSet</returns>
        DataSet ExecProcedure(string storedProcName, params string[] tableNames);

        /// <summary>
        /// 执行存储过程,带参数,并返回指定表数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">DbParameter 参数</param>
        /// <param name="tableNames">表名</param>
        /// <returns>DataSet</returns>
        DataSet ExecProcedure(string storedProcName, IList<DbParameter> parameters, params string[] tableNames);

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="dataTable">DataTable，必须设置主键。</param>
        /// <param name="sql">Table对应的SQL，必须包含主键列。</param>
        /// <returns></returns>
        void Update(DataTable dataTable, string sql);

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="dataSet">DataSet，必须设置主键，多表时需要设置每个的TableName。</param>
        /// <param name="sql">每个Table对应的SQL，必须包含主键列。</param>
        /// <returns></returns>
        void Update(DataSet dataSet, params string[] sql);

        /// <summary>
        /// 更新数据库(事务)
        /// </summary>
        /// <param name="dataSet">DataSet，必须设置主键，多表时需要设置每个的TableName。</param>
        /// <param name="sql">每个Table对应的SQL，必须包含主键列。</param>
        /// <returns></returns>
        void UpdateTran(DataSet dataSet, params string[] sql);
    }
}
