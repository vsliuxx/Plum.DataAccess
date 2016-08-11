using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vic.Data
{
    /// <summary>
    /// 数据访问返回的结果
    /// </summary>
    interface IDbResult
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        bool IsSucceed
        {
            get;
        }

        /// <summary>
        /// 执行异常时的错误码
        /// </summary>
        int ErrCode
        {
            get;
        }

        /// <summary>
        /// 执行异常时的错误信息
        /// </summary>
        string ErrMessage
        {
            get;
        }

        /// <summary>
        /// 执行返回的结果
        /// </summary>
        object Result
        {
            get;
        }

        /// <summary>
        /// 获取一个参数值
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns>object</returns>
        object GetParamValue(string parameterName);

        /// <summary>
        /// 获取一个参数值
        /// </summary>
        /// <param name="index">参数索引,从0开始</param>
        /// <returns>object</returns>
        object GetParamValue(int index);
    }
}
