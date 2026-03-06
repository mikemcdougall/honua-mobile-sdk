using System.Collections.ObjectModel;

namespace Honua.Mobile.Models;

/// <summary>
/// Represents a geographic location with latitude and longitude coordinates
/// </summary>
public record Location
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? Altitude { get; init; }
    public double? Accuracy { get; init; }
    public double? VerticalAccuracy { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public Location() { }

    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public Location(double latitude, double longitude, double? altitude)
        : this(latitude, longitude)
    {
        Altitude = altitude;
    }

    /// <summary>
    /// Calculate distance to another location in meters
    /// </summary>
    public double DistanceTo(Location other)
    {
        const double EarthRadiusKm = 6371.0;

        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c * 1000; // Convert to meters
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

    public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
}

/// <summary>
/// Geographic bounds defined by north, south, east, and west coordinates
/// </summary>
public record GeographicBounds
{
    public double North { get; init; }
    public double South { get; init; }
    public double East { get; init; }
    public double West { get; init; }

    public GeographicBounds() { }

    public GeographicBounds(double north, double south, double east, double west)
    {
        North = north;
        South = south;
        East = east;
        West = west;
    }

    /// <summary>
    /// Center point of the bounds
    /// </summary>
    public Location Center => new(
        (North + South) / 2,
        (East + West) / 2
    );

    /// <summary>
    /// Check if a location is within these bounds
    /// </summary>
    public bool Contains(Location location)
    {
        return location.Latitude >= South &&
               location.Latitude <= North &&
               location.Longitude >= West &&
               location.Longitude <= East;
    }

    /// <summary>
    /// Expand bounds to include a location
    /// </summary>
    public GeographicBounds Include(Location location)
    {
        return new GeographicBounds(
            Math.Max(North, location.Latitude),
            Math.Min(South, location.Latitude),
            Math.Max(East, location.Longitude),
            Math.Min(West, location.Longitude)
        );
    }
}

/// <summary>
/// Geographic area for offline data operations
/// </summary>
public record GeographicArea
{
    public double MinLatitude { get; init; }
    public double MaxLatitude { get; init; }
    public double MinLongitude { get; init; }
    public double MaxLongitude { get; init; }
    public string? Name { get; init; }
    public int? ZoomLevel { get; init; }

    public GeographicArea() { }

    public GeographicArea(double minLat, double maxLat, double minLon, double maxLon)
    {
        MinLatitude = minLat;
        MaxLatitude = maxLat;
        MinLongitude = minLon;
        MaxLongitude = maxLon;
    }

    /// <summary>
    /// Convert to geographic bounds
    /// </summary>
    public GeographicBounds ToBounds() => new(MaxLatitude, MinLatitude, MaxLongitude, MinLongitude);

    /// <summary>
    /// Calculate area in square meters (approximate)
    /// </summary>
    public double AreaSquareMeters()
    {
        var topLeft = new Location(MaxLatitude, MinLongitude);
        var topRight = new Location(MaxLatitude, MaxLongitude);
        var bottomLeft = new Location(MinLatitude, MinLongitude);

        var width = topLeft.DistanceTo(topRight);
        var height = topLeft.DistanceTo(bottomLeft);

        return width * height;
    }
}

/// <summary>
/// Location tracking options
/// </summary>
public record LocationOptions
{
    public LocationAccuracy DesiredAccuracy { get; init; } = LocationAccuracy.Medium;
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
    public bool EnableHighAccuracy { get; init; } = false;
    public bool EnableBackgroundUpdates { get; init; } = false;
    public double? DistanceThreshold { get; init; }
    public TimeSpan? UpdateInterval { get; init; }
}

/// <summary>
/// Location accuracy levels
/// </summary>
public enum LocationAccuracy
{
    Lowest,
    Low,
    Medium,
    High,
    Best
}

/// <summary>
/// Location permissions status
/// </summary>
public enum LocationPermissionStatus
{
    Unknown,
    Denied,
    Granted,
    Restricted,
    GrantedWhenInUse,
    GrantedAlways
}

/// <summary>
/// Location tracking status
/// </summary>
public enum LocationTrackingStatus
{
    Stopped,
    Starting,
    Running,
    Paused,
    Error
}