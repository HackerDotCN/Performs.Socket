using CCWin;
using CCWin.SkinControl;
using System;
using System.Windows.Forms;
using SocketServerCommonLib;
using System.Threading;
using System.Drawing;
using System.IO;

using System.Collections.Generic;

namespace SocketServer
{
    public partial class TCPUDPServer : CCSkinMain
    {
        AsyncSocketServer TcpServer;


        /// <summary>
        /// 用户连接多少次
        /// </summary>
        int TCPUserCount = 0;
        int TCPDeviceCount = 0;
        //设备连接实时保存集合
        List<string> ConnectAry;
        public TCPUDPServer()
        {
            InitializeComponent();
            DelegateState.ServerStateInfo = ServerShowStateInfo;
            DelegateState.TeartbeatServerStateInfo = TeartbeatShowStateInfo;
            DelegateState.AddTCPuserStateInfo = AddTCPuser;
            DelegateState.AddTCPdeviceStateInfo = AddTCPdevice;
            DelegateState.ReomveTCPStateInfo = ReomveTCP;
            DelegateState.ServerConnStateInfo = ConnStateInfo;

        }

        #region  AmosLi produce <启动服务模块>

        /// <summary>
        /// 启动TCP服务
        /// </summary>
        private void btnTCP_Click(object sender, EventArgs e)
        {
            btnTCP.Enabled = false;
            if (TcpServer == null)
                TcpServer = new AsyncSocketServer();
            if (!TcpServer.IsStartListening)
            {
                TcpServer.Start();
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "TCP服务器启动" + Environment.NewLine);
                lblTCP.Text = "TCP服务器地址:" + TcpServer.serverconfig.ListenIp + ":" + TcpServer.serverconfig.ListenPort;
                PicBoxTCP.BackgroundImage = Properties.Resources._07822_48x48x8BPP_;
                btnTCP.Text = "TCP停止服务";
            }
            else
            {
                TcpServer.Stop();
                PicBoxTCP.BackgroundImage = Properties.Resources._07821_48x48x8BPP_;
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "TCP服务器停止" + Environment.NewLine);
                btnTCP.Text = "TCP服务器启动";
            }
            btnTCP.Enabled = true;
        }


        #region

        #region TCP回调函数操作
        void AddTCPuser(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                if (userToken.ConnectSocket == null)
                    return;
                tpe3list1.Refresh();
                TCPUserCount++;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = TCPUserCount.ToString();
                lvi.SubItems.Add(userToken.ConnectSocket.RemoteEndPoint.ToString());
                lvi.SubItems.Add(userToken.UserName);
                lvi.SubItems.Add(userToken.ConnectDateTime.ToString());
                lvi.SubItems.Add("TCP");
                tpe3list1.Items.Add(lvi);
            }));
        }

        /// <summary>
        /// 删除用户或者设备
        /// </summary>
        /// <param name="userToken"></param>
        void ReomveTCP(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                try
                {
                    if (userToken.isDevice)
                    {
                       
                    }
                    else
                    {
                        for (int i = 0; i < tpe3list1.Items.Count; i++)
                        {
                            if (tpe3list1.Items[i].SubItems[1].Text.Contains(userToken.RemotDeviceIp) && userToken.UserName == tpe3list1.Items[i].SubItems[2].Text)
                            {
                                tpe3list1.Items.Remove(tpe3list1.Items[i]);
                                break;
                            }
                        }
                    }
                }
                catch { }
            }));
        }

        void AddTCPdevice(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                if (userToken.ConnectSocket == null)
                    return;

           
                TCPDeviceCount++;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = TCPDeviceCount.ToString();
                lvi.SubItems.Add(userToken.ConnectSocket.RemoteEndPoint.ToString() + "-" + userToken.RemotDeviceIp);
                lvi.SubItems.Add(userToken.UserName);
                lvi.SubItems.Add(userToken.ConnectDateTime.ToString());
                lvi.SubItems.Add(userToken.firmware);
            }));
        }
        #endregion

        void ConnStateInfo(string RemoteIp, string TCPUDP)
        {
            this.Invoke(new ThreadStart(delegate
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = listAllView.Items.Count.ToString();
                lvi.SubItems.Add(RemoteIp);
                lvi.SubItems.Add(DateTime.Now.ToString());
                lvi.SubItems.Add(TCPUDP);
                listAllView.Items.Add(lvi);
            }));
        }


        /// <summary>
        /// 信息添加
        /// </summary>
        /// <param name="msg"></param>
        void ServerShowStateInfo(string msg)
        {
            this.Invoke(new ThreadStart(delegate
            {
                tpe2txtMsg.AppendText(DateTime.Now + ":" + msg + Environment.NewLine);
            }));
        }

        /// <summary>
        /// 心跳时间
        /// </summary>
        void TeartbeatShowStateInfo(int num)
        {
            this.Invoke(new ThreadStart(delegate
            {
                txtMsg.AppendText(Environment.NewLine + DateTime.Now + ":" + num + "连接检测");
                lblNum1.NormlBack = ImageListAllUpdate(num / 10 % 10);
                lblNum2.NormlBack = ImageListAllUpdate(num % 10);
                if (listAllView.Items.Count > 1000 || tpe2txtMsg.Text.Length > 10000)
                {
                    listAllView.Items.Clear();
                    tpe2txtMsg.Clear();
                    txtMsg.Clear();
                    linkOut_LinkClicked(null, null);
                }
            }));
        }
        #endregion
        /// <summary>
        /// 图片更换
        /// </summary>
        Image ImageListAllUpdate(int Num)
        {
            switch (Num)
            {
                case 0:
                    return Properties.Resources._00034_17x25x8BPP_;
                case 1:
                    return Properties.Resources._00035_17x25x8BPP_;
                case 2:
                    return Properties.Resources._00036_17x25x8BPP_;
                case 3:
                    return Properties.Resources._00037_17x25x8BPP_;
                case 4:
                    return Properties.Resources._00038_17x25x8BPP_;
                case 5:
                    return Properties.Resources._00039_17x25x8BPP_;
                case 6:
                    return Properties.Resources._00040_17x25x8BPP_;
                case 7:
                    return Properties.Resources._00041_17x25x8BPP_;
                case 8:
                    return Properties.Resources._00042_17x25x8BPP_;
                case 9:
                    return Properties.Resources._00043_17x25x8BPP_;
                default:
                    return null;
            }
        }
        #endregion
        /// <summary>
        /// 刷新设备列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkdeviceRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TcpServer == null)
                return;

          
            GC.Collect();
            GC.Collect();
        }

        private void linkuserRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TcpServer == null)
                return;

            linkuserRefresh.Enabled = false;
            GC.Collect();
            GC.Collect();
            lbluser.Text = "上次刷新是在 " + DateTime.Now.Hour + " 点，共连接 " + TcpServer.AsyncSocketUserList.Userlist.Count + "/" + TcpServer.serverconfig.numConnections + " 个端口。";
            linkuserRefresh.Enabled = true;
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        private void linkSaveMsg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter write = new StreamWriter(saveFileDialog1.FileName))
                {
                    write.WriteLine(tpe2txtMsg.Text);
                }
            }
        }

        /// <summary>
        /// 加入防火墙
        /// </summary>
        private void btnNetFw_Click(object sender, EventArgs e)
        {
            INetFwManger.NetFwAddApps("SocketServer", Application.ExecutablePath);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        private void TCPUDPServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            e.Cancel = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }
        /// <summary>
        /// 设备连接去重复
        /// </summary>
        private void linkOut_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConnectAry = new List<string>();
       
        }

    }
}
