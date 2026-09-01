namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

public sealed class ScriptExtensionsTest : BunitContext
{
    public ScriptExtensionsTest()
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
