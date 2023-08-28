using System;
using System.Threading;
namespace YuoTools
{
    public class Singleton<T> where T : new()
    {

        //默认是t类型
        private static T S_Singleton = default(T);
        //实例化一个object
        private static object S_objectlock = new object();

        public static T Instance
        {
            get
            {
                if (S_Singleton == null)
                {
                    object obj;
                    //防止在同一时间有2个线程创建实例
                    //Monitor能够对值类型进行加锁，实质上是Monitor.Enter(object)对值类型装箱
                    Monitor.Enter(obj = S_objectlock);
                    try
                    {
                        //实例化对象  基于接口的Remoting对象是不能用new来创建的，可以直接使用Activator来创建
                        S_Singleton = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
                    }
                    finally
                    {
                        Monitor.Exit(obj);
                    }
                }
                return S_Singleton;
            }
        }
        //仅限当前类和子类中访问
        protected Singleton() { }

    }
}