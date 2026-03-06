# Platform Setup Guide

This guide provides detailed setup instructions for each platform supported by the Honua Mobile SDK.

## Android Setup

### Prerequisites

- **Android SDK**: API level 21 (Android 5.0) or higher
- **Java Development Kit**: JDK 11 or higher
- **Android Emulator**: For testing (optional but recommended)

### 1. Install Android SDK

#### Via Visual Studio
1. Open Visual Studio 2022
2. Go to **Tools** → **Android** → **Android SDK Manager**
3. Install the following:
   - **Android SDK Platform** for API 34+
   - **Android SDK Build-Tools** (latest version)
   - **Google Play Services**
   - **Android Emulator** (optional)

#### Via Command Line
```bash
# Install via Android Studio or sdkmanager
sdkmanager "platforms;android-34" "build-tools;34.0.0" "platform-tools"
```

### 2. Configure Google Maps (Android)

1. **Get Google Maps API Key:**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select existing one
   - Enable **Maps SDK for Android**
   - Create credentials → API Key
   - Restrict the API key to your app's package name

2. **Add API Key to AndroidManifest.xml:**

```xml
<application android:allowBackup="true" android:icon="@mipmap/appicon" android:supportsRtl="true">
    <!-- Google Maps API Key -->
    <meta-data android:name="com.google.android.geo.API_KEY"
               android:value="YOUR_GOOGLE_MAPS_API_KEY" />

    <!-- Additional configurations... -->
</application>
```

### 3. Configure Permissions

Add required permissions to `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- For background location updates (optional) -->
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />

<!-- For camera features -->
<uses-feature android:name="android.hardware.camera" android:required="false" />
<uses-feature android:name="android.hardware.camera.autofocus" android:required="false" />

<!-- For location features -->
<uses-feature android:name="android.hardware.location" android:required="false" />
<uses-feature android:name="android.hardware.location.gps" android:required="false" />
<uses-feature android:name="android.hardware.location.network" android:required="false" />
```

### 4. File Security Configuration

For Android API 24+, add network security config in `Platforms/Android/Resources/xml/network_security_config.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <domain-config cleartextTrafficPermitted="true">
        <domain includeSubdomains="true">your-honua-server.com</domain>
        <domain includeSubdomains="true">localhost</domain>
        <domain includeSubdomains="true">10.0.2.2</domain> <!-- Android emulator -->
    </domain-config>
</network-security-config>
```

Update `AndroidManifest.xml` to reference it:

```xml
<application android:networkSecurityConfig="@xml/network_security_config">
    <!-- ... -->
</application>
```

### 5. ProGuard Configuration (Release Builds)

Add to `Platforms/Android/proguard.cfg`:

```
# Honua Mobile SDK
-keep class Honua.Mobile.** { *; }
-keep class com.google.android.gms.maps.** { *; }
-keep class com.google.android.gms.location.** { *; }

# gRPC
-keep class io.grpc.** { *; }
-keep class com.google.protobuf.** { *; }

# SQLite
-keep class SQLite.** { *; }
```

## iOS Setup

### Prerequisites

- **macOS**: macOS 10.15 (Catalina) or later
- **Xcode**: Version 15.0 or later
- **iOS Deployment Target**: iOS 11.0 or later

### 1. Install Xcode

1. Download Xcode from the Mac App Store
2. Install iOS Simulator and additional components
3. Accept Xcode license agreements:

```bash
sudo xcodebuild -license accept
```

### 2. Configure Info.plist

Add privacy usage descriptions to `Platforms/iOS/Info.plist`:

```xml
<!-- Location Services -->
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs location access to collect field data with GPS coordinates and show your position on the map.</string>

<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>This app needs location access to enable background synchronization and offline data collection.</string>

<!-- Camera and Media -->
<key>NSCameraUsageDescription</key>
<string>This app needs camera access to capture photos for field data collection and documentation.</string>

<key>NSMicrophoneUsageDescription</key>
<string>This app needs microphone access to record audio notes and field observations.</string>

<key>NSPhotoLibraryUsageDescription</key>
<string>This app needs photo library access to save captured images and access existing photos.</string>

<key>NSPhotoLibraryAddUsageDescription</key>
<string>This app needs permission to save captured photos to your photo library.</string>

<!-- Background Processing -->
<key>UIBackgroundModes</key>
<array>
    <string>location</string>
    <string>background-processing</string>
</array>
```

### 3. Configure App Transport Security

For development with local servers, configure ATS in Info.plist:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <false/>
    <key>NSExceptionDomains</key>
    <dict>
        <key>your-honua-server.com</key>
        <dict>
            <key>NSExceptionAllowsInsecureHTTPLoads</key>
            <true/>
            <key>NSExceptionMinimumTLSVersion</key>
            <string>TLSv1.0</string>
        </dict>
        <!-- For local development -->
        <key>localhost</key>
        <dict>
            <key>NSExceptionAllowsInsecureHTTPLoads</key>
            <true/>
        </dict>
    </dict>
</dict>
```

### 4. Code Signing and Provisioning

For device testing and distribution:

1. **Apple Developer Account**: Required for device deployment
2. **Provisioning Profiles**: Configure in Xcode or Apple Developer Portal
3. **Code Signing**: Set up automatic or manual signing in project settings

### 5. Platform-Specific Dependencies

The iOS platform automatically includes:
- **MapKit**: For native map functionality
- **CoreLocation**: For GPS and location services
- **AVFoundation**: For camera and media capture
- **Photos**: For photo library access

## Windows Setup

### Prerequisites

- **Windows 10**: Version 1809 (build 17763) or later
- **Windows SDK**: Version 19041 or later
- **Visual Studio 2022**: With MAUI and Windows development workloads

### 1. Install Windows SDK

```bash
# Via Visual Studio Installer
# 1. Open Visual Studio Installer
# 2. Modify your VS 2022 installation
# 3. Add "Windows 10/11 SDK" components
```

### 2. Configure Package.appxmanifest

Update `Platforms/Windows/Package.appxmanifest`:

```xml
<Package ...>
    <Applications>
        <Application>
            <!-- Capabilities -->
            <Capabilities>
                <Capability Name="internetClient" />
                <DeviceCapability Name="location" />
                <DeviceCapability Name="webcam" />
                <DeviceCapability Name="microphone" />
                <Capability Name="picturesLibrary" />
                <Capability Name="videosLibrary" />
            </Capabilities>
        </Application>
    </Applications>
</Package>
```

### 3. Configure Maps (Optional)

For Bing Maps integration on Windows:

1. Get Bing Maps API key from [Bing Maps Dev Center](https://www.bingmapsportal.com/)
2. Configure in app settings:

```csharp
// In MauiProgram.cs
builder.Services.AddHonuaMobile(options =>
{
    options.WindowsMapsApiKey = "YOUR_BING_MAPS_API_KEY";
});
```

## macOS Setup (Mac Catalyst)

### Prerequisites

- **macOS**: macOS 10.15 (Catalina) or later
- **Xcode**: Version 15.0 or later
- **Mac Catalyst**: Enabled in project settings

### 1. Enable Mac Catalyst

In your `.csproj` file:

```xml
<PropertyGroup>
    <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-maccatalyst</TargetFrameworks>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">13.1</SupportedOSPlatformVersion>
</PropertyGroup>
```

### 2. Configure Privacy Settings

Similar to iOS, add usage descriptions to `Info.plist`:

```xml
<!-- Same privacy descriptions as iOS -->
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs location access for field data collection.</string>

<key>NSCameraUsageDescription</key>
<string>This app needs camera access to capture field photos.</string>
```

### 3. macOS-Specific Entitlements

Add `Platforms/MacCatalyst/Entitlements.plist`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>com.apple.security.app-sandbox</key>
    <true/>
    <key>com.apple.security.network.client</key>
    <true/>
    <key>com.apple.security.device.camera</key>
    <true/>
    <key>com.apple.security.personal-information.location</key>
    <true/>
    <key>com.apple.security.files.user-selected.read-write</key>
    <true/>
    <key>com.apple.security.files.pictures.read-write</key>
    <true/>
</dict>
</plist>
```

## Development Environment Setup

### Visual Studio 2022

1. **Install Required Workloads:**
   - .NET Multi-platform App UI development
   - Mobile development with .NET

2. **Install Required Components:**
   - Android SDK and NDK
   - iOS and macOS SDKs (macOS only)
   - Windows 10/11 SDK

### Visual Studio Code

1. **Install Extensions:**
   ```bash
   code --install-extension ms-dotnettools.csharp
   code --install-extension ms-dotnettools.dotnet-maui
   ```

2. **Install .NET MAUI CLI:**
   ```bash
   dotnet workload install maui
   ```

## Platform-Specific Testing

### Android Emulator

```bash
# List available emulators
emulator -list-avds

# Start emulator
emulator -avd Pixel_5_API_34

# Deploy and run
dotnet build -f net8.0-android
dotnet run -f net8.0-android
```

### iOS Simulator

```bash
# List simulators (macOS only)
xcrun simctl list devices

# Build and deploy
dotnet build -f net8.0-ios
dotnet run -f net8.0-ios
```

### Windows

```bash
# Build and run Windows app
dotnet build -f net8.0-windows10.0.19041.0
dotnet run -f net8.0-windows10.0.19041.0
```

## Troubleshooting

### Common Android Issues

**Build Errors:**
```bash
# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

**Emulator Issues:**
```bash
# Reset Android SDK tools
$ANDROID_HOME/tools/bin/sdkmanager --update
```

### Common iOS Issues

**Code Signing:**
- Verify Apple Developer account
- Check provisioning profiles
- Ensure bundle identifier matches

**Simulator Not Starting:**
```bash
# Reset iOS Simulator
xcrun simctl shutdown all
xcrun simctl erase all
```

### Performance Optimization

**Android:**
- Enable R8 code shrinking
- Use AOT compilation for better performance
- Optimize image resources

**iOS:**
- Enable LLVM optimizations
- Use linking for smaller app size
- Test on physical devices for accurate performance

## Next Steps

After completing platform setup:

1. Return to [Getting Started Guide](getting-started.md)
2. Build and test your first Honua Mobile app
3. Explore [Advanced Features](advanced-features.md)

---

For platform-specific issues, check our [Troubleshooting Guide](troubleshooting.md) or visit the [Community Discussions](https://github.com/mikemcdougall/honua-mobile-sdk/discussions).