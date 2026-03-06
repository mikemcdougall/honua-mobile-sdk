namespace Honua.Mobile.Models;

/// <summary>
/// Represents a field data collection form
/// </summary>
public record FieldForm
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Version { get; init; } = "1.0";
    public IEnumerable<FieldSection> Sections { get; init; } = Array.Empty<FieldSection>();
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ModifiedAt { get; init; } = DateTimeOffset.UtcNow;
    public bool IsActive { get; init; } = true;
}

/// <summary>
/// Represents a section within a field form
/// </summary>
public record FieldSection
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Order { get; init; }
    public bool IsRequired { get; init; }
    public IEnumerable<FieldDefinition> Fields { get; init; } = Array.Empty<FieldDefinition>();
}

/// <summary>
/// Defines a field within a form section
/// </summary>
public record FieldDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
    public FieldType Type { get; init; } = FieldType.Text;
    public bool IsRequired { get; init; }
    public bool IsReadOnly { get; init; }
    public int Order { get; init; }
    public object? DefaultValue { get; init; }
    public FieldValidation? Validation { get; init; }
    public IEnumerable<FieldOption>? Options { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Field types supported in mobile forms
/// </summary>
public enum FieldType
{
    Text,
    Number,
    Date,
    Time,
    DateTime,
    Boolean,
    SingleSelect,
    MultiSelect,
    Photo,
    Video,
    Audio,
    Signature,
    Location,
    Barcode,
    QrCode,
    File
}

/// <summary>
/// Validation rules for form fields
/// </summary>
public record FieldValidation
{
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public double? MinValue { get; init; }
    public double? MaxValue { get; init; }
    public string? Pattern { get; init; }
    public string? CustomValidation { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Options for select fields
/// </summary>
public record FieldOption
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
    public bool IsActive { get; init; } = true;
}

/// <summary>
/// Represents completed field data
/// </summary>
public record FieldDataEntry
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string FormId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public Location? CollectionLocation { get; init; }
    public DateTimeOffset CollectedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? SubmittedAt { get; init; }
    public FieldDataStatus Status { get; init; } = FieldDataStatus.Draft;
    public IEnumerable<FieldValue> Values { get; init; } = Array.Empty<FieldValue>();
    public IEnumerable<MediaAttachment> Attachments { get; init; } = Array.Empty<MediaAttachment>();
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Status of field data collection
/// </summary>
public enum FieldDataStatus
{
    Draft,
    Completed,
    Submitted,
    Synced,
    Error
}

/// <summary>
/// Represents a field value in a data entry
/// </summary>
public record FieldValue
{
    public string FieldId { get; init; } = string.Empty;
    public object? Value { get; init; }
    public DateTimeOffset CollectedAt { get; init; } = DateTimeOffset.UtcNow;
    public Location? Location { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Represents media attachments (photos, videos, audio)
/// </summary>
public record MediaAttachment
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string FieldId { get; init; } = string.Empty;
    public MediaType Type { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string LocalPath { get; init; } = string.Empty;
    public string? RemoteUrl { get; init; }
    public long FileSize { get; init; }
    public string MimeType { get; init; } = string.Empty;
    public Location? CaptureLocation { get; init; }
    public DateTimeOffset CapturedAt { get; init; } = DateTimeOffset.UtcNow;
    public Dictionary<string, object> Metadata { get; init; } = new();
    public bool IsSynced { get; init; }
}

/// <summary>
/// Types of media attachments
/// </summary>
public enum MediaType
{
    Photo,
    Video,
    Audio,
    Document,
    Signature
}