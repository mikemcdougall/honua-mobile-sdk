namespace Honua.Mobile.Services;

/// <summary>
/// Service for managing local data storage for offline capabilities
/// </summary>
public interface IOfflineStorage
{
    /// <summary>
    /// Store feature data locally
    /// </summary>
    /// <param name="feature">Feature to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StoreFeatureAsync(FeatureData feature, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a feature by ID
    /// </summary>
    /// <param name="featureId">Feature identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Feature data or null if not found</returns>
    Task<FeatureData?> GetFeatureAsync(string featureId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Query features within a geographic area
    /// </summary>
    /// <param name="area">Geographic area to search</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Features within the specified area</returns>
    Task<IEnumerable<FeatureData>> GetFeaturesInAreaAsync(GeographicArea area, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a feature from local storage
    /// </summary>
    /// <param name="featureId">Feature identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFeatureAsync(string featureId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all features marked as modified locally
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Modified features awaiting sync</returns>
    Task<IEnumerable<FeatureData>> GetModifiedFeaturesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark a feature as synced
    /// </summary>
    /// <param name="featureId">Feature identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MarkFeatureAsSyncedAsync(string featureId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all locally stored data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearAllDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get storage statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage usage information</returns>
    Task<StorageInfo> GetStorageInfoAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents feature data with metadata
/// </summary>
public record FeatureData
{
    public string Id { get; init; } = string.Empty;
    public string LayerId { get; init; } = string.Empty;
    public string GeometryJson { get; init; } = string.Empty;
    public Dictionary<string, object> Attributes { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ModifiedAt { get; init; }
    public bool IsModified { get; init; }
    public bool IsDeleted { get; init; }
    public string? CreatedBy { get; init; }
    public string? ModifiedBy { get; init; }
}

/// <summary>
/// Storage usage information
/// </summary>
public record StorageInfo
{
    public long TotalSize { get; init; }
    public int FeatureCount { get; init; }
    public int ModifiedFeatureCount { get; init; }
    public DateTimeOffset LastSyncTime { get; init; }
    public string DatabasePath { get; init; } = string.Empty;
}