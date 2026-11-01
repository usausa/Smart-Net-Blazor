namespace Smart.AspNetCore.Components;

using System.Diagnostics.CodeAnalysis;

using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

public sealed class ScriptModule : IAsyncDisposable
{
    private const DynamicallyAccessedMemberTypes JsonSerialized =
        DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties;

#if NET9_0_OR_GREATER
    private readonly Lock sync = new();
#else
    private readonly object sync = new();
#endif

    private readonly List<PendingCall> pending = [];

    private readonly IJSRuntime runtime;

    private Task? loading;

    private IJSObjectReference? module;

    private bool disposed;

    public ScriptModule(IJSRuntime runtime, string path)
    {
        this.runtime = runtime;
        Path = path;
    }

    public string Path { get; }

    public async ValueTask InvokeVoidAsync(string identifier, params object?[]? args) =>
        await InvokeAsync<IJSVoidResult>(identifier, args).ConfigureAwait(false);

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(JsonSerialized)] TValue>(string identifier, params object?[]? args)
    {
        IJSObjectReference? loaded;
        lock (sync)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            loaded = module;
            if (loaded is null)
            {
                var call = new PendingCall<TValue>(identifier, args);
                pending.Add(call);
                if ((loading is null) || loading.IsCompleted)
                {
                    loading = LoadAsync();
                }

                return new ValueTask<TValue>(call.Completion);
            }
        }

        return loaded.InvokeAsync<TValue>(identifier, args);
    }

    public ValueTask DisposeAsync()
    {
        IJSObjectReference? loaded;
        PendingCall[] calls;
        lock (sync)
        {
            if (disposed)
            {
                return ValueTask.CompletedTask;
            }

            disposed = true;
            loaded = module;
            module = null;
            calls = [.. pending];
            pending.Clear();
        }

        var exception = new ObjectDisposedException(nameof(ScriptModule));
        foreach (var call in calls)
        {
            call.Fail(exception);
        }

        return loaded?.ReleaseAsync() ?? ValueTask.CompletedTask;
    }

    private async Task LoadAsync()
    {
        var import = ImportAsync();
        await ((Task)import).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        if (!import.IsCompletedSuccessfully)
        {
            var exception = import.Exception?.InnerException ?? new TaskCanceledException(import);
            PendingCall[] calls;
            lock (sync)
            {
                loading = null;
                calls = [.. pending];
                pending.Clear();
            }

            foreach (var call in calls)
            {
                call.Fail(exception);
            }

            return;
        }

        var loaded = await import.ConfigureAwait(false);
        while (true)
        {
            PendingCall[] calls;
            lock (sync)
            {
                if (disposed)
                {
                    break;
                }

                if (pending.Count == 0)
                {
                    module = loaded;
                    return;
                }

                calls = [.. pending];
                pending.Clear();
            }

            foreach (var call in calls)
            {
                call.Run(loaded);
            }
        }

        await loaded.ReleaseAsync().ConfigureAwait(false);
    }

    private async Task<IJSObjectReference> ImportAsync() =>
        await runtime.InvokeAsync<IJSObjectReference>("import", Path).ConfigureAwait(false);

    private abstract class PendingCall
    {
        public abstract void Run(IJSObjectReference loaded);

        public abstract void Fail(Exception exception);
    }

    private sealed class PendingCall<[DynamicallyAccessedMembers(JsonSerialized)] TValue> : PendingCall
    {
        private readonly TaskCompletionSource<TValue> source = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly string identifier;

        private readonly object?[]? args;

        public PendingCall(string identifier, object?[]? args)
        {
            this.identifier = identifier;
            this.args = args;
        }

        public Task<TValue> Completion => source.Task;

        public override void Run(IJSObjectReference loaded) =>
            _ = CompleteAsync(loaded);

        public override void Fail(Exception exception) =>
            source.TrySetException(exception);

        private async Task CompleteAsync(IJSObjectReference loaded)
        {
            var call = InvokeCoreAsync(loaded);
            await ((Task)call).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            if (call.IsCompletedSuccessfully)
            {
                source.TrySetResult(await call.ConfigureAwait(false));
            }
            else if (call.IsCanceled)
            {
                source.TrySetCanceled();
            }
            else
            {
                source.TrySetException(call.Exception!.InnerExceptions);
            }
        }

        private async Task<TValue> InvokeCoreAsync(IJSObjectReference loaded) =>
            await loaded.InvokeAsync<TValue>(identifier, args).ConfigureAwait(false);
    }
}
