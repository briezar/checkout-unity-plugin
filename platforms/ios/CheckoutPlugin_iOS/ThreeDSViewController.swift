import Frames
import UIKit

final class ThreeDSViewController: UIViewController, UIViewControllerTransitioningDelegate {
    private var _isChallengeActive = false

    override func viewDidLoad() {
        super.viewDidLoad()
        view.backgroundColor = UIColor(white: 1, alpha: 0)
    }

    func startChallenge(authUrlString: String, successUrlString: String, failUrlString: String) {
        guard let authUrl = URL(string: authUrlString),
            let successUrl = URL(string: successUrlString),
            let failUrl = URL(string: failUrlString)
        else {
            Callbacks.onThreeDSChallengeCompleted?(false, "Invalid URL")
            return
        }

        let threeDSWebViewController = ThreedsWebViewController(
            environment: .sandbox,
            successUrl: successUrl,
            failUrl: failUrl)
        threeDSWebViewController.authURL = authUrl
        threeDSWebViewController.delegate = self
        threeDSWebViewController.transitioningDelegate = self
        present(threeDSWebViewController, animated: true) { self._isChallengeActive = true }
    }

    func animationController(forDismissed dismissed: UIViewController) -> UIViewControllerAnimatedTransitioning? {
        if _isChallengeActive {
            _isChallengeActive = false
            Callbacks.onThreeDSChallengeCompleted?(false, "User Cancelled")
        }
        return nil
    }
}

extension ThreeDSViewController: ThreedsWebViewControllerDelegate {
    func threeDSWebViewControllerAuthenticationDidSucceed(_ vc: ThreedsWebViewController, token: String?) {
        _isChallengeActive = false
        DispatchQueue.main.async {
            vc.dismiss(animated: true)
            self.dismiss(animated: false)
            Callbacks.onThreeDSChallengeCompleted?(true, token ?? "")
        }
    }

    func threeDSWebViewControllerAuthenticationDidFail(_ vc: ThreedsWebViewController) {
        _isChallengeActive = false
        DispatchQueue.main.async {
            vc.dismiss(animated: true)
            self.dismiss(animated: false)
            Callbacks.onThreeDSChallengeCompleted?(false, "3DS Challenge Failed")
        }
    }
}
