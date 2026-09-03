namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public sealed class HasErrorTests : BunitContext
{
    [Fact]
    public async Task RendersChildContentOnValidationStateChange()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var messageStore = new ValidationMessageStore(editContext);

        var cut = Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(static p => p.Value, editContext)
            .Add(static p => p.ChildContent, HasErrorFragment));

        Assert.DoesNotContain("error-shown", cut.Markup, StringComparison.Ordinal);

        await cut.InvokeAsync(() =>
        {
            messageStore.Add(editContext.Field(nameof(TestModel.Name)), "error");
            editContext.NotifyValidationStateChanged();
        });

        Assert.Contains("error-shown", cut.Markup, StringComparison.Ordinal);
    }

    private static readonly RenderFragment HasErrorFragment = static builder =>
    {
        builder.OpenComponent<HasError>(0);
        builder.AddAttribute(1, nameof(HasError.ChildContent), (RenderFragment)(static b => b.AddContent(0, "error-shown")));
        builder.CloseComponent();
    };

    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
