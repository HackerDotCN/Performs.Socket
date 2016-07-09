using System;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 委托回调函数 this.Invoke(new ThreadStart(delegate{})) 实现与UI交换
    /// </summary>
    public class DelegateState
    {

        public delegate void SocketStateCallBack(string msg);
        /// <summary>
        /// 信息显示
        /// </summary>
        public static SocketStateCallBack ServerStateInfo;
    }
}
