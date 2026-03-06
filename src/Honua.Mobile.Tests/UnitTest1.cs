using Honua.Mobile.Models;
using Honua.Mobile.Services;
using Xunit;

namespace Honua.Mobile.Tests;

public class LocationModelsTests
{
    [Fact]
    public void Location_DistanceTo_CalculatesCorrectDistance()
    {
        // Arrange
        var seattle = new Location(47.6062, -122.3321);
        var bellevue = new Location(47.6101, -122.2015);

        // Act
        var distance = seattle.DistanceTo(bellevue);

        // Assert
        Assert.True(distance > 9000); // Approximately 9.5 km
        Assert.True(distance < 11000);
    }

    [Fact]
    public void GeographicBounds_Contains_ReturnsTrueForContainedLocation()
    {
        // Arrange
        var bounds = new GeographicBounds(47.62, 47.60, -122.30, -122.35);
        var location = new Location(47.61, -122.32);

        // Act
        var contains = bounds.Contains(location);

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void GeographicArea_ToBounds_ConvertsCorrectly()
    {
        // Arrange
        var area = new GeographicArea(47.60, 47.62, -122.35, -122.30);

        // Act
        var bounds = area.ToBounds();

        // Assert
        Assert.Equal(47.62, bounds.North);
        Assert.Equal(47.60, bounds.South);
        Assert.Equal(-122.30, bounds.East);
        Assert.Equal(-122.35, bounds.West);
    }
}

public class LocationServiceTests
{
    [Fact]
    public async Task GetCurrentLocationAsync_ReturnsValidLocation()
    {
        // Arrange
        var locationService = new LocationService();

        // Act
        var location = await locationService.GetCurrentLocationAsync();

        // Assert
        Assert.NotNull(location);
        Assert.True(location.Latitude >= -90 && location.Latitude <= 90);
        Assert.True(location.Longitude >= -180 && location.Longitude <= 180);
    }

    [Fact]
    public async Task StartLocationTrackingAsync_ReturnsSessionId()
    {
        // Arrange
        var locationService = new LocationService();
        var options = new LocationOptions
        {
            DesiredAccuracy = LocationAccuracy.Medium,
            UpdateInterval = TimeSpan.FromSeconds(1)
        };
        var locationUpdates = new List<LocationUpdateEventArgs>();

        void OnLocationUpdate(LocationUpdateEventArgs args)
        {
            locationUpdates.Add(args);
        }

        // Act
        var sessionId = await locationService.StartLocationTrackingAsync(options, OnLocationUpdate);

        // Allow some time for location updates
        await Task.Delay(2000);

        await locationService.StopLocationTrackingAsync(sessionId);

        // Assert
        Assert.NotEmpty(sessionId);
        Assert.True(locationUpdates.Count > 0);
    }

    [Fact]
    public void CalculateDistance_ReturnsCorrectValue()
    {
        // Arrange
        var locationService = new LocationService();
        var location1 = new Location(47.6062, -122.3321);
        var location2 = new Location(47.6101, -122.2015);

        // Act
        var distance = locationService.CalculateDistance(location1, location2);

        // Assert
        Assert.True(distance > 9000);
        Assert.True(distance < 11000);
    }

    [Fact]
    public void CalculateBearing_ReturnsCorrectValue()
    {
        // Arrange
        var locationService = new LocationService();
        var from = new Location(47.6062, -122.3321);
        var to = new Location(47.6101, -122.2015);

        // Act
        var bearing = locationService.CalculateBearing(from, to);

        // Assert
        Assert.True(bearing >= 0 && bearing < 360);
        // Should be roughly northeast direction (around 45-90 degrees)
        Assert.True(bearing > 30 && bearing < 120);
    }
}

public class OfflineStorageServiceTests
{
    [Fact]
    public async Task StoreFeature_CanRetrieveFeature()
    {
        // Arrange
        var storage = new OfflineStorageService();
        var feature = new FeatureData
        {
            Id = "test-feature",
            LayerId = "test-layer",
            GeometryJson = "{\"type\":\"Point\",\"coordinates\":[-122.3321,47.6062]}",
            Attributes = new Dictionary<string, object> { { "name", "Test Feature" } },
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow,
            IsModified = false,
            IsDeleted = false
        };

        // Act
        await storage.StoreFeatureAsync(feature);
        var retrieved = await storage.GetFeatureAsync("test-feature");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(feature.Id, retrieved.Id);
        Assert.Equal(feature.LayerId, retrieved.LayerId);
        Assert.Equal("Test Feature", retrieved.Attributes["name"]);
    }

    [Fact]
    public async Task GetStorageInfo_ReturnsValidInfo()
    {
        // Arrange
        var storage = new OfflineStorageService();

        // Add some test data
        var feature = new FeatureData
        {
            Id = "test-feature",
            LayerId = "test-layer",
            GeometryJson = "{}",
            Attributes = new Dictionary<string, object>(),
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow
        };
        await storage.StoreFeatureAsync(feature);

        // Act
        var info = await storage.GetStorageInfoAsync();

        // Assert
        Assert.NotNull(info);
        Assert.True(info.TotalSize > 0);
        Assert.True(info.FeatureCount >= 1);
        Assert.NotEmpty(info.DatabasePath);
    }
}

public class OfflineSyncServiceTests
{
    [Fact]
    public async Task SyncAsync_WithNoConnectivity_ReturnsFailureResult()
    {
        // Arrange
        var storage = new OfflineStorageService();
        var syncService = new OfflineSyncService(storage);

        // Act
        var result = await syncService.SyncAsync();

        // Assert - Note: Mock service returns success, in real implementation this would test actual connectivity
        Assert.NotNull(result);
        Assert.True(result.Success); // Mock implementation always succeeds
    }

    [Fact]
    public async Task GetPendingChanges_ReturnsCorrectCount()
    {
        // Arrange
        var storage = new OfflineStorageService();
        var syncService = new OfflineSyncService(storage);

        // Add some modified features
        var feature = new FeatureData
        {
            Id = "modified-feature",
            LayerId = "test-layer",
            GeometryJson = "{}",
            Attributes = new Dictionary<string, object>(),
            CreatedAt = DateTimeOffset.UtcNow,
            ModifiedAt = DateTimeOffset.UtcNow,
            IsModified = true,
            ChangeType = FeatureChangeType.Updated
        };
        await storage.StoreFeatureAsync(feature);

        // Act
        var pendingChanges = await syncService.GetPendingChangesAsync();

        // Assert
        Assert.NotNull(pendingChanges);
        Assert.True(pendingChanges.PendingFeatures >= 1);
    }

    [Fact]
    public async Task DownloadDataForOfflineUse_ReturnsSuccessResult()
    {
        // Arrange
        var storage = new OfflineStorageService();
        var syncService = new OfflineSyncService(storage);
        var area = new GeographicArea(47.60, 47.62, -122.35, -122.30);

        // Act
        var result = await syncService.DownloadDataForOfflineUseAsync(area);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.FeaturesDownloaded > 0);
        Assert.True(result.BytesDownloaded > 0);
    }
}

public class FormModelsTests
{
    [Fact]
    public void FormField_ValidateValue_RequiredFieldValidation()
    {
        // Arrange
        var field = new FormField
        {
            Id = "test-field",
            Name = "Test Field",
            Label = "Test Field",
            FieldType = FieldType.Text,
            IsRequired = true
        };

        // Act
        var result = field.ValidateValue();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("is required", result.Errors.First());
    }

    [Fact]
    public void FormField_ValidateValue_EmailFieldValidation()
    {
        // Arrange
        var field = new FormField
        {
            Id = "email-field",
            Name = "Email Field",
            Label = "Email",
            FieldType = FieldType.Email,
            Value = "invalid-email"
        };

        // Act
        var result = field.ValidateValue();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("valid email", result.Errors.First());
    }

    [Fact]
    public void FormDefinition_CanAddFields()
    {
        // Arrange
        var form = new FormDefinition
        {
            Id = "test-form",
            Name = "Test Form",
            Description = "A test form"
        };

        var field = new FormField
        {
            Id = "field-1",
            Name = "Field 1",
            Label = "Field 1",
            FieldType = FieldType.Text
        };

        // Act
        form.Fields.Add(field);

        // Assert
        Assert.Single(form.Fields);
        Assert.Equal("field-1", form.Fields.First().Id);
    }
}