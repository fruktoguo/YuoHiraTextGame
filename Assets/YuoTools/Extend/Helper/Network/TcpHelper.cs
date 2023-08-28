using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace YuoTools.Extend.Helper.Network
{
    public class TcpHelper
    {
        public TcpClient Client { get; set; }
        public TcpListener Server { get; set; }

        public void ConnectToServer(string ipAddress, int port)
        {
            Client = new TcpClient();
            Client.Connect(ipAddress, port);
        }

        public void StartServer(int port)
        {
            Server = new TcpListener(NetworkHelper.GetLocalAddress(), port);
            Server.Start();
        }

        public bool IsConnected()
        {
            return Client is { Connected: true };
        }

        private Socket socketServer;

        private void StartService()
        {
            //tcp :协议端
            //实例化一个Tcp协议的socket对象
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //给服务器绑定地址与端口
            IPAddress ipaddress = IPAddress.Parse("192.168.218.12");
            IPEndPoint iPEndPoint = new IPEndPoint(ipaddress, 7999);
            //把ip与端口绑定在Socket里面
            socketServer.Bind(iPEndPoint);
            //侦听
            socketServer.Listen(100);
            Debug.Log("tcp启动");
            Task.Run(() =>
            {
                while (true) //循环等待客户端连接
                {
                    //会等待客户端的连接,会卡在这里,不往下运行,直到有下一个宽带连接才会继续往下运行
                    //当有客户端连接的时候,会返回客户端连接的对象
                    Socket socket = socketServer.Accept();
                    byte[] bytes = EncodeToBytes("你今天吃饭了吗");
                    socket.Send(bytes);
                    Task.Run(() =>
                    {
                        try
                        {
                            while (true) //循环接收数据
                            {
                                //接受数据
                                byte[] buffer = new byte[1024];
                                //返回接收到的数据大小,接收数据也会卡住,只有接收到数据的时候才会继续往下进行
                                int length = socket.Receive(buffer);
                                if (length == 0)
                                {
                                    Debug.Log("一个客户端断开连接");
                                    return;
                                }

                                //把字符转成字符串
                                string str = EncodeToString(buffer, 0, length);
                                Debug.Log(str);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("出现异常" + ex.Message);
                        }
                        finally
                        {
                            socket.Close();
                        }
                    });
                }
            });
        }

        private string EncodeToString(byte[] buffer, int offset, int length) =>
            System.Text.Encoding.Default.GetString(buffer, offset, length);

        private byte[] EncodeToBytes(string str) => System.Text.Encoding.Default.GetBytes(str);
    }
}