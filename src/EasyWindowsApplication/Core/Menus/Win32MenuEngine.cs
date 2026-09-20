using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Core.Menus;

internal static class Win32MenuEngine
{
    private static int _nextId;

    internal static int NextId() => Interlocked.Increment(ref _nextId);

    internal static nint BuildMenu(MenuModel model)
    {
        nint hMenu = Win32.CreatePopupMenu();
        if (hMenu == 0)
            throw new InvalidOperationException("CreatePopupMenu failed.");
        try
        {
            AppendItems(hMenu, model.Items);
        }
        catch
        {
            Win32.DestroyMenu(hMenu);
            throw;
        }
        return hMenu;
    }

    internal static bool Dispatch(MenuModel model, int id)
    {
        if (id <= 0) return false;
        var item = model.FindById(id);
        if (item is null || !item.IsEnabled || item.IsSeparator) return false;
        item.OnClick?.Invoke();
        return true;
    }

    internal static int Show(nint hwndOwner, MenuModel model, int x, int y, uint align = TPM.LEFTALIGN | TPM.TOPALIGN)
    {
        nint hMenu = BuildMenu(model);
        try
        {
            Win32.SetForegroundWindow(hwndOwner);
            int id = Win32.TrackPopupMenuEx(hMenu, align | TPM.RETURNCMD, x, y, hwndOwner, 0);
            Win32.PostMessageW(hwndOwner, WM.NULL, 0, 0);
            if (id > 0) Dispatch(model, id);
            return id;
        }
        finally
        {            
            Win32.DestroyMenu(hMenu);
        }
    }

    private static void AppendItems(nint hMenu, List<MenuItemModel> items)
    {
        foreach (var item in items)
        {
            if (item.IsSeparator)
            {
                Win32.AppendMenuW(hMenu, MF.SEPARATOR, 0, null);
                continue;
            }
            if (item.Children.Count > 0)
            {
                nint hSub = Win32.CreatePopupMenu();
                if (hSub == 0)
                    throw new InvalidOperationException("CreatePopupMenu failed (submenu).");
                try
                {
                    AppendItems(hSub, item.Children);
                }
                catch
                {
                    Win32.DestroyMenu(hSub);
                    throw;
                }
                uint flags = MF.POPUP;
                if (!item.IsEnabled) flags |= MF.GRAYED;
                if (item.IsChecked) flags |= MF.CHECKED;
                Win32.AppendMenuW(hMenu, flags, (nuint)hSub, item.Text);
            }
            else
            {
                uint flags = MF.STRING;
                if (!item.IsEnabled) flags |= MF.GRAYED;
                if (item.IsChecked) flags |= MF.CHECKED;
                Win32.AppendMenuW(hMenu, flags, (nuint)item.Id, item.Text);
            }
        }
    }
}
