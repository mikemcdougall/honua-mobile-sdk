using Honua.Mobile.Models;

namespace Honua.Mobile.Services;

/// <summary>
/// Service for managing local data storage for offline capabilities
/// </summary>
public interface IOfflineStorage
{
    #region Feature Storage

    /// <summary>
    /// Store feature data locally
    /// </summary>
    /// <param name="feature">Feature to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StoreFeatureAsync(FeatureData feature, CancellationToken cancellationToken = default);

    /// <summary>
    /// Store multiple features in a batch operation
    /// </summary>
    /// <param name="features">Features to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StoreFeaturesAsync(IEnumerable<FeatureData> features, CancellationToken cancellationToken = default);

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
    /// <param name="layerIds">Optional layer IDs to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Features within the specified area</returns>
    Task<IEnumerable<FeatureData>> GetFeaturesInAreaAsync(GeographicArea area, IEnumerable<string>? layerIds = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Query features by attribute filter
    /// </summary>
    /// <param name="filter">Attribute filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching features</returns>
    Task<IEnumerable<FeatureData>> QueryFeaturesAsync(AttributeFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a feature from local storage
    /// </summary>
    /// <param name="featureId">Feature identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFeatureAsync(string featureId, CancellationToken cancellationToken = default);

    #endregion

    #region Form Storage

    /// <summary>
    /// Store form submission data locally
    /// </summary>
    /// <param name="submission">Form submission to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StoreFormSubmissionAsync(FormSubmission submission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve form submission by ID
    /// </summary>
    /// <param name="submissionId">Submission identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Form submission or null if not found</returns>
    Task<FormSubmission?> GetFormSubmissionAsync(string submissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all form submissions awaiting sync
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pending form submissions</returns>
    Task<IEnumerable<FormSubmission>> GetPendingFormSubmissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete form submission from local storage
    /// </summary>
    /// <param name="submissionId">Submission identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFormSubmissionAsync(string submissionId, CancellationToken cancellationToken = default);

    #endregion

    #region Attachment Storage

    /// <summary>
    /// Store attachment data locally
    /// </summary>
    /// <param name="attachment">Attachment to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StoreAttachmentAsync(FormAttachment attachment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve attachment by ID
    /// </summary>
    /// <param name="attachmentId">Attachment identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Attachment data or null if not found</returns>
    Task<FormAttachment?> GetAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all attachments for a form submission
    /// </summary>
    /// <param name="submissionId">Form submission identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Attachments for the submission</returns>
    Task<IEnumerable<FormAttachment>> GetAttachmentsForSubmissionAsync(string submissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete attachment from local storage
    /// </summary>
    /// <param name="attachmentId">Attachment identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default);

    #endregion

    #region Sync Management

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
    /// Mark form submission as synced
    /// </summary>
    /// <param name="submissionId">Submission identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MarkFormSubmissionAsSyncedAsync(string submissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sync conflict data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Features with sync conflicts</returns>
    Task<IEnumerable<SyncConflict>> GetSyncConflictsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolve sync conflict
    /// </summary>
    /// <param name="conflictId">Conflict identifier</param>
    /// <param name="resolution">Conflict resolution strategy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ResolveSyncConflictAsync(string conflictId, ConflictResolution resolution, CancellationToken cancellationToken = default);

    #endregion

    #region Data Management

    /// <summary>
    /// Clear all locally stored data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearAllDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear data for specific layer
    /// </summary>
    /// <param name="layerId">Layer identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearLayerDataAsync(string layerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vacuum and optimize database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task OptimizeStorageAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get storage statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Storage usage information</returns>
    Task<StorageInfo> GetStorageInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if storage is initialized and healthy
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if storage is ready for use</returns>
    Task<bool> IsStorageHealthyAsync(CancellationToken cancellationToken = default);

    #endregion
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
    public int Version { get; init; } = 1;
    public string? SourceId { get; init; } // Original server ID
    public FeatureChangeType ChangeType { get; init; } = FeatureChangeType.None;
}

/// <summary>
/// Storage usage information
/// </summary>
public record StorageInfo
{
    public long TotalSize { get; init; }
    public long AvailableSpace { get; init; }
    public int FeatureCount { get; init; }
    public int ModifiedFeatureCount { get; init; }
    public int FormSubmissionCount { get; init; }
    public int AttachmentCount { get; init; }
    public int ConflictCount { get; init; }
    public DateTimeOffset LastSyncTime { get; init; }
    public string DatabasePath { get; init; } = string.Empty;
    public string DatabaseVersion { get; init; } = string.Empty;
    public Dictionary<string, LayerInfo> LayerInfo { get; init; } = new();
}

/// <summary>
/// Layer information for storage statistics
/// </summary>
public record LayerInfo
{
    public string LayerId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int FeatureCount { get; init; }
    public int ModifiedFeatureCount { get; init; }
    public long StorageSize { get; init; }
    public DateTimeOffset LastModified { get; init; }
}

/// <summary>
/// Attribute filter for querying features
/// </summary>
public record AttributeFilter
{
    public string? LayerId { get; init; }
    public Dictionary<string, object> Conditions { get; init; } = new();
    public string? SqlWhereClause { get; init; }
    public int? Limit { get; init; }
    public int? Offset { get; init; }
    public string? OrderBy { get; init; }
}

/// <summary>
/// Sync conflict information
/// </summary>
public record SyncConflict
{
    public string Id { get; init; } = string.Empty;
    public string FeatureId { get; init; } = string.Empty;
    public FeatureData LocalVersion { get; init; } = default!;
    public FeatureData ServerVersion { get; init; } = default!;
    public ConflictType ConflictType { get; init; }
    public DateTimeOffset ConflictDetectedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? ConflictDetails { get; init; }
}

/// <summary>
/// Types of feature changes
/// </summary>
public enum FeatureChangeType
{
    None,
    Created,
    Updated,
    Deleted
}

/// <summary>
/// Types of sync conflicts
/// </summary>
public enum ConflictType
{
    UpdateConflict,    // Both local and server versions modified
    DeleteConflict,    // One side deleted, other modified
    AttributeConflict, // Conflicting attribute values
    GeometryConflict   // Conflicting geometry
}

/// <summary>
/// Conflict resolution strategies
/// </summary>
public enum ConflictResolution
{
    UseLocal,     // Keep local version
    UseServer,    // Use server version
    Merge,        // Attempt automatic merge
    Manual        // Manual resolution required
}