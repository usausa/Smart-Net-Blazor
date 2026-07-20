namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

public static class ScriptExtensions
{
    public static ValueTask SetFocus(this IJSRuntime runtime, string id) =>
        runtime.InvokeVoidAsync("Smart.setFocus", id);

    public static async ValueTask SaveAsFile(this IJSRuntime runtime, string filename, string contentType, byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var streamReference = new DotNetStreamReference(stream, leaveOpen: true);
        await runtime.InvokeVoidAsync("Smart.saveAsFile", filename, contentType, streamReference).ConfigureAwait(false);
    }

    public static async ValueTask OpenNewWindow(this IJSRuntime runtime, string contentType, byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var streamReference = new DotNetStreamReference(stream, leaveOpen: true);
        await runtime.InvokeVoidAsync("Smart.openNewWindow", contentType, streamReference).ConfigureAwait(false);
    }
}
