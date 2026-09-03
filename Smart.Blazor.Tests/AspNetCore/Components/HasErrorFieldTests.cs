namespace Smart.AspNetCore.Components;

using System.Linq.Expressions;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public sealed class HasErrorFieldTests : BunitContext
{
    [Fact]
    public async Task RendersChildContentOnValidationStateChange()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var messageStore = new ValidationMessageStore(editContext);

        var cut = RenderHost(model, editContext);

        Assert.DoesNotContain("field-error-shown", cut.Markup, StringComparison.Ordinal);

        await cut.InvokeAsync(() =>
        {
            messageStore.Add(editContext.Field(nameof(TestModel.Name)), "error");
            editContext.NotifyValidationStateChanged();
        });

        Assert.Contains("field-error-shown", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ReRendersOnlyWhenTargetFieldChanges()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);

        var cut = RenderHost(model, editContext);
        var field = cut.FindComponent<HasErrorField<string>>();

        var countBeforeOther = field.RenderCount;
        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Email))));
        Assert.Equal(countBeforeOther, field.RenderCount);

        var countBeforeTarget = field.RenderCount;
        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name))));
        Assert.True(field.RenderCount > countBeforeTarget);
    }

    private IRenderedComponent<CascadingValue<EditContext>> RenderHost(TestModel model, EditContext editContext)
    {
        Expression<Func<string>> forExpression = () => model.Name;

        return Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(p => p.Value, editContext)
            .Add(p => p.ChildContent, builder =>
            {
                builder.OpenComponent<HasErrorField<string>>(0);
                builder.AddAttribute(1, nameof(HasErrorField<>.For), forExpression);
                builder.AddAttribute(2, nameof(HasErrorField<>.ChildContent), (RenderFragment)(b => b.AddContent(0, "field-error-shown")));
                builder.CloseComponent();
            }));
    }

    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
