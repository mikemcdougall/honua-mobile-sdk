using Honua.Mobile.Models;

namespace Honua.Mobile.Services;

/// <summary>
/// Service for managing offline data synchronization with the Honua server
/// </summary>
public interface IOfflineSyncService
{
    #region Sync Operations

    /// <summary>
    /// Sync data with the server when connectivity is available
    /// </summary>
    /// <param name="options">Sync options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result with success status and details</returns>
    Task<SyncResult> SyncAsync(SyncOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sync specific layers only
    /// </summary>
    /// <param name="layerIds">Layer IDs to sync</param>
    /// <param name="options">Sync options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result</returns>
    Task<SyncResult> SyncLayersAsync(IEnumerable<string> layerIds, SyncOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sync form submissions to server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result for form submissions</returns>
    Task<SyncResult> SyncFormSubmissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload pending changes to server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result</returns>
    Task<SyncResult> UploadPendingChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Download latest changes from server
    /// </summary>
    /// <param name="since">Download changes since this timestamp</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result</returns>
    Task<SyncResult> DownloadLatestChangesAsync(DateTimeOffset? since = null, CancellationToken cancellationToken = default);

    #endregion

    #region Offline Data Management

    /// <summary>
    /// Download data for offline use within a geographic area
    /// </summary>
    /// <param name="area">Geographic area to download</param>
    /// <param name="options">Download options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result</returns>
    Task<DownloadResult> DownloadDataForOfflineUseAsync(GeographicArea area, OfflineDownloadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download specific layers for offline use
    /// </summary>
    /// <param name="layerIds">Layer IDs to download</param>
    /// <param name="area">Geographic area to download</param>
    /// <param name="options">Download options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result</returns>
    Task<DownloadResult> DownloadLayersForOfflineUseAsync(IEnumerable<string> layerIds, GeographicArea area, OfflineDownloadOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get areas that are available offline
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Available offline areas</returns>
    Task<IEnumerable<OfflineArea>> GetAvailableOfflineAreasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove offline data for a specific area
    /// </summary>
    /// <param name="areaId">Offline area identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveOfflineAreaAsync(string areaId, CancellationToken cancellationToken = default);

    #endregion

    #region Auto-Sync Management

    /// <summary>
    /// Enable automatic background synchronization
    /// </summary>
    /// <param name="options">Auto-sync options</param>
    Task EnableAutoSyncAsync(AutoSyncOptions options);

    /// <summary>
    /// Disable automatic background synchronization
    /// </summary>
    Task DisableAutoSyncAsync();

    /// <summary>
    /// Check if auto-sync is enabled
    /// </summary>
    /// <returns>True if auto-sync is enabled</returns>
    bool IsAutoSyncEnabled { get; }

    /// <summary>
    /// Get auto-sync configuration
    /// </summary>
    /// <returns>Auto-sync options or null if disabled</returns>
    AutoSyncOptions? GetAutoSyncOptions();

    #endregion

    #region Status and Monitoring

    /// <summary>
    /// Check if there are pending changes to sync
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pending changes information</returns>
    Task<PendingChangesInfo> GetPendingChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current sync status
    /// </summary>
    /// <returns>Current sync status</returns>
    SyncStatus CurrentStatus { get; }

    /// <summary>
    /// Get sync statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync statistics</returns>
    Task<SyncStatistics> GetSyncStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check network connectivity status
    /// </summary>
    /// <returns>True if connected to network</returns>
    Task<bool> IsConnectedAsync();

    /// <summary>
    /// Test connection to Honua server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if server is reachable</returns>
    Task<bool> CanReachServerAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Conflict Resolution

    /// <summary>
    /// Get sync conflicts that require resolution
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync conflicts</returns>
    Task<IEnumerable<SyncConflict>> GetSyncConflictsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolve a sync conflict
    /// </summary>
    /// <param name="conflictId">Conflict identifier</param>
    /// <param name="resolution">Resolution strategy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ResolveSyncConflictAsync(string conflictId, ConflictResolution resolution, CancellationToken cancellationToken = default);

    #endregion

    #region Events

    /// <summary>
    /// Event raised when sync status changes
    /// </summary>
    event EventHandler<SyncStatusChangedEventArgs>? SyncStatusChanged;

    /// <summary>
    /// Event raised when sync progress updates
    /// </summary>
    event EventHandler<SyncProgressEventArgs>? SyncProgressChanged;

    /// <summary>
    /// Event raised when sync conflicts are detected
    /// </summary>
    event EventHandler<SyncConflictDetectedEventArgs>? ConflictDetected;

    /// <summary>
    /// Event raised when network connectivity changes
    /// </summary>
    event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;

    #endregion
}

/// <summary>
/// Options for sync operations
/// </summary>
public record SyncOptions
{
    public bool UploadChanges { get; init; } = true;
    public bool DownloadChanges { get; init; } = true;
    public bool ResolveConflictsAutomatically { get; init; } = true;
    public ConflictResolution DefaultConflictResolution { get; init; } = ConflictResolution.UseServer;
    public TimeSpan? Timeout { get; init; }
    public bool RequireWifi { get; init; } = false;
    public int MaxRetryAttempts { get; init; } = 3;
    public IEnumerable<string>? LayerFilter { get; init; }
}

/// <summary>
/// Auto-sync configuration options
/// </summary>
public record AutoSyncOptions
{
    public TimeSpan SyncInterval { get; init; } = TimeSpan.FromMinutes(30);
    public bool EnabledOnWifiOnly { get; init; } = true;
    public bool EnabledWhenCharging { get; init; } = false;
    public TimeSpan BackoffMultiplier { get; init; } = TimeSpan.FromMinutes(2);
    public int MaxBackoffMinutes { get; init; } = 60;
    public bool SyncOnAppStart { get; init; } = true;
    public bool SyncOnDataChange { get; init; } = false;
    public bool SyncInBackground { get; init; } = true;
}

/// <summary>
/// Options for offline data downloads
/// </summary>
public record OfflineDownloadOptions
{
    public IEnumerable<string>? LayerIds { get; init; }
    public int? MaxZoomLevel { get; init; }
    public bool IncludeBaseMaps { get; init; } = false;
    public bool OverwriteExisting { get; init; } = false;
    public long? MaxDownloadSize { get; init; } // bytes
    public TimeSpan? Timeout { get; init; }
    public bool RequireWifi { get; init; } = true;
}

/// <summary>
/// Result of a sync operation
/// </summary>
public record SyncResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public Exception? Exception { get; init; }
    public int FeaturesUploaded { get; init; }
    public int FeaturesDownloaded { get; init; }
    public int FormSubmissionsUploaded { get; init; }
    public int AttachmentsUploaded { get; init; }
    public int ConflictsResolved { get; init; }
    public int ConflictsRemaining { get; init; }
    public long BytesTransferred { get; init; }
    public DateTimeOffset SyncStartTime { get; init; }
    public DateTimeOffset SyncEndTime { get; init; }
    public TimeSpan Duration => SyncEndTime - SyncStartTime;
    public List<string> Warnings { get; init; } = new();
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Result of an offline data download operation
/// </summary>
public record DownloadResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public Exception? Exception { get; init; }
    public long BytesDownloaded { get; init; }
    public int FeaturesDownloaded { get; init; }
    public int LayersDownloaded { get; init; }
    public TimeSpan Duration { get; init; }
    public GeographicArea Area { get; init; } = default!;
    public List<string> DownloadedLayers { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
}

/// <summary>
/// Information about pending changes
/// </summary>
public record PendingChangesInfo
{
    public int PendingFeatures { get; init; }
    public int PendingFormSubmissions { get; init; }
    public int PendingAttachments { get; init; }
    public int ConflictCount { get; init; }
    public long EstimatedUploadSize { get; init; }
    public DateTimeOffset? OldestPendingChange { get; init; }
}

/// <summary>
/// Offline area information
/// </summary>
public record OfflineArea
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public GeographicArea Area { get; init; } = default!;
    public List<string> LayerIds { get; init; } = new();
    public DateTimeOffset DownloadedAt { get; init; }
    public DateTimeOffset? LastSyncAt { get; init; }
    public long StorageSize { get; init; }
    public int FeatureCount { get; init; }
}

/// <summary>
/// Sync statistics
/// </summary>
public record SyncStatistics
{
    public int TotalSyncSessions { get; init; }
    public int SuccessfulSyncs { get; init; }
    public int FailedSyncs { get; init; }
    public DateTimeOffset? LastSuccessfulSync { get; init; }
    public DateTimeOffset? LastFailedSync { get; init; }
    public TimeSpan AverageSyncDuration { get; init; }
    public long TotalBytesTransferred { get; init; }
    public int TotalFeaturesTransferred { get; init; }
    public int TotalConflictsResolved { get; init; }
}

/// <summary>
/// Current sync status
/// </summary>
public enum SyncStatus
{
    Idle,
    Initializing,
    UploadingChanges,
    DownloadingChanges,
    ResolvingConflicts,
    Completing,
    Error,
    NoConnectivity,
    Cancelled
}

/// <summary>
/// Event arguments for sync status changes
/// </summary>
public class SyncStatusChangedEventArgs : EventArgs
{
    public SyncStatus OldStatus { get; }
    public SyncStatus NewStatus { get; }
    public string? Message { get; }

    public SyncStatusChangedEventArgs(SyncStatus oldStatus, SyncStatus newStatus, string? message = null)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Message = message;
    }
}

/// <summary>
/// Event arguments for sync progress updates
/// </summary>
public class SyncProgressEventArgs : EventArgs
{
    public SyncStatus Status { get; }
    public int ProgressPercentage { get; }
    public string? CurrentOperation { get; }
    public long? BytesTransferred { get; }
    public long? TotalBytes { get; }
    public int? ItemsProcessed { get; }
    public int? TotalItems { get; }

    public SyncProgressEventArgs(SyncStatus status, int progressPercentage, string? currentOperation = null,
        long? bytesTransferred = null, long? totalBytes = null, int? itemsProcessed = null, int? totalItems = null)
    {
        Status = status;
        ProgressPercentage = progressPercentage;
        CurrentOperation = currentOperation;
        BytesTransferred = bytesTransferred;
        TotalBytes = totalBytes;
        ItemsProcessed = itemsProcessed;
        TotalItems = totalItems;
    }
}

/// <summary>
/// Event arguments for sync conflict detection
/// </summary>
public class SyncConflictDetectedEventArgs : EventArgs
{
    public SyncConflict Conflict { get; }

    public SyncConflictDetectedEventArgs(SyncConflict conflict)
    {
        Conflict = conflict;
    }
}

/// <summary>
/// Event arguments for connectivity changes
/// </summary>
public class ConnectivityChangedEventArgs : EventArgs
{
    public bool IsConnected { get; }
    public bool IsWifiAvailable { get; }
    public bool IsMobileDataAvailable { get; }

    public ConnectivityChangedEventArgs(bool isConnected, bool isWifiAvailable = false, bool isMobileDataAvailable = false)
    {
        IsConnected = isConnected;
        IsWifiAvailable = isWifiAvailable;
        IsMobileDataAvailable = isMobileDataAvailable;
    }
}