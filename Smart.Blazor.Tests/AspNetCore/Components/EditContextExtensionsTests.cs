namespace Smart.AspNetCore.Components;

using Microsoft.AspNetCore.Components.Forms;

public sealed class EditContextExtensionsTests
{
    [Fact]
    public void NotifyFieldChangedByNameRaisesOnFieldChanged()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var changed = new List<string>();
        editContext.OnFieldChanged += (_, e) => changed.Add(e.FieldIdentifier.FieldName);

        editContext.NotifyFieldChanged(nameof(TestModel.Name));

        string[] expected = [nameof(TestModel.Name)];
        Assert.Equal(expected, changed);
        Assert.True(editContext.IsModified(editContext.Field(nameof(TestModel.Name))));
    }

    [Fact]
    public void NotifyFieldsChangedRaisesOnFieldChangedPerField()
    {
        var model = new TestModel();
        var editContext = new EditContext(model);
        var changed = new List<string>();
        editContext.OnFieldChanged += (_, e) => changed.Add(e.FieldIdentifier.FieldName);

        editContext.NotifyFieldsChanged(nameof(TestModel.Name), nameof(TestModel.Email));

        string[] expected = [nameof(TestModel.Name), nameof(TestModel.Email)];
        Assert.Equal(expected, changed);
        Assert.Same(model, editContext.Field(nameof(TestModel.Email)).Model);
    }

    [Fact]
    public void NotifyFieldsChangedWithoutFieldsDoesNothing()
    {
        var editContext = new EditContext(new TestModel());
        var count = 0;
        editContext.OnFieldChanged += (_, _) => count++;

        editContext.NotifyFieldsChanged();

        Assert.Equal(0, count);
    }

    private sealed class TestModel
    {
        public string? Name { get; set; }

        public string? Email { get; set; }
    }
}
