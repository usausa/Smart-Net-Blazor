namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

public static class ScriptExtensions
{
    public static async ValueTask SetFocus(this IJSRuntime runtime, string id)
    {
        await runtime.InvokeAsync<string>("Smart.setFocus", id).ConfigureAwait(false);
    }

    public static async ValueTask<string> SaveAsFile(this IJSRuntime runtime, string filename, string contentType, byte[] bytes)
    {
        return await runtime.InvokeAsync<string>("Smart.saveAsFile", filename, contentType, Convert.ToBase64String(bytes)).ConfigureAwait(false);
    }

    public static async ValueTask<string> OpenNewWindow(this IJSRuntime runtime, string contentType, byte[] bytes)
    {
        return await runtime.InvokeAsync<string>("Smart.openNewWindow", contentType, Convert.ToBase64String(bytes)).ConfigureAwait(false);
    }
}
