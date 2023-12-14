// Decompiled with JetBrains decompiler
// Type: NF.Collections.Generic.PriorityQueueBinaryHeap`1
// Assembly: NF.AI.PathFinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37055A0D-66F8-4AD6-857F-C2DE2FA44746
// Assembly location: C:\Project\YuoHiraTextGame\Assets\Scripts\Jps+\NF.AI.PathFinding.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace NF.Collections.Generic
{
  public class PriorityQueueBinaryHeap<T> where T : IComparable<T>
  {
    private List<T> mHeap = new List<T>();

    public void Push(T t)
    {
      this.mHeap.Add(t);
      int index1;
      for (int index2 = this.mHeap.Count - 1; index2 > 0; index2 = index1)
      {
        index1 = (index2 - 1) / 2;
        if (this.mHeap[index2].CompareTo(this.mHeap[index1]) < 0)
          break;
        T obj = this.mHeap[index2];
        this.mHeap[index2] = this.mHeap[index1];
        this.mHeap[index1] = obj;
      }
    }

    public T Pop()
    {
      T obj1 = this.mHeap[0];
      int index1 = this.mHeap.Count - 1;
      this.mHeap[0] = this.mHeap[index1];
      this.mHeap.RemoveAt(index1);
      int num1 = index1 - 1;
      int index2 = 0;
      while (true)
      {
        int index3 = 2 * index2 + 1;
        int index4 = 2 * index2 + 2;
        int index5 = index2;
        T obj2;
        int num2;
        if (index3 <= num1)
        {
          obj2 = this.mHeap[index5];
          num2 = obj2.CompareTo(this.mHeap[index3]) < 0 ? 1 : 0;
        }
        else
          num2 = 0;
        if (num2 != 0)
          index5 = index3;
        int num3;
        if (index4 <= num1)
        {
          obj2 = this.mHeap[index5];
          num3 = obj2.CompareTo(this.mHeap[index4]) < 0 ? 1 : 0;
        }
        else
          num3 = 0;
        if (num3 != 0)
          index5 = index4;
        if (index5 != index2)
        {
          T obj3 = this.mHeap[index2];
          this.mHeap[index2] = this.mHeap[index5];
          this.mHeap[index5] = obj3;
          index2 = index5;
        }
        else
          break;
      }
      return obj1;
    }

    public int Count => this.mHeap.Count;
  }
}
