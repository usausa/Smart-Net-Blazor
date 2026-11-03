namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

public sealed class ModifiedNavigationLockTests : BunitContext
{
    public ModifiedNavigationLockTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void NavigatesWithoutConfirmWhenNotModified()
    {
        var editContext = new EditContext(new TestModel());
        var calls = 0;
        RenderLock(editContext, () =>
        {
            calls++;
            return Task.FromResult(false);
        });
        var navigation = Services.GetRequiredService<NavigationManager>();

        navigation.NavigateTo("other");

        Assert.EndsWith("/other", navigation.Uri, StringComparison.Ordinal);
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task PreventsNavigationWhenConfirmReturnsFalse()
    {
        var editContext = new EditContext(new TestModel());
        var cut = RenderLock(editContext, static () => Task.FromResult(false));
        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name))));
        var navigation = Services.GetRequiredService<NavigationManager>();

        navigation.NavigateTo("other");

        Assert.DoesNotContain("/other", navigation.Uri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NavigatesWhenConfirmReturnsTrue()
    {
        var editContext = new EditContext(new TestModel());
        var calls = 0;
        var cut = RenderLock(editContext, () =>
        {
            calls++;
            return Task.FromResult(true);
        });
        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name))));
        var navigation = Services.GetRequiredService<NavigationManager>();

        navigation.NavigateTo("other");

        Assert.EndsWith("/other", navigation.Uri, StringComparison.Ordinal);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task NavigatesAfterMarkAsUnmodified()
    {
        var editContext = new EditContext(new TestModel());
        var cut = RenderLock(editContext, static () => Task.FromResult(false));
        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name))));
        var navigation = Services.GetRequiredService<NavigationManager>();

        editContext.MarkAsUnmodified();
        navigation.NavigateTo("other");

        Assert.EndsWith("/other", navigation.Uri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task NavigatesWhenConfirmIsNotSet()
    {
        var editContext = new EditContext(new TestModel());
        var cut = RenderLock(editContext, null);
        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name))));
        var navigation = Services.GetRequiredService<NavigationManager>();

        navigation.NavigateTo("other");

        Assert.EndsWith("/other", navigation.Uri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConfirmsExternalNavigationOnlyWhileModified()
    {
        var editContext = new EditContext(new TestModel());
        var cut = RenderLock(editContext, static () => Task.FromResult(true));
        var before = JSInterop.Invocations.Count(static x => x.Identifier.Contains("enableNavigationPrompt", StringComparison.Ordinal));

        await cut.InvokeAsync(() => editContext.NotifyFieldChanged(editContext.Field(nameof(TestModel.Name))));

        var after = JSInterop.Invocations.Count(static x => x.Identifier.Contains("enableNavigationPrompt", StringComparison.Ordinal));
        Assert.Equal(0, before);
        Assert.Equal(1, after);
    }

    [Fact]
    public void ThrowsWithoutEditContext()
    {
        Assert.Throws<InvalidOperationException>(() => Render<ModifiedNavigationLock>());
    }

    private IRenderedComponent<ModifiedNavigationLock> RenderLock(EditContext editContext, Func<Task<bool>>? confirm)
    {
        var cut = Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(static p => p.Value, editContext)
            .Add(static p => p.ChildContent, builder =>
            {
                builder.OpenComponent<ModifiedNavigationLock>(0);
                builder.AddAttribute(1, nameof(ModifiedNavigationLock.OnConfirm), confirm);
                builder.CloseComponent();
            }));
        return cut.FindComponent<ModifiedNavigationLock>();
    }

    private sealed class TestModel
    {
        public string? Name { get; set; }
    }
}
