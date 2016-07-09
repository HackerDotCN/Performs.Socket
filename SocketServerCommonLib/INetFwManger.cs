﻿
using System;
using NetFwTypeLib;
using System.Security.Principal;//COM+ 组件 NetFwTypeLib

namespace SocketServerCommonLib
{
    /// <summary>
    /// Windows防火墙
    /// </summary>
    public static class INetFwManger
    {
        /// <summary>
        /// 添加防火墙例外端口
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="port">端口</param>
        /// <param name="protocol">协议(TCP、UDP)</param>
        public static void NetFwAddPorts(string name, int port, string protocol)
        {
            try
            {
                //创建firewall管理类的实例
                INetFwMgr netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                INetFwOpenPort objPort = (INetFwOpenPort)Activator.CreateInstance(
                    Type.GetTypeFromProgID("HNetCfg.FwOpenPort"));
                objPort.Name = name;
                objPort.Port = port;
                if (protocol.ToUpper() == "TCP")
                {
                    objPort.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                }
                else
                {
                    objPort.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
                }
                objPort.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                objPort.Enabled = true;
                bool exist = false;
                foreach (INetFwOpenPort mPort in netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts)
                {
                    if (objPort == mPort)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist) netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Add(objPort);
            }
            catch { }
        }
        /// <summary>
        /// 将应用程序添加到防火墙例外
        /// </summary>
        /// <param name="name">应用程序名称</param>
        /// <param name="executablePath">应用程序可执行文件全路径</param>
        public static void NetFwAddApps(string name, string executablePath)
        {
            try
            {
                //创建firewall管理类的实例
                INetFwMgr netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                INetFwAuthorizedApplication app = (INetFwAuthorizedApplication)Activator.CreateInstance(
                    Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication"));
                //在例外列表里，程序显示的名称
                app.Name = name;
                //程序的路径及文件名
                app.ProcessImageFileName = executablePath;
                //是否启用该规则
                app.Enabled = true;
                //加入到防火墙的管理策略
                netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(app);
                bool exist = false;
                //加入到防火墙的管理策略
                foreach (INetFwAuthorizedApplication mApp in netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications)
                {
                    if (app == mApp)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist) netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(app);
            }
            catch { }
        }
        /// <summary>
        /// 删除防火墙例外端口
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="protocol">协议（TCP、UDP）</param>
        public static void NetFwDelApps(int port, string protocol)
        {
            try
            {
                INetFwMgr netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                if (protocol == "TCP")
                    netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Remove(port, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
                else
                    netFwMgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Remove(port, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP);
            }
            catch { }
        }
        /// <summary>
        /// 删除防火墙例外中应用程序
        /// </summary>
        /// <param name="executablePath">程序的绝对路径</param>
        public static void NetFwDelApps(string executablePath)
        {
            try
            {
                INetFwMgr netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Remove(executablePath);
            }
            catch { }
        }

        /// <summary>
        /// 是否是管理员身份运行
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}