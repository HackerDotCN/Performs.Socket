using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketClientCommonLib
{
    /// <summary>
    /// 输出时信息组装
    /// </summary>
    public class AssemblyOutDataParser
    {
        private List<string> m_protocolText;

        /// <summary>
        /// 初始化协议
        /// </summary>
        public AssemblyOutDataParser()
        {
            m_protocolText = new List<string>();
        }

        /// <summary>
        /// 清楚以前协议
        /// </summary>
        public void Clear()
        {
            m_protocolText.Clear();
        }
        /// <summary>
        /// 发送信息通过协议组装后的信息
        /// </summary>
        /// <returns>string</returns>
        public string GetProtocolText()
        {
            string tmpStr = "";
            if (m_protocolText.Count > 0)
            {
                tmpStr = m_protocolText[0];
                for (int i = 1; i < m_protocolText.Count; i++)
                {
                    tmpStr += ProtocolKeys.ReturnWrap + m_protocolText[i];
                }
            }
            return tmpStr;
        }

        /// <summary>
        /// 协议组装Key只和Value值
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <param name="value"></param>
        public void AddValue(string protocolKey, string value)
        {
            m_protocolText.Add(protocolKey + "=" + value.ToString());
        }
        /// <summary>
        /// 添加返回头部
        /// </summary>
        public void AddRequest()
        {
            m_protocolText.Add(ProtocolKeys.LeftBrackets + ProtocolKeys.Request + ProtocolKeys.RightBrackets);
        }

        /// <summary>
        /// 添加成功参数
        /// </summary>
        public void AddSuccess()
        {
            m_protocolText.Add(ProtocolKeys.Code + ProtocolKeys.EqualSign + ProtocolCodes.Success.ToString());
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="commandKey"></param>
        public void AddCommand(string commandKey)
        {
            m_protocolText.Add(ProtocolKeys.Command + ProtocolKeys.EqualSign + commandKey);
        }
        /// <summary>
        /// 填入提交头部
        /// </summary>
        public void AddResponse()
        {
            m_protocolText.Add(ProtocolKeys.LeftBrackets + ProtocolKeys.Response + ProtocolKeys.RightBrackets);
        }
    }
}
