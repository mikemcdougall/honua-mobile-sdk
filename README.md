# Honua Mobile SDK

.NET MAUI mobile SDK for the Honua geospatial platform, enabling field data collection, offline synchronization, and native map integration for cross-platform mobile applications.

[![Apache 2.0 License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Build Status](https://img.shields.io/github/actions/workflow/status/mikemcdougall/honua-mobile-sdk/ci.yml?branch=main)](https://github.com/mikemcdougall/honua-mobile-sdk/actions)
[![NuGet Version](https://img.shields.io/nuget/v/Honua.Mobile.svg)](https://www.nuget.org/packages/Honua.Mobile/)

## Features

### 🗺️ Native Map Integration
- Cross-platform map controls with platform-specific native renderers
- Support for multiple map types (satellite, road, hybrid, terrain)
- Interactive feature layers and overlays
- Gesture support (pan, zoom, rotate)

### 📱 Field Data Collection
- Dynamic form builder with multiple field types
- Photo, video, and audio capture with geotagging
- Barcode and QR code scanning
- Digital signature collection
- Location-aware data entry

### 🔄 Offline Capabilities
- Background data synchronization
- Local SQLite storage for offline operations
- Conflict resolution for concurrent edits
- Selective area download for offline maps

### 🎯 Native Platform Features
- GPS and location services integration
- Camera and media capture
- Device sensors (compass, accelerometer)
- Push notifications
- Biometric authentication

## Quick Start

### Installation

Install the NuGet package:

```bash
dotnet add package Honua.Mobile
```

### Basic Setup

1. **Configure your MAUI app** in `MauiProgram.cs`:

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseHonuaMobile() // Add Honua Mobile SDK
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Configure Honua services
        builder.Services.AddHonuaMobile(config =>
        {
            config.ServerUrl = "https://your-honua-server.com";
            config.ApiKey = "your-api-key";
            config.EnableOfflineSync = true;
        });

        return builder.Build();
    }
}
```

2. **Add a map to your page**:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:honua="clr-namespace:Honua.Mobile.Controls;assembly=Honua.Mobile"
             x:Class="MyApp.MapPage">
    <Grid>
        <honua:HonuaMapView x:Name="MapView"
                           MapType="Satellite"
                           ZoomLevel="15"
                           Center="{Binding CurrentLocation}"
                           MapTapped="OnMapTapped" />
    </Grid>
</ContentPage>
```

3. **Handle location services**:

```csharp
public partial class MapPage : ContentPage
{
    private readonly ILocationService _locationService;

    public MapPage(ILocationService locationService)
    {
        InitializeComponent();
        _locationService = locationService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var location = await _locationService.GetCurrentLocationAsync();
        if (location != null)
        {
            MapView.Center = location;
        }
    }
}
```

## Platform Requirements

| Platform | Minimum Version | Notes |
|----------|----------------|-------|
| **Android** | API 21 (Android 5.0) | Requires Google Play Services |
| **iOS** | iOS 11.0+ | Requires location permissions |
| **Windows** | Windows 10 v1809+ | UWP and WinUI 3 supported |
| **macOS** | macOS 10.15+ | Via Mac Catalyst |

## Development Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 17.8+ or Visual Studio Code
- Platform-specific SDKs:
  - **Android**: Android SDK 34+, Java 11+
  - **iOS**: Xcode 15+, macOS development machine
  - **Windows**: Windows 10 SDK 19041+

### Platform Setup

For detailed platform setup instructions, see [Platform Setup Guide](docs/platform-setup.md).

### Build and Run

```bash
# Clone the repository
git clone https://github.com/mikemcdougall/honua-mobile-sdk.git
cd honua-mobile-sdk

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run example app (Android)
dotnet build examples/FieldDataCollection -f net8.0-android
```

## Architecture

The Honua Mobile SDK is built on .NET MAUI and follows a layered architecture:

```
┌─────────────────────────────────────┐
│           MAUI App Layer            │
│     (Your Application Code)        │
├─────────────────────────────────────┤
│          Honua.Mobile SDK           │
│  ┌─────────────┬─────────────────┐  │
│  │  Controls   │    Services     │  │
│  │  ┌────────┐ │  ┌────────────┐ │  │
│  │  │ Map    │ │  │ Location   │ │  │
│  │  │ Camera │ │  │ Storage    │ │  │
│  │  │ Forms  │ │  │ Sync       │ │  │
│  │  └────────┘ │  └────────────┘ │  │
│  └─────────────┴─────────────────┘  │
├─────────────────────────────────────┤
│       Platform Handlers             │
│  ┌─────┬─────┬─────┬─────────────┐  │
│  │ iOS │ And │ Win │   macOS     │  │
│  └─────┴─────┴─────┴─────────────┘  │
├─────────────────────────────────────┤
│           .NET MAUI                 │
└─────────────────────────────────────┘
```

## Examples

### Field Data Collection App
A complete reference implementation demonstrating:
- Custom form builders
- Photo capture with geotagging
- Offline data collection
- Background synchronization

See the [Field Data Collection Example](examples/FieldDataCollection/) for the complete source code.

### Common Use Cases

#### Custom Map Layers
```csharp
// Add a feature layer to the map
var layer = new FeatureLayer
{
    Id = "assets",
    Name = "Infrastructure Assets",
    ServiceUrl = "https://your-server.com/features/assets"
};

MapView.AddLayer(layer);
```

#### Offline Data Download
```csharp
// Download data for offline use
var area = new GeographicArea
{
    MinLatitude = 40.7128,
    MaxLatitude = 40.7589,
    MinLongitude = -74.0060,
    MaxLongitude = -73.9352
};

var result = await _offlineSyncService.DownloadDataForOfflineUseAsync(area);
if (result.Success)
{
    Console.WriteLine($"Downloaded {result.FeaturesDownloaded} features");
}
```

#### Photo Capture with Metadata
```csharp
var options = new CaptureOptions
{
    Quality = 90,
    IncludeExifData = true,
    Resolution = new Size(1920, 1080)
};

var photo = await CameraView.CapturePhotoAsync(options);
Console.WriteLine($"Captured at: {photo.GpsLocation}");
```

## Documentation

- [Getting Started Guide](docs/getting-started.md)
- [Platform Setup Instructions](docs/platform-setup.md)
- [API Documentation](https://docs.honua.dev/mobile-sdk/)
- [Sample Applications](examples/)

## Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Workflow

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

The Apache 2.0 license allows for commercial use, modification, distribution, and private use while providing patent protection and requiring attribution.

## Support

- 📚 [Documentation](https://docs.honua.dev/mobile-sdk/)
- 💬 [Discussions](https://github.com/mikemcdougall/honua-mobile-sdk/discussions)
- 🐛 [Issue Tracker](https://github.com/mikemcdougall/honua-mobile-sdk/issues)
- 📧 [Email Support](mailto:support@honua.dev)

## Ecosystem

Part of the Honua geospatial platform:

- **[honua-server](https://github.com/mikemcdougall/honua-server)** - Core geospatial server
- **[honua-core-sdk](https://github.com/mikemcdougall/honua-core-sdk)** - .NET SDK for server integration
- **[honua-admin-tools](https://github.com/mikemcdougall/honua-admin-tools)** - DevOps and administration tools
- **[geospatial-grpc](https://github.com/mikemcdougall/geospatial-grpc)** - gRPC protocol definitions

---

**Built with ❤️ for the geospatial community**