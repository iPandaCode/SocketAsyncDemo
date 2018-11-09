using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SocketAsyncOfServerLibrary;

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SocketAsyncOfServer Server = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string ip = tbId.Text;
            int port = int.Parse(tbPort.Text);
            Task.Factory.StartNew(() =>
            {
                if (Server == null)
                {
                    Server = new SocketAsyncOfServer(ip, port);
                    Server.AutoRestart = true;
                    Server.Interval = 1000;
                    Server.ExceptionOccured += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += $"Socket服务器启动失败：{ex.Message}" + "\r\n";
                            tbMsg.ScrollToEnd();
                            Server = null;
                        });
                    };
                    Server.Started += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += "Socket服务器已开启！" + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    Server.Closed += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += "Socket服务器已关闭！" + "\r\n";
                            tbMsg.ScrollToEnd();
                            Server = null;
                        });
                    };
                    Server.ConnectedToRemoteEndPoint += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += $"已连接客户端{ex.SocketUserToken.RemoteEndPoint}!" + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    Server.ReceivedFromRemoteEndPoint += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += $"已接收来自客户端{ex.SocketUserToken.RemoteEndPoint}的消息：" + Encoding.UTF8.GetString(ex.Buffer) + "\r\n";
                            tbMsg.ScrollToEnd();
                            Server.SendToRemoteEndPoint((cbox.IsChecked == true ? Server.Pool : new SocketUserTokenPool() { ex.SocketUserToken }), ex.Buffer);
                        });
                    };
                    Server.ExceptionOccuredOfRemoteEndPoint += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += $"客户端{ex.SocketUserToken.RemoteEndPoint}通讯异常，已断开连接" + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    Server.Start();
                }
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Server != null)
            {
                Server.Close();
            }
        }
    }
}
