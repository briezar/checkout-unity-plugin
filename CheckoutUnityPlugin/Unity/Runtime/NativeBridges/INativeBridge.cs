using System;
using Cysharp.Threading.Tasks;

namespace NativePlugins.CheckoutPlugin
{
    internal interface INativeBridge
    {
        UniTask<(bool success, TokenDetails tokenDetails, TokenRequestError error)> ShowPaymentForm(bool isSandbox, string apiKey, CardSchemes supportedSchemes);
        UniTask<(bool success, string msg)> InitializeRiskSdk(bool isSandbox, string publicKey);
        UniTask<(bool success, string sessionId, string errorMsg)> FetchRiskSessionId();
        UniTask<(bool success, string token, string errorMsg)> StartThreeDSChallenge(string authUrl, string successUrl, string failUrl);
    }

    public record TokenRequestError(TokenRequestErrorCode Code, string Message);

    /// <summary> TokenDetails object representing the fields needed in the tokenization request payload. </summary>
    public class TokenDetails
    {
        public enum TokenType { Card, ApplePay }

        public TokenType type;
        public string token;
        public string expiresOn;
        public string expiryDate; // 04/24
        public string scheme;
        public string schemeLocal;
        public string last4;
        public string bin;
        public string cardType;
        public string cardCategory;
        public string issuer;
        public string issuerCountry;
        public string productId;
        public string productType;
        public string name;
        public string phoneNumber;
        public string phoneNumberCountryCode;
    }
}

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit { }
}
