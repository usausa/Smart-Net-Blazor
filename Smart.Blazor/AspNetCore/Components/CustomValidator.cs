namespace Smart.AspNetCore.Components;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public sealed class CustomValidator : ComponentBase, IDisposable
{
    private ValidationMessageStore? messageStore;

    private EditContext? subscribedEditContext;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnParametersSet()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException($"{nameof(EditContext)} is required.");
        }

        if (ReferenceEquals(CurrentEditContext, subscribedEditContext))
        {
            return;
        }

        Unsubscribe();

        messageStore = new(CurrentEditContext);
        subscribedEditContext = CurrentEditContext;

        CurrentEditContext.OnValidationRequested += HandleValidationRequested;
        CurrentEditContext.OnFieldChanged += HandleFieldChanged;
    }

    public void Dispose()
    {
        Unsubscribe();
    }

    private void Unsubscribe()
    {
        if (subscribedEditContext is not null)
        {
            subscribedEditContext.OnValidationRequested -= HandleValidationRequested;
            subscribedEditContext.OnFieldChanged -= HandleFieldChanged;
            subscribedEditContext = null;
        }

        messageStore = null;
    }

    private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs e) =>
        messageStore?.Clear();

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e) =>
        messageStore?.Clear(e.FieldIdentifier);

    [RequiresDynamicCode("Expression trees are not supported in AOT environments. Use the FieldIdentifier overload instead.")]
    [RequiresUnreferencedCode("Expression trees are not supported in trimmed environments. Use the FieldIdentifier overload instead.")]
    public void DisplayError<TField>(Expression<Func<TField>> expression, string error)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(FieldIdentifier.Create(expression), error);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void DisplayError(FieldIdentifier fieldIdentifier, string error)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(fieldIdentifier, error);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    [RequiresDynamicCode("Expression trees are not supported in AOT environments. Use the FieldIdentifier overload instead.")]
    [RequiresUnreferencedCode("Expression trees are not supported in trimmed environments. Use the FieldIdentifier overload instead.")]
    public void DisplayErrors<TField>(Expression<Func<TField>> expression, IEnumerable<string> errors)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(FieldIdentifier.Create(expression), errors);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void DisplayErrors(FieldIdentifier fieldIdentifier, IEnumerable<string> errors)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(fieldIdentifier, errors);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void DisplayError(string name, string error)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(CurrentEditContext.Field(name), error);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void DisplayErrors(string name, IEnumerable<string> errors)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(CurrentEditContext.Field(name), errors);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void ClearErrors()
    {
        messageStore?.Clear();
        CurrentEditContext?.NotifyValidationStateChanged();
    }
}
