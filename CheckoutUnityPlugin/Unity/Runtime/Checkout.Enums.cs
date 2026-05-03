using System;

namespace NativePlugins.CheckoutPlugin
{
    [Flags]
    public enum CardSchemes
    {
        Unknown = 1 << 0,
        Mada = 1 << 1,
        Visa = 1 << 2,
        Mastercard = 1 << 3,
        Maestro = 1 << 4,
        AmericanExpress = 1 << 5,
        Discover = 1 << 6,
        DinersClub = 1 << 7,
        JCB = 1 << 8,
    }

    public enum TokenRequestErrorCode
    {
        UserCancelled = 0,
        ApplePayTokenInvalid = 1100,
        CouldNotBuildUrlForRequest = 3001,
        MissingApiKey = 4000,

        // cardValidationError
        // cardNumber
        InvalidCharacters = 1001,
        // cvv
        ContainsNonDigits = 1002,
        InvalidLength = 1003,
        // phone
        NumberIncorrectLength = 1018,
        CountryCodeIncorrectLength = 1019,
        // billingAddress
        AddressLine1IncorrectLength = 1012,
        AddressLine2IncorrectLength = 1013,
        InvalidCityLength = 1014,
        InvalidCountry = 1015,
        InvalidStateLength = 1016,
        InvalidZipLength = 1017,

        // networkError
        NoInternetConnectivity = 2000,
        ConnectionFailed = 2001,
        ConnectionTimeout = 2002,
        ConnectionLost = 2003,
        InternationalRoamingOff = 2004,
        Unknown = 2005,
        CertificateTransparencyChecksFailed = 2006,
        CouldNotDecodeValues = 2007,
        EmptyResponse = 2008,

        ServerError = 3000,
    }

}
