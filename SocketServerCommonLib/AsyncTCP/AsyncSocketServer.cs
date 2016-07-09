using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using SocketServerCommonLib.AsyncTCP.Core;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 服务器-同步监听端口、异步发送数据
    /// </summary>
    public class AsyncSocketServer
    {

        private Socket listenSocket;
        /// <summary>
        /// 是否已启动监听
        /// </summary>
        public bool IsStartListening = false;
        /// <summary>
        /// 服务器配置
        /// </summary>
        private ServerConfig Serverconfig { get; set; }

        public ServerConfig serverconfig
        {
            get { return Serverconfig; }
            set { Serverconfig = value; }
        }
        /// <summary>
        /// 用户集合
        /// </summary>
        private SocketUserClientList m_asyncSocketUserList;
        public SocketUserClientList AsyncSocketUserList { get { return m_asyncSocketUserList; } }
        /// <summary>
        /// 用户池定义
        /// </summary>
        private AsyncSocketUserPool m_asyncSocketUserTokenPool;


        /// <summary>
        /// 用户池初始化
        /// </summary>
        public void Init()
        {
            Serverconfig = new ServerConfig();
            m_asyncSocketUserList = new SocketUserClientList();
            m_asyncSocketUserTokenPool = new AsyncSocketUserPool(serverconfig.numConnections);
            SocketUserToken userToken;
            for (int i = 0; i < serverconfig.numConnections; i++) //按照连接数建立读写对象
            {
                userToken = new SocketUserToken(serverconfig.buffSize);
                //异步回调函数初始化
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                //异步回调函数初始化
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                //开辟固定空间
                m_asyncSocketUserTokenPool.Push(userToken);
            }
        }

        /// <summary>
        /// 异步发送及接收回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            SocketUserToken userToken = asyncEventArgs.UserToken as SocketUserToken;
            userToken.ActiveDateTime = DateTime.Now;

            lock (userToken)
            {
                try
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(asyncEventArgs);
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                        ProcessSend(asyncEventArgs);
                    else
                        throw new ArgumentException("最后一次操作完成套接字接收或发送");
                }
                catch (Exception ex)
                {
                    DelegateState.ServerStateInfo("异步发送及接收回调函数: IO_Completed  error, message: " + userToken.UserName + ":" + userToken.RemotDeviceIp + ex.Message);
                }
            }
        }
        /// <summary>
        /// 异步发送回调函数
        /// </summary>
        /// <param name="asyncEventArgs"></param>
        private bool ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            SocketUserToken userToken = sendEventArgs.UserToken as SocketUserToken;
            if (sendEventArgs.SocketError == SocketError.Success)
                return SendCompleted(userToken); //调用子类回调函数
            else
            {
                CloseClientSocket(userToken);
                return false;
            }
        }

        /// <summary>
        /// 发送回调函数特殊客户连接即发送(一般情况:协议成功后才可使用)
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public virtual bool SendCompleted(SocketUserToken userToken)
        {
            userToken.m_sendAsync = false;
            AsyncSendBufferManager asyncSendBufferManager = userToken.SendBuffer;
            asyncSendBufferManager.ClearFirstPacket(); //清除已发送的包
            int offset = 0;
            int count = 0;
            if (asyncSendBufferManager.GetFirstPacket(ref offset, ref count))//缓存中是否还有数据没有发送,有就继续发送
            {
                userToken.m_sendAsync = true;
                return SendAsyncEvent(userToken.ConnectSocket, userToken.SendEventArgs,
                    asyncSendBufferManager.m_dynamicBufferManager.Buffer, offset, count);
            }
            else
                return true;
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="connectSocket">Socket</param>
        /// <param name="sendEventArgs">SocketAsyncEventArgs</param>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset">int</param>
        /// <param name="count">int</param>
        /// <returns>bool</returns>
        public bool SendAsyncEvent(Socket connectSocket, SocketAsyncEventArgs sendEventArgs, byte[] buffer, int offset, int count)
        {
            if (connectSocket == null)
                return false;
            sendEventArgs.SetBuffer(buffer, offset, count);//设置缓冲区域
            bool willRaiseEvent = connectSocket.SendAsync(sendEventArgs);//异步发送
            if (!willRaiseEvent)
            {
                return ProcessSend(sendEventArgs);
            }
            else
                return true;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void Start()
        {
            if (IsStartListening)
                return;

            IsStartListening = true;
            Init();
            listenSocket = new Socket(serverconfig.locahostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(serverconfig.locahostEndPoint);
            listenSocket.Listen(serverconfig.numConnections);
            DelegateState.ServerStateInfo("TCP服务器启动...");
            StartAccept(null);
            serverconfig.m_daemonThread = new DaemonThread(this);//启动连接超时判断
        }

        /// <summary>
        /// 监听程序
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)//第一运
            {
                //acceptEventArgs == null  绑定异步回调函数AcceptEventArg_Completed
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                //为什么没有while ?  AcceptEventArg_Completed()异步回调函数中递归了StartAccept()方法
                //再次调用StartAccept()方法时候AcceptSocket处于上一个用户的绑定状态,所以释放上次绑定的Socket，等待下一个Socket连接
                acceptEventArgs.AcceptSocket = null;
            }

            serverconfig.semap.WaitOne();//减少个一信号量 ，退出时候会返回
            //异步监控：回调函数AcceptEventArg_Completed
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArgs);
            if (!willRaiseEvent)
            {
                //不成功再次连接
                ProcessAccept(acceptEventArgs);
            }
        }
        /// <summary>
        /// 异步监控回调函数
        /// </summary>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                ProcessAccept(e);
            }
            catch (Exception ex)
            {
                DelegateState.ServerStateInfo("连接异常:" + ex.Message);
            }
        }
        /// <summary>
        /// 添加新客户
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs.AcceptSocket.RemoteEndPoint == null)
            {
                acceptEventArgs.AcceptSocket.Close();
                StartAccept(acceptEventArgs);
                return;
            }
            DelegateState.ServerStateInfo(" TCP - 客户端：" + acceptEventArgs.AcceptSocket.RemoteEndPoint + "连接");
            DelegateState.ServerConnStateInfo(acceptEventArgs.AcceptSocket.RemoteEndPoint.ToString(), "TCP");
            SocketUserToken userToken = m_asyncSocketUserTokenPool.Pop();
            m_asyncSocketUserList.Add(userToken);
            userToken.ConnectSocket = acceptEventArgs.AcceptSocket;
            userToken.ConnectDateTime = DateTime.Now;
            //HTTP连接时初始化HTTP协议
            userToken.InvokeElement = new HTTPCoreProtocol(this, userToken);
            try
            {
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs);//异步回调函数确定
                if (!willRaiseEvent)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            catch (Exception e)
            {
                DelegateState.ServerStateInfo("连接端 " + userToken.ConnectSocket.RemoteEndPoint + " 错误, 错误信息: " + e.Message);
                CloseClientSocket(userToken);
            }
            StartAccept(acceptEventArgs);//递归继续异步监控客户端
        }

        /// <summary>
        /// 异步接收请求
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void ProcessReceive(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            SocketUserToken userToken = socketAsyncEventArgs.UserToken as SocketUserToken;
            if (userToken.ConnectSocket == null)
                return;

            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                int offset = userToken.ReceiveEventArgs.Offset;
                int count = userToken.ReceiveEventArgs.BytesTransferred;
                if ((userToken.InvokeElement == null) & (userToken.ConnectSocket != null))//当连接没有协议时，根据命令初始化协议
                {
                    BuildingInvokeElement(userToken);//特殊自定不截取,直接进行协议赋予
                    offset = 0;
                    count = 0;
                }
                if (userToken.InvokeElement == null) //如果没有解析对象，提示非法连接并关闭连接
                {
                    //* DelegateState.ServerStateInfo("非法连接:" + userToken.ConnectSocket.RemoteEndPoint);
                    CloseClientSocket(userToken);
                }
                else
                {
                    if (count > 0) //处理接收数据
                    {
                        if (!userToken.InvokeElement.ProcessReceive(userToken.ReceiveEventArgs.Buffer, offset, count))//处理数据
                        {
                            //如果处理数据返回失败，则断开连接
                            CloseClientSocket(userToken);
                        }
                        else
                        {
                            bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //继续异步接收
                            if (!willRaiseEvent)
                                ProcessReceive(userToken.ReceiveEventArgs);
                        }
                    }
                    else
                    {
                        bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                        if (!willRaiseEvent)
                            ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            else
            {
                CloseClientSocket(userToken);
            }
        }
        /// <summary>
        /// 初始化类,确定是登录或者发送信息 和其它的API
        /// </summary>
        /// <param name="userToken"></param>
        private void BuildingInvokeElement(SocketUserToken userToken)
        {
            //<!-- 特殊处理,设备登录无法协议,直接进行初始化登录协议 -->
            //获取接收的0个字节-初始化是登录还是信息
            byte flag = userToken.ReceiveEventArgs.Buffer[userToken.ReceiveEventArgs.Offset];
            //if (flag == (byte)ProtocolFlags.Login)
            //    userToken.InvokeElement = new LoginSocketProtocol(this, userToken);
            userToken.InvokeElement = new HTTPCoreProtocol(this, userToken);
        }

        /// <summary>
        /// 清除客户端
        /// </summary>
        /// <param name="userToken"></param>
        public void CloseClientSocket(SocketUserToken userToken)
        {
            userToken.LoginFlag = false;
            DelegateState.ReomveTCPStateInfo(userToken);

            if (userToken.ConnectSocket != null)
            {
                try
                {
                    DelegateState.ServerStateInfo(string.Format("断线 {0}", userToken.ConnectSocket.RemoteEndPoint.ToString() + "-" + userToken.UserName + ":" + userToken.RemotDeviceIp));
                    userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
                    userToken.ConnectSocket.Close();
                }
                catch (Exception ex)
                {
                    DelegateState.ServerStateInfo("断开连接" + userToken.UserName + ":" + userToken.RemotDeviceIp + " error, message:" + ex.Message);
                }
            }
            else
                DelegateState.ServerStateInfo(string.Format("断线 {0}", userToken.UserName + ":" + userToken.RemotDeviceIp));

            userToken.ForwarSocket = null;
            userToken.InvokeElement = null;
            userToken.m_sendAsync = false;
            userToken.ReceiveBuffer.Clear();
            userToken.SendBuffer.ClearPacket();
            userToken.ConnectSocket = null; //释放引用，并清理缓存，包括释放协议对象等资源
            userToken.isDevice = false;
            userToken.LoginFlag = false;
            serverconfig.semap.Release();//增加个一信号量
            m_asyncSocketUserTokenPool.Push(userToken);
            AsyncSocketUserList.Remove(userToken);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="commandText">string</param>
        /// <param name="userToken">SocketUserToken 连接客户端</param>
        /// <returns>bool</returns>
        public bool SendMsgClientMsg(byte[] bufferUTF8, SocketUserToken userToken)
        {
            int totalLength = bufferUTF8.Length; //获取总大小
            AsyncSendBufferManager asyncSendBufferManager = userToken.SendBuffer;
            asyncSendBufferManager.StartPacket();
            asyncSendBufferManager.m_dynamicBufferManager.WriteBuffer(bufferUTF8); //写入命令内容
            asyncSendBufferManager.EndPacket();
            bool result = true;
            if (!userToken.m_sendAsync)
            {
                int packetOffset = 0;
                int packetCount = 0;
                if (asyncSendBufferManager.GetFirstPacket(ref packetOffset, ref packetCount))
                {
                    userToken.m_sendAsync = true;
                    result = SendAsyncEvent(userToken.ConnectSocket, userToken.SendEventArgs,
                        asyncSendBufferManager.m_dynamicBufferManager.Buffer, packetOffset, packetCount);
                }
            }
            return result;
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            Serverconfig.m_daemonThread.Close();
            IsStartListening = false;
            listenSocket.Close();
            if (m_asyncSocketUserList.Userlist != null)
                foreach (SocketUserToken userToken in m_asyncSocketUserList.Userlist)
                    userToken.ConnectSocket = null;
            if (m_asyncSocketUserTokenPool != null)
                m_asyncSocketUserTokenPool.Clear();
            GC.Collect();
        }
    }
}
