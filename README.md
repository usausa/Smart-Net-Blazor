# Smart.Blazor .NET - Blazor support library

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.Blazor.svg)](https://www.nuget.org/packages/Usa.Smart.Blazor)

## Features

* `ExtendedAuthorizeRouteView` — extended authorize route view for Blazor routing
* `CustomValidator` — custom validation component for Blazor forms
* `EditContextExtensions` — notify field changes by field name
* `LoadingContainer` — render content by loading status
* Script helper extensions for JavaScript interop
* `ScriptModule` — load a JavaScript module on the first call
* `MediaQuery` — bind the result of a CSS media query
* `HotKey` — call a method on a keyboard shortcut
* `NativeDialog` — open and close a native `<dialog>` by binding

## Usage

### EditContextExtensions

```csharp
editContext.NotifyFieldChanged(nameof(Model.Name));
editContext.NotifyFieldsChanged(nameof(Model.Name), nameof(Model.Email));
```

### LoadingContainer

```razor
<LoadingContainer Status="status" Value="items" Context="list">
    <LoadingContent><p>Loading...</p></LoadingContent>
    <FailedContent><p>Failed to load.</p></FailedContent>
    <ChildContent>
        <ListItem Items="list" Context="item">
            <ItemTemplate><li>@item.Name</li></ItemTemplate>
            <EmptyContent><p>No items.</p></EmptyContent>
        </ListItem>
    </ChildContent>
</LoadingContainer>
```

`Status` is one of `NotLoaded`, `Loading`, `Loaded` and `Failed`. `ChildContent` receives `Value` when `Loaded`.

### Script helper extensions

```csharp
await JSRuntime.SetFocus("name");
await JSRuntime.SaveAsFile("report.pdf", "application/pdf", bytes);
await JSRuntime.OpenNewWindow("application/pdf", bytes);
await JSRuntime.HistoryBack();
```

### ScriptModule

```csharp
module = new ScriptModule(JSRuntime, "./js/chart.js");
await module.InvokeVoidAsync("draw", element, values);
await module.DisposeAsync();
```

### MediaQuery

```razor
<MediaQuery Query="(max-width: 600px)" @bind-Matches="narrow" />
```

### HotKey

```razor
<HotKey Key="Ctrl+S" AllowInInput="true" OnPressed="SaveAsync" />
<HotKey Key="F2" OnPressed="Edit" />
```

`Key` is modifiers (`Ctrl`, `Alt`, `Shift`, `Meta`) and a `KeyboardEvent.key` name joined by `+`.

### NativeDialog

```razor
<NativeDialog @bind-Open="open" OnClose="OnClose" class="confirm">
    <form method="dialog">
        <p>Delete?</p>
        <button value="ok">OK</button>
        <button value="cancel">Cancel</button>
    </form>
</NativeDialog>
```

`OnClose` receives the `value` of the button that closed the dialog.
