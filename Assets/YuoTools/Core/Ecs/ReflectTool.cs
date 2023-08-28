using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YuoTools.Main.Ecs
{
    /// <summary>
    /// 属性类，仿造反射中的PropertyInfo
    /// </summary>
    public class YuoProperty
    {
        private readonly PropertyGetter _getter;
        private readonly PropertySetter _setter;
        public String Name { get; }
        public PropertyInfo Info { get; }

        public YuoProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new NullReferenceException("属性不能为空");
            this.Name = propertyInfo.Name;
            this.Info = propertyInfo;
            if (this.Info.CanRead)
            {
                this._getter = new PropertyGetter(propertyInfo);
            }

            if (this.Info.CanWrite)
            {
                this._setter = new PropertySetter(propertyInfo);
            }
        }

        /// <summary>
        /// 获取对象的值
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Object GetValue(Object instance)
        {
            return _getter?.Invoke(instance);
        }

        public T GetValue<T>(Object instance)
        {
            return (T) GetValue(instance);
        }

        /// <summary>
        /// 赋值操作
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public void SetValue(Object instance, Object value)
        {
            this._setter?.Invoke(instance, value);
        }

        public void SetValue<T>(Object instance, T value)
        {
            this.SetValue(instance, (Object) value);
        }

        private static readonly ConcurrentDictionary<Type, YuoProperty[]> SecurityCache = new();

        public static YuoProperty[] GetProperties(Type type)
        {
            return SecurityCache.GetOrAdd(type, t => t.GetProperties().Select(p => new YuoProperty(p)).ToArray());
        }

        /// <summary>
        /// 属性Get操作类
        /// </summary>
        private class PropertyGetter
        {
            private readonly Func<Object, Object> _funcGet;

            public PropertyGetter(PropertyInfo propertyInfo) : this(propertyInfo?.DeclaringType, propertyInfo?.Name)
            {
            }

            public PropertyGetter(Type declareType, String propertyName)
            {
                if (declareType == null)
                {
                    throw new ArgumentNullException(nameof(declareType));
                }

                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }


                this._funcGet = CreateGetValueDeleagte(declareType, propertyName);
            }


            //代码核心部分
            private static Func<Object, Object> CreateGetValueDeleagte(Type declareType, String propertyName)
            {
                // (object instance) => (object)((declaringType)instance).propertyName

                var paramInstance = Expression.Parameter(typeof(Object));
                var bodyObjToType = Expression.Convert(paramInstance, declareType);
                var bodyGetTypeProperty = Expression.Property(bodyObjToType, propertyName);
                var bodyReturn = Expression.Convert(bodyGetTypeProperty, typeof(Object));
                return Expression.Lambda<Func<Object, Object>>(bodyReturn, paramInstance).Compile();
            }

            public Object Invoke(Object instance)
            {
                return this._funcGet?.Invoke(instance);
            }
        }

        private class PropertySetter
        {
            private readonly Action<Object, Object> setFunc;

            public PropertySetter(PropertyInfo property)
            {
                if (property == null)

                {
                    throw new ArgumentNullException(nameof(property));
                }

                this.setFunc = CreateSetValueDelagate(property);
            }

            private static Action<Object, Object> CreateSetValueDelagate(PropertyInfo property)
            {
                //声明方法需要的参数
                var paramInstance = Expression.Parameter(typeof(Object));
                var paramValue = Expression.Parameter(typeof(Object));

                var bodyInstance = Expression.Convert(paramInstance, property.DeclaringType);
                var bodyValue = Expression.Convert(paramValue, property.PropertyType);
                var bodyCall = Expression.Call(bodyInstance, property.GetSetMethod(), bodyValue);

                return Expression.Lambda<Action<Object, Object>>(bodyCall, paramInstance, paramValue).Compile();
            }

            public void Invoke(Object instance, Object value)
            {
                this.setFunc?.Invoke(instance, value);
            }
        }
    }
    
    
}

