using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace YuoTools.Chat
{
    [Serializable]
    public class MessageData
    {
        public UserData user;
        public string Message;
        public Sprite image = null;
        public Vector2 Rect;
        public Vector2 TextRect;
    }

    [System.Serializable]
    public class UserData
    {
        public Sprite Head;
        public string UserName;
    }

    public class ChatManager : SingletonMono<ChatManager>
    {
        [ShowInInspector] public static List<(MessageType messageType, string data, IPEndPoint)>
            MessageList = ChatUdpTest.MessageList;

        public YuoChatLoopList loopList;
        public Sprite[] Heads = new Sprite[10];
        public TextMeshProUGUI Check;
        public string Player;
        public List<UserData> us = new List<UserData>();
        public YuoChatLoopListItem CheckItem;

        private void Start()
        {
            //添加ui事件
            TMPText.onSubmit.AddListener(x => YuoSendMessage());
            send.onClick.AddListener(YuoSendMessage);
        }

        private void Update()
        {
            if (ChatUdpTest.MessageList.Count > 0)
            {
                //copy一份数据
                var data = MessageList.ToArray();
                foreach ((MessageType messageType, string data, IPEndPoint ipEndPoint) message in data)
                {
                    try
                    {
                        ReceiveMessage(message.messageType, message.data, message.ipEndPoint);
                        Debug.Log($"收到消息:{message.messageType} {message.data} {message.ipEndPoint}");
                    }
                    catch (Exception e)
                    {
                        e.LogError();
                    }
                }

                MessageList.Clear();
            }
        }

        private void ReceiveMessage(MessageType messageType, string data, IPEndPoint ipEnd)
        {
            switch (messageType)
            {
                case MessageType.Connect:
                    break;
                case MessageType.Disconnect:
                    break;
                case MessageType.GetDeviceList:
                    break;
                case MessageType.CallServer:
                    break;
                case MessageType.Call or MessageType.CallAll:
                    var jsonNode = JSONNode.Parse(data);
                    if (jsonNode == null) break;
                    Send(jsonNode["DeviceMessage"]?.Value, GetOrCreateUser(jsonNode["DeviceName"]?.Value));
                    break;
                case MessageType.GetAllMessage:
                    //用jsonNode解析数据
                    var jsonArray = JSONNode.Parse(data) as JSONArray;
                    if (jsonArray != null)
                        foreach (var node in jsonArray.Values)
                        {
                            Send(node["DeviceMessage"]?.Value, GetOrCreateUser(node["DeviceName"]?.Value));
                        }

                    break;
                case MessageType.SystemMessage:
                    Send(data, GetOrCreateUser("系统"));
                    break;
                default:
                    break;
            }
        }

        UserData GetOrCreateUser(string name)
        {
            foreach (var item in us)
            {
                if (item.UserName == name)
                {
                    return item;
                }
            }

            UserData user = new UserData();
            user.UserName = name;
            user.Head = Heads[0];
            us.Add(user);
            return user;
        }

        public Button send;
        public TMP_InputField TMPText;

        public float bubbleOffset = 30;

        public UnityAction<string> OnSendMessage;

        public void YuoSendMessage()
        {
            OnSendMessage?.Invoke(TMPText.text);
            Send(TMPText.text, GetOrCreateUser(PlayerPrefs.GetString("UserName")));
            ChatUdpTest.SendToServer(MessageType.CallServer, ChatUdpTest.StringToMessage(TMPText.text));
            TMPText.text = null;
        }

        public float maxMessageWidth = 800;

        public void Send(string message, UserData user)
        {
            Check.gameObject.Show();
            MessageData data = new MessageData();
            data.user = user;
            data.Message = message;
            Check.text = message;
            Temp.V2.x = LayoutUtility.GetPreferredWidth(Check.rectTransform).RClamp(maxMessageWidth);
            Check.rectTransform.sizeDelta = Temp.V2;
            Temp.V2.y = LayoutUtility.GetPreferredHeight(Check.rectTransform);
            Check.rectTransform.sizeDelta = Temp.V2;
            data.TextRect = Temp.V2;
            if (data.image && CheckItem.image.rectTransform.sizeDelta.y > Temp.V2.y)
            {
                Temp.V2.y = CheckItem.image.rectTransform.sizeDelta.y;
            }

            if (Temp.V2.y > 60)
                Temp.V2.SetY(40 + Temp.V2.y);
            else
                Temp.V2.y = 100;

            data.Rect = Temp.V2;
            data.Rect.x += 100 + bubbleOffset / 2;
            data.Rect.y += bubbleOffset;
            Check.gameObject.Hide();
            loopList.AddDataOnEnd(data, data.Rect);
            loopList.LocateRenderItemAtTarget(data, 0.3f);
        }
    }

    public class ChatUdpTest
    {
        static string serverIp = "43.138.44.246";
        const int UDPPort = 9211;

        static Socket client;
        static IPEndPoint serverEndpoint;

        public static bool IsConnected => client is { Connected: true };

        public static void StartClient()
        {
            serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), UDPPort);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //建立与服务器的远程连接
            client.Connect(serverEndpoint);
            Thread thread = new Thread(ReceiveMessage)
            {
                IsBackground = true
            };
            thread.Start(client);
            Debug.Log("客户端已成功开启!");
        }

        static void ReceiveMessage(object obj)
        {
            Socket socket = obj as Socket;
            while (true)
            {
                try
                {
                    //用来保存发送方的ip和端口号
                    EndPoint point = new IPEndPoint(IPAddress.Any, UDPPort);
                    //定义客户端接收到的信息大小
                    byte[] buffer = new byte[1024 * 1024];
                    //接收到的信息大小(所占字节数)
                    if (socket != null)
                    {
                        int length = socket.ReceiveFrom(buffer, ref point);
                        //定义数据标头
                        var header = new byte[4];
                        //将数据标头复制到header中
                        Buffer.BlockCopy(buffer, 0, header, 0, 4);
                        MessageType type = (MessageType)int.Parse(header[0].ToString());
                        //定义数据体
                        var body = new byte[length - 4];
                        //将数据体复制到body中
                        Buffer.BlockCopy(buffer, 4, body, 0, body.Length);
                        //后面是消息内容
                        string message = Encoding.UTF8.GetString(body);
                        MessageList.Add((type, message, point as IPEndPoint));
                    }

                    // Debug.Log($"收到来自服务器的{type}类型消息:\n{message}");
                }
                catch (Exception exception)
                {
                    Debug.Log("出现错误,接收已关闭");
                    Debug.Log(exception);
                    if (socket != null) socket.Close();
                    return;
                }
            }
        }

        public static void Send(MessageType type, string data, IPEndPoint ip)
        {
            data = $"{(int)type:0000}{data}";
            client.SendTo(Encoding.UTF8.GetBytes(data), ip);
        }

        public static void SendToServer(MessageType type, string data)
        {
            Send(type, data, serverEndpoint);
        }

        public static string StringToMessage(string message)
        {
            var node = new JSONObject
            {
                ["message"] = message
            };
            return node.ToString();
        }

        //储存子线程的消息,先入先出
        public static List<(MessageType messageType, string data, IPEndPoint)> MessageList = new();
    }

    public enum MessageType
    {
        Connect = 0,
        Disconnect = 1,
        Call = 2,
        CallAll = 3,
        GetDeviceList = 4,
        CallServer = 5,
        GetAllMessage = 6,
        SystemMessage = 7
    }
}