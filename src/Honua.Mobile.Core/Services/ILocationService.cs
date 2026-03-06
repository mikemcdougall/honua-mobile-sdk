using Honua.Mobile.Models;

namespace Honua.Mobile.Services;

/// <summary>
/// Service for managing GPS location tracking and geolocation features
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Get the current device location
    /// </summary>
    /// <param name="options">Location tracking options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current location or null if unavailable</returns>
    Task<Location?> GetCurrentLocationAsync(LocationOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Start continuous location tracking
    /// </summary>
    /// <param name="options">Location tracking options</param>
    /// <param name="callback">Callback for location updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tracking session ID</returns>
    Task<string> StartLocationTrackingAsync(LocationOptions options, Action<LocationUpdateEventArgs> callback, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop continuous location tracking
    /// </summary>
    /// <param name="sessionId">Tracking session ID</param>
    Task StopLocationTrackingAsync(string sessionId);

    /// <summary>
    /// Stop all location tracking sessions
    /// </summary>
    Task StopAllLocationTrackingAsync();

    /// <summary>
    /// Check if location services are enabled and permissions are granted
    /// </summary>
    Task<LocationPermissionStatus> GetLocationPermissionStatusAsync();

    /// <summary>
    /// Request location permissions from the user
    /// </summary>
    /// <param name="requestAlwaysPermission">Whether to request "always" permission for background tracking</param>
    Task<LocationPermissionStatus> RequestLocationPermissionsAsync(bool requestAlwaysPermission = false);

    /// <summary>
    /// Check if location services are enabled on the device
    /// </summary>
    Task<bool> IsLocationServicesEnabledAsync();

    /// <summary>
    /// Get the last known location (cached)
    /// </summary>
    Task<Location?> GetLastKnownLocationAsync();

    /// <summary>
    /// Calculate distance between two locations
    /// </summary>
    /// <param name="location1">First location</param>
    /// <param name="location2">Second location</param>
    /// <returns>Distance in meters</returns>
    double CalculateDistance(Location location1, Location location2);

    /// <summary>
    /// Calculate bearing from one location to another
    /// </summary>
    /// <param name="from">Starting location</param>
    /// <param name="to">Destination location</param>
    /// <returns>Bearing in degrees (0-360)</returns>
    double CalculateBearing(Location from, Location to);

    /// <summary>
    /// Get current tracking status
    /// </summary>
    LocationTrackingStatus TrackingStatus { get; }

    /// <summary>
    /// Event raised when location tracking status changes
    /// </summary>
    event EventHandler<LocationTrackingStatusChangedEventArgs>? TrackingStatusChanged;

    /// <summary>
    /// Event raised when location permission status changes
    /// </summary>
    event EventHandler<LocationPermissionChangedEventArgs>? PermissionStatusChanged;
}

/// <summary>
/// Event arguments for location updates
/// </summary>
public class LocationUpdateEventArgs : EventArgs
{
    public Location Location { get; }
    public string SessionId { get; }
    public LocationTrackingStatus Status { get; }
    public string? Error { get; }

    public LocationUpdateEventArgs(Location location, string sessionId, LocationTrackingStatus status = LocationTrackingStatus.Running, string? error = null)
    {
        Location = location;
        SessionId = sessionId;
        Status = status;
        Error = error;
    }
}

/// <summary>
/// Event arguments for tracking status changes
/// </summary>
public class LocationTrackingStatusChangedEventArgs : EventArgs
{
    public LocationTrackingStatus OldStatus { get; }
    public LocationTrackingStatus NewStatus { get; }
    public string? SessionId { get; }

    public LocationTrackingStatusChangedEventArgs(LocationTrackingStatus oldStatus, LocationTrackingStatus newStatus, string? sessionId = null)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
        SessionId = sessionId;
    }
}

/// <summary>
/// Event arguments for permission status changes
/// </summary>
public class LocationPermissionChangedEventArgs : EventArgs
{
    public LocationPermissionStatus OldStatus { get; }
    public LocationPermissionStatus NewStatus { get; }

    public LocationPermissionChangedEventArgs(LocationPermissionStatus oldStatus, LocationPermissionStatus newStatus)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}