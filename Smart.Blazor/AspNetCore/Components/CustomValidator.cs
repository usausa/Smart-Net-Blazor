namespace Smart.AspNetCore.Components;

using System.Linq.Expressions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public sealed class CustomValidator : ComponentBase
{
    private ValidationMessageStore? messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException($"{nameof(EditContext)} is required.");
        }

        messageStore = new(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += (_, _) => messageStore?.Clear();
        CurrentEditContext.OnFieldChanged += (_, e) => messageStore?.Clear(e.FieldIdentifier);
    }

    public void DisplayError<TField>(Expression<Func<TField>> expression, string error)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(FieldIdentifier.Create(expression), error);

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void DisplayErrors<TField>(Expression<Func<TField>> expression, IEnumerable<string> errors)
    {
        if ((CurrentEditContext is not null) && (messageStore is not null))
        {
            messageStore.Add(FieldIdentifier.Create(expression), errors);

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
