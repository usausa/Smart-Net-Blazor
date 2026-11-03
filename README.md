# Smart.Blazor .NET - Blazor support library

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.Blazor.svg)](https://www.nuget.org/packages/Usa.Smart.Blazor)

## Features

* `ExtendedAuthorizeRouteView` — extended authorize route view for Blazor routing
* `CustomValidator` — custom validation component for Blazor forms
* `EditContextExtensions` — notify field changes by field name
* `LoadingContainer` — render content by loading status
* `ModifiedNavigationLock` — confirm leaving a form with changes

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

### ModifiedNavigationLock

```razor
<EditForm EditContext="editContext" OnValidSubmit="SaveAsync">
    <ModifiedNavigationLock OnConfirm="ConfirmLeaveAsync" />
</EditForm>
```

While the `EditContext` of the form is modified, navigating to another page of the app calls `OnConfirm`, and the navigation is canceled when it returns `false`. Without `OnConfirm`, the navigation goes on. `ConfirmExternalNavigation` (`true` by default) asks the browser to confirm closing the tab, reloading or going to another site while the form is modified. Call `editContext.MarkAsUnmodified()` after saving.
