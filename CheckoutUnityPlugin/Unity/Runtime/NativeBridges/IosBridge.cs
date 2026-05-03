#if UNITY_EDITOR || UNITY_IOS

using System;
using System.Runtime.InteropServices;
using AOT;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace NativePlugins.CheckoutPlugin
{
    internal class IosBridge : INativeBridge
    {
        private static IosBridge _instance;

        private static UniTaskCompletionSource<(bool success, TokenDetails tokenDetails, TokenRequestError error)> _tokenRequestTcs;
        private static UniTaskCompletionSource<(bool success, string msg)> _initializeRiskSdkTcs;
        private static UniTaskCompletionSource<(bool success, string sessionId, string errorMsg)> _fetchRiskSessionIdTcs;
        private static UniTaskCompletionSource<(bool success, string token, string errorMsg)> _completeThreeDSChallengeTcs;

        internal IosBridge()
        {
            if (_instance != null)
            {
                Debug.LogError($"Instantiating multiple instances of {nameof(IosBridge)} is not allowed!");
                return;
            }
            _instance = this;
        }

        public async UniTask<(bool success, TokenDetails tokenDetails, TokenRequestError error)> ShowPaymentForm(bool isSandbox, string apiKey, CardSchemes supportedSchemes)
        {
            _tokenRequestTcs = new();
            _ShowPaymentForm(isSandbox, apiKey, (int)supportedSchemes, InvokeOnTokenRequestSuccess, InvokeOnTokenRequestFailed);
            return await _tokenRequestTcs.Task;
        }

        public UniTask<(bool success, string msg)> InitializeRiskSdk(bool isSandbox, string publicKey)
        {
            _initializeRiskSdkTcs = new();
            _InitializeRiskSdk(isSandbox, publicKey, InvokeOnInitializeRiskSdk);
            return _initializeRiskSdkTcs.Task;
        }

        public UniTask<(bool success, string sessionId, string errorMsg)> FetchRiskSessionId()
        {
            _fetchRiskSessionIdTcs = new();
            _FetchRiskSessionId(InvokeOnFetchRiskSessionId);
            return _fetchRiskSessionIdTcs.Task;
        }

        public UniTask<(bool success, string token, string errorMsg)> StartThreeDSChallenge(string authUrl, string successUrl, string failUrl)
        {
            _completeThreeDSChallengeTcs = new();
            _StartThreeDSChallenge(authUrl, successUrl, failUrl, InvokeOnCompleteThreeDSChallenge);
            return _completeThreeDSChallengeTcs.Task;
        }

        private delegate void TokenRequestSuccessDelegate(string tokenDetailsJson);
        private delegate void TokenRequestFailedDelegate(int errorCode, string errorMessage);
        private delegate void BoolStringDelegate(bool success, string message);

        [MonoPInvokeCallback(typeof(TokenRequestSuccessDelegate))]
        private static void InvokeOnTokenRequestSuccess(string tokenDetailsJson) => _tokenRequestTcs?.TrySetResult((true, JsonConvert.DeserializeObject<TokenDetails>(tokenDetailsJson), null));

        [MonoPInvokeCallback(typeof(TokenRequestFailedDelegate))]
        private static void InvokeOnTokenRequestFailed(int errorCode, string errorMsg) => _tokenRequestTcs?.TrySetResult((false, null, new((TokenRequestErrorCode)errorCode, errorMsg)));

        [MonoPInvokeCallback(typeof(BoolStringDelegate))]
        private static void InvokeOnInitializeRiskSdk(bool success, string msg) => _initializeRiskSdkTcs?.TrySetResult((success, msg));

        [MonoPInvokeCallback(typeof(BoolStringDelegate))]
        private static void InvokeOnFetchRiskSessionId(bool success, string data) => _fetchRiskSessionIdTcs?.TrySetResult((success, success ? data : null, success ? null : data));

        [MonoPInvokeCallback(typeof(BoolStringDelegate))]
        private static void InvokeOnCompleteThreeDSChallenge(bool success, string data) => _completeThreeDSChallengeTcs?.TrySetResult((success, success ? data : null, success ? null : data));

        [DllImport("__Internal")] private static extern void _ShowPaymentForm(bool isSandbox, string apiKey, int supportedSchemesMask, TokenRequestSuccessDelegate onSuccess, TokenRequestFailedDelegate onFailed);
        [DllImport("__Internal")] private static extern void _InitializeRiskSdk(bool isSandbox, string publicKey, BoolStringDelegate onInitialize);
        [DllImport("__Internal")] private static extern void _FetchRiskSessionId(BoolStringDelegate onFetch);
        [DllImport("__Internal")] private static extern void _StartThreeDSChallenge(string authUrl, string successUrl, string failUrl, BoolStringDelegate onComplete);
    }
}

#endif