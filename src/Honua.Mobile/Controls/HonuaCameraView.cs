using System.Collections.ObjectModel;

namespace Honua.Mobile.Controls;

/// <summary>
/// Cross-platform camera control for field data collection and photo capture
/// </summary>
public class HonuaCameraView : View, IHonuaCameraView
{
    public static readonly BindableProperty CameraDirectionProperty =
        BindableProperty.Create(nameof(CameraDirection), typeof(CameraDirection), typeof(HonuaCameraView), CameraDirection.Back);

    public static readonly BindableProperty FlashModeProperty =
        BindableProperty.Create(nameof(FlashMode), typeof(FlashMode), typeof(HonuaCameraView), FlashMode.Auto);

    public static readonly BindableProperty ZoomFactorProperty =
        BindableProperty.Create(nameof(ZoomFactor), typeof(double), typeof(HonuaCameraView), 1.0);

    public static readonly BindableProperty IsGeotaggingEnabledProperty =
        BindableProperty.Create(nameof(IsGeotaggingEnabled), typeof(bool), typeof(HonuaCameraView), true);

    public static readonly BindableProperty ShowOverlayProperty =
        BindableProperty.Create(nameof(ShowOverlay), typeof(bool), typeof(HonuaCameraView), false);

    /// <summary>
    /// Camera direction (front or back)
    /// </summary>
    public CameraDirection CameraDirection
    {
        get => (CameraDirection)GetValue(CameraDirectionProperty);
        set => SetValue(CameraDirectionProperty, value);
    }

    /// <summary>
    /// Flash mode setting
    /// </summary>
    public FlashMode FlashMode
    {
        get => (FlashMode)GetValue(FlashModeProperty);
        set => SetValue(FlashModeProperty, value);
    }

    /// <summary>
    /// Current zoom factor
    /// </summary>
    public double ZoomFactor
    {
        get => (double)GetValue(ZoomFactorProperty);
        set => SetValue(ZoomFactorProperty, value);
    }

    /// <summary>
    /// Whether to include GPS coordinates in captured photos
    /// </summary>
    public bool IsGeotaggingEnabled
    {
        get => (bool)GetValue(IsGeotaggingEnabledProperty);
        set => SetValue(IsGeotaggingEnabledProperty, value);
    }

    /// <summary>
    /// Whether to show camera overlay for field data collection
    /// </summary>
    public bool ShowOverlay
    {
        get => (bool)GetValue(ShowOverlayProperty);
        set => SetValue(ShowOverlayProperty, value);
    }

    /// <summary>
    /// Event raised when a photo is captured
    /// </summary>
    public event EventHandler<PhotoCapturedEventArgs>? PhotoCaptured;

    /// <summary>
    /// Event raised when video recording starts/stops
    /// </summary>
    public event EventHandler<VideoRecordingEventArgs>? VideoRecordingStateChanged;

    /// <summary>
    /// Event raised when camera preview is ready
    /// </summary>
    public event EventHandler<CameraPreviewEventArgs>? PreviewReady;

    /// <summary>
    /// Event raised when an error occurs
    /// </summary>
    public event EventHandler<CameraErrorEventArgs>? ErrorOccurred;

    /// <summary>
    /// Available camera devices
    /// </summary>
    public ObservableCollection<CameraDevice> AvailableCameras { get; } = new();

    /// <summary>
    /// Capture a photo with the current camera settings
    /// </summary>
    /// <param name="options">Capture options</param>
    /// <returns>Captured photo data</returns>
    public async Task<CapturedPhoto> CapturePhotoAsync(CaptureOptions? options = null)
    {
        var tcs = new TaskCompletionSource<CapturedPhoto>();

        void OnPhotoCaptured(object? sender, PhotoCapturedEventArgs e)
        {
            PhotoCaptured -= OnPhotoCaptured;
            if (e.Success)
                tcs.SetResult(e.Photo);
            else
                tcs.SetException(new Exception(e.ErrorMessage ?? "Failed to capture photo"));
        }

        PhotoCaptured += OnPhotoCaptured;
        Handler?.Invoke(nameof(CapturePhotoAsync), options ?? new CaptureOptions());

        return await tcs.Task;
    }

    /// <summary>
    /// Start video recording
    /// </summary>
    /// <param name="options">Recording options</param>
    public void StartVideoRecording(VideoRecordingOptions? options = null)
    {
        Handler?.Invoke(nameof(StartVideoRecording), options ?? new VideoRecordingOptions());
    }

    /// <summary>
    /// Stop video recording
    /// </summary>
    /// <returns>Recorded video data</returns>
    public async Task<RecordedVideo> StopVideoRecordingAsync()
    {
        var tcs = new TaskCompletionSource<RecordedVideo>();

        void OnVideoStateChanged(object? sender, VideoRecordingEventArgs e)
        {
            if (e.State == VideoRecordingState.Stopped)
            {
                VideoRecordingStateChanged -= OnVideoStateChanged;
                tcs.SetResult(e.Video!);
            }
        }

        VideoRecordingStateChanged += OnVideoStateChanged;
        Handler?.Invoke(nameof(StopVideoRecordingAsync));

        return await tcs.Task;
    }

    /// <summary>
    /// Check if camera permission is granted
    /// </summary>
    /// <returns>True if permission is granted</returns>
    public async Task<bool> HasCameraPermissionAsync()
    {
        var result = Handler?.Invoke(nameof(HasCameraPermissionAsync));
        return result is bool permission && permission;
    }

    /// <summary>
    /// Request camera permission from the user
    /// </summary>
    /// <returns>True if permission was granted</returns>
    public async Task<bool> RequestCameraPermissionAsync()
    {
        var result = Handler?.Invoke(nameof(RequestCameraPermissionAsync));
        return result is bool permission && permission;
    }

    /// <summary>
    /// Get available camera devices
    /// </summary>
    /// <returns>List of available camera devices</returns>
    public async Task<IEnumerable<CameraDevice>> GetAvailableCamerasAsync()
    {
        var result = Handler?.Invoke(nameof(GetAvailableCamerasAsync));
        return result as IEnumerable<CameraDevice> ?? Array.Empty<CameraDevice>();
    }

    protected virtual void OnPhotoCaptured(PhotoCapturedEventArgs e) => PhotoCaptured?.Invoke(this, e);
    protected virtual void OnVideoRecordingStateChanged(VideoRecordingEventArgs e) => VideoRecordingStateChanged?.Invoke(this, e);
    protected virtual void OnPreviewReady(CameraPreviewEventArgs e) => PreviewReady?.Invoke(this, e);
    protected virtual void OnErrorOccurred(CameraErrorEventArgs e) => ErrorOccurred?.Invoke(this, e);
}

/// <summary>
/// Interface for the camera view
/// </summary>
public interface IHonuaCameraView : IView
{
    CameraDirection CameraDirection { get; }
    FlashMode FlashMode { get; }
    double ZoomFactor { get; }
    bool IsGeotaggingEnabled { get; }
    bool ShowOverlay { get; }
}

/// <summary>
/// Camera direction options
/// </summary>
public enum CameraDirection
{
    Front,
    Back
}

/// <summary>
/// Flash mode options
/// </summary>
public enum FlashMode
{
    Auto,
    On,
    Off,
    Torch
}

/// <summary>
/// Video recording state
/// </summary>
public enum VideoRecordingState
{
    Stopped,
    Starting,
    Recording,
    Stopping
}

/// <summary>
/// Represents a camera device
/// </summary>
public record CameraDevice
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public CameraDirection Direction { get; init; }
    public bool HasFlash { get; init; }
    public double MaxZoom { get; init; }
    public IEnumerable<Size> SupportedResolutions { get; init; } = Array.Empty<Size>();
}

/// <summary>
/// Capture options for photos
/// </summary>
public record CaptureOptions
{
    public Size? Resolution { get; init; }
    public int Quality { get; init; } = 80;
    public bool IncludeExifData { get; init; } = true;
    public bool SaveToGallery { get; init; } = false;
    public string? FileName { get; init; }
}

/// <summary>
/// Video recording options
/// </summary>
public record VideoRecordingOptions
{
    public Size? Resolution { get; init; }
    public int Quality { get; init; } = 80;
    public TimeSpan? MaxDuration { get; init; }
    public bool SaveToGallery { get; init; } = false;
    public string? FileName { get; init; }
}

/// <summary>
/// Captured photo data
/// </summary>
public record CapturedPhoto
{
    public byte[] ImageData { get; init; } = Array.Empty<byte>();
    public string FilePath { get; init; } = string.Empty;
    public Size Resolution { get; init; }
    public Location? GpsLocation { get; init; }
    public DateTimeOffset CapturedAt { get; init; } = DateTimeOffset.UtcNow;
    public Dictionary<string, object> ExifData { get; init; } = new();
}

/// <summary>
/// Recorded video data
/// </summary>
public record RecordedVideo
{
    public string FilePath { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public Size Resolution { get; init; }
    public Location? GpsLocation { get; init; }
    public DateTimeOffset RecordedAt { get; init; } = DateTimeOffset.UtcNow;
    public long FileSize { get; init; }
}

/// <summary>
/// Event arguments for photo capture events
/// </summary>
public class PhotoCapturedEventArgs : EventArgs
{
    public bool Success { get; init; }
    public CapturedPhoto Photo { get; init; } = default!;
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Event arguments for video recording state changes
/// </summary>
public class VideoRecordingEventArgs : EventArgs
{
    public VideoRecordingState State { get; init; }
    public RecordedVideo? Video { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Event arguments for camera preview events
/// </summary>
public class CameraPreviewEventArgs : EventArgs
{
    public Size PreviewSize { get; init; }
    public bool IsReady { get; init; }
}

/// <summary>
/// Event arguments for camera errors
/// </summary>
public class CameraErrorEventArgs : EventArgs
{
    public string ErrorMessage { get; init; } = string.Empty;
    public Exception? Exception { get; init; }
}