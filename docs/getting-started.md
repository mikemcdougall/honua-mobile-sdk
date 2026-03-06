# Getting Started with Honua Mobile SDK

This guide will walk you through setting up and using the Honua Mobile SDK in your .NET MAUI application for geospatial field data collection.

## Prerequisites

Before you begin, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022 17.8+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension
- Platform-specific tools (see [Platform Setup](platform-setup.md))
- A Honua server instance (local or cloud)

## Installation

### 1. Install the NuGet Package

Add the Honua Mobile SDK to your MAUI project:

```bash
dotnet add package Honua.Mobile
```

Or via Package Manager UI in Visual Studio:
```
Install-Package Honua.Mobile
```

### 2. Configure MauiProgram.cs

Update your `MauiProgram.cs` to initialize the Honua SDK:

```csharp
using Honua.Mobile.Extensions;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseHonuaMobile() // Initialize Honua Mobile SDK
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Configure Honua services
        builder.Services.AddHonuaMobile(options =>
        {
            options.ServerUrl = "https://your-honua-server.com";
            options.ApiKey = "your-api-key";
            options.EnableOfflineSync = true;
            options.OfflineDatabasePath = "honua_offline.db";
            options.AutoSyncInterval = TimeSpan.FromMinutes(5);
        });

        return builder.Build();
    }
}
```

### 3. Add Platform Permissions

Configure platform-specific permissions for location, camera, and storage access.

#### Android (`Platforms/Android/AndroidManifest.xml`)

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- For background location (optional) -->
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />

<!-- Google Maps API Key (replace with your key) -->
<meta-data android:name="com.google.android.geo.API_KEY"
           android:value="YOUR_GOOGLE_MAPS_API_KEY" />
```

#### iOS (`Platforms/iOS/Info.plist`)

```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This app needs location access to collect field data with GPS coordinates.</string>

<key>NSLocationAlwaysAndWhenInUseUsageDescription</key>
<string>This app needs location access to enable offline data synchronization.</string>

<key>NSCameraUsageDescription</key>
<string>This app needs camera access to capture photos for field data collection.</string>

<key>NSMicrophoneUsageDescription</key>
<string>This app needs microphone access to record audio notes.</string>

<key>NSPhotoLibraryUsageDescription</key>
<string>This app needs photo library access to save captured images.</string>
```

## Basic Implementation

### 1. Create a Map Page

Create a new content page with the Honua map control:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage x:Class="YourApp.Pages.MapPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:honua="clr-namespace:Honua.Mobile.Controls;assembly=Honua.Mobile"
             Title="Field Map">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <!-- Map Control -->
        <honua:HonuaMapView x:Name="MapView"
                           Grid.Row="0"
                           MapType="Satellite"
                           ZoomLevel="15"
                           Center="{Binding CurrentLocation}"
                           IsZoomEnabled="True"
                           IsPanEnabled="True"
                           IsRotationEnabled="True"
                           MapTapped="OnMapTapped"
                           FeatureSelected="OnFeatureSelected" />

        <!-- Toolbar -->
        <StackLayout Grid.Row="1" Orientation="Horizontal" Padding="10">
            <Button Text="My Location" Clicked="OnMyLocationClicked" />
            <Button Text="Add Point" Clicked="OnAddPointClicked" />
            <Button Text="Sync Data" Clicked="OnSyncDataClicked" />
        </StackLayout>
    </Grid>

</ContentPage>
```

### 2. Implement the Code-Behind

```csharp
using Honua.Mobile.Controls;
using Honua.Mobile.Services;

namespace YourApp.Pages;

public partial class MapPage : ContentPage
{
    private readonly ILocationService _locationService;
    private readonly IOfflineSyncService _syncService;
    private readonly IOfflineStorage _storage;

    public MapPage(ILocationService locationService,
                   IOfflineSyncService syncService,
                   IOfflineStorage storage)
    {
        InitializeComponent();
        _locationService = locationService;
        _syncService = syncService;
        _storage = storage;

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Request location permissions
        var hasPermission = await _locationService.RequestLocationPermissionsAsync();
        if (hasPermission)
        {
            await LoadCurrentLocation();
            await LoadOfflineData();
        }
    }

    private async Task LoadCurrentLocation()
    {
        try
        {
            var location = await _locationService.GetCurrentLocationAsync();
            if (location != null)
            {
                MapView.Center = location;
                MapView.ZoomLevel = 16;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to get location: {ex.Message}", "OK");
        }
    }

    private async Task LoadOfflineData()
    {
        // Load offline features in the current map area
        var bounds = new GeographicArea
        {
            MinLatitude = MapView.Center.Latitude - 0.01,
            MaxLatitude = MapView.Center.Latitude + 0.01,
            MinLongitude = MapView.Center.Longitude - 0.01,
            MaxLongitude = MapView.Center.Longitude + 0.01
        };

        var features = await _storage.GetFeaturesInAreaAsync(bounds);

        // Add features to map (implementation depends on your layer structure)
        foreach (var feature in features)
        {
            // Add feature to appropriate map layer
        }
    }

    private async void OnMapTapped(object sender, MapTappedEventArgs e)
    {
        // Handle map taps - could show context menu, add points, etc.
        var location = e.Location;
        await DisplayAlert("Map Tapped",
                          $"Lat: {location.Latitude:F6}, Lon: {location.Longitude:F6}",
                          "OK");
    }

    private void OnFeatureSelected(object sender, FeatureSelectedEventArgs e)
    {
        // Handle feature selection - show details, edit, etc.
        var featureId = e.FeatureId;
        // Navigate to feature details page or show popup
    }

    private async void OnMyLocationClicked(object sender, EventArgs e)
    {
        var location = await _locationService.GetCurrentLocationAsync();
        if (location != null)
        {
            MapView.MoveToLocation(location, 16, animated: true);
        }
    }

    private async void OnAddPointClicked(object sender, EventArgs e)
    {
        // Navigate to data collection form
        await Shell.Current.GoToAsync("datacollection");
    }

    private async void OnSyncDataClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await _syncService.SyncAsync();
            if (result.Success)
            {
                await DisplayAlert("Sync Complete",
                                 $"Uploaded: {result.UploadedChanges}, Downloaded: {result.DownloadedChanges}",
                                 "OK");
            }
            else
            {
                await DisplayAlert("Sync Failed", result.ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Sync Error", ex.Message, "OK");
        }
    }
}
```

### 3. Create a Data Collection Form

Create a simple data collection form:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage x:Class="YourApp.Pages.DataCollectionPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:honua="clr-namespace:Honua.Mobile.Controls;assembly=Honua.Mobile"
             Title="Collect Data">

    <ScrollView>
        <StackLayout Padding="20">

            <!-- Location Info -->
            <Frame BackgroundColor="LightBlue" Padding="10">
                <StackLayout>
                    <Label Text="Location" FontAttributes="Bold" />
                    <Label Text="{Binding CurrentLocationText}" />
                </StackLayout>
            </Frame>

            <!-- Basic Form Fields -->
            <Label Text="Asset Type" />
            <Picker x:Name="AssetTypePicker" ItemsSource="{Binding AssetTypes}" />

            <Label Text="Condition" />
            <Picker x:Name="ConditionPicker" ItemsSource="{Binding Conditions}" />

            <Label Text="Notes" />
            <Editor x:Name="NotesEditor" Placeholder="Enter notes..." HeightRequest="100" />

            <!-- Photo Capture -->
            <Button Text="Take Photo" Clicked="OnTakePhotoClicked" />
            <Image x:Name="CapturedImage" IsVisible="False" HeightRequest="200" />

            <!-- Action Buttons -->
            <StackLayout Orientation="Horizontal" HorizontalOptions="FillAndExpand">
                <Button Text="Save Draft" Clicked="OnSaveDraftClicked" HorizontalOptions="FillAndExpand" />
                <Button Text="Submit" Clicked="OnSubmitClicked" HorizontalOptions="FillAndExpand" />
            </StackLayout>

        </StackLayout>
    </ScrollView>

</ContentPage>
```

### 4. Implement Data Collection Logic

```csharp
using Honua.Mobile.Services;
using Honua.Mobile.Models;

namespace YourApp.Pages;

public partial class DataCollectionPage : ContentPage
{
    private readonly ILocationService _locationService;
    private readonly IOfflineStorage _storage;
    private Location? _currentLocation;
    private CapturedPhoto? _capturedPhoto;

    public List<string> AssetTypes { get; } = new()
    {
        "Fire Hydrant", "Stop Sign", "Street Light", "Utility Pole"
    };

    public List<string> Conditions { get; } = new()
    {
        "Excellent", "Good", "Fair", "Poor", "Critical"
    };

    public string CurrentLocationText => _currentLocation != null
        ? $"{_currentLocation.Latitude:F6}, {_currentLocation.Longitude:F6}"
        : "Getting location...";

    public DataCollectionPage(ILocationService locationService, IOfflineStorage storage)
    {
        InitializeComponent();
        _locationService = locationService;
        _storage = storage;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCurrentLocation();
    }

    private async Task GetCurrentLocation()
    {
        try
        {
            _currentLocation = await _locationService.GetCurrentLocationAsync();
            OnPropertyChanged(nameof(CurrentLocationText));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Location Error", ex.Message, "OK");
        }
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            // Note: You would need to implement camera functionality
            // This is a simplified example
            var options = new CaptureOptions
            {
                Quality = 80,
                IncludeExifData = true
            };

            // _capturedPhoto = await cameraService.CapturePhotoAsync(options);
            // CapturedImage.Source = ImageSource.FromFile(_capturedPhoto.FilePath);
            // CapturedImage.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Camera Error", ex.Message, "OK");
        }
    }

    private async void OnSaveDraftClicked(object sender, EventArgs e)
    {
        await SaveFieldData(FieldDataStatus.Draft);
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        await SaveFieldData(FieldDataStatus.Completed);
    }

    private async Task SaveFieldData(FieldDataStatus status)
    {
        try
        {
            var fieldData = new FieldDataEntry
            {
                FormId = "asset-inspection",
                CollectionLocation = _currentLocation,
                Status = status,
                Values = new[]
                {
                    new FieldValue
                    {
                        FieldId = "asset_type",
                        Value = AssetTypePicker.SelectedItem?.ToString()
                    },
                    new FieldValue
                    {
                        FieldId = "condition",
                        Value = ConditionPicker.SelectedItem?.ToString()
                    },
                    new FieldValue
                    {
                        FieldId = "notes",
                        Value = NotesEditor.Text
                    }
                }
            };

            // Convert to FeatureData for storage
            var feature = ConvertToFeatureData(fieldData);
            await _storage.StoreFeatureAsync(feature);

            await DisplayAlert("Success", $"Data {status.ToString().ToLower()}", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Save Error", ex.Message, "OK");
        }
    }

    private FeatureData ConvertToFeatureData(FieldDataEntry fieldData)
    {
        return new FeatureData
        {
            Id = fieldData.Id,
            LayerId = "collected-data",
            GeometryJson = fieldData.CollectionLocation != null
                ? $"{{\"type\":\"Point\",\"coordinates\":[{fieldData.CollectionLocation.Longitude},{fieldData.CollectionLocation.Latitude}]}}"
                : "",
            Attributes = fieldData.Values.ToDictionary(v => v.FieldId, v => v.Value ?? ""),
            CreatedAt = fieldData.CollectedAt,
            ModifiedAt = fieldData.CollectedAt,
            IsModified = true
        };
    }
}
```

## Next Steps

Now that you have a basic implementation:

1. **Explore Advanced Features:**
   - [Camera Integration](camera-integration.md)
   - [Offline Synchronization](offline-sync.md)
   - [Custom Map Layers](custom-layers.md)
   - [Form Builder](form-builder.md)

2. **Review Examples:**
   - [Field Data Collection App](../examples/FieldDataCollection/)
   - [Asset Management App](../examples/AssetManagement/)

3. **Configure for Production:**
   - [Platform Setup Guide](platform-setup.md)
   - [Performance Optimization](performance.md)
   - [Security Best Practices](security.md)

## Troubleshooting

### Common Issues

**Location Services Not Working:**
- Ensure location permissions are properly configured
- Check that GPS is enabled on the device
- Verify network connectivity for assisted GPS

**Map Not Loading:**
- Verify Google Maps API key (Android)
- Check internet connectivity
- Ensure proper platform setup

**Sync Failing:**
- Verify server URL and API key
- Check network connectivity
- Review server logs for errors

### Getting Help

- 📚 [Full Documentation](https://docs.honua.dev/mobile-sdk/)
- 💬 [Community Discussions](https://github.com/mikemcdougall/honua-mobile-sdk/discussions)
- 🐛 [Report Issues](https://github.com/mikemcdougall/honua-mobile-sdk/issues)

---

Continue to [Platform Setup Guide](platform-setup.md) for detailed platform configuration.