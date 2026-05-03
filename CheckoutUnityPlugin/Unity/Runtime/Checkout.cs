using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NativePlugins.CheckoutPlugin
{
    public static class Checkout
    {
        private static readonly INativeBridge _bridge;

        static Checkout()
        {
#if UNITY_EDITOR
            _bridge = new EditorBridge();
#elif UNITY_IOS
            _bridge = new IosBridge();
#elif UNITY_ANDROID
            _bridge = new AndroidBridge(); // todo
#endif
        }

        /// <summary>
        /// Check if Checkout is supported on the current platform.
        /// </summary>
        public static bool IsSupported => _bridge != null;

        /// <summary>
        /// Shows the payment form.
        /// </summary>
        /// <param name="isSandbox">Whether to use the Sandbox or Production environment.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="supportedSchemes">The card schemes supported for this payment.</param>
        /// <returns>
        /// A tuple containing:<br/>
        /// - A boolean indicating success or failure.<br/>
        /// - On success, a TokenDetails object with the token information; null on failure.<br/>
        /// - On failure, a TokenRequestError object with error details (e.g. <see cref="TokenRequestErrorCode.UserCancelled"/>); null on success.<br/>
        /// </returns>
        public static UniTask<(bool success, TokenDetails tokenDetails, TokenRequestError error)> ShowPaymentForm(bool isSandbox, string apiKey, CardSchemes supportedSchemes) => _bridge.ShowPaymentForm(isSandbox, apiKey, supportedSchemes);

        /// <summary>
        /// Fetches a token from the checkout.com/tokens API using card info.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="tokenUrl">The Checkout token URL.</param>
        /// <returns>The requested token.</returns>
        public static UniTask<string> FetchCardToken(string apiKey, string tokenUrl, string cardNumber, int expiryMonth, int expiryYear, int cvv)
        {
            var body = new
            {
                type = "card",
                number = cardNumber,
                expiry_month = expiryMonth,
                expiry_year = expiryYear,
                cvv = cvv
            };
            return FetchToken(apiKey, tokenUrl, body);
        }

        /// <summary>
        /// Fetches a token from the checkout.com/tokens API using ApplePay payment data.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="tokenUrl">The Checkout token URL.</param>
        /// <returns>The requested token.</returns>
        public static UniTask<string> FetchApplePayToken(string apiKey, string tokenUrl, string paymentDataJson)
        {
            if (Application.isEditor)
            {
                return FetchEditorToken(apiKey, tokenUrl);
            }

            var body = new
            {
                type = "applepay",
                token_data = JsonConvert.DeserializeObject(paymentDataJson)
            };
            return FetchToken(apiKey, tokenUrl, body);
        }

        public static UniTask<string> FetchEditorToken(string apiKey, string tokenUrl)
        {
            if (!Application.isEditor)
            {
                throw new InvalidOperationException($"{nameof(FetchEditorToken)} can only be called in the Unity Editor.");
            }
            return FetchCardToken(apiKey, tokenUrl, "4242424242424242", 8, 2088, 888);
        }

        private static async UniTask<string> FetchToken(string apiKey, string tokenUrl, object body)
        {
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Platform", "iOS" },
                { "Authorization", apiKey }
            };

            var tokenJson = await Utils.Post(tokenUrl, JsonConvert.SerializeObject(body), headers);
            var token = JsonConvert.DeserializeObject<JObject>(tokenJson)["token"].ToString();
            return token;
        }

        /// <summary>
        /// Initializes the risk SDK. This should be called before fetching the risk session ID.</br/>
        /// It is safe to call this method multiple times; subsequent calls will reinitialize the SDK if parameters are changed.
        /// </summary>
        /// <param name="isSandbox">Whether to use the Sandbox or Production environment.</param>
        /// <param name="publicKey">The public key for the risk SDK.</param>
        /// <returns>
        /// A tuple containing:<br/>
        /// - A boolean indicating success or failure.<br/>
        /// - A message providing additional information about the result.<br/>
        /// </returns>
        public static UniTask<(bool success, string msg)> InitializeRiskSdk(bool isSandbox, string publicKey) => _bridge.InitializeRiskSdk(isSandbox, publicKey);

        /// <summary>
        /// Fetches the risk session ID. Ensure that <see cref="InitializeRiskSdk(bool, string)"/> has been called successfully before invoking this method.
        /// </summary>
        /// <returns>
        /// A tuple containing:<br/>
        /// - A boolean indicating success or failure.<br/>
        /// - On success, the device session ID; null on failure.<br/>
        /// - On failure, an error message; null on success.<br/>
        /// </returns>
        public static UniTask<(bool success, string sessionId, string errorMsg)> FetchRiskSessionId() => _bridge.FetchRiskSessionId();

        /// <summary>
        /// Starts the 3DS challenge process.
        /// </summary>
        /// <param name="authUrl">The authentication URL for the 3DS challenge.</param>
        /// <param name="successUrl">The URL to redirect to upon successful authentication.</param>
        /// <param name="failUrl">The URL to redirect to upon failed authentication.</param>
        /// <returns>
        /// A tuple containing:<br/>
        /// - A boolean indicating success or failure.<br/>
        /// - On success, the challenge token; null on failure.<br/>
        /// - On failure, an error message; null on success.<br/>
        /// </returns>
        public static UniTask<(bool success, string token, string errorMsg)> StartThreeDSChallenge(string authUrl, string successUrl, string failUrl) => _bridge.StartThreeDSChallenge(authUrl, successUrl, failUrl);
    }
}
