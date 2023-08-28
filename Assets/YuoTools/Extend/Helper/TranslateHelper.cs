using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace YuoTools.Extend.Helper
{
    public class TranslateHelper
    {
        //翻译
        public static string Translate(string text)
        {
            string url =
                "http://fanyi.youdao.com/openapi.do?keyfrom=youyao&key=123456&type=data&doctype=json&version=1.1&q=" +
                text;
            string result = HttpHelper.Get(url);
            return result;
        }

        //百度翻译
        public static async Task<BaiduTranslateResult> BaiduTranslate(string text, string from = "auto", string to = "auto")
        {
            var appid = "20200728000528239";
            var key = "K2gRwYhRVbziZ70ex82d";
            var salt = RandomHelper.RandInt32();
            var sign = (appid + text + salt + key).ConvertToMD5_32();
            string url = $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={text}&from={from}&to={to}&" +
                         $"appid={appid}&salt={salt}&sign={sign}";
            string result = await HttpHelper.GetAsync(url);
            return JsonConvert.DeserializeObject<BaiduTranslateResult>(result);
        }

        [Serializable]
        public class trans_result
        {
            [JsonProperty("src")]
            public string key;
            [JsonProperty("dst")]
            public string result;
        }

        [Serializable]
        public class BaiduTranslateResult
        {
            public string from;
            public string to;
            [JsonProperty("trans_result")]
            public List<trans_result> results;
        }
    }
}