import Checkout
import Frames
import Risk
import UIKit

public typealias TokenRequestSuccessDelegate = @convention(c) (CStringPtr) -> Void
public typealias TokenRequestFailedDelegate = @convention(c) (Int, CStringPtr) -> Void

public final class CheckoutPlugin {
    private var _framesViewController: UIViewController?

    private var _riskSdk: Risk?
    private var _cachedRiskPublicKey: String?
    private var _cachedRiskEnvironment: RiskEnvironment?

    public func showPaymentForm(isSandbox: Bool, apiKey: String, supportedSchemesMask: Int32) {
        let config = PaymentFormConfiguration(
            apiKey: apiKey,
            environment: isSandbox ? .sandbox : .live,
            supportedSchemes: decodeCardSchemes(from: supportedSchemesMask),
            billingFormData: nil
        )

        var paymentFormStyle = DefaultPaymentFormStyle()
        paymentFormStyle.addBillingSummary = nil
        paymentFormStyle.editBillingSummary = nil

        let completionHandler: (Result<TokenDetails, TokenRequestError>) -> Void = { result in
            DispatchQueue.main.async {
                self._framesViewController?.dismiss(animated: true)

                switch result {
                case .failure(let error):
                    Callbacks.onTokenRequestFailed?(error.code, error.localizedDescription)
                case .success(let details):
                    Callbacks.onTokenRequestSuccess?(CheckoutPlugin.tokenDetailsToJSONString(details))
                }
            }
        }

        let framesViewController = PaymentFormFactory.buildViewController(
            configuration: config,
            style: PaymentStyle(paymentFormStyle: paymentFormStyle),
            completionHandler: completionHandler
        )

        _framesViewController = framesViewController

        DispatchQueue.main.async {
            if let root = UIApplication.shared.windows.first?.rootViewController {
                let nav = UINavigationController(rootViewController: framesViewController)
                root.present(nav, animated: true)
            }
        }
    }

    func decodeCardSchemes(from mask: Int32) -> [CardScheme] {
        var schemes: [CardScheme] = []
        func hasBit(_ n: Int32) -> Bool { (mask & (Int32(1) << n)) != 0 }

        if hasBit(1) { schemes.append(.mada) }
        if hasBit(2) { schemes.append(.visa) }
        if hasBit(3) { schemes.append(.mastercard) }
        if hasBit(4) { schemes.append(.maestro) }
        if hasBit(5) { schemes.append(.americanExpress) }
        if hasBit(6) { schemes.append(.discover) }
        if hasBit(7) { schemes.append(.dinersClub) }
        if hasBit(8) { schemes.append(.jcb) }

        return schemes
    }

    public func initializeRiskSdk(isSandbox: Bool, publicKey: String) {
        let environment: RiskEnvironment = isSandbox ? .sandbox : .production
        if publicKey == _cachedRiskPublicKey, environment == _cachedRiskEnvironment, _riskSdk != nil {
            Callbacks.onInitializeRiskSdk?(true, "RiskSDK already initialized.")
            return
        }

        let riskConfig = RiskConfig(publicKey: publicKey, environment: environment)
        let riskSdk = Risk(config: riskConfig)

        riskSdk.configure { result in
            switch result {
            case .failure(let error):
                Callbacks.onInitializeRiskSdk?(false, error.localizedDescription)
            case .success:
                self._riskSdk = riskSdk
                self._cachedRiskPublicKey = publicKey
                self._cachedRiskEnvironment = environment
                Callbacks.onInitializeRiskSdk?(true, "RiskSDK initialized successfully!")
            }
        }
    }

    public func fetchRiskSessionId() {
        guard let riskSdk = _riskSdk else {
            Callbacks.onFetchRiskSessionId?(false, "RiskSDK is not initialized!")
            return
        }

        riskSdk.publishData { publishResult in
            switch publishResult {
            case .success(let response):
                Callbacks.onFetchRiskSessionId?(true, response.deviceSessionId)
            case .failure(let error):
                Callbacks.onFetchRiskSessionId?(false, error.localizedDescription)
            }
        }
    }

    public func startThreeDSChallenge(authUrl: String, successUrl: String, failUrl: String) {
        let vc = ThreeDSViewController()
        DispatchQueue.main.async {
            if let root = UIApplication.shared.windows.first?.rootViewController {
                let nav = UINavigationController(rootViewController: vc)
                root.present(nav, animated: false) {
                    vc.startChallenge(authUrlString: authUrl, successUrlString: successUrl, failUrlString: failUrl)
                }
            }
        }
    }

    public static func tokenDetailsToJSONString(_ token: TokenDetails) -> String {
        var dict = [String: String]()

        dict["type"] = token.type.rawValue
        dict["token"] = token.token
        dict["expiresOn"] = token.expiresOn
        dict["expiryDate"] = String(format: "%02d/%02d", token.expiryDate.month, token.expiryDate.year % 100)
        dict["scheme"] = token.scheme ?? ""
        dict["schemeLocal"] = token.schemeLocal ?? ""
        dict["last4"] = token.last4
        dict["bin"] = token.bin
        dict["cardType"] = token.cardType ?? ""
        dict["cardCategory"] = token.cardCategory ?? ""
        dict["issuer"] = token.issuer ?? ""
        dict["issuerCountry"] = token.issuerCountry ?? ""
        dict["productId"] = token.productId ?? ""
        dict["productType"] = token.productType ?? ""
        dict["name"] = token.name ?? ""
        dict["phoneNumber"] = token.phone?.number ?? ""
        dict["phoneCountryCode"] = token.phone?.countryCode ?? ""

        do {
            let jsonData = try JSONSerialization.data(withJSONObject: dict)
            if let jsonString = String(data: jsonData, encoding: .utf8) {
                return jsonString
            }
        } catch {
            print("Failed to encode TokenDetails JSON:", error)
        }
        return "{}"
    }
}
