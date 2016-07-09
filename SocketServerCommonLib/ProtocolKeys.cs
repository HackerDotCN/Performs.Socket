using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 协议初始定义
    /// </summary>
    class ProtocolKeys
    {
        public static string ReturnWrap = "\r\n";
        //值
        public static string Command = "Command";
        //唯一标志
        public static string OnlyUser = "Only";
        //Command= ?    UserName= ? 定义值
        public static string EqualSign = "=";

        //Command：?   UserName: ? 冒号
        public static string ColonSign = ":";

        //状态 ProtocolCode
        public static string Code = "Code";


        /// <summary>
        /// 版本信息
        /// </summary>
        public static string firmware = "firmware";

        /// <summary>
        /// 重新登录
        /// </summary>
        public static string ResetLogin = "ResetLogin";

        public static string LeftBrackets = "[";
        public static string RightBrackets = "]";
        //返回
        public static string Request = "Request";
        //发送
        public static string Response = "Response";

        //密钥
        public static string PubKey = "PubKey";

        
    }

    /// <summary>
    /// 状态码
    /// </summary>
    public class ProtocolCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static int Success = 0x0;
        /// <summary>
        /// 失败
        /// </summary>
        public static int failure = 0x1;
    }
    //标记信息
    public enum ProtocolFlags
    {
        Login = 49,
        /// <summary>
        /// 信息
        /// </summary>
        Information = 5,
        /// <summary>
        /// Wifi登录协议
        /// </summary>
        LoginWIFI = 100,
    }

    public class ProtocolBytes
    {
        /// <summary>
        /// 心跳验证字节
        /// </summary>
        public static byte[] Heartbeatbytes = new byte[] { 187, 153, 05 };
    }
}
