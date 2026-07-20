namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public sealed class CustomValidatorTest : BunitContext
{
    [Fact]
    public void DisplayError_WithExpression_AddsMessage()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;

        validator.DisplayError(() => model.Name, "name error");

        Assert.Contains("name error", editContext.GetValidationMessages());
    }

    [Fact]
    public void DisplayError_WithFieldIdentifier_AddsMessage()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;

        validator.DisplayError(editContext.Field(nameof(TestModel.Name)), "field error");

        Assert.Contains("field error", editContext.GetValidationMessages());
    }

    [Fact]
    public void DisplayError_WithFieldName_AddsMessage()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;

        validator.DisplayError(nameof(TestModel.Name), "named error");

        Assert.Contains("named error", editContext.GetValidationMessages());
    }

    [Fact]
    public void DisplayErrors_AddsMultipleMessages()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;

        validator.DisplayErrors(nameof(TestModel.Name), ["error1", "error2"]);

        var messages = editContext.GetValidationMessages().ToArray();
        Assert.Contains("error1", messages);
        Assert.Contains("error2", messages);
    }

    [Fact]
    public void ClearErrors_RemovesMessages()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;
        validator.DisplayError(nameof(TestModel.Name), "error");

        validator.ClearErrors();

        Assert.Empty(editContext.GetValidationMessages());
    }

    [Fact]
    public void FieldChanged_ClearsFieldMessages()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;
        validator.DisplayError(nameof(TestModel.Name), "error");

        editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name)));

        Assert.Empty(editContext.GetValidationMessages());
    }

    [Fact]
    public void ValidationRequested_ClearsMessages()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;
        validator.DisplayError(nameof(TestModel.Name), "error");

        editContext.Validate();

        Assert.Empty(editContext.GetValidationMessages());
    }

    [Fact]
    public void FollowsEditContextSwap()
    {
        var editContext1 = new EditContext(new TestModel());
        var editContext2 = new EditContext(new TestModel());

        var cut = Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(static p => p.Value, editContext1)
            .Add(static p => p.ChildContent, ValidatorFragment));

        cut.Render(parameters => parameters
            .Add(static p => p.Value, editContext2)
            .Add(static p => p.ChildContent, ValidatorFragment));

        var validator = cut.FindComponent<CustomValidator>().Instance;
        validator.DisplayError(nameof(TestModel.Name), "swapped");

        Assert.Contains("swapped", editContext2.GetValidationMessages());
        Assert.DoesNotContain("swapped", editContext1.GetValidationMessages());
    }

    [Fact]
    public async Task Dispose_UnsubscribesFromEditContext()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var validator = RenderValidator(editContext).Instance;
        validator.DisplayError(nameof(TestModel.Name), "error");

        await DisposeComponentsAsync();

        editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name)));

        Assert.Contains("error", editContext.GetValidationMessages());
    }

    private IRenderedComponent<CustomValidator> RenderValidator(EditContext editContext)
    {
        var cut = Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(static p => p.Value, editContext)
            .Add(static p => p.ChildContent, ValidatorFragment));
        return cut.FindComponent<CustomValidator>();
    }

    private static readonly RenderFragment ValidatorFragment = static builder =>
    {
        builder.OpenComponent<CustomValidator>(0);
        builder.CloseComponent();
    };

    private sealed class TestModel
    {
        public string? Name { get; set; }
    }
}
