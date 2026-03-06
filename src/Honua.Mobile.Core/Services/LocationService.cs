using Honua.Mobile.Models;

namespace Honua.Mobile.Services;

/// <summary>
/// Default implementation of ILocationService
/// </summary>
public class LocationService : ILocationService
{
    private readonly Dictionary<string, CancellationTokenSource> _trackingSessions = new();
    private LocationTrackingStatus _trackingStatus = LocationTrackingStatus.Stopped;

    public LocationTrackingStatus TrackingStatus => _trackingStatus;

    public event EventHandler<LocationTrackingStatusChangedEventArgs>? TrackingStatusChanged;
    public event EventHandler<LocationPermissionChangedEventArgs>? PermissionStatusChanged;

    public async Task<Location?> GetCurrentLocationAsync(LocationOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Platform-specific implementation would be injected here
        // For now, return a mock location
        await Task.Delay(100, cancellationToken);

        return new Location(47.6062, -122.3321) // Seattle coordinates
        {
            Altitude = 56.0,
            Accuracy = 10.0,
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<string> StartLocationTrackingAsync(LocationOptions options, Action<LocationUpdateEventArgs> callback, CancellationToken cancellationToken = default)
    {
        var sessionId = Guid.NewGuid().ToString();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _trackingSessions[sessionId] = cts;

        var oldStatus = _trackingStatus;
        _trackingStatus = LocationTrackingStatus.Starting;
        TrackingStatusChanged?.Invoke(this, new LocationTrackingStatusChangedEventArgs(oldStatus, _trackingStatus, sessionId));

        // Start background tracking simulation
        _ = Task.Run(async () =>
        {
            try
            {
                _trackingStatus = LocationTrackingStatus.Running;
                TrackingStatusChanged?.Invoke(this, new LocationTrackingStatusChangedEventArgs(LocationTrackingStatus.Starting, _trackingStatus, sessionId));

                var random = new Random();
                var baseLat = 47.6062;
                var baseLng = -122.3321;

                while (!cts.Token.IsCancellationRequested)
                {
                    // Simulate location updates
                    var location = new Location(
                        baseLat + (random.NextDouble() - 0.5) * 0.01,
                        baseLng + (random.NextDouble() - 0.5) * 0.01)
                    {
                        Altitude = 56.0 + (random.NextDouble() - 0.5) * 10,
                        Accuracy = 5.0 + random.NextDouble() * 10,
                        Timestamp = DateTime.UtcNow
                    };

                    var args = new LocationUpdateEventArgs(location, sessionId, LocationTrackingStatus.Running);
                    callback(args);

                    // Update interval based on options
                    var delay = options.UpdateInterval ?? TimeSpan.FromSeconds(5);
                    await Task.Delay(delay, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when tracking is stopped
            }
            finally
            {
                _trackingSessions.Remove(sessionId);
                if (_trackingSessions.Count == 0)
                {
                    var oldStatus = _trackingStatus;
                    _trackingStatus = LocationTrackingStatus.Stopped;
                    TrackingStatusChanged?.Invoke(this, new LocationTrackingStatusChangedEventArgs(oldStatus, _trackingStatus, sessionId));
                }
            }
        }, cts.Token);

        return sessionId;
    }

    public async Task StopLocationTrackingAsync(string sessionId)
    {
        if (_trackingSessions.TryGetValue(sessionId, out var cts))
        {
            cts.Cancel();
            _trackingSessions.Remove(sessionId);
        }

        await Task.CompletedTask;
    }

    public async Task StopAllLocationTrackingAsync()
    {
        foreach (var cts in _trackingSessions.Values)
        {
            cts.Cancel();
        }

        _trackingSessions.Clear();

        var oldStatus = _trackingStatus;
        _trackingStatus = LocationTrackingStatus.Stopped;
        TrackingStatusChanged?.Invoke(this, new LocationTrackingStatusChangedEventArgs(oldStatus, _trackingStatus));

        await Task.CompletedTask;
    }

    public async Task<LocationPermissionStatus> GetLocationPermissionStatusAsync()
    {
        // Platform-specific implementation would check actual permissions
        await Task.Delay(50);
        return LocationPermissionStatus.Granted;
    }

    public async Task<LocationPermissionStatus> RequestLocationPermissionsAsync(bool requestAlwaysPermission = false)
    {
        // Platform-specific implementation would show permission dialog
        await Task.Delay(100);
        return LocationPermissionStatus.Granted;
    }

    public async Task<bool> IsLocationServicesEnabledAsync()
    {
        // Platform-specific implementation would check system settings
        await Task.Delay(50);
        return true;
    }

    public async Task<Location?> GetLastKnownLocationAsync()
    {
        // Platform-specific implementation would return cached location
        await Task.Delay(50);

        return new Location(47.6062, -122.3321)
        {
            Altitude = 56.0,
            Accuracy = 50.0, // Lower accuracy for cached location
            Timestamp = DateTime.UtcNow.AddMinutes(-5) // Older timestamp
        };
    }

    public double CalculateDistance(Location location1, Location location2)
    {
        return location1.DistanceTo(location2);
    }

    public double CalculateBearing(Location from, Location to)
    {
        var lat1Rad = DegreesToRadians(from.Latitude);
        var lat2Rad = DegreesToRadians(to.Latitude);
        var deltaLngRad = DegreesToRadians(to.Longitude - from.Longitude);

        var y = Math.Sin(deltaLngRad) * Math.Cos(lat2Rad);
        var x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) - Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(deltaLngRad);

        var bearing = RadiansToDegrees(Math.Atan2(y, x));
        return (bearing + 360) % 360; // Normalize to 0-360
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    private static double RadiansToDegrees(double radians) => radians * 180.0 / Math.PI;
}