namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

public static class ScriptExtensions
{
    public static ValueTask SetFocus(this IJSRuntime runtime, string id) =>
        runtime.InvokeVoidAsync("Smart.setFocus", id);

    public static ValueTask<string> SaveAsFile(this IJSRuntime runtime, string filename, string contentType, byte[] bytes) =>
        runtime.InvokeAsync<string>("Smart.saveAsFile", filename, contentType, Convert.ToBase64String(bytes));

    public static ValueTask<string> OpenNewWindow(this IJSRuntime runtime, string contentType, byte[] bytes) =>
        runtime.InvokeAsync<string>("Smart.openNewWindow", contentType, Convert.ToBase64String(bytes));
}
