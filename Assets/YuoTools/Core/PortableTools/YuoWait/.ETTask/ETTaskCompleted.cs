using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace YuoTools
{
#pragma warning disable CS0436 // 类型与导入类型冲突

    [AsyncMethodBuilder(typeof(AsyncETTaskCompletedMethodBuilder))]
    public struct ETTaskCompleted : ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public ETTaskCompleted GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public bool IsCompleted => true;

        [DebuggerHidden]
        public void GetResult()
        {
        }

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