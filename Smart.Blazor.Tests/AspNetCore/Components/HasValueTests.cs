namespace Smart.AspNetCore.Components;

public sealed class HasValueTests : BunitContext
{
    [Fact]
    public void RendersChildContentWhenValuePresent()
    {
        var cut = Render<HasValue<string?>>(parameters => parameters
            .Add(p => p.Value, "hello")
            .Add(p => p.ChildContent, v => $"<span>{v}</span>"));

        cut.MarkupMatches("<span>hello</span>");
    }

    [Fact]
    public void RendersNullContentWhenValueNull()
    {
        var cut = Render<HasValue<string?>>(parameters => parameters
            .Add(p => p.Value, null)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.NullContent, "<span>null</span>"));

        cut.MarkupMatches("<span>null</span>");
    }

    [Fact]
    public void RendersNothingWhenValueNullAndNoNullContent()
    {
        var cut = Render<HasValue<string?>>(parameters => parameters
            .Add(p => p.Value, null)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>"));

        Assert.Empty(cut.Markup);
    }
}
