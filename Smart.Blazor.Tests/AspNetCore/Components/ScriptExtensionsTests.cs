namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

public sealed class ScriptExtensionsTests : BunitContext
{
    public ScriptExtensionsTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task SetFocusInvokesJs()
    {
        await JSInterop.JSRuntime.SetFocus("my-id");

        var invocation = JSInterop.VerifyInvoke("Smart.setFocus");
        Assert.Equal("my-id", invocation.Arguments[0]);
    }

    [Fact]
    public async Task HistoryBackInvokesJs()
    {
        await JSInterop.JSRuntime.HistoryBack();

        JSInterop.VerifyInvoke("history.back");
    }

    [Fact]
    public async Task HistoryForwardInvokesJs()
    {
        await JSInterop.JSRuntime.HistoryForward();

        JSInterop.VerifyInvoke("history.forward");
    }

    [Fact]
    public async Task HistoryGoInvokesJsWithDelta()
    {
        await JSInterop.JSRuntime.HistoryGo(-2);

        var invocation = JSInterop.VerifyInvoke("history.go");
        Assert.Equal(-2, invocation.Arguments[0]);
    }

    [Fact]
    public async Task SaveAsFileInvokesJsWithStreamReference()
    {
        await JSInterop.JSRuntime.SaveAsFile("file.txt", "text/plain", [1, 2, 3]);

        var invocation = JSInterop.VerifyInvoke("Smart.saveAsFile");
        Assert.Equal("file.txt", invocation.Arguments[0]);
        Assert.Equal("text/plain", invocation.Arguments[1]);
        Assert.IsType<DotNetStreamReference>(invocation.Arguments[2]);
    }

    [Fact]
    public async Task OpenNewWindowInvokesJsWithStreamReference()
    {
        await JSInterop.JSRuntime.OpenNewWindow("application/pdf", [1, 2, 3]);

        var invocation = JSInterop.VerifyInvoke("Smart.openNewWindow");
        Assert.Equal("application/pdf", invocation.Arguments[0]);
        Assert.IsType<DotNetStreamReference>(invocation.Arguments[1]);
    }
}
