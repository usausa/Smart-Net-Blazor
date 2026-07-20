namespace Smart.AspNetCore.Components;

public sealed class HasValueTest : BunitContext
{
    [Fact]
    public void RendersChildContent_WhenValuePresent()
    {
        var cut = Render<HasValue<string?>>(parameters => parameters
            .Add(p => p.Value, "hello")
            .Add(p => p.ChildContent, v => $"<span>{v}</span>"));

        cut.MarkupMatches("<span>hello</span>");
    }

    [Fact]
    public void RendersNullContent_WhenValueNull()
    {
        var cut = Render<HasValue<string?>>(parameters => parameters
            .Add(p => p.Value, null)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.NullContent, "<span>null</span>"));

        cut.MarkupMatches("<span>null</span>");
    }

    [Fact]
    public void RendersNothing_WhenValueNullAndNoNullContent()
    {
        var cut = Render<HasValue<string?>>(parameters => parameters
            .Add(p => p.Value, null)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>"));

        Assert.Empty(cut.Markup);
    }
}
