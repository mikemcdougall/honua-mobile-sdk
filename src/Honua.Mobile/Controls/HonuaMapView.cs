using Microsoft.Maui.Handlers;
using System.Collections.ObjectModel;
using Honua.Mobile.Models;

namespace Honua.Mobile.Controls;

/// <summary>
/// Cross-platform map control for displaying geospatial data
/// </summary>
public class HonuaMapView : View, IHonuaMapView
{
    public static readonly BindableProperty CenterProperty =
        BindableProperty.Create(nameof(Center), typeof(Location), typeof(HonuaMapView), default(Location));

    public static readonly BindableProperty ZoomLevelProperty =
        BindableProperty.Create(nameof(ZoomLevel), typeof(double), typeof(HonuaMapView), 10.0);

    public static readonly BindableProperty MapTypeProperty =
        BindableProperty.Create(nameof(MapType), typeof(MapType), typeof(HonuaMapView), MapType.Satellite);

    public static readonly BindableProperty IsRotationEnabledProperty =
        BindableProperty.Create(nameof(IsRotationEnabled), typeof(bool), typeof(HonuaMapView), true);

    public static readonly BindableProperty IsZoomEnabledProperty =
        BindableProperty.Create(nameof(IsZoomEnabled), typeof(bool), typeof(HonuaMapView), true);

    public static readonly BindableProperty IsPanEnabledProperty =
        BindableProperty.Create(nameof(IsPanEnabled), typeof(bool), typeof(HonuaMapView), true);

    /// <summary>
    /// Current center location of the map
    /// </summary>
    public Location Center
    {
        get => (Location)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    /// <summary>
    /// Current zoom level of the map
    /// </summary>
    public double ZoomLevel
    {
        get => (double)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    /// <summary>
    /// Type of map display (satellite, road, hybrid)
    /// </summary>
    public MapType MapType
    {
        get => (MapType)GetValue(MapTypeProperty);
        set => SetValue(MapTypeProperty, value);
    }

    /// <summary>
    /// Whether map rotation is enabled
    /// </summary>
    public bool IsRotationEnabled
    {
        get => (bool)GetValue(IsRotationEnabledProperty);
        set => SetValue(IsRotationEnabledProperty, value);
    }

    /// <summary>
    /// Whether map zooming is enabled
    /// </summary>
    public bool IsZoomEnabled
    {
        get => (bool)GetValue(IsZoomEnabledProperty);
        set => SetValue(IsZoomEnabledProperty, value);
    }

    /// <summary>
    /// Whether map panning is enabled
    /// </summary>
    public bool IsPanEnabled
    {
        get => (bool)GetValue(IsPanEnabledProperty);
        set => SetValue(IsPanEnabledProperty, value);
    }

    /// <summary>
    /// Event raised when the map is tapped
    /// </summary>
    public event EventHandler<MapTappedEventArgs>? MapTapped;

    /// <summary>
    /// Event raised when the map camera changes
    /// </summary>
    public event EventHandler<MapCameraChangedEventArgs>? CameraChanged;

    /// <summary>
    /// Event raised when a feature is selected
    /// </summary>
    public event EventHandler<FeatureSelectedEventArgs>? FeatureSelected;

    /// <summary>
    /// Collection of map layers
    /// </summary>
    public ObservableCollection<IMapLayer> Layers { get; } = new();

    /// <summary>
    /// Collection of map overlays
    /// </summary>
    public ObservableCollection<IMapOverlay> Overlays { get; } = new();

    /// <summary>
    /// Add a feature layer to the map
    /// </summary>
    /// <param name="layer">Feature layer to add</param>
    public void AddLayer(IMapLayer layer)
    {
        Layers.Add(layer);
    }

    /// <summary>
    /// Remove a feature layer from the map
    /// </summary>
    /// <param name="layer">Feature layer to remove</param>
    public void RemoveLayer(IMapLayer layer)
    {
        Layers.Remove(layer);
    }

    /// <summary>
    /// Add an overlay to the map
    /// </summary>
    /// <param name="overlay">Overlay to add</param>
    public void AddOverlay(IMapOverlay overlay)
    {
        Overlays.Add(overlay);
    }

    /// <summary>
    /// Remove an overlay from the map
    /// </summary>
    /// <param name="overlay">Overlay to remove</param>
    public void RemoveOverlay(IMapOverlay overlay)
    {
        Overlays.Remove(overlay);
    }

    /// <summary>
    /// Animate the map to a new location
    /// </summary>
    /// <param name="location">Target location</param>
    /// <param name="zoomLevel">Target zoom level</param>
    /// <param name="animated">Whether to animate the transition</param>
    public void MoveToLocation(Location location, double? zoomLevel = null, bool animated = true)
    {
        Handler?.Invoke(nameof(MoveToLocation), new { location, zoomLevel, animated });
    }

    /// <summary>
    /// Fit the map to show all features in the specified bounds
    /// </summary>
    /// <param name="bounds">Geographic bounds</param>
    /// <param name="padding">Padding around the bounds</param>
    /// <param name="animated">Whether to animate the transition</param>
    public void FitToBounds(GeographicBounds bounds, Thickness padding = default, bool animated = true)
    {
        Handler?.Invoke(nameof(FitToBounds), new { bounds, padding, animated });
    }

    public virtual void OnMapTapped(MapTappedEventArgs e) => MapTapped?.Invoke(this, e);
    public virtual void OnCameraChanged(MapCameraChangedEventArgs e) => CameraChanged?.Invoke(this, e);
    public virtual void OnFeatureSelected(FeatureSelectedEventArgs e) => FeatureSelected?.Invoke(this, e);
}

/// <summary>
/// Interface for the map view
/// </summary>
public interface IHonuaMapView : IView
{
    Location Center { get; }
    double ZoomLevel { get; }
    MapType MapType { get; }
    ObservableCollection<IMapLayer> Layers { get; }
    ObservableCollection<IMapOverlay> Overlays { get; }
}

/// <summary>
/// Map display types
/// </summary>
public enum MapType
{
    Road,
    Satellite,
    Hybrid,
    Terrain
}

/// <summary>
/// Event arguments for map tap events
/// </summary>
public class MapTappedEventArgs : EventArgs
{
    public Location Location { get; init; } = default!;
    public Point ScreenPosition { get; init; }
}

/// <summary>
/// Event arguments for map camera change events
/// </summary>
public class MapCameraChangedEventArgs : EventArgs
{
    public Location Center { get; init; } = default!;
    public double ZoomLevel { get; init; }
    public double Rotation { get; init; }
    public GeographicBounds VisibleRegion { get; init; } = default!;
}

/// <summary>
/// Event arguments for feature selection events
/// </summary>
public class FeatureSelectedEventArgs : EventArgs
{
    public string FeatureId { get; init; } = string.Empty;
    public string LayerId { get; init; } = string.Empty;
    public Dictionary<string, object> Attributes { get; init; } = new();
}

/// <summary>
/// Geographic bounds
/// </summary>
public record GeographicBounds
{
    public double North { get; init; }
    public double South { get; init; }
    public double East { get; init; }
    public double West { get; init; }
}

/// <summary>
/// Map layer interface
/// </summary>
public interface IMapLayer
{
    string Id { get; }
    string Name { get; }
    bool IsVisible { get; set; }
    double Opacity { get; set; }
}

/// <summary>
/// Map overlay interface
/// </summary>
public interface IMapOverlay
{
    string Id { get; }
    bool IsVisible { get; set; }
}