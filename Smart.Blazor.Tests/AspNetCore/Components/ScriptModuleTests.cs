namespace Smart.AspNetCore.Components;

using Microsoft.JSInterop;

using TestContext = Xunit.TestContext;

public sealed class ScriptModuleTests
{
    private const string ModulePath = "./module.js";

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    private static Task WaitAsync(Task task) =>
        task.WaitAsync(Timeout, TestContext.Current.CancellationToken);

    private static Task<TValue> WaitAsync<TValue>(Task<TValue> task) =>
        task.WaitAsync(Timeout, TestContext.Current.CancellationToken);

    [Fact]
    public async Task ImportsOnceForCallsMadeWhileLoading()
    {
        var runtime = new FakeRuntime();
        var module = new ScriptModule(runtime, ModulePath);

        var first = module.InvokeVoidAsync("first").AsTask();
        var second = module.InvokeVoidAsync("second").AsTask();
        runtime.CompleteImport(0);
        var script = runtime.Modules[0];
        await WaitAsync(Task.WhenAll(first, second));

        Assert.Single(runtime.Imports);
        Assert.Equal(ModulePath, runtime.ImportPaths[0]);
        Assert.Equal("first,second", string.Join(',', script.Calls));

        await module.DisposeAsync();
    }

    [Fact]
    public async Task CallMadeWhileSendingQueuedCallsGoesLast()
    {
        var runtime = new FakeRuntime();
        var module = new ScriptModule(runtime, ModulePath);
        var third = Task.CompletedTask;

        var first = module.InvokeVoidAsync("first").AsTask();
        var second = module.InvokeVoidAsync("second").AsTask();
        runtime.CompleteImport(0, invoked: identifier =>
        {
            if (identifier == "first")
            {
                third = module.InvokeVoidAsync("third").AsTask();
            }
        });
        await WaitAsync(Task.WhenAll(first, second));
        await WaitAsync(third);

        Assert.Equal("first,second,third", string.Join(',', runtime.Modules[0].Calls));

        await module.DisposeAsync();
    }

    [Fact]
    public async Task ReturnsValueFromModule()
    {
        var runtime = new FakeRuntime();
        var module = new ScriptModule(runtime, ModulePath);

        var queued = module.InvokeAsync<int>("value").AsTask();
        runtime.CompleteImport(0, result: 42);

        Assert.Equal(42, await WaitAsync(queued));
        Assert.Equal(42, await module.InvokeAsync<int>("value"));

        await module.DisposeAsync();
    }

    [Fact]
    public async Task RetriesImportAfterFailure()
    {
        var runtime = new FakeRuntime();
        var module = new ScriptModule(runtime, ModulePath);

        var failed = module.InvokeVoidAsync("first").AsTask();
        runtime.FailImport(0, new JSException("import failed"));
        await Assert.ThrowsAsync<JSException>(() => WaitAsync(failed));

        var retried = module.InvokeVoidAsync("second").AsTask();
        runtime.CompleteImport(1);
        await WaitAsync(retried);

        Assert.Equal(2, runtime.Imports.Count);
        Assert.Equal("second", string.Join(',', runtime.Modules[0].Calls));

        await module.DisposeAsync();
    }

    [Fact]
    public async Task DisposeWhileLoadingFailsQueuedCallsAndReleasesModule()
    {
        var runtime = new FakeRuntime();
        var module = new ScriptModule(runtime, ModulePath);

        var queued = module.InvokeVoidAsync("first").AsTask();
        await module.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => WaitAsync(queued));

        runtime.CompleteImport(0);
        await WaitAsync(runtime.Modules[0].Disposal);

        Assert.Empty(runtime.Modules[0].Calls);
    }

    [Fact]
    public async Task DisposeIgnoresDisconnectedRuntime()
    {
        var runtime = new FakeRuntime();
        var module = new ScriptModule(runtime, ModulePath);

        var call = module.InvokeVoidAsync("first").AsTask();
        runtime.CompleteImport(0, disposeException: new JSDisconnectedException("disconnected"));
        await WaitAsync(call);
        await module.DisposeAsync();

        Assert.True(runtime.Modules[0].Disposal.IsCompleted);
    }

    [Fact]
    public async Task CallAfterDisposeThrows()
    {
        var module = new ScriptModule(new FakeRuntime(), ModulePath);
        await module.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => module.InvokeVoidAsync("first").AsTask());
    }

    private sealed class FakeRuntime : IJSRuntime
    {
        public List<TaskCompletionSource<IJSObjectReference>> Imports { get; } = [];

        public List<string?> ImportPaths { get; } = [];

        public List<FakeModule> Modules { get; } = [];

        public void CompleteImport(int index, object? result = null, Exception? disposeException = null, Action<string>? invoked = null)
        {
            var module = new FakeModule(result, disposeException, invoked);
            Modules.Add(module);
            Imports[index].SetResult(module);
        }

        public void FailImport(int index, Exception exception) => Imports[index].SetException(exception);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            var import = new TaskCompletionSource<IJSObjectReference>(TaskCreationOptions.RunContinuationsAsynchronously);
            Imports.Add(import);
            ImportPaths.Add(args?[0] as string);
            return new ValueTask<TValue>((Task<TValue>)(object)import.Task);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }

    private sealed class FakeModule : IJSObjectReference
    {
        private readonly TaskCompletionSource disposal = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly object? result;

        private readonly Exception? disposeException;

        private readonly Action<string>? invoked;

        public FakeModule(object? result, Exception? disposeException, Action<string>? invoked)
        {
            this.result = result;
            this.disposeException = disposeException;
            this.invoked = invoked;
        }

        public List<string> Calls { get; } = [];

        public Task Disposal => disposal.Task;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Calls.Add(identifier);
            invoked?.Invoke(identifier);
            return new ValueTask<TValue>(result is TValue value ? value : default!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);

        public ValueTask DisposeAsync()
        {
            disposal.TrySetResult();
            return disposeException is null ? ValueTask.CompletedTask : ValueTask.FromException(disposeException);
        }
    }
}
