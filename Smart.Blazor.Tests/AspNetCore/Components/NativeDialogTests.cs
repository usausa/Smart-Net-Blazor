namespace Smart.AspNetCore.Components;

public sealed class NativeDialogTests : BunitContext
{
    private const string ModulePath = "./_content/Usa.Smart.Blazor/native-dialog.js";

    public NativeDialogTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersDialogWithAttributesAndContent()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupModule("attach", static _ => true);

        var cut = Render<NativeDialog>(static parameters => parameters
            .AddUnmatched("class", "confirm")
            .AddChildContent("<p>body</p>"));

        var dialog = cut.Find("dialog");
        Assert.Equal("confirm", dialog.GetAttribute("class"));
        Assert.Equal("body", dialog.TextContent);
        Assert.False(dialog.HasAttribute("open"));
    }

    [Fact]
    public void AttachesWithCloseOptions()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupModule("attach", static _ => true);

        Render<NativeDialog>(static parameters => parameters
            .Add(static p => p.CloseOnEscape, false)
            .Add(static p => p.CloseOnBackdropClick, true));

        var arguments = module.VerifyInvoke("attach").Arguments;
        Assert.Equal("False,True", string.Join(',', arguments.Skip(2)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ShowsWhenOpen(bool modal)
    {
        var module = JSInterop.SetupModule(ModulePath);
        var dialog = module.SetupModule("attach", static _ => true);
        dialog.SetupVoid("show", static _ => true).SetVoidResult();

        var cut = Render<NativeDialog>(parameters => parameters
            .Add(static p => p.Open, true)
            .Add(static p => p.Modal, modal));

        cut.WaitForAssertion(() => Assert.Equal(modal, dialog.VerifyInvoke("show").Arguments[0]));
    }

    [Fact]
    public void ClosesWhenOpenTurnsFalse()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var dialog = module.SetupModule("attach", static _ => true);
        dialog.SetupVoid("show", static _ => true).SetVoidResult();
        dialog.SetupVoid("close").SetVoidResult();

        var cut = Render<NativeDialog>(static parameters => parameters.Add(static p => p.Open, true));
        cut.WaitForAssertion(() => dialog.VerifyInvoke("show"));

        cut.Render(static parameters => parameters.Add(static p => p.Open, false));

        cut.WaitForAssertion(() => dialog.VerifyInvoke("close"));
    }

    [Fact]
    public void UpdatesCloseOptions()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var dialog = module.SetupModule("attach", static _ => true);
        dialog.SetupVoid("update", static _ => true).SetVoidResult();

        var cut = Render<NativeDialog>(static parameters => parameters.Add(static p => p.CloseOnEscape, true));
        cut.WaitForAssertion(() => module.VerifyInvoke("attach"));

        cut.Render(static parameters => parameters.Add(static p => p.CloseOnEscape, false));

        cut.WaitForAssertion(() => Assert.Equal("False,False", string.Join(',', dialog.VerifyInvoke("update").Arguments)));
    }

    [Fact]
    public async Task NotifyCloseUpdatesBindingAndRaisesOnClose()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var dialog = module.SetupModule("attach", static _ => true);
        dialog.SetupVoid("show", static _ => true).SetVoidResult();
        var opens = new List<bool>();
        var values = new List<string>();

        var cut = Render<NativeDialog>(parameters => parameters
            .Add(static p => p.Open, true)
            .Add(static p => p.OpenChanged, opens.Add)
            .Add(static p => p.OnClose, values.Add));
        await cut.WaitForAssertionAsync(() => dialog.VerifyInvoke("show"));

        await cut.InvokeAsync(() => cut.Instance.NotifyClose("ok"));

        Assert.False(Assert.Single(opens));
        Assert.Equal("ok", Assert.Single(values));
    }

    [Fact]
    public async Task NotifyCloseWithoutOpenRaisesOnlyOnClose()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupModule("attach", static _ => true);
        var opens = new List<bool>();
        var values = new List<string>();

        var cut = Render<NativeDialog>(parameters => parameters
            .Add(static p => p.OpenChanged, opens.Add)
            .Add(static p => p.OnClose, values.Add));
        await cut.WaitForAssertionAsync(() => module.VerifyInvoke("attach"));

        await cut.InvokeAsync(() => cut.Instance.NotifyClose(string.Empty));

        Assert.Empty(opens);
        Assert.Equal(string.Empty, Assert.Single(values));
    }

    [Fact]
    public async Task DisposeReleasesDialog()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var dialog = module.SetupModule("attach", static _ => true);
        dialog.SetupVoid("dispose").SetVoidResult();

        var cut = Render<NativeDialog>();
        await cut.WaitForAssertionAsync(() => module.VerifyInvoke("attach"));

        await DisposeComponentsAsync();

        dialog.VerifyInvoke("dispose");
    }
}
