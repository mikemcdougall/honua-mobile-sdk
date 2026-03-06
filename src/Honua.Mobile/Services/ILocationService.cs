namespace Honua.Mobile.Services;

/// <summary>
/// Service for managing GPS location tracking and geolocation features
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Get the current device location
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current location or null if unavailable</returns>
    Task<Location?> GetCurrentLocationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Start continuous location tracking
    /// </summary>
    /// <param name="accuracy">Desired location accuracy</param>
    /// <param name="callback">Callback for location updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StartLocationTrackingAsync(LocationAccuracy accuracy, Action<Location> callback, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop continuous location tracking
    /// </summary>
    Task StopLocationTrackingAsync();

    /// <summary>
    /// Check if location services are enabled and permissions are granted
    /// </summary>
    Task<bool> IsLocationAvailableAsync();

    /// <summary>
    /// Request location permissions from the user
    /// </summary>
    Task<bool> RequestLocationPermissionsAsync();
}

/// <summary>
/// Location accuracy levels
/// </summary>
public enum LocationAccuracy
{
    /// <summary>
    /// Lowest accuracy, best battery life
    /// </summary>
    Low,

    /// <summary>
    /// Balanced accuracy and battery usage
    /// </summary>
    Medium,

    /// <summary>
    /// Highest accuracy, most battery usage
    /// </summary>
    High
}

/// <summary>
/// Represents a geographic location
/// </summary>
public record Location
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? Altitude { get; init; }
    public double? Accuracy { get; init; }
    public double? Bearing { get; init; }
    public double? Speed { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}