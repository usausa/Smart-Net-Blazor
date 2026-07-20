namespace Smart.AspNetCore.Components;

public sealed class ConditionTest : BunitContext
{
    [Fact]
    public void RendersChildContent_WhenValueIsTrue()
    {
        var cut = Render<Condition>(parameters => parameters
            .Add(static p => p.Value, true)
            .Add(static p => p.ChildContent, "<span>shown</span>"));

        cut.MarkupMatches("<span>shown</span>");
    }

    [Fact]
    public void RendersElseContent_WhenValueIsFalse()
    {
        var cut = Render<Condition>(parameters => parameters
            .Add(static p => p.Value, false)
            .Add(static p => p.ChildContent, "<span>shown</span>")
            .Add(static p => p.ElseContent, "<span>else</span>"));

        cut.MarkupMatches("<span>else</span>");
    }

    [Fact]
    public void RendersNothing_WhenValueIsFalseAndNoElseContent()
    {
        var cut = Render<Condition>(parameters => parameters
            .Add(static p => p.Value, false)
            .Add(static p => p.ChildContent, "<span>shown</span>"));

        Assert.Empty(cut.Markup);
    }
}
