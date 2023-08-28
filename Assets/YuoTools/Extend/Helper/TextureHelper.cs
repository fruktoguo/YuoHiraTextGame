using System;
using System.Collections;
using System.IO;
using ET;
using UnityEngine;
using UnityEngine.Networking;

namespace YuoTools.Extend.Helper
{
    public static class TextureHelper
    {
        /// <summary>
        ///  裁剪Texture2D
        /// </summary>
        /// <param name="originalTexture"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="originalWidth"></param>
        /// <param name="originalHeight"></param>
        /// <returns></returns>
        public static Texture2D ScaleTextureCutOut(Texture2D originalTexture, int offsetX, int offsetY,
            float originalWidth, float originalHeight)
        {
            var newTexture = new Texture2D(Mathf.CeilToInt(originalWidth), Mathf.CeilToInt(originalHeight));
            var maxX = originalTexture.width - 1;
            var maxY = originalTexture.height - 1;
            for (var y = 0; y < newTexture.height; y++)
            for (var x = 0; x < newTexture.width; x++)
            {
                float targetX = x + offsetX;
                float targetY = y + offsetY;
                var x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                var y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                var x2 = Mathf.Min(maxX, x1 + 1);
                var y2 = Mathf.Min(maxY, y1 + 1);

                var u = targetX - x1;
                var v = targetY - y1;
                var w1 = (1 - u) * (1 - v);
                var w2 = u * (1 - v);
                var w3 = (1 - u) * v;
                var w4 = u * v;
                var color1 = originalTexture.GetPixel(x1, y1);
                var color2 = originalTexture.GetPixel(x2, y1);
                var color3 = originalTexture.GetPixel(x1, y2);
                var color4 = originalTexture.GetPixel(x2, y2);
                var color = new Color(
                    Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                    Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                    Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                    Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                );
                newTexture.SetPixel(x, y, color);
            }

            newTexture.anisoLevel = 2;
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        ///  缩放Texture2D
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        public static Texture2D ScaleTexture(Texture2D source, float targetWidth, float targetHeight)
        {
            var result = new Texture2D((int)targetWidth, (int)targetHeight, source.format, false);

            var incX = 1.0f / targetWidth;
            var incY = 1.0f / targetHeight;

            for (var i = 0; i < result.height; ++i)
            for (var j = 0; j < result.width; ++j)
            {
                var newColor = source.GetPixelBilinear(j / (float)result.width,
                    i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }

            result.Apply();
            return result;
        }

        /// <summary>
        ///  水平翻转
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D HorizontalFlipTexture(Texture2D texture)
        {
            //得到图片的宽高
            var width = texture.width;
            var height = texture.height;

            var flipTexture = new Texture2D(width, height);

            for (var i = 0; i < width; i++)
                flipTexture.SetPixels(i, 0, 1, height, texture.GetPixels(width - i - 1, 0, 1, height));

            flipTexture.Apply();

            return flipTexture;
        }

        /// <summary>
        ///  垂直翻转
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D VerticalFlipTexture(Texture2D texture)
        {
            //得到图片的宽高
            var width = texture.width;
            var height = texture.height;

            var flipTexture = new Texture2D(width, height);
            for (var i = 0; i < height; i++)
                flipTexture.SetPixels(0, i, width, 1, texture.GetPixels(0, height - i - 1, width, 1));

            flipTexture.Apply();
            return flipTexture;
        }

        /// <summary>
        ///  图片逆时针旋转90度
        /// </summary>
        /// <param name="src">原图片二进制数据</param>
        /// <param name="srcW">原图片宽度</param>
        /// <param name="srcH">原图片高度</param>
        public static Texture2D RotationLeft90(Color32[] src, int srcW, int srcH)
        {
            var des = new Color32[src.Length];
            var desTexture = new Texture2D(srcH, srcW);
            //if (desTexture.width != srcH || desTexture.height != srcW)
            //{
            //    desTexture.Resize(srcH, srcW);
            //}

            for (var i = 0; i < srcW; i++)
            for (var j = 0; j < srcH; j++)
                des[i * srcH + j] = src[(srcH - 1 - j) * srcW + i];

            desTexture.SetPixels32(des);
            desTexture.Apply();
            return desTexture;
        }

        /// <summary>
        ///  图片顺时针旋转90度
        /// </summary>
        /// <param name="src">原图片二进制数据</param>
        /// <param name="srcW">原图片宽度</param>
        /// <param name="srcH">原图片高度</param>
        public static Texture2D RotationRight90(Color32[] src, int srcW, int srcH)
        {
            var des = new Color32[src.Length];
            var desTexture = new Texture2D(srcH, srcW);

            for (var i = 0; i < srcH; i++)
            for (var j = 0; j < srcW; j++)
                des[(srcW - j - 1) * srcH + i] = src[i * srcW + j];

            desTexture.SetPixels32(des);
            desTexture.Apply();
            return desTexture;
        }

        /// <summary>
        ///  两张图合并
        /// </summary>
        /// <param name="baseTexture2D"></param>
        /// <param name="texture2D"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Texture2D MergeImage(Texture2D baseTexture2D, Texture2D texture2D, int x, int y, int w,
            int h)
        {
            //取图
            var color = texture2D.GetPixels32(0);
            for (var j = 0; j < 3; j++)
            for (var i = 0; i < 3; i++)
                baseTexture2D.SetPixels32(x + i * (texture2D.width + w), y + j * (texture2D.height + h),
                    texture2D.width, texture2D.height, color); //宽度

            //应用
            baseTexture2D.Apply();
            return baseTexture2D;
        }

        public static async void ScreenShootAndSave(string path)
        {
            var tex = await ScreenShoot();
            tex = ScaleTexture(tex, tex.width / 4, tex.height / 4);
            SaveTexture(tex, path);
        }

        public static ETTask<Texture2D> ScreenShoot()
        {
            var task = ETTask<Texture2D>.Create();
            YuoAwait_Mono.Instance.StartCoroutine(IScreenShoot(task));
            return task;
        }

        public static async void SaveTexture(Texture2D tex, string path)
        {
            var byt = tex.EncodeToPNG();
            await FileHelper.CheckOrCreateFilePathAsync(path);
            await File.WriteAllBytesAsync(path, byt);
        }

        /// <summary>
        ///  读取图片
        /// </summary>
        public static ETTask<Texture2D> LoadTexture(string path)
        {
            var task = ETTask<Texture2D>.Create(true);
            YuoAwait_Mono.Instance.StartCoroutine(LoadTexture(task, path));
            return task;
        }

        private static IEnumerator LoadTexture(ETTask<Texture2D> task, string path)
        {
            var webRequest = UnityWebRequestTexture.GetTexture(path);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                path.Log();
                Debug.LogError("LoadTexture Error:" + webRequest.downloadHandler.error);
                task.SetException(new Exception("LoadTexture Error"));
            }
            else
            {
                task.SetResult(DownloadHandlerTexture.GetContent(webRequest));
            }
        }

        public static Sprite Texture2Sprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }


        /// <summary>
        ///  截屏
        /// </summary>
        private static IEnumerator IScreenShoot(ETTask<Texture2D> task)
        {
            //图片大小  
            var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            //截取下一帧
            yield return new WaitForEndOfFrame();

            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true); //读像素
            tex.Apply();
            yield return tex;
            task.SetResult(tex);
        }
        
        // /// <summary>
        // /// 修改图片水平和垂直像素量
        // /// </summary>
        // public static Texture2D SetPictureResolution(string _path)
        // {
        //     Bitmap bm = new Bitmap(_path);
        //     bm.SetResolution(300, 300);
        //     string _idPath = Application.persistentDataPath + "/";
        //     string _name = "print.jpg";
        //     bm.Save(_idPath + _name, ImageFormat.Jpeg);
        //     Texture2D tex = loadTexture(_idPath, _name);
        //     File.WriteAllBytes(Application.persistentDataPath + "/aa.jpg", tex.EncodeToJPG());
        //     bm.Dispose();
        //     return tex;
        // }
    }

    public class Bitmap
    {
        public Bitmap(string path)
        {
            throw new NotImplementedException();
        }
    }
}