namespace Smart.AspNetCore.Components;

public sealed class RangeTest : BunitContext
{
    [Fact]
    public void RendersSequence_FromZero()
    {
        var cut = Render<Range>(parameters => parameters
            .Add(p => p.Count, 3)
            .Add(p => p.ChildContent, i => $"<span>{i}</span>"));

        cut.MarkupMatches("<span>0</span><span>1</span><span>2</span>");
    }

    [Fact]
    public void RendersSequence_WithStartAndStep()
    {
        var cut = Render<Range>(parameters => parameters
            .Add(p => p.Start, 10)
            .Add(p => p.Step, 5)
            .Add(p => p.Count, 3)
            .Add(p => p.ChildContent, i => $"<span>{i}</span>"));

        cut.MarkupMatches("<span>10</span><span>15</span><span>20</span>");
    }

    [Fact]
    public void RendersNothing_WhenCountZero()
    {
        var cut = Render<Range>(parameters => parameters
            .Add(p => p.Count, 0)
            .Add(p => p.ChildContent, i => $"<span>{i}</span>"));

        Assert.Empty(cut.Markup);
    }
}
