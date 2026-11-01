namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

internal static class ScriptReferenceExtensions
{
    public static async ValueTask ReleaseAsync(this IJSObjectReference reference, string? disposeIdentifier = null)
    {
        if (disposeIdentifier is not null)
        {
            await IgnoreTeardownAsync(reference.InvokeVoidAsync(disposeIdentifier)).ConfigureAwait(false);
        }

        await IgnoreTeardownAsync(reference.DisposeAsync()).ConfigureAwait(false);
    }

    private static async ValueTask IgnoreTeardownAsync(ValueTask task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
