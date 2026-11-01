namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public sealed class HotKey : ComponentBase, IAsyncDisposable
{
    private const string ModulePath = "./_content/Usa.Smart.Blazor/hot-key.js";

    private ScriptModule? module;

    private DotNetObjectReference<HotKey>? reference;

    private IJSObjectReference? registration;

    private Options current;

    private Options registered;

    private bool disposed;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public string Key { get; set; } = default!;

    [Parameter]
    public EventCallback OnPressed { get; set; }

    [Parameter]
    public bool PreventDefault { get; set; } = true;

    [Parameter]
    public bool AllowInInput { get; set; }

    [Parameter]
    public bool AllowRepeat { get; set; }

    protected override void OnParametersSet()
    {
        current = Options.Create(Key, PreventDefault, AllowInInput, AllowRepeat);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = new ScriptModule(JSRuntime, ModulePath);
            reference = DotNetObjectReference.Create(this);
            registered = current;

            IJSObjectReference created;
            try
            {
                created = await module.InvokeAsync<IJSObjectReference>("register", [reference, .. registered.ToArguments()]).ConfigureAwait(true);
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

            registration = created;
        }

        if ((registration is null) || disposed || (current == registered))
        {
            return;
        }

        registered = current;
        await registration.InvokeVoidAsync("update", registered.ToArguments()).ConfigureAwait(true);
    }

    [JSInvokable]
    public Task NotifyPressed() =>
        disposed ? Task.CompletedTask : OnPressed.InvokeAsync();

    public async ValueTask DisposeAsync()
    {
        disposed = true;

        if (registration is not null)
        {
            await registration.ReleaseAsync("dispose").ConfigureAwait(true);
            registration = null;
        }

        reference?.Dispose();

        if (module is not null)
        {
            await module.DisposeAsync().ConfigureAwait(true);
        }
    }

    private readonly record struct Options(
        string Key,
        bool Ctrl,
        bool Alt,
        bool Shift,
        bool Meta,
        bool PreventDefault,
        bool AllowInInput,
        bool AllowRepeat)
    {
        public static Options Create(string value, bool preventDefault, bool allowInInput, bool allowRepeat)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var parts = value.Split('+', StringSplitOptions.TrimEntries);
            var ctrl = false;
            var alt = false;
            var shift = false;
            var meta = false;
            for (var i = 0; i < parts.Length - 1; i++)
            {
                switch (parts[i].ToUpperInvariant())
                {
                    case "CTRL":
                        ctrl = true;
                        break;
                    case "ALT":
                        alt = true;
                        break;
                    case "SHIFT":
                        shift = true;
                        break;
                    case "META":
                        meta = true;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported key combination. value=[{value}]", nameof(value));
                }
            }

            var key = parts[^1];
            if (key.Length == 0)
            {
                throw new ArgumentException($"Unsupported key combination. value=[{value}]", nameof(value));
            }

            key = String.Equals(key, "Space", StringComparison.OrdinalIgnoreCase) ? " " : key.ToUpperInvariant();
            return new Options(key, ctrl, alt, shift, meta, preventDefault, allowInInput, allowRepeat);
        }

        public object[] ToArguments() =>
            [Key, Ctrl, Alt, Shift, Meta, PreventDefault, AllowInInput, AllowRepeat];
    }
}
