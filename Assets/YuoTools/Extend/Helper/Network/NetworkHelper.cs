using System.Net;
using System.Net.NetworkInformation;

namespace YuoTools.Extend.Helper.Network
{
    public static class NetworkHelper
    {
        public static IPAddress GetLocalAddress()
        {
            // 获取本地计算机上的所有网络接口
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // 过滤掉非活动的网络接口和回环接口
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    // 获取网络接口的IP属性
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    // 获取网络接口的所有IP地址
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        // 过滤掉IPv6地址和非本地地址
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ip.Address))
                        {
                            LocalAddress = ip.Address;
                            return ip.Address;
                        }
                    }
                }
            }

            return null;
        }

        public static IPEndPoint ToIPEndPoint(string host, int port)
        {
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static IPEndPoint ToIPEndPoint(string address)
        {
            var index = address.LastIndexOf(':');
            var host = address.Substring(0, index);
            var p = address.Substring(index + 1);
            var port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }

        public static IPAddress LocalAddress { get; private set; }
    }
}