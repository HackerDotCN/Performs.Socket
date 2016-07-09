/********************************************************************
 * * Copyright (C) 2014-2250 Corporation All rights reserved.
 * * 作者： Amos Li QQ：443061626 
 * * 创建时间：2014-08-05
 * * 说明：
********************************************************************/


using System;
using System.Threading;
using SocketServerCommonLib.AsyncTCP.Core;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 心跳包：可服务器发送信息判断，也可以根据接收发送时间判断
    /// </summary>
    public class DaemonThread
    {
        private Thread m_thread;
        private AsyncSocketServer m_asyncSocketServer;
        int TeartbeatCount = 0;
        bool isStara = true;
        public DaemonThread(AsyncSocketServer asyncSocketServer)
        {
            m_asyncSocketServer = asyncSocketServer;
            isStara = true;
            m_thread = new Thread(DaemonThreadStart);
            m_thread.Start();
        }

        public void DaemonThreadStart()
        {
            while (m_thread.IsAlive)
            {
                SocketUserToken[] userTokenArray = null;
                m_asyncSocketServer.AsyncSocketUserList.CopyList(ref userTokenArray);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    if (!m_thread.IsAlive)
                        break;
                    try
                    {
                        if (userTokenArray[i].ConnectSocket != null && (DateTime.Now - userTokenArray[i].ActiveDateTime).TotalSeconds > m_asyncSocketServer.serverconfig.socketTimeoutMs)
                        {
                            m_asyncSocketServer.SendMsgClientMsg(HTTPCore.PackData("心跳包检测"), userTokenArray[i]);//异步发送时,如果发送不成功就自动关闭客户端
                        }
                    }
                    catch (Exception ex)
                    {
                        DelegateState.ServerStateInfo("心跳:" + ex.Message);
                    }
                }
                for (int x = 0; x < 60 * 1000 / 10; x++) //每一分钟检测一次
                {
                    if (!m_thread.IsAlive & !isStara)
                        break;
                    Thread.Sleep(10);
                }
                TeartbeatCount++;
                DelegateState.TeartbeatServerStateInfo(TeartbeatCount);
            }
        }
        /// <summary>
        /// 停用线程
        /// </summary>
        public void Close()
        {
            isStara = false;
            m_thread.Abort();
        }

    }
}
