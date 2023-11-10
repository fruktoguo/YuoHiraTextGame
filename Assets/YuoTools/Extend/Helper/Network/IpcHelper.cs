using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace YuoTools.Extend.Helper
{
    public enum IpcMethod
    {
        NamedPipes,
        Sockets,
        SharedMemory
    }

    public static class IpcHelper
    {
        public static string IpcName { get; set; } = "YuoHiraIpc";
        public static IpcMethod CurrentMethod { get; set; } = IpcMethod.Sockets;

        public static event Action<string> MessageReceived;

        public static void Send(string message)
        {
            switch (CurrentMethod)
            {
                case IpcMethod.NamedPipes:
                    NamedPipesSend(message);
                    break;
                case IpcMethod.Sockets:
                    UdpSend(message);
                    break;
                case IpcMethod.SharedMemory:
                    SharedMemorySend(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async void NamedPipesSend(string message)
        {
            await using NamedPipeServerStream pipeServer = new NamedPipeServerStream(IpcName, PipeDirection.Out);
            // 等待客户端连接
            await pipeServer.WaitForConnectionAsync();
            // 写入数据
            await using StreamWriter sw = new StreamWriter(pipeServer);
            await sw.WriteLineAsync(message);
        }

        private static async void UdpSend(string message)
        {
            UdpClient udpClient = new UdpClient();
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(messageBytes, messageBytes.Length, new IPEndPoint(ipAddress, port));
        }

        private static void SharedMemorySend(string message)
        {
            using MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(IpcName, 1024);
            using MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            accessor.WriteArray(0, messageBytes, 0, messageBytes.Length);
        }

        public static bool IsStop = false;

        public static async void Listen()
        {
            IsStop = false;
            switch (CurrentMethod)
            {
                case IpcMethod.NamedPipes:
                    NamedPipesReceive();
                    break;
                case IpcMethod.Sockets:
                    UdpReceive();
                    break;
                case IpcMethod.SharedMemory:
                    SharedMemoryReceive();
                    break;
            }
            $"监听已成功启动,当前的IPC模式是：{CurrentMethod}".Log();
        }

        private static async void NamedPipesReceive()
        {
            while (!IsStop)
            {
                await using NamedPipeServerStream pipeServer = new NamedPipeServerStream(IpcName, PipeDirection.In);
                // 等待客户端连接
                await pipeServer.WaitForConnectionAsync();
                // 读取数据
                using StreamReader sr = new StreamReader(pipeServer);
                string message = await sr.ReadLineAsync();
                MessageReceived?.Invoke(message);
            }
        }

        public static int Port = 921;

        private static async void UdpReceive()
        {
            UdpClient udpClient = new UdpClient(Port);
            while (!IsStop)
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                MessageReceived?.Invoke(message);
            }
        }

        private static async void SharedMemoryReceive()
        {
            using MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(IpcName, 1024);
            using MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
            while (!IsStop)
            {
                byte[] messageBytes = new byte[1024];
                accessor.ReadArray(0, messageBytes, 0, messageBytes.Length);
                string message = Encoding.UTF8.GetString(messageBytes);
                MessageReceived?.Invoke(message);
                await Task.Delay(10); // 等待10毫秒
            }
        }

        public static void Destroy()
        {
            IsStop = true;
        }
    }
}