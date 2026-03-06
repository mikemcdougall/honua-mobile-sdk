using System.Collections.ObjectModel;
using Honua.Mobile.Models;

namespace Honua.Mobile.Controls;

/// <summary>
/// Dynamic form control for field data collection
/// </summary>
public class HonuaFormView : View, IHonuaFormView
{
    public static readonly BindableProperty FormDefinitionProperty =
        BindableProperty.Create(nameof(FormDefinition), typeof(FormDefinition), typeof(HonuaFormView),
            default(FormDefinition), propertyChanged: OnFormDefinitionChanged);

    public static readonly BindableProperty IsReadOnlyProperty =
        BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(HonuaFormView), false);

    public static readonly BindableProperty ShowValidationErrorsProperty =
        BindableProperty.Create(nameof(ShowValidationErrors), typeof(bool), typeof(HonuaFormView), true);

    public static readonly BindableProperty FormStateProperty =
        BindableProperty.Create(nameof(FormState), typeof(FormState), typeof(HonuaFormView), FormState.Draft);

    /// <summary>
    /// Form definition containing fields and metadata
    /// </summary>
    public FormDefinition? FormDefinition
    {
        get => (FormDefinition?)GetValue(FormDefinitionProperty);
        set => SetValue(FormDefinitionProperty, value);
    }

    /// <summary>
    /// Whether the form is read-only
    /// </summary>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Whether to show validation errors inline
    /// </summary>
    public bool ShowValidationErrors
    {
        get => (bool)GetValue(ShowValidationErrorsProperty);
        set => SetValue(ShowValidationErrorsProperty, value);
    }

    /// <summary>
    /// Current form state
    /// </summary>
    public FormState FormState
    {
        get => (FormState)GetValue(FormStateProperty);
        set => SetValue(FormStateProperty, value);
    }

    /// <summary>
    /// Event raised when a field value changes
    /// </summary>
    public event EventHandler<FieldValueChangedEventArgs>? FieldValueChanged;

    /// <summary>
    /// Event raised when form validation state changes
    /// </summary>
    public event EventHandler<FormValidationEventArgs>? ValidationStateChanged;

    /// <summary>
    /// Event raised when form submission is requested
    /// </summary>
    public event EventHandler<FormSubmissionEventArgs>? SubmissionRequested;

    /// <summary>
    /// Event raised when a field requires special input (photo, location, etc.)
    /// </summary>
    public event EventHandler<SpecialInputEventArgs>? SpecialInputRequested;

    /// <summary>
    /// Current form field values
    /// </summary>
    public Dictionary<string, object?> FieldValues { get; } = new();

    /// <summary>
    /// Form attachments (photos, files, etc.)
    /// </summary>
    public ObservableCollection<FormAttachment> Attachments { get; } = new();

    /// <summary>
    /// Current validation errors
    /// </summary>
    public ObservableCollection<string> ValidationErrors { get; } = new();

    /// <summary>
    /// Get the value of a specific field
    /// </summary>
    /// <typeparam name="T">Expected value type</typeparam>
    /// <param name="fieldId">Field identifier</param>
    /// <returns>Field value or default</returns>
    public T? GetFieldValue<T>(string fieldId)
    {
        if (FieldValues.TryGetValue(fieldId, out var value) && value is T typedValue)
            return typedValue;
        return default;
    }

    /// <summary>
    /// Set the value of a specific field
    /// </summary>
    /// <param name="fieldId">Field identifier</param>
    /// <param name="value">Field value</param>
    public void SetFieldValue(string fieldId, object? value)
    {
        var oldValue = FieldValues.TryGetValue(fieldId, out var existing) ? existing : null;
        FieldValues[fieldId] = value;

        var field = FormDefinition?.Fields.FirstOrDefault(f => f.Id == fieldId);
        if (field != null)
        {
            field.Value = value;
            OnFieldValueChanged(new FieldValueChangedEventArgs(field, oldValue));
        }

        // Trigger validation
        ValidateForm();

        // Update platform handler
        Handler?.Invoke(nameof(SetFieldValue), new { fieldId, value });
    }

    /// <summary>
    /// Validate the entire form
    /// </summary>
    /// <returns>Validation result</returns>
    public ValidationResult ValidateForm()
    {
        var result = new ValidationResult();

        if (FormDefinition == null)
        {
            result.AddError("Form definition is required");
            return result;
        }

        // Validate individual fields
        foreach (var field in FormDefinition.Fields)
        {
            var fieldResult = field.ValidateValue();
            result.Errors.AddRange(fieldResult.Errors);
            result.Warnings.AddRange(fieldResult.Warnings);
        }

        // Validate cross-field rules
        foreach (var rule in FormDefinition.ValidationRules.CrossFieldRules)
        {
            if (!EvaluateRule(rule.Rule))
            {
                result.AddError(rule.ErrorMessage);
            }
        }

        UpdateValidationErrors(result);
        OnValidationStateChanged(new FormValidationEventArgs(result));

        return result;
    }

    /// <summary>
    /// Submit the form with current values
    /// </summary>
    /// <returns>Form submission data</returns>
    public async Task<FormSubmission> SubmitFormAsync()
    {
        var validation = ValidateForm();
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Form validation failed: {string.Join(", ", validation.Errors)}");
        }

        var submission = new FormSubmission
        {
            FormId = FormDefinition?.Id ?? string.Empty,
            FormVersion = FormDefinition?.Version ?? string.Empty,
            FieldValues = new Dictionary<string, object?>(FieldValues),
            Attachments = Attachments.ToList(),
            SubmittedAt = DateTime.UtcNow
        };

        FormState = FormState.Submitted;
        OnSubmissionRequested(new FormSubmissionEventArgs(submission));

        return submission;
    }

    /// <summary>
    /// Clear all form values and reset to defaults
    /// </summary>
    public void ClearForm()
    {
        FieldValues.Clear();
        Attachments.Clear();
        ValidationErrors.Clear();

        if (FormDefinition != null)
        {
            foreach (var field in FormDefinition.Fields)
            {
                field.Value = field.DefaultValue;
                if (field.DefaultValue != null)
                    FieldValues[field.Id] = field.DefaultValue;
            }
        }

        FormState = FormState.Draft;
        Handler?.Invoke(nameof(ClearForm));
    }

    /// <summary>
    /// Load form from JSON definition
    /// </summary>
    /// <param name="json">JSON form definition</param>
    public void LoadFromJson(string json)
    {
        try
        {
            var definition = System.Text.Json.JsonSerializer.Deserialize<FormDefinition>(json);
            FormDefinition = definition;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid form JSON: {ex.Message}", nameof(json), ex);
        }
    }

    /// <summary>
    /// Export form definition to JSON
    /// </summary>
    /// <returns>JSON representation of form definition</returns>
    public string ExportToJson()
    {
        if (FormDefinition == null)
            return "{}";

        return System.Text.Json.JsonSerializer.Serialize(FormDefinition, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Add an attachment to the form
    /// </summary>
    /// <param name="attachment">Form attachment</param>
    public void AddAttachment(FormAttachment attachment)
    {
        Attachments.Add(attachment);
        Handler?.Invoke(nameof(AddAttachment), attachment);
    }

    /// <summary>
    /// Remove an attachment from the form
    /// </summary>
    /// <param name="attachment">Form attachment to remove</param>
    public void RemoveAttachment(FormAttachment attachment)
    {
        Attachments.Remove(attachment);
        Handler?.Invoke(nameof(RemoveAttachment), attachment);
    }

    /// <summary>
    /// Request special input for a field (photo, location, etc.)
    /// </summary>
    /// <param name="fieldId">Field identifier</param>
    /// <param name="inputType">Type of special input</param>
    public async Task RequestSpecialInputAsync(string fieldId, SpecialInputType inputType)
    {
        var field = FormDefinition?.Fields.FirstOrDefault(f => f.Id == fieldId);
        if (field == null)
            throw new ArgumentException($"Field '{fieldId}' not found", nameof(fieldId));

        var args = new SpecialInputEventArgs(field, inputType);
        OnSpecialInputRequested(args);

        // Platform handler will handle the special input request
        Handler?.Invoke(nameof(RequestSpecialInputAsync), new { fieldId, inputType });
    }

    private static void OnFormDefinitionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HonuaFormView form && newValue is FormDefinition definition)
        {
            form.InitializeFromDefinition(definition);
        }
    }

    private void InitializeFromDefinition(FormDefinition definition)
    {
        FieldValues.Clear();
        Attachments.Clear();
        ValidationErrors.Clear();

        // Initialize field values with defaults
        foreach (var field in definition.Fields)
        {
            if (field.DefaultValue != null)
            {
                FieldValues[field.Id] = field.DefaultValue;
                field.Value = field.DefaultValue;
            }

            // Subscribe to field value changes
            field.ValueChanged += OnFieldValueChangedInternal;
        }

        FormState = FormState.Draft;
        Handler?.Invoke(nameof(InitializeFromDefinition), definition);
    }

    private void OnFieldValueChangedInternal(object? sender, FieldValueChangedEventArgs e)
    {
        FieldValues[e.Field.Id] = e.NewValue;
        OnFieldValueChanged(e);

        // Apply conditional rules
        ApplyConditionalRules(e.Field);

        // Validate form
        ValidateForm();
    }

    private void ApplyConditionalRules(FormField changedField)
    {
        if (FormDefinition == null) return;

        foreach (var rule in FormDefinition.ValidationRules.ConditionalRules)
        {
            if (rule.FieldId == changedField.Id)
            {
                var targetField = FormDefinition.Fields.FirstOrDefault(f => f.Id == rule.TargetFieldId);
                if (targetField != null && EvaluateRule(rule.Condition))
                {
                    switch (rule.Action.ToLower())
                    {
                        case "show":
                            targetField.IsVisible = true;
                            break;
                        case "hide":
                            targetField.IsVisible = false;
                            break;
                        case "enable":
                            targetField.IsReadOnly = false;
                            break;
                        case "disable":
                            targetField.IsReadOnly = true;
                            break;
                    }
                }
            }
        }
    }

    private bool EvaluateRule(string rule)
    {
        // Simple rule evaluation - in a real implementation,
        // you might use a proper expression evaluator
        // For now, just return true as a placeholder
        return true;
    }

    private void UpdateValidationErrors(ValidationResult result)
    {
        ValidationErrors.Clear();
        foreach (var error in result.Errors)
        {
            ValidationErrors.Add(error);
        }
    }

    protected virtual void OnFieldValueChanged(FieldValueChangedEventArgs e) => FieldValueChanged?.Invoke(this, e);
    protected virtual void OnValidationStateChanged(FormValidationEventArgs e) => ValidationStateChanged?.Invoke(this, e);
    protected virtual void OnSubmissionRequested(FormSubmissionEventArgs e) => SubmissionRequested?.Invoke(this, e);
    protected virtual void OnSpecialInputRequested(SpecialInputEventArgs e) => SpecialInputRequested?.Invoke(this, e);
}

/// <summary>
/// Interface for the form view
/// </summary>
public interface IHonuaFormView : IView
{
    FormDefinition? FormDefinition { get; }
    bool IsReadOnly { get; }
    FormState FormState { get; }
    Dictionary<string, object?> FieldValues { get; }
}

/// <summary>
/// Special input types for form fields
/// </summary>
public enum SpecialInputType
{
    Photo,
    Location,
    Signature,
    Barcode,
    QRCode,
    Audio,
    Video
}

/// <summary>
/// Event arguments for form validation events
/// </summary>
public class FormValidationEventArgs : EventArgs
{
    public ValidationResult ValidationResult { get; }

    public FormValidationEventArgs(ValidationResult validationResult)
    {
        ValidationResult = validationResult;
    }
}

/// <summary>
/// Event arguments for form submission events
/// </summary>
public class FormSubmissionEventArgs : EventArgs
{
    public FormSubmission Submission { get; }

    public FormSubmissionEventArgs(FormSubmission submission)
    {
        Submission = submission;
    }
}

/// <summary>
/// Event arguments for special input requests
/// </summary>
public class SpecialInputEventArgs : EventArgs
{
    public FormField Field { get; }
    public SpecialInputType InputType { get; }

    public SpecialInputEventArgs(FormField field, SpecialInputType inputType)
    {
        Field = field;
        InputType = inputType;
    }
}