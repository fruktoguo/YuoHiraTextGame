using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace YuoTools
{
#pragma warning disable CS0436 // 类型与导入类型冲突

    [AsyncMethodBuilder(typeof(AsyncETVoidMethodBuilder))]
    internal struct ETVoid : ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public void Coroutine()
        {
        }

        [DebuggerHidden]
        public bool IsCompleted => true;

        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
        }
    }
}