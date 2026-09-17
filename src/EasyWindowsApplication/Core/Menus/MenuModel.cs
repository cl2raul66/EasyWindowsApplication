namespace EasyWindowsApplication.Core.Menus;

internal sealed class MenuItemModel
{
    internal int Id;
    internal string Text = "";
    internal bool IsEnabled = true;
    internal bool IsChecked;
    internal bool IsSeparator;
    internal List<MenuItemModel> Children = new();
    internal Action? OnClick;
}

internal sealed class MenuModel
{
    internal List<MenuItemModel> Items = new();

    internal MenuItemModel? FindById(int id)
    {
        foreach (var item in Items)
        {
            var found = FindRecursive(item, id);
            if (found is not null) return found;
        }
        return null;
    }

    private static MenuItemModel? FindRecursive(MenuItemModel item, int id)
    {
        if (item.Id == id) return item;
        foreach (var child in item.Children)
        {
            var found = FindRecursive(child, id);
            if (found is not null) return found;
        }
        return null;
    }
}
