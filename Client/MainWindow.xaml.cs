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
using SocketAsyncOfClientLibrary;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SocketAsyncOfClient client = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            string ip = tbId.Text;
            int port = int.Parse(tbPort.Text);
            Task.Factory.StartNew(() =>
            {
                if (client == null)
                {
                    client = new SocketAsyncOfClient(ip, port);
                    client.AutoReconnect = true;
                    client.Interval = 1000;
                    client.Connected += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += "已连接服务器！" + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    client.Received += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += "已接收：" + Encoding.UTF8.GetString(ex.Buffer) + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    client.ExceptionOccured += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += "发生异常, 已断开连接：" + ex.Message + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    client.Closed += (s, ex) =>
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            tbMsg.Text += "已关闭连接！" + "\r\n";
                            tbMsg.ScrollToEnd();
                        });
                    };
                    client.Start();
                }
            });
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Send(Encoding.UTF8.GetBytes(tbSend.Text));
            }
        }
    }
}

