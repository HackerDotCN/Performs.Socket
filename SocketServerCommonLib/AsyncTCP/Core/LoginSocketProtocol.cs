﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 登录时协议
    /// </summary>
    public class LoginSocketProtocol : SocketInvokeElement
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="asyncSocketServer">AsyncSocketServer</param>
        /// <param name="socketUserToken">SocketUserToken</param>
        public LoginSocketProtocol(AsyncSocketServer asyncSocketServer, SocketUserToken socketUserToken)
            : base(asyncSocketServer, socketUserToken)
        {
        }

        /// <summary>
        /// 协议通过发送客户端信息
        /// </summary>
        /// <param name="buffer">接收到的信息</param>
        /// <param name="offset">从第几位开始</param>
        /// <param name="count">共有几位</param>
        /// <returns>true</returns>
        public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
        {
            bool issuccessful = DoLogin();
            if (issuccessful)
                socketUserToken.InvokeElement = new MesgTransmitSocketProtocol(m_asyncSocketServer, socketUserToken);
            DoSendResult();
            return issuccessful;
        }
        //登录
        public bool DoLogin()
        {
            string userName = "";
            string password = "";
            string _value = "";
            if (InDataParser.GetValue("username", ref userName))
            {
                socketUserToken.UserName = userName;
                socketUserToken.isDevice = false;
                DelegateState.ServerStateInfo(userName + socketUserToken.ConnectSocket.RemoteEndPoint.ToString() + "用户登录成功");
                socketUserToken.RemotDeviceIp = socketUserToken.ConnectSocket.RemoteEndPoint.ToString();
                DelegateState.AddTCPuserStateInfo(socketUserToken);

                socketUserToken.LoginFlag = true;
                //添加返回信息   插入-1
                return true;
            }
            socketUserToken.LoginFlag = false;
            return false;
        }
        /// <summary>
        /// 发送返回值
        /// </summary>
        /// <returns>bool</returns>
        public bool DoSendResult()
        {
            try
            {
                //返回给软件 UTF8 编码
                string commandText = OutDataParser.GetProtocolText();//已经添加了返回信息          插入-1
                byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
                int totalLength = bufferUTF8.Length; //获取总大小
                AsyncSendBufferManager asyncSendBufferManager = socketUserToken.SendBuffer;
                asyncSendBufferManager.StartPacket();
                asyncSendBufferManager.m_dynamicBufferManager.WriteBuffer(bufferUTF8); //写入命令内容
                asyncSendBufferManager.EndPacket();
                bool result = true;
                if (!m_sendAsync)
                {
                    int packetOffset = 0;
                    int packetCount = 0;
                    if (asyncSendBufferManager.GetFirstPacket(ref packetOffset, ref packetCount))
                    {
                        m_sendAsync = true;
                        result = m_asyncSocketServer.SendAsyncEvent(socketUserToken.ConnectSocket, socketUserToken.SendEventArgs,
                            socketUserToken.SendBuffer.m_dynamicBufferManager.Buffer, packetOffset, packetCount);
                    }
                }
            }
            catch { m_asyncSocketServer.CloseClientSocket(socketUserToken); }
            return true;
        }
        /// <summary>
        /// 重新登录,清空已经连接的列表
        /// </summary>
        public void SocketUserClearSendUser()
        {
            SocketUserToken[] userTokenArray = null;
            m_asyncSocketServer.AsyncSocketUserList.CopyList(ref userTokenArray);
            foreach (SocketUserToken user in userTokenArray)
            {
                if (user.UserName == socketUserToken.UserName)//用户名相等
                {
                    user.ForwarSocket = null;
                    if (!user.isDevice && user.LoginFlag)//用户名相等
                    {
                        lock (user)
                        {
                            m_asyncSocketServer.CloseClientSocket(user);
                        }
                    }
                }
            }
        }
    }
}
