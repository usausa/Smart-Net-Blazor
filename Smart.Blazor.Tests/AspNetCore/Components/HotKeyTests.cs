namespace Smart.AspNetCore.Components;

public sealed class HotKeyTests : BunitContext
{
    private const string ModulePath = "./_content/Usa.Smart.Blazor/hot-key.js";

    public HotKeyTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RegistersParsedKeyAndOptions()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupModule("register", static _ => true);

        Render<HotKey>(static parameters => parameters
            .Add(static p => p.Key, "Ctrl + Shift + s")
            .Add(static p => p.AllowInInput, true));

        var arguments = module.VerifyInvoke("register").Arguments;
        Assert.Equal("S,True,False,True,False,True,True,False", string.Join(',', arguments.Skip(1)));
    }

    [Theory]
    [InlineData("Space", " ")]
    [InlineData("f2", "F2")]
    [InlineData("Meta+Enter", "ENTER")]
    [InlineData("Alt+ArrowUp", "ARROWUP")]
    public void NormalizesKeyName(string key, string expected)
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupModule("register", static _ => true);

        Render<HotKey>(parameters => parameters.Add(static p => p.Key, key));

        Assert.Equal(expected, module.VerifyInvoke("register").Arguments[1]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Ctrl+")]
    [InlineData("Hyper+S")]
    public void RejectsUnsupportedKey(string key)
    {
        JSInterop.SetupModule(ModulePath);

        Assert.Throws<ArgumentException>(() => Render<HotKey>(parameters => parameters.Add(static p => p.Key, key)));
    }

    [Fact]
    public async Task InvokesOnPressedFromScript()
    {
        var module = JSInterop.SetupModule(ModulePath);
        module.SetupModule("register", static _ => true);
        var count = 0;

        var cut = Render<HotKey>(parameters => parameters
            .Add(static p => p.Key, "F2")
            .Add(static p => p.OnPressed, () => count++));
        await cut.WaitForAssertionAsync(() => module.VerifyInvoke("register"));

        await cut.InvokeAsync(cut.Instance.NotifyPressed);

        Assert.Equal(1, count);
    }

    [Fact]
    public void UpdatesWhenParametersChange()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var registration = module.SetupModule("register", static _ => true);
        registration.SetupVoid("update", static _ => true).SetVoidResult();

        var cut = Render<HotKey>(static parameters => parameters.Add(static p => p.Key, "F2"));
        cut.WaitForAssertion(() => module.VerifyInvoke("register"));

        cut.Render(static parameters => parameters
            .Add(static p => p.Key, "Ctrl+F2")
            .Add(static p => p.AllowRepeat, true));

        cut.WaitForAssertion(() => Assert.Equal("F2,True,False,False,False,True,False,True", string.Join(',', registration.VerifyInvoke("update").Arguments)));
        Assert.Single(module.Invocations["register"]);
    }

    [Fact]
    public async Task DisposeReleasesRegistration()
    {
        var module = JSInterop.SetupModule(ModulePath);
        var registration = module.SetupModule("register", static _ => true);
        registration.SetupVoid("dispose").SetVoidResult();

        var cut = Render<HotKey>(static parameters => parameters.Add(static p => p.Key, "F2"));
        await cut.WaitForAssertionAsync(() => module.VerifyInvoke("register"));

        await DisposeComponentsAsync();

        registration.VerifyInvoke("dispose");
    }
}
