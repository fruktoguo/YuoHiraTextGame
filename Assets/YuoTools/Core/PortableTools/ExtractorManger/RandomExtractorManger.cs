using System.Collections.Generic;

using UnityEngine;

namespace YuoTools
{
    public class RandomExtractorManger<T> : Singleton<RandomExtractorManger<T>>
    {
        private Dictionary<string, RandomExtractor<T>> extractors = new Dictionary<string, RandomExtractor<T>>();

        /// <summary>
        /// 获取一个抽取器,如果没有则会创建一个新的
        /// </summary>
        /// <param name="extractorName"></param>
        /// <returns></returns>
        public RandomExtractor<T> GetExtractor(string extractorName)
        {
            if (!extractors.ContainsKey(extractorName))
            {
                RandomExtractor<T> ex = new RandomExtractor<T>();
                ex.Name = extractorName;
                extractors.Add(extractorName, ex);
                //$"创建抽取器 [{extractorName.LogSetColor(YuoStrColor.番茄)}] 成功".Log();
            }
            return extractors[extractorName];
        }

        public void Remove(string extractorName)
        {
            if (!extractors.ContainsKey(extractorName))
            {
                Debug.Log($"抽取器 [{extractorName}] 不存在");
            }
            else
            {
                extractors.Remove(extractorName);
                Debug.Log($"移除 抽取器 [{extractorName}] 成功");
            }
        }

        public void RemoveAll()
        {
            extractors.Clear();
        }
    }
}