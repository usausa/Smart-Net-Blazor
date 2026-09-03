namespace Smart.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

public sealed class ExtendedAuthorizeRouteViewTests : BunitContext
{
    [Fact]
    public void NotAuthorizedFallsBackToDefaultLayoutWhenNotAuthorizedLayoutNotSet()
    {
        AddAuthorization().SetNotAuthorized();

        var cut = Render<ExtendedAuthorizeRouteView>(parameters => parameters
            .Add(p => p.RouteData, new RouteData(typeof(SecurePage), new Dictionary<string, object?>()))
            .Add(p => p.DefaultLayout, typeof(DefaultTestLayout)));

        Assert.Contains("default-layout", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Not authorized", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void NotAuthorizedUsesNotAuthorizedLayoutWhenSet()
    {
        AddAuthorization().SetNotAuthorized();

        var cut = Render<ExtendedAuthorizeRouteView>(parameters => parameters
            .Add(p => p.RouteData, new RouteData(typeof(SecurePage), new Dictionary<string, object?>()))
            .Add(p => p.DefaultLayout, typeof(DefaultTestLayout))
            .Add(p => p.NotAuthorizedLayout, typeof(NotAuthorizedTestLayout)));

        Assert.Contains("not-authorized-layout", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("default-layout", cut.Markup, StringComparison.Ordinal);
    }

    [Authorize]
    public sealed class SecurePage : ComponentBase;

    public sealed class DefaultTestLayout : LayoutComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "default-layout");
            builder.AddContent(2, Body);
            builder.CloseElement();
        }
    }

    public sealed class NotAuthorizedTestLayout : LayoutComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "not-authorized-layout");
            builder.AddContent(2, Body);
            builder.CloseElement();
        }
    }
}
