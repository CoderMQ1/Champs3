// 
// 2023/12/13

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QFramework;
using UnityEngine;
using UnityEngine.Networking;

namespace SquareHero.Hotfix
{
    public static class HttpHelper
    {

        public readonly static string AppId = "7a8hxez99pgvdIroTaDZh9mfON97LO1wldDGVMwEhWsHo";
        public readonly static string AppKey = "3XEhbwprbgf3akD8JplkTxYgx8JUIQT75hOIc9ldd8Wv6";
        public static string Token = "98qTWGduyUm7jj4fF8rVqnZPbC9XqThG2MQuG1zJb9t9";
        public static void PostJson(this MonoBehaviour self, string url, string data, Action<string> onCompleted)
        {
            self.StartCoroutine(PostJson(url, data, onCompleted));
        }
        
        public static IEnumerator PostJson(string url, string data, Action<string> onCompleted)
        {
            LogKit.I($"Post {url } - data :{data}");
            UnityWebRequest request = new UnityWebRequest(url, "Post");
            request.SetRequestHeader("x-app-id", AppId);
            request.SetRequestHeader("x-app-key", AppKey);
            request.SetRequestHeader("Authentication", Token);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
            request.uploadHandler.contentType = "application/json";
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                var result = request.downloadHandler.text;
                LogKit.I($"Post {url } done result is : {result}");

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onCompleted?.Invoke("");
                }
                else
                {
                    onCompleted?.Invoke(result);
                }
                request.Dispose();
            }
        }

        public static IEnumerator Get(string url, Action<string> onCompleted)
        {
            if (!url.StartsWith("http"))
            {
                url = GameUrlConstValue.AssetAddress + url;
            }
            UnityWebRequest request = new UnityWebRequest(url, "Get");
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("x-app-id", AppId);
            request.SetRequestHeader("x-app-key", AppKey);
            request.SetRequestHeader("Authentication", Token);
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                var result = request.downloadHandler.text;
                LogKit.I($"Get {url } done result is : {result}");
                onCompleted?.Invoke(result);
                request.Dispose();
            }
        }

        public static IEnumerator GetSkinList(Action<string> onCompleted)
        {
            yield return Get(GameUrlConstValue.GetRolesData.Url(), onCompleted);
        }

        public static IEnumerator GetPropList(Action<string> onCompleted)
        {
            yield return Get(GameUrlConstValue.AllItem.Url(), onCompleted);
        }

    }
}