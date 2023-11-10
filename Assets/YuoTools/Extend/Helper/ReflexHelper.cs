using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YuoTools.Extend.Helper
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public static class ReflexHelper
    {
        /// <summary>
        /// 调用对象的方法
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回值</returns>
        public static object InvokeMethod<T>(T obj, string methodName, params object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);
            if (method != null) return method.Invoke(obj, parameters);
            return null;
        }

        /// <summary>
        /// 判断是否存在某个方法
        ///  </summary>
        public static bool IsExistMethod<T>(T obj, string methodName)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);
            if (method != null) return true;
            return false;
        }

        /// <summary>
        /// 获取以某个前缀开头的方法
        ///  </summary>
        public static MethodInfo[] GetMethodByPrefix<T>(T obj, string prefix)
        {
            Type type = obj.GetType();
            MethodInfo[] methods = type.GetMethods();
            List<MethodInfo> list = new List<MethodInfo>();
            foreach (MethodInfo method in methods)
            {
                if (method.Name.StartsWith(prefix))
                {
                    list.Add(method);
                }
            }

            return list.ToArray();
        }

        public static void InvokeMethodByPrefix<T>(T obj, string prefix, params object[] parameters)
        {
            MethodInfo[] methods = GetMethodByPrefix(obj, prefix);
            foreach (MethodInfo method in methods)
            {
                method.Invoke(obj, parameters);
            }
        }

        /// <summary>
        /// 获取私有字段的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPrivateField<T>(object instance, string fieldName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo info = type.GetField(fieldName, flags);
            return (T)info.GetValue(instance);
        }

        /// <summary>
        /// 设置私有字段的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void SetPrivateField(object instance, string fieldName, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo info = type.GetField(fieldName, flags);
            info.SetValue(instance, value);
        }

        /// <summary>
        /// 获取私有属性的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPrivateProperty<T>(object instance, string propertyName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo info = type.GetProperty(propertyName, flags);
            return (T)info.GetValue(instance, null);
        }

        /// <summary>
        /// 设置私有属性的值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetPrivateProperty<T>(object instance, string propertyName, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo info = type.GetProperty(propertyName, flags);
            info.SetValue(instance, value, null);
        }

        /// <summary>
        /// 调用私有方法
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object CallPrivateMethod(object instance, string methodName, params object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            MethodInfo info = type.GetMethod(methodName, flags);
            return info.Invoke(instance, param);
        }

        /// <summary>
        ///  获取某个类的所有子类,仅限于一层继承
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static List<Type> GetTypesOfBase(Type baseType)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type type in assemblyTypes)
                {
                    if (type.BaseType == baseType)
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }

        public static List<Type> GetTypesOfBase<T>()
        {
            Type baseType = typeof(T);
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type type in assemblyTypes)
                {
                    if (type.BaseType == baseType)
                    {
                        types.Add(type);
                    }
                }
            }

            return types;
        }

        public static List<T> GetInstanceOfBase<T>()
        {
            Type baseType = typeof(T);
            List<T> types = new List<T>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type type in assemblyTypes)
                {
                    if (type.BaseType == baseType)
                    {
                        types.Add((T)Activator.CreateInstance(type));
                    }
                }
            }

            return types;
        }

        public static Dictionary<Type, List<object>> GetInstanceOfBase(params Type[] types)
        {
            var typeDic = new Dictionary<Type, List<object>>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (var type in assemblyTypes)
                {
                    foreach (var baseType in types)
                    {
                        if (baseType == null || type.BaseType != baseType) continue;
                        if (!typeDic.ContainsKey(baseType))
                        {
                            typeDic.Add(baseType, new List<object>());
                        }

                        typeDic[baseType].Add(Activator.CreateInstance(type));
                    }
                }
            }

            return typeDic;
        }

        /// <summary>
        ///  Debug.Log 该变量的所有字段和属性,并将其转成json格式
        /// </summary>
        /// <param name="obj"></param>
        public static void LogAll(object obj)
        {
            GetAll(obj).Log();
        }

        public static string GetAll(object obj)
        {
            Type type = obj.GetType();
            string result = "";
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    object value = field.GetValue(obj);
                    //结果=访问权限修饰符+字段类型+字段名+字段值
                    result += $"{field.Name} : {value}\n";
                }
                catch
                {
                    //Ignore
                }
            }

            PropertyInfo[] properties =
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    object value = property.GetValue(obj);
                    result += $"{property.Name} : {value}\n";
                }
                catch
                {
                    //Ignore
                }
            }

            return result;
        }
    }

    public static class ReflexExtension
    {
        public static IEnumerable<T> GetAttributes<T>(
            this ICustomAttributeProvider member,
            bool inherit)
            where T : Attribute
        {
            try
            {
                return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
            }
            catch
            {
                return (IEnumerable<T>)new T[0];
            }
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider member, bool inherit) where T : Attribute
        {
            T[] array = member.GetAttributes<T>(inherit).ToArray();
            return array.Length != 0 ? array[0] : default;
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : Attribute =>
            member.GetAttribute<T>(false);
    }
}