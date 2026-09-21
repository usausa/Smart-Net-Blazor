namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components.Forms;

public static class EditContextExtensions
{
    public static void NotifyFieldChanged(this EditContext editContext, string fieldName) =>
        editContext.NotifyFieldChanged(editContext.Field(fieldName));

    public static void NotifyFieldsChanged(this EditContext editContext, params ReadOnlySpan<string> fieldNames)
    {
        foreach (var fieldName in fieldNames)
        {
            editContext.NotifyFieldChanged(editContext.Field(fieldName));
        }
    }
}
