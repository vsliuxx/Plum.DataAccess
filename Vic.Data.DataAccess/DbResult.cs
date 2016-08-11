using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vic.Data
{
    /// <summary>
    /// 数据访问返回的结果
    /// </summary>
    public class DbResult : MarshalByRefObject, IDbResult
    {
        private bool _isSucceed;
        private int _errCode;
        private string _errMessage;
        private object _result;
        private Dictionary<string, object> parms;

        /// <summary>
        /// 数据访问返回的结果实例
        /// </summary>
        /// <param name="isSucceed">是否执行成功</param>
        /// <param name="errCode">执行异常时的错误码</param>
        /// <param name="errMessage">执行异常时的错误信息</param>
        /// <param name="result">执行成功后返回的结果</param>
        internal DbResult(bool isSucceed, int errCode, string errMessage, object result)
        {
            this._isSucceed = isSucceed;
            this._errCode = errCode;
            this._errMessage = errMessage;
            this._result = result;
        }

        /// <summary>
        /// 数据访问返回的结果实例
        /// </summary>
        /// <param name="isSucceed">是否执行成功</param>
        /// <param name="errCode">执行异常时的错误码</param>
        /// <param name="errMessage">执行异常时的错误信息</param>
        /// <param name="result">执行成功后返回的结果</param>
        /// <param name="parms">参数集合</param>
        internal DbResult(bool isSucceed, int errCode, string errMessage, object result, Dictionary<string, object> parms)
        {
            this._isSucceed = isSucceed;
            this._errCode = errCode;
            this._errMessage = errMessage;
            this._result = result;
            this.parms = parms;
        }

        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSucceed
        {
            get { return this._isSucceed; }
            internal set { this._isSucceed = value; }
        }

        /// <summary>
        /// 执行异常时的错误码
        /// </summary>
        public int ErrCode
        {
            get { return this._errCode; }
            internal set { this._errCode = value; }
        }

        /// <summary>
        /// 执行异常时的错误信息
        /// </summary>
        public string ErrMessage
        {
            get { return this._errMessage; }
            internal set { this._errMessage = value; }
        }

        /// <summary>
        /// 执行返回的结果
        /// </summary>
        public object Result
        {
            get { return this._result; }
            internal set { this._result = value; }
        }

        /// <summary>
        /// 获取一个参数值
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
        /// 获取一个参数值
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
    }
}
