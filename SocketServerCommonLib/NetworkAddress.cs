
using System;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Specialized;
using System.Net.Sockets;

namespace SocketServerCommonLib
{
    public class NetworkAddress
    {
        #region =========检测机器地址==========
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);

        /// <summary>
        /// 机器是否联网
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedToInternet()
        {
            int Desc = 0;
            return InternetGetConnectedState(Desc, 0);
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns>本机IP</returns>
        public static string GetIPAddress()
        {
            IPAddress[] localIPs;
            localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            StringCollection IpCollection = new StringCollection();
            foreach (IPAddress ip in localIPs)
            {
                //根据AddressFamily判断是否为ipv4,如果是InterNetWork则为ipv6
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    IpCollection.Add(ip.ToString());
            }
            string[] IpArray = new string[IpCollection.Count];
            IpCollection.CopyTo(IpArray, 0);
            return IpArray[0];
        }


        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        /// <summary>
        /// 获取MAC地址
        /// </summary>
        /// <param name="clientIP">指定IP地址</param>
        /// <returns>MAC地址</returns>
        public static string GetRemoteMac(string clientIP)
        {
            string ip = clientIP;
            if (clientIP == null || clientIP == "" || ip == "127.0.0.1")
                ip = GetIPAddress();
            Int32 ldest = inet_addr(ip);
            Int64 macinfo = new Int64();
            Int32 len = 6;
            try
            {
                SendARP(ldest, 0, ref macinfo, ref len);
            }
            catch
            {
                return "";

            }
            string originalMACAddress = Convert.ToString(macinfo, 16);
            if (originalMACAddress.Length < 12)
            {
                originalMACAddress = originalMACAddress.PadLeft(12, '0');
            }
            string macAddress;
            if (originalMACAddress != "0000" && originalMACAddress.Length == 12)
            {
                string mac1, mac2, mac3, mac4, mac5, mac6;
                mac1 = originalMACAddress.Substring(10, 2);
                mac2 = originalMACAddress.Substring(8, 2);
                mac3 = originalMACAddress.Substring(6, 2);
                mac4 = originalMACAddress.Substring(4, 2);
                mac5 = originalMACAddress.Substring(2, 2);
                mac6 = originalMACAddress.Substring(0, 2);
                macAddress = mac1 + "-" + mac2 + "-" + mac3 + "-" + mac4 + "-" + mac5 + "-" + mac6;
            }
            else
            {
                macAddress = "";
            }
            return macAddress.ToUpper();
        }

        /// <summary>
        /// 获取公网IP
        /// </summary>
        /// <returns>公网IP地址</returns>
        public static string PUBLICIP()
        {
            try
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    string html = wc.DownloadString("http://ip.qq.com");
                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(html, "<span class=\"red\">([^<]+)</span>");
                    if (m.Success) return m.Groups[1].Value;

                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}
