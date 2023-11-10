// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;
// using YuoTools;
//
// public class WenXinYiYan
// {
//     private const string ApiKey = "KjaBEBBh2XCFDeGKB2C45Rm2";
//     private const string SecretKey = "1R5Dvm1SCduVaD6sfAR7Szbyx0McYk3k";
//
//     static async Task<string> GetResponse(string body, List<(string, string)> headers = null)
//     {
//         var url =
//             $"https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=[{ApiKey}]&client_secret=[{SecretKey}]";
//
//         UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
//
//         headers ??= new List<(string, string)>
//         {
//             ("Content-Type", "application/json"),
//             ("Accept", "application/json")
//         };
//         for (int i = 0; i < headers.Count; i++)
//         {
//             webRequest.SetRequestHeader(headers[i].Item1, headers[i].Item2);
//         }
//
//         byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
//         webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
//         webRequest.downloadHandler = new DownloadHandlerBuffer();
//         webRequest.disposeDownloadHandlerOnDispose = true;
//         webRequest.disposeUploadHandlerOnDispose = true;
//
//         await GetAwaiter(webRequest.SendWebRequest());
//
//         string result = ""; 
//         switch (webRequest.result)
//         {
//             case UnityWebRequest.Result.ConnectionError:
//             case UnityWebRequest.Result.DataProcessingError:
//                 Debug.LogError("Error: " + webRequest.error);
//                 break;
//             case UnityWebRequest.Result.ProtocolError:
//                 Debug.LogError("HTTP Error: " + webRequest.error);
//                 if (url.EndsWith("/completions"))
//                 {
//                     Debug.LogError(webRequest.downloadHandler.text);
//                     if (webRequest.error == "HTTP/1.1 429 Too Many Requests")
//                     {
//                         Debug.Log("请求太快,请求失败,正在重试...");
//                         await Task.Delay(2000);
//                         result = await GetResponse(body, headers);
//                     }
//                 }
//
//                 break;
//             case UnityWebRequest.Result.Success:
//                 result = webRequest.downloadHandler.text;
//                 break;
//         }
//
//         webRequest.Dispose();
//         return result;
//     }
// }