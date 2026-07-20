namespace Smart.AspNetCore.Components;

public sealed class ListItemTest : BunitContext
{
    private static readonly string[] Items = ["a", "b"];

    [Fact]
    public void RendersNullContent_WhenItemsNull()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, null)
            .Add(p => p.NullContent, "<span>null</span>"));

        cut.MarkupMatches("<span>null</span>");
    }

    [Fact]
    public void RendersEmptyContent_WhenItemsEmpty()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, Array.Empty<string>())
            .Add(p => p.EmptyContent, "<span>empty</span>"));

        cut.MarkupMatches("<span>empty</span>");
    }

    [Fact]
    public void RendersItemTemplate_PerItem()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, Items)
            .Add(p => p.ItemTemplate, item => $"<li>{item}</li>"));

        cut.MarkupMatches("<li>a</li><li>b</li>");
    }

    [Fact]
    public void RendersListContent_WhenNoItemTemplate()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, Items)
            .Add(p => p.ListContent, items => $"<div>{items.Count}</div>"));

        cut.MarkupMatches("<div>2</div>");
    }
}
