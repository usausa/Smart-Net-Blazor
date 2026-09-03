namespace Smart.AspNetCore.Components;

public sealed class ListItemTests : BunitContext
{
    private static readonly string[] Items = ["a", "b"];

    [Fact]
    public void RendersNullContentWhenItemsNull()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, null)
            .Add(p => p.NullContent, "<span>null</span>"));

        cut.MarkupMatches("<span>null</span>");
    }

    [Fact]
    public void RendersEmptyContentWhenItemsEmpty()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, Array.Empty<string>())
            .Add(p => p.EmptyContent, "<span>empty</span>"));

        cut.MarkupMatches("<span>empty</span>");
    }

    [Fact]
    public void RendersItemTemplatePerItem()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, Items)
            .Add(p => p.ItemTemplate, item => $"<li>{item}</li>"));

        cut.MarkupMatches("<li>a</li><li>b</li>");
    }

    [Fact]
    public void RendersListContentWhenNoItemTemplate()
    {
        var cut = Render<ListItem<string>>(parameters => parameters
            .Add(p => p.Items, Items)
            .Add(p => p.ListContent, items => $"<div>{items.Count}</div>"));

        cut.MarkupMatches("<div>2</div>");
    }
}
