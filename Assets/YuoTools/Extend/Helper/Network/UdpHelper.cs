using System;
using System.Net.Sockets;
using UnityEngine;

namespace YuoTools.Extend.Helper.Network
{
    public class UdpHelper
    {
        public static UdpClient UDPClient { get; private set; }

        public static UdpClient UDPServer { get; private set; }

        public static bool IsInit => UDPClient != null;

        public static void InitClient(string ip, int port)
        {
            UDPClient = new UdpClient(ip, port);
        }

        public static void Send(byte[] data)
        {
            if (!IsInit || data == null)
            {
                Debug.LogError("UDPClient is null or data is null");
                return;
            }

            UDPClient.Send(data, data.Length);
        }

        public static void Send(string data)
        {
            if (!IsInit || data == null)
            {
                Debug.LogError("UDPClient is null or data is null");
                return;
            }

            UDPClient.Send(System.Text.Encoding.UTF8.GetBytes(data), data.Length);
        }

        public static void Bind(string ip, int port)
        {
            if (UDPServer == null)
            {
                UDPServer = new UdpClient();
            }
            else if (UDPServer.Client.IsBound)
            {
                UDPServer.Close();
                UDPServer = new UdpClient();
            }

            UDPServer.Client.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
            UDPServer.BeginReceive(Receive, null);
        }

        private static void Receive(IAsyncResult ar)
        {
            var ip = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
            var data = UDPServer.EndReceive(ar, ref ip);
            OnReceive?.Invoke(System.Text.Encoding.UTF8.GetString(data));
            UDPServer.BeginReceive(Receive, null);
        }

        public static Action<string> OnReceive;
        
        public static void Close()
        {
            UDPClient?.Close();
            UDPServer?.Close();
        }
    }
}