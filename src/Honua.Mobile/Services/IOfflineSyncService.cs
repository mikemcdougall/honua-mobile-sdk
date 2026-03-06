namespace Honua.Mobile.Services;

/// <summary>
/// Service for managing offline data synchronization with the Honua server
/// </summary>
public interface IOfflineSyncService
{
    /// <summary>
    /// Sync data with the server when connectivity is available
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result with success status and details</returns>
    Task<SyncResult> SyncAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enable automatic background synchronization
    /// </summary>
    /// <param name="interval">Sync interval</param>
    Task EnableAutoSyncAsync(TimeSpan interval);

    /// <summary>
    /// Disable automatic background synchronization
    /// </summary>
    Task DisableAutoSyncAsync();

    /// <summary>
    /// Check if there are pending changes to sync
    /// </summary>
    /// <returns>True if there are pending changes</returns>
    Task<bool> HasPendingChangesAsync();

    /// <summary>
    /// Get the current sync status
    /// </summary>
    /// <returns>Current sync status</returns>
    Task<SyncStatus> GetSyncStatusAsync();

    /// <summary>
    /// Force download all data for offline use
    /// </summary>
    /// <param name="area">Geographic area to download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result</returns>
    Task<DownloadResult> DownloadDataForOfflineUseAsync(GeographicArea area, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when sync status changes
    /// </summary>
    event EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;
}

/// <summary>
/// Result of a sync operation
/// </summary>
public record SyncResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int UploadedChanges { get; init; }
    public int DownloadedChanges { get; init; }
    public DateTimeOffset SyncTime { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Current sync status
/// </summary>
public enum SyncStatus
{
    Idle,
    Syncing,
    Downloading,
    Uploading,
    Error,
    NoConnectivity
}

/// <summary>
/// Result of an offline data download operation
/// </summary>
public record DownloadResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public long BytesDownloaded { get; init; }
    public int FeaturesDownloaded { get; init; }
}

/// <summary>
/// Geographic area for offline data download
/// </summary>
public record GeographicArea
{
    public double MinLatitude { get; init; }
    public double MaxLatitude { get; init; }
    public double MinLongitude { get; init; }
    public double MaxLongitude { get; init; }
    public int? ZoomLevel { get; init; }
}

/// <summary>
/// Event arguments for sync status changes
/// </summary>
public class SyncStatusChangedEventArgs : EventArgs
{
    public SyncStatus Status { get; init; }
    public string? Message { get; init; }
    public int? Progress { get; init; }
}