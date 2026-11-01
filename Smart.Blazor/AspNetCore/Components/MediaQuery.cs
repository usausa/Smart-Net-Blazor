namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public sealed class MediaQuery : ComponentBase, IAsyncDisposable
{
    private const string ModulePath = "./_content/Usa.Smart.Blazor/media-query.js";

    private ScriptModule? module;

    private DotNetObjectReference<MediaQuery>? reference;

    private IJSObjectReference? observer;

    private string? observedQuery;

    private bool disposed;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public string Query { get; set; } = default!;

    [Parameter]
    public bool Matches { get; set; }

    [Parameter]
    public EventCallback<bool> MatchesChanged { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = new ScriptModule(JSRuntime, ModulePath);
            reference = DotNetObjectReference.Create(this);
            observedQuery = Query;

            IJSObjectReference created;
            try
            {
                created = await module.InvokeAsync<IJSObjectReference>("observe", reference, observedQuery).ConfigureAwait(true);
            }
            catch (ObjectDisposedException) when (disposed)
            {
                return;
            }

            if (disposed)
            {
                await created.ReleaseAsync("dispose").ConfigureAwait(true);
                return;
            }

            observer = created;
            await NotifyChange(await observer.InvokeAsync<bool>("matches").ConfigureAwait(true)).ConfigureAwait(true);
        }

        if ((observer is null) || disposed || (Query == observedQuery))
        {
            return;
        }

        observedQuery = Query;
        await NotifyChange(await observer.InvokeAsync<bool>("update", observedQuery).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [JSInvokable]
    public Task NotifyChange(bool matches) =>
        (disposed || (matches == Matches)) ? Task.CompletedTask : MatchesChanged.InvokeAsync(matches);

    public async ValueTask DisposeAsync()
    {
        disposed = true;

        if (observer is not null)
        {
            await observer.ReleaseAsync("dispose").ConfigureAwait(true);
            observer = null;
        }

        reference?.Dispose();

        if (module is not null)
        {
            await module.DisposeAsync().ConfigureAwait(true);
        }
    }
}
