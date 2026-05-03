#if UNITY_EDITOR || UNITY_ANDROID

using System;
using Cysharp.Threading.Tasks;

namespace NativePlugins.CheckoutPlugin
{
    internal class AndroidBridge : INativeBridge
    {
        public UniTask<(bool success, TokenDetails tokenDetails, TokenRequestError error)> ShowPaymentForm(bool isSandbox, string apiKey, CardSchemes supportedSchemes)
        {
            throw new NotImplementedException();
        }

        public UniTask<(bool success, string msg)> InitializeRiskSdk(bool isSandbox, string publicKey)
        {
            throw new NotImplementedException();
        }

        public UniTask<(bool success, string sessionId, string errorMsg)> FetchRiskSessionId()
        {
            throw new NotImplementedException();
        }

        public UniTask<(bool success, string token, string errorMsg)> StartThreeDSChallenge(string authUrl, string successUrl, string failUrl)
        {
            throw new NotImplementedException();
        }
    }
}

#endif