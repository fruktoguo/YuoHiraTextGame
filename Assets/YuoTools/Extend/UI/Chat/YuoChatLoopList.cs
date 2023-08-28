using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YuoTools.Chat
{
    /// <summary>
    /// 由于使用了泛型,所以必须要继承一下才能挂载
    /// </summary>
    public class YuoChatLoopList : YuoLoopList<MessageData>
    {
    }
}