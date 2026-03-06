using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Honua.Mobile.Models;

/// <summary>
/// Dynamic form definition for field data collection
/// </summary>
public class FormDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public ObservableCollection<FormField> Fields { get; set; } = new();
    public FormMetadata Metadata { get; set; } = new();
    public ValidationRules ValidationRules { get; set; } = new();
}

/// <summary>
/// Form field definition
/// </summary>
public class FormField : INotifyPropertyChanged
{
    private object? _value;
    private bool _isVisible = true;
    private bool _isRequired = false;
    private bool _isReadOnly = false;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FieldType FieldType { get; set; }
    public object? DefaultValue { get; set; }
    public string? PlaceholderText { get; set; }

    public object? Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                OnValueChanged();
            }
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }

    public bool IsRequired
    {
        get => _isRequired;
        set
        {
            if (_isRequired != value)
            {
                _isRequired = value;
                OnPropertyChanged(nameof(IsRequired));
            }
        }
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set
        {
            if (_isReadOnly != value)
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }
    }

    public FieldValidation Validation { get; set; } = new();
    public FieldOptions Options { get; set; } = new();
    public Dictionary<string, object> CustomProperties { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<FieldValueChangedEventArgs>? ValueChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnValueChanged()
    {
        ValueChanged?.Invoke(this, new FieldValueChangedEventArgs(this));
    }

    /// <summary>
    /// Validate the current field value
    /// </summary>
    public ValidationResult ValidateValue()
    {
        var result = new ValidationResult();

        // Check required field
        if (IsRequired && (Value == null || string.IsNullOrWhiteSpace(Value.ToString())))
        {
            result.AddError($"{Label} is required");
            return result;
        }

        // Type-specific validation
        switch (FieldType)
        {
            case FieldType.Email:
                if (Value is string email && !IsValidEmail(email))
                    result.AddError($"{Label} must be a valid email address");
                break;

            case FieldType.Number:
                if (Value != null && !IsNumeric(Value))
                    result.AddError($"{Label} must be a valid number");
                break;

            case FieldType.Date:
                if (Value != null && !DateTime.TryParse(Value.ToString(), out _))
                    result.AddError($"{Label} must be a valid date");
                break;
        }

        // Custom validation rules
        if (Validation.MinLength.HasValue && Value?.ToString()?.Length < Validation.MinLength)
            result.AddError($"{Label} must be at least {Validation.MinLength} characters");

        if (Validation.MaxLength.HasValue && Value?.ToString()?.Length > Validation.MaxLength)
            result.AddError($"{Label} must be no more than {Validation.MaxLength} characters");

        if (Validation.Pattern != null && Value != null)
        {
            var regex = new System.Text.RegularExpressions.Regex(Validation.Pattern);
            if (!regex.IsMatch(Value.ToString()!))
                result.AddError($"{Label} format is invalid");
        }

        return result;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsNumeric(object value)
    {
        return double.TryParse(value?.ToString(), out _);
    }
}

/// <summary>
/// Form field types
/// </summary>
public enum FieldType
{
    Text,
    Number,
    Email,
    Password,
    TextArea,
    Date,
    Time,
    DateTime,
    Checkbox,
    Radio,
    Select,
    MultiSelect,
    File,
    Photo,
    Signature,
    Location,
    Barcode,
    QRCode
}

/// <summary>
/// Field validation rules
/// </summary>
public class FieldValidation
{
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public List<string> CustomRules { get; set; } = new();
}

/// <summary>
/// Field options for select/radio fields
/// </summary>
public class FieldOptions
{
    public List<FieldOption> Items { get; set; } = new();
    public bool AllowMultiple { get; set; } = false;
    public bool AllowCustomValue { get; set; } = false;
    public string? DataSource { get; set; }
}

/// <summary>
/// Field option item
/// </summary>
public record FieldOption
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public bool IsDefault { get; init; } = false;
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Form metadata
/// </summary>
public class FormMetadata
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, object> CustomData { get; set; } = new();
}

/// <summary>
/// Form validation rules
/// </summary>
public class ValidationRules
{
    public List<ConditionalRule> ConditionalRules { get; set; } = new();
    public List<CrossFieldRule> CrossFieldRules { get; set; } = new();
}

/// <summary>
/// Conditional field rule
/// </summary>
public class ConditionalRule
{
    public string FieldId { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty; // JavaScript-like expression
    public string Action { get; set; } = string.Empty; // "show", "hide", "enable", "disable"
    public string TargetFieldId { get; set; } = string.Empty;
}

/// <summary>
/// Cross-field validation rule
/// </summary>
public class CrossFieldRule
{
    public string Id { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty; // JavaScript-like expression
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> AffectedFields { get; set; } = new();
}

/// <summary>
/// Validation result
/// </summary>
public class ValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}

/// <summary>
/// Form submission data
/// </summary>
public class FormSubmission
{
    public string FormId { get; set; } = string.Empty;
    public string FormVersion { get; set; } = string.Empty;
    public Dictionary<string, object?> FieldValues { get; set; } = new();
    public List<FormAttachment> Attachments { get; set; } = new();
    public Location? SubmissionLocation { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public string? SubmittedBy { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Form attachment (photos, files, etc.)
/// </summary>
public class FormAttachment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FieldId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[]? Data { get; set; }
    public string? FilePath { get; set; }
    public Location? CaptureLocation { get; set; }
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Event arguments for field value changes
/// </summary>
public class FieldValueChangedEventArgs : EventArgs
{
    public FormField Field { get; }
    public object? OldValue { get; }
    public object? NewValue => Field.Value;

    public FieldValueChangedEventArgs(FormField field, object? oldValue = null)
    {
        Field = field;
        OldValue = oldValue;
    }
}

/// <summary>
/// Form state
/// </summary>
public enum FormState
{
    Draft,
    InProgress,
    Completed,
    Submitted,
    Error
}