namespace Smart.AspNetCore.Components;

public sealed class ConditionTest : BunitContext
{
    [Fact]
    public void RendersChildContentWhenValueIsTrue()
    {
        var cut = Render<Condition>(parameters => parameters
            .Add(static p => p.Value, true)
            .Add(static p => p.ChildContent, "<span>shown</span>"));

        cut.MarkupMatches("<span>shown</span>");
    }

    [Fact]
    public void RendersElseContentWhenValueIsFalse()
    {
        var cut = Render<Condition>(parameters => parameters
            .Add(static p => p.Value, false)
            .Add(static p => p.ChildContent, "<span>shown</span>")
            .Add(static p => p.ElseContent, "<span>else</span>"));

        cut.MarkupMatches("<span>else</span>");
    }

    [Fact]
    public void RendersNothingWhenValueIsFalseAndNoElseContent()
    {
        var cut = Render<Condition>(parameters => parameters
            .Add(static p => p.Value, false)
            .Add(static p => p.ChildContent, "<span>shown</span>"));

        Assert.Empty(cut.Markup);
    }
}
