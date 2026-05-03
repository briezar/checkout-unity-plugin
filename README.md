# Checkout Unity Plugin
Adds Checkout functionality to your Unity project.

## Dependencies
- UniTask (for asynchronous operations)
- EDM4U (External Dependency Manager)

## Installation
- Drag the `project-root/CheckoutUnityPlugin` into Unity at `Assets/Plugins/`
- `Assets/External Dependency Manager/iOS Resolver/Settings` -> Untick `Link Frameworks statically`

## Usage
- `Unity/Runtime/Checkout.cs` contains all API with documentation.

## Modify/Update Project
### iOS
- If you need to change the version of Checkout dependencies, you need to update both `./platforms/ios/Podfile` and `./CheckoutUnityPlugin/Unity/Editor/CheckoutDependencies.xml`
- Right-click `./platforms/ios` -> `New Terminal at Folder`, run `pod install` (optionally ` --repo-update` to update pods).
- Open `./platforms/ios/CheckoutPlugin_iOS.xcworkspace` and modify the project.
- From top menu choose `Product/Archive`, then choose `Product/Show Build Folder in Finder`, and navigate to `.../Build/Intermediates.noindex/ArchiveIntermediates/CheckoutPlugin_iOS/BuildProductsPath/Release-iphoneos/CheckoutPlugin_iOS.framework`. Right-click -> Show Original, then Copy (and replace) `CheckoutPlugin_iOS.framework` to `./CheckoutPlugin_iOS/iOS/`.

### Android
- N/A
