using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 用户集合类
    /// </summary>
    public class SocketUserToken
    {

        /// <summary>
        ///  接收缓冲区域
        /// </summary>
        protected DynamicBufferManager m_receiveBuffer;
        public DynamicBufferManager ReceiveBuffer { get { return m_receiveBuffer; } set { m_receiveBuffer = value; } }

        /// <summary>
        ///发送缓冲区域
        /// </summary>
        protected AsyncSendBufferManager m_sendBuffer;
        public AsyncSendBufferManager SendBuffer { get { return m_sendBuffer; } set { m_sendBuffer = value; } }

        /// <summary>
        /// 异步发送函数
        /// </summary>
        protected SocketAsyncEventArgs m_sendEventArgs;
        public SocketAsyncEventArgs SendEventArgs { get { return m_sendEventArgs; } set { m_sendEventArgs = value; } }
        /// <summary>
        /// 异步接收函数
        /// </summary>
        protected SocketAsyncEventArgs m_receiveEventArgs;
        public SocketAsyncEventArgs ReceiveEventArgs { get { return m_receiveEventArgs; } set { m_receiveEventArgs = value; } }

        /// <summary>
        /// 接收信息和发送信息协议过滤组装
        /// </summary>
        private SocketInvokeElement m_invokeElement;
        public SocketInvokeElement InvokeElement { get { return m_invokeElement; } set { m_invokeElement = value; } }

        /// <summary>
        /// 接收数据时初始定义
        /// </summary>
        protected byte[] m_asyncReceiveBuffer;
        /// <summary>
        /// 是否处于异步发送中
        /// </summary>
        public bool m_sendAsync = false;
        /// <summary>
        /// 登录标记是否登录成功
        /// </summary>
        public bool LoginFlag = false;
        /// <summary>
        /// 登录标记是用户
        /// </summary>
        public bool isDevice = false;
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectDateTime;
        /// <summary>
        /// 消息时间 心跳时判断
        /// </summary>
        public DateTime ActiveDateTime;
        /// <summary>
        /// 连接客户端
        /// </summary>
        public Socket m_connectSocket;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName = string.Empty;
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string OnlyName = string.Empty;
        /// <summary>
        /// 远端设备IP地址
        /// </summary>
        public string RemotDeviceIp = string.Empty;
        /// <summary>
        /// 远端设备IP型号
        /// </summary>
        public string firmware = string.Empty;
        /// <summary>
        /// 转发客户端
        /// </summary>
        public Socket ForwarSocket;

        public Socket ConnectSocket
        {
            get
            {
                return m_connectSocket;
            }
            set
            {
                m_connectSocket = value;
                if (m_connectSocket == null)
                {
                    //清理接收缓存
                    m_receiveBuffer.Clear(m_receiveBuffer.DataCount);
                    m_sendBuffer.ClearPacket();
                }
                //异步监听赋予回调函数
                m_sendEventArgs.AcceptSocket = m_connectSocket;
                m_receiveEventArgs.AcceptSocket = m_connectSocket;
            }
        }

        /// <summary>
        /// 初始化客户端
        /// </summary>
        /// <param name="asyncReceiveBufferSize">接收和发送缓存大小</param>
        public SocketUserToken(int asyncReceiveBufferSize)
        {
            m_connectSocket = null;
            m_receiveEventArgs = new SocketAsyncEventArgs();

            //异步回调函数时候的值(this)
            m_receiveEventArgs.UserToken = this;
            m_asyncReceiveBuffer = new byte[asyncReceiveBufferSize];
            m_receiveEventArgs.SetBuffer(m_asyncReceiveBuffer, 0, m_asyncReceiveBuffer.Length);

            //异步回调函数时候的值(this)
            m_sendEventArgs = new SocketAsyncEventArgs();
            m_sendEventArgs.UserToken = this;
            //初始化缓存大小
            m_receiveBuffer = new DynamicBufferManager(asyncReceiveBufferSize);
            m_sendBuffer = new AsyncSendBufferManager(asyncReceiveBufferSize);
        }
    }
}
