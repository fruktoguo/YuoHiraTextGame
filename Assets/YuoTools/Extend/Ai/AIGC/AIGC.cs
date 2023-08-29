using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace YuoTools.Extend.Ai.AIGC
{
    public class AIGC
    {
        public static async Task<Texture2D> Draw(Text2ImgRequestDto data)
        {
            Encoding myEncoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:7860/sdapi/v1/txt2img");
            request.Method = "POST";
            request.ContentType = "application/json";
            var param = JsonUtility.ToJson(data);
            if (!string.IsNullOrWhiteSpace(param))
            {
                byte[] byteArray = myEncoding.GetBytes(param); //转化
                request.ContentLength = byteArray.Length;
                Stream newStream = request.GetRequestStream();
                await newStream.WriteAsync(byteArray, 0, byteArray.Length); //写入参数
                newStream.Close();
            }

            using WebResponse wr = await request.GetResponseAsync();
            //在这里对接收到的页面内容进行处理
            string response = await new StreamReader(wr.GetResponseStream(), myEncoding).ReadToEndAsync();
            if (!string.IsNullOrWhiteSpace(response))
            {
                Text2ImgResponseDto result = JsonConvert.DeserializeObject<Text2ImgResponseDto>(response);
                var bytes = Convert.FromBase64String(result.images[0]);
                var texture = new Texture2D((int)data.width, (int)data.height);
                texture.LoadImage(bytes);
                texture.Apply();
                return texture;
            }

            return null;
        }

        public static async Task<Texture2D> Draw(string parameters,string negative_prompt = "") =>
            await Draw(new Text2ImgRequestDto
            {
                prompt = parameters,
                negative_prompt = negative_prompt
            });
    }

    /// <summary>
    ///  Txt2Img 文生图 请求实体类
    /// </summary>
    public partial class Text2ImgRequestDto
    {
        /// <summary>
        /// 高度（不适用这个去设置，容易导致图片生成重复的内容，因sd的模型，一般都是基于512去训练的）
        /// </summary>
        public long? height { get; set; } = 512;

        /// <summary>
        /// 宽度（不适用这个去设置，容易导致图片生成重复的内容，因sd的模型，一般都是基于512去训练的）
        /// </summary>
        public long? width { get; set; } = 512;

        /// <summary>
        /// 反向提示词
        /// </summary>
        public string? negative_prompt { get; set; }

        /// <summary>
        /// 正向提示词
        /// </summary>
        public string prompt { get; set; }

        /// <summary>
        /// 面部修复（画人像的时候可以考虑使用）
        /// </summary>
        public bool? restore_faces { get; set; } = false;

        /// <summary>
        /// 总批次数
        /// </summary>  
        public long? n_iter { get; set; } = 1;

        /// <summary>
        /// 单批数量（每次生成的图片数量）
        /// </summary>
        public long? batch_size { get; set; } = 4;

        /// <summary>
        /// Sampler 采样方法，默认Euler
        /// </summary>
        public string sampler_index { get; set; } = "Euler a";

        /// <summary>
        /// 随机种子数，默认为-1
        /// </summary>
        public long? seed { get; set; } = -1;

        /// <summary>
        /// 生成步数，默认20
        /// </summary>
        public long? steps { get; set; } = 20;


        /// <summary>
        /// 平铺
        /// </summary>
        public bool? tiling { get; set; } = false;

        public double? cfg_scale { get; set; } = 7;
    }

    /// <summary>
    /// 文生图 响应体类
    /// </summary>
    public class Text2ImgResponseDto
    {
        /// <summary>
        /// 生成的图片数组
        /// </summary>
        public List<string> images = new List<string>();

        /// <summary>
        /// request请求中的body
        /// </summary>
        public Text2ImgRequestDto parameters = new Text2ImgRequestDto();

        /// <summary>
        /// 返回的图片数组生成参数信息
        /// </summary>
        public string? info { get; set; }
    }
}