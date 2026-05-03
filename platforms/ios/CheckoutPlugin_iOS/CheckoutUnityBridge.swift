import Foundation

public final class Callbacks {
    public static var onInitializeRiskSdk: BoolStringDelegate?
    public static var onFetchRiskSessionId: BoolStringDelegate?

    public static var onTokenRequestSuccess: TokenRequestSuccessDelegate?
    public static var onTokenRequestFailed: TokenRequestFailedDelegate?

    public static var onThreeDSChallengeCompleted: BoolStringDelegate?
}

let _plugin = CheckoutPlugin()

@_cdecl("_ShowPaymentForm")
public func _ShowPaymentForm(
    isSandbox: Bool,
    apiKey: CStringPtr,
    supportedSchemesMask: Int32,
    onTokenRequestSuccess: @escaping TokenRequestSuccessDelegate,
    onTokenRequestFailed: @escaping TokenRequestFailedDelegate
) {
    Callbacks.onTokenRequestFailed = onTokenRequestFailed
    Callbacks.onTokenRequestSuccess = onTokenRequestSuccess

    _plugin.showPaymentForm(isSandbox: isSandbox, apiKey: apiKey.toString(), supportedSchemesMask: supportedSchemesMask)
}

@_cdecl("_InitializeRiskSdk")
public func _InitializeRiskSdk(isSandbox: Bool, publicKey: CStringPtr, onInitialize: @escaping BoolStringDelegate) {
    Callbacks.onInitializeRiskSdk = onInitialize
    _plugin.initializeRiskSdk(isSandbox: isSandbox, publicKey: publicKey.toString())
}

@_cdecl("_FetchRiskSessionId")
public func _FetchRiskSessionId(onFetch: @escaping BoolStringDelegate) {
    Callbacks.onFetchRiskSessionId = onFetch
    _plugin.fetchRiskSessionId()
}

@_cdecl("_StartThreeDSChallenge")
public func _StartThreeDSChallenge(authUrl: CStringPtr, successUrl: CStringPtr, failUrl: CStringPtr, onComplete: @escaping BoolStringDelegate) {
    Callbacks.onThreeDSChallengeCompleted = onComplete
    _plugin.startThreeDSChallenge(
        authUrl: authUrl.toString(),
        successUrl: successUrl.toString(),
        failUrl: failUrl.toString()
    )
}
