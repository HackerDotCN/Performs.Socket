using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 协议初始定义
    /// </summary>
    class ProtocolKeys
    {
        //判断是否存在
        public static string ReturnWrap = "\r\n";
        //值
        public static string Command = "Command";
        //用户名
        public static string UserName = "UserName";
        //密码
        public static string Password = "Password";
        //唯一标志
        public static string OnlyUser = "Only";
        //Command= ?    UserName= ? 定义值
        public static string EqualSign = "=";

        //状态 ProtocolCode
        public static string Code = "Code";


        public static string LeftBrackets = "[";
        public static string RightBrackets = "]";
        //返回
        public static string Request = "Request";
        //发送
        public static string Response = "Response";
    }

    /// <summary>
    /// 状态码
    /// </summary>
    public class ProtocolCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static int Success = 0x00000000;
        /// <summary>
        /// 失败
        /// </summary>
        public static int failure = 0x10000000;
    }
    //标记信息
    public enum ProtocolFlags
    {
        /// <summary>
        /// 登录
        /// </summary>
        Login = 4, 
        /// <summary>
        /// 信息
        /// </summary>
        Information = 5,
    }
}
