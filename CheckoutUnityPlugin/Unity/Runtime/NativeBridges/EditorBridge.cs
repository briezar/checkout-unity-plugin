#if UNITY_EDITOR

using System;
using Cysharp.Threading.Tasks;

namespace NativePlugins.CheckoutPlugin
{
    internal class EditorBridge : INativeBridge
    {
        public async UniTask<(bool success, TokenDetails tokenDetails, TokenRequestError error)> ShowPaymentForm(bool isSandbox, string apiKey, CardSchemes supportedSchemes)
            => (true, new TokenDetails() { token = "test_token" }, null);

        public async UniTask<(bool success, string msg)> InitializeRiskSdk(bool isSandbox, string publicKey) => (true, "Success");
        public async UniTask<(bool success, string sessionId, string errorMsg)> FetchRiskSessionId() => (true, "dsid_wj75egdzdhsevpuhgdcm55bncm", null);
        public async UniTask<(bool success, string token, string errorMsg)> StartThreeDSChallenge(string authUrl, string successUrl, string failUrl) => (true, "token", null);

    }
}

#endif