using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerCommonLib
{

    /// <summary>
    /// 信息转发模式(独特的转发没有经过组装的信息)
    /// </summary>
    public class MesgTransmitSocketProtocol : SocketInvokeElement
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="asyncSocketServer">AsyncSocketServer</param>
        /// <param name="socketUserToken">SocketUserToken</param>
        public MesgTransmitSocketProtocol(AsyncSocketServer asyncSocketServer, SocketUserToken socketUserToken)
            : base(asyncSocketServer, socketUserToken)
        {
        }

        /// <summary>
        /// 重写ProcessReceive  信息直接发送
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset">int</param>
        /// <param name="count">int</param>
        /// <returns>bool</returns>
        public override bool ProcessReceive(byte[] buffer, int offset, int count) //接收异步事件返回的数据，用于对数据进行缓存和分包
        {
            //try 一定要加：避免接收端突然关闭,导致设备端口异常
            try
            {
                SocketUserToken[] userTokenArray = null;
                m_asyncSocketServer.AsyncSocketUserList.CopyList(ref userTokenArray);
                byte[] sendbuff = new byte[count];
                Array.Copy(buffer, offset, sendbuff, 0, count);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    m_asyncSocketServer.SendMsgClientMsg(sendbuff, userTokenArray[i]);
                }
            }
            catch { }
            return true;
        }
    }
}
