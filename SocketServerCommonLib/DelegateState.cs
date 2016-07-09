using System;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 委托回调函数 this.Invoke(new ThreadStart(delegate{})) 实现与UI交换
    /// </summary>
    public class DelegateState
    {
        //信息显示
        public delegate void SocketStateCallBack(string msg);

        public delegate void SockeTeartbeatStateCallBack(int num);
        public delegate void SocketConnStateCallBack(string RemoteIp,string TCPUDP);
        /// <summary>
        /// 信息显示
        /// </summary>
        public static SocketStateCallBack ServerStateInfo;

        /// <summary>
        /// 心跳检测信息
        /// </summary>
        public static SockeTeartbeatStateCallBack TeartbeatServerStateInfo;

        /// <summary>
        /// 信息显示
        /// </summary>
        public static SocketConnStateCallBack ServerConnStateInfo;


        #region TCP服务
        
        public delegate void SocketTCPStateCallBack(string msg);
        public delegate void SocketAddTCPuserStateCallBack(SocketUserToken userToken);

        public delegate void SocketAddTCPdeviceStateCallBack(SocketUserToken userToken);
        public delegate void SocketReomveTCPStateCallBack(SocketUserToken userToken);

        /// <summary>
        /// TCP信息显示
        /// </summary>
        public static SocketTCPStateCallBack ServerTCPStateInfo;
        /// <summary>
        /// TCP添加用户
        /// </summary>
        public static SocketAddTCPuserStateCallBack AddTCPuserStateInfo;
        /// <summary>
        /// TCP添加设备
        /// </summary>
        public static SocketAddTCPdeviceStateCallBack AddTCPdeviceStateInfo;
        /// <summary>
        /// TCP删除连接
        /// </summary>
        public static SocketReomveTCPStateCallBack ReomveTCPStateInfo;
        #endregion

    }
}
