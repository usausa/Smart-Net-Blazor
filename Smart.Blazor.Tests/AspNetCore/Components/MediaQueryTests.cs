namespace Smart.AspNetCore.Components;

public sealed class MediaQueryTests : BunitContext
{
    private const string ModulePath = "./_content/Usa.Smart.Blazor/media-query.js";

    public MediaQueryTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void ObservesQueryAndNotifiesCurrentValue()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var observer = module.SetupModule("observe", static _ => true);
        observer.Setup<bool>("matches").SetResult(true);
        var values = new List<bool>();

        var cut = Render<MediaQuery>(parameters => parameters
            .Add(static p => p.Query, "(max-width: 600px)")
            .Add(static p => p.MatchesChanged, values.Add));

        cut.WaitForAssertion(() => Assert.True(Assert.Single(values)));
        Assert.Equal("(max-width: 600px)", module.VerifyInvoke("observe").Arguments[1]);
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void DoesNotNotifyWhenValueIsUnchanged()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var observer = module.SetupModule("observe", static _ => true);
        observer.Setup<bool>("matches").SetResult(false);
        var values = new List<bool>();

        var cut = Render<MediaQuery>(parameters => parameters
            .Add(static p => p.Query, "(prefers-color-scheme: dark)")
            .Add(static p => p.MatchesChanged, values.Add));

        cut.WaitForAssertion(() => observer.VerifyInvoke("matches"));
        Assert.Empty(values);
    }

    [Fact]
    public async Task NotifiesChangeFromScript()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var observer = module.SetupModule("observe", static _ => true);
        observer.Setup<bool>("matches").SetResult(false);
        var values = new List<bool>();

        var cut = Render<MediaQuery>(parameters => parameters
            .Add(static p => p.Query, "(pointer: coarse)")
            .Add(static p => p.MatchesChanged, values.Add));
        await cut.WaitForAssertionAsync(() => observer.VerifyInvoke("matches"));

        await cut.InvokeAsync(() => cut.Instance.NotifyChange(true));

        Assert.True(Assert.Single(values));
    }

    [Fact]
    public void UpdatesWhenQueryChanges()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var observer = module.SetupModule("observe", static _ => true);
        observer.Setup<bool>("matches").SetResult(false);
        observer.Setup<bool>("update", static _ => true).SetResult(true);
        var values = new List<bool>();

        var cut = Render<MediaQuery>(parameters => parameters
            .Add(static p => p.Query, "(max-width: 600px)")
            .Add(static p => p.MatchesChanged, values.Add));
        cut.WaitForAssertion(() => observer.VerifyInvoke("matches"));

        cut.Render(static parameters => parameters.Add(static p => p.Query, "(max-width: 900px)"));

        cut.WaitForAssertion(() => Assert.True(Assert.Single(values)));
        Assert.Equal("(max-width: 900px)", observer.VerifyInvoke("update").Arguments[0]);
        Assert.Single(module.Invocations["observe"]);
    }

    [Fact]
    public async Task DisposeReleasesObserver()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var observer = module.SetupModule("observe", static _ => true);
        observer.Setup<bool>("matches").SetResult(false);
        observer.SetupVoid("dispose").SetVoidResult();

        var cut = Render<MediaQuery>(static parameters => parameters
            .Add(static p => p.Query, "(max-width: 600px)"));
        await cut.WaitForAssertionAsync(() => observer.VerifyInvoke("matches"));

        await DisposeComponentsAsync();

        observer.VerifyInvoke("dispose");
    }
}
