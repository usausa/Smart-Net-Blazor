namespace Smart.AspNetCore.Components;

public sealed class LoadingContainerTests : BunitContext
{
    [Fact]
    public void RendersNotLoadedContentWhenNotLoaded()
    {
        var cut = Render<LoadingContainer<string>>(parameters => parameters
            .Add(p => p.Status, LoadingStatus.NotLoaded)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.NotLoadedContent, "<span>not-loaded</span>"));

        cut.MarkupMatches("<span>not-loaded</span>");
    }

    [Fact]
    public void RendersLoadingContentWhenLoading()
    {
        var cut = Render<LoadingContainer<string>>(parameters => parameters
            .Add(p => p.Status, LoadingStatus.Loading)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.LoadingContent, "<span>loading</span>"));

        cut.MarkupMatches("<span>loading</span>");
    }

    [Fact]
    public void RendersChildContentWithValueWhenLoaded()
    {
        var cut = Render<LoadingContainer<string>>(parameters => parameters
            .Add(p => p.Status, LoadingStatus.Loaded)
            .Add(p => p.Value, "hello")
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.LoadingContent, "<span>loading</span>"));

        cut.MarkupMatches("<span>hello</span>");
    }

    [Fact]
    public void RendersFailedContentWhenFailed()
    {
        var cut = Render<LoadingContainer<string>>(parameters => parameters
            .Add(p => p.Status, LoadingStatus.Failed)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.FailedContent, "<span>failed</span>"));

        cut.MarkupMatches("<span>failed</span>");
    }

    [Fact]
    public void RendersNothingWhenOptionalContentNotSet()
    {
        var cut = Render<LoadingContainer<string>>(parameters => parameters
            .Add(p => p.Status, LoadingStatus.Loading)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>"));

        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void FollowsStatusChange()
    {
        var cut = Render<LoadingContainer<string>>(parameters => parameters
            .Add(p => p.Status, LoadingStatus.Loading)
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.LoadingContent, "<span>loading</span>"));

        cut.Render(parameters => parameters
            .Add(p => p.Status, LoadingStatus.Loaded)
            .Add(p => p.Value, "done")
            .Add(p => p.ChildContent, v => $"<span>{v}</span>")
            .Add(p => p.LoadingContent, "<span>loading</span>"));

        cut.MarkupMatches("<span>done</span>");
    }
}
