using System;
using System.Net;
using System.Threading;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 服务器配置
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// 最大支持数量
        /// </summary>
        public int numConnections = 10;

        /// <summary>
        /// 限制访问接收连接的线程数，用来控制最大并发数-如果有numConnections 线程全部阻塞,等待一个用户退出,才能继续
        /// </summary>
        public Semaphore semap = new Semaphore(36000, 36000);

        /// <summary>
        /// 监听IP-端口
        /// </summary>
        public string ListenIp = "127.0.0.1";
        public int ListenPort = 16000;
        public IPEndPoint locahostEndPoint;
        /// <summary>
        /// Socket连接超时(秒),检测Socket是否在线间隔(检测时间会实时的差距)
        /// </summary>
        public int socketTimeoutMs = 40;

        /// <summary>
        /// 守护连接进程
        /// </summary>
        public DaemonThread m_daemonThread;

        /// <summary>
        /// 缓存大小
        /// </summary>
        public int buffSize = 1024 * 5;

        public ServerConfig()
        {
            ListenIp = NetworkAddress.GetIPAddress();
            locahostEndPoint = new IPEndPoint(IPAddress.Parse(ListenIp), ListenPort);
        }
    }
}
