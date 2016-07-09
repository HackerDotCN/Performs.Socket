using System;
using System.Net.Sockets;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 客户端
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
        /// 缓冲区大小
        /// </summary>
        public int BuffSize = 1024 * 10;
        /// <summary>
        /// 接收套节字
        /// </summary>
        public NetworkStream m_nStream { get; set; }

        /// <summary>
        /// 异步接收后包的大小
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 当前连接服务端端口号
        /// </summary>
        public string m_ServerIp;
        public int m_ServerPort;

        /// <summary>
        /// 搜索设备的对应名称
        /// </summary>
        public string UserName;
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string OnlyName;

        /// <summary>
        /// 客户端
        /// </summary>
        TcpClient m_client;

        /// <summary>
        /// 创建Sockets对象
        /// </summary>
        /// <param name="ip">Ip地址</param>
        /// <param name="client">TcpClient</param>
        /// <param name="ns">承载客户端Socket的网络流</param>
        public SocketUserToken(string ServerIp, int ServerPort, TcpClient client, NetworkStream nStream)
        {
            m_ServerIp = ServerIp;
            m_ServerPort = ServerPort;
            m_client = client;
            m_nStream = nStream;
            m_receiveBuffer = new DynamicBufferManager(BuffSize);
            m_sendBuffer = new AsyncSendBufferManager(BuffSize);
        }
    }
}
