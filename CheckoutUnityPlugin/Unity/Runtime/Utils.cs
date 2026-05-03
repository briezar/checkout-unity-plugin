using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NativePlugins.CheckoutPlugin
{
    public static class Utils
    {
        /// <summary>  Try-catch <see cref="UnityWebRequestException"/> to get error details </summary>
        public static async UniTask<string> Post(string url, string requestBody, Dictionary<string, string> headers)
        {
            // Empty string so it will not create another uploadHandler which causes memory leak
            using var webRequest = UnityWebRequest.Post(url, string.Empty);
            webRequest.timeout = 30;

            if (!string.IsNullOrEmpty(requestBody))
            {
                var uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody))
                {
                    contentType = "application/json"
                };
                webRequest.uploadHandler = uploadHandler;
            }

            headers ??= new();
            foreach (var header in headers)
            {
                webRequest.SetRequestHeader(header.Key, header.Value);
            }

            try
            {
                await webRequest.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.LogError($"POST exception from [{url}]\n{ex}");
                throw;
            }
            return webRequest.downloadHandler.text;
        }
    }
}