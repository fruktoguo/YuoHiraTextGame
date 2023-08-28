using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace YuoTools
{
    public class YuoValue
    {
        [ReadOnly] [ShowInInspector] public double Value { get; private set; }

        private double baseValue;

        public YuoValue(double value)
        {
            baseValue = value;
            UpdateValue();
        }

        public YuoValue(float value)
        {
            baseValue = value;
            UpdateValue();
        }


        /// <summary>
        ///  基础值
        /// </summary>
        [HorizontalGroup()]
        [ShowInInspector]
        public double BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                UpdateValue();
            }
        }

        /// <summary>
        ///  额外值
        /// </summary>
        [ShowInInspector]
        [HorizontalGroup()]
        [ReadOnly]
        public double AdditionalValues { get; private set; }


        /// <summary>
        ///  额外值
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public double AddValue
        {
            get
            {
                double value = 0;
                foreach (var v in addValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        /// <summary>
        ///  额外值乘数
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public double MulAddValue
        {
            get
            {
                double value = 1;
                foreach (var v in mulAddValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        /// <summary>
        ///  基础值乘数
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public double MulBaseValue
        {
            get
            {
                double value = 1;
                foreach (var v in mulBaseValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        /// <summary>
        ///  最终值乘数
        /// </summary>
        [FoldoutGroup("详细信息")]
        [ShowInInspector]
        public double MulValue
        {
            get
            {
                double value = 1;
                foreach (var v in mulValue.Values)
                {
                    value += v;
                }

                return value;
            }
        }

        private Dictionary<int, DoubleAction<YuoValue>> valueChange = new();

        public void AddValueChangeAction(int id, DoubleAction<YuoValue> action)
        {
            valueChange[id] = action;

            UpdateValue();
        }

        public void RemoveValueChangeAction(int id)
        {
            if (valueChange.ContainsKey(id))
            {
                valueChange.Remove(id);
            }
        }

        /// <summary>
        ///  当数值改变时调用,第一个参数为改变前的值,第二个参数为改变后的值
        /// </summary>
        public Action<double, double> OnValueChange;

        public double RatioValue { get; private set; }

        [FoldoutGroup("详细信息2")] [ShowInInspector]
        private Dictionary<int, double> addValue = new();

        /// <summary>
        ///  添加额外值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void AddAddValue(int id, double value)
        {
            addValue[id] = value;

            UpdateValue();
        }

        /// <summary>
        ///  移除额外值
        /// </summary>
        /// <param name="id"></param>
        public void RemoveAddValue(int id)
        {
            if (addValue.ContainsKey(id))
            {
                addValue.Remove(id);
                UpdateValue();
            }
        }

        [FoldoutGroup("详细信息2")] [ShowInInspector]
        private Dictionary<int, double> mulAddValue = new();

        /// <summary>
        ///  添加额外值乘数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void AddMulAddValue(int id, double value)
        {
            mulAddValue[id] = value;

            UpdateValue();
        }

        /// <summary>
        ///  移除额外值乘数
        /// </summary>
        /// <param name="id"></param>
        public void RemoveMulAddValue(int id)
        {
            if (mulAddValue.ContainsKey(id))
            {
                mulAddValue.Remove(id);
            }

            UpdateValue();
        }

        [FoldoutGroup("详细信息2")] [ShowInInspector]
        private Dictionary<int, double> mulBaseValue = new();

        /// <summary>
        ///  添加基础值乘数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void AddMulBaseValue(int id, double value)
        {
            mulBaseValue[id] = value;

            UpdateValue();
        }

        /// <summary>
        ///  移除基础值乘数
        /// </summary>
        /// <param name="id"></param>
        public void RemoveMulBaseValue(int id)
        {
            if (mulBaseValue.ContainsKey(id))
            {
                mulBaseValue.Remove(id);
            }

            UpdateValue();
        }

        [FoldoutGroup("详细信息2")] [ShowInInspector]
        private Dictionary<int, double> mulValue = new();

        /// <summary>
        ///  添加最终值乘数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void AddMulValue(int id, double value)
        {
            mulValue[id] = value;

            UpdateValue();
        }

        /// <summary>
        ///  移除最终值乘数
        /// </summary>
        /// <param name="id"></param>
        public void RemoveMulValue(int id)
        {
            if (mulValue.ContainsKey(id))
            {
                mulValue.Remove(id);
            }

            UpdateValue();
        }

        public void UpdateValue()
        {
            var oldValue = Value;
            Value = (baseValue * MulBaseValue + AddValue * MulAddValue) * MulValue;
            foreach (var action in valueChange.Values)
            {
                Value += action(this);
            }

            AdditionalValues = Value - baseValue;

            RatioValue = ValueToPercent(Value, RatioDivisor);

            OnValueChange?.Invoke(oldValue, Value);
        }

        public double RatioDivisor = 100;

        double ValueToPercent(double value, double divisor = 100)
        {
            if (value >= 0)
                return 1 - value / (value + divisor);
            value = -value;
            return -(1 - value / (value + divisor));
        }

        #region operator

        public static implicit operator double(YuoValue value) => value.Value;

        public static implicit operator float(YuoValue value) => (float)value.Value;

        public static implicit operator YuoValue(int value) => new(value);

        public static implicit operator YuoValue(double value) => new(value);

        public static implicit operator YuoValue(float value) => new(value);

        public static double operator +(YuoValue a, YuoValue b)
        {
            return a.Value + b.Value;
        }

        public static double operator -(YuoValue a, YuoValue b)
        {
            return a.Value - b.Value;
        }

        public static double operator *(YuoValue a, YuoValue b)
        {
            return a.Value * b.Value;
        }

        public static double operator /(YuoValue a, YuoValue b)
        {
            return a.Value / b.Value;
        }

        public static bool operator >(YuoValue a, YuoValue b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(YuoValue a, YuoValue b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(YuoValue a, YuoValue b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(YuoValue a, YuoValue b)
        {
            return a.Value <= b.Value;
        }

        #endregion

        #region operator double

        public static double operator +(YuoValue a, double b)
        {
            return a.Value + b;
        }

        public static double operator -(YuoValue a, double b)
        {
            return a.Value - b;
        }

        public static double operator *(YuoValue a, double b)
        {
            return a.Value * b;
        }

        public static double operator /(YuoValue a, double b)
        {
            return a.Value / b;
        }

        public static double operator +(double a, YuoValue b)
        {
            return a + b.Value;
        }

        public static double operator -(double a, YuoValue b)
        {
            return a - b.Value;
        }

        public static double operator *(double a, YuoValue b)
        {
            return a * b.Value;
        }

        public static double operator /(double a, YuoValue b)
        {
            return a / b.Value;
        }

        public static bool operator >(YuoValue a, double b)
        {
            return a.Value > b;
        }

        public static bool operator <(YuoValue a, double b)
        {
            return a.Value < b;
        }

        public static bool operator >=(YuoValue a, double b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(YuoValue a, double b)
        {
            return a.Value <= b;
        }

        public static bool operator >(double a, YuoValue b)
        {
            return a > b.Value;
        }

        public static bool operator <(double a, YuoValue b)
        {
            return a < b.Value;
        }

        public static bool operator >=(double a, YuoValue b)
        {
            return a >= b.Value;
        }

        public static bool operator <=(double a, YuoValue b)
        {
            return a <= b.Value;
        }

        #endregion

        #region operator int

        public static bool operator >(int a, YuoValue b)
        {
            return a > b.Value;
        }

        public static bool operator <(int a, YuoValue b)
        {
            return a < b.Value;
        }

        public static bool operator >=(int a, YuoValue b)
        {
            return a >= b.Value;
        }

        public static bool operator <=(int a, YuoValue b)
        {
            return a <= b.Value;
        }

        public static bool operator >(YuoValue a, int b)
        {
            return a.Value > b;
        }

        public static bool operator <(YuoValue a, int b)
        {
            return a.Value < b;
        }

        public static bool operator >=(YuoValue a, int b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(YuoValue a, int b)
        {
            return a.Value <= b;
        }

        #endregion

        #region operator float

        public static bool operator >(float a, YuoValue b)
        {
            return a > b.Value;
        }

        public static bool operator <(float a, YuoValue b)
        {
            return a < b.Value;
        }

        public static bool operator >=(float a, YuoValue b)
        {
            return a >= b.Value;
        }

        public static bool operator <=(float a, YuoValue b)
        {
            return a <= b.Value;
        }

        public static bool operator >(YuoValue a, float b)
        {
            return a.Value > b;
        }

        public static bool operator <(YuoValue a, float b)
        {
            return a.Value < b;
        }

        public static bool operator >=(YuoValue a, float b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(YuoValue a, float b)
        {
            return a.Value <= b;
        }

        public static double operator +(YuoValue a, float b)
        {
            return a.Value + b;
        }

        public static double operator -(YuoValue a, float b)
        {
            return a.Value - b;
        }

        public static double operator *(YuoValue a, float b)
        {
            return a.Value * b;
        }

        public static double operator /(YuoValue a, float b)
        {
            return a.Value / b;
        }

        public static double operator +(float a, YuoValue b)
        {
            return a + b.Value;
        }

        public static double operator -(float a, YuoValue b)
        {
            return a - b.Value;
        }

        public static double operator *(float a, YuoValue b)
        {
            return a * b.Value;
        }

        public static double operator /(float a, YuoValue b)
        {
            return a / b.Value;
        }

        #endregion

        #region operator long

        public static bool operator >(long a, YuoValue b)
        {
            return a > b.Value;
        }

        public static bool operator <(long a, YuoValue b)
        {
            return a < b.Value;
        }

        public static bool operator >=(long a, YuoValue b)
        {
            return a >= b.Value;
        }

        public static bool operator <=(long a, YuoValue b)
        {
            return a <= b.Value;
        }

        public static bool operator >(YuoValue a, long b)
        {
            return a.Value > b;
        }

        public static bool operator <(YuoValue a, long b)
        {
            return a.Value < b;
        }

        public static bool operator >=(YuoValue a, long b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(YuoValue a, long b)
        {
            return a.Value <= b;
        }

        #endregion

        #region override

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            YuoValue other = (YuoValue)obj;
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("F1");
        }

        #endregion
    }
}