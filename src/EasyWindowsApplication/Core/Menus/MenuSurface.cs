using System.Diagnostics;
using System.Runtime.InteropServices;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Menus;

internal sealed class MenuSurface : IMenu
{
    private readonly ContentModel? _content;
    private nint _ownerHwnd;
    private Func<(int X, int Y)>? _iconAnchor;
    private MenuModel? _cachedModel;

    internal MenuSurface(ContentModel? content) => _content = content;

    internal HandleRegistry? Registry { get; set; }

    internal void SetOwner(nint hwnd) => _ownerHwnd = hwnd;

    internal void SetIconAnchor(nint trayHwnd, uint trayId)
    {
        _iconAnchor = () =>
        {
            var identifier = new NOTIFYICONIDENTIFIER
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONIDENTIFIER>(),
                hWnd = trayHwnd,
                uID = trayId,
                guidItem = Guid.Empty
            };
            // S_OK (0) = rect válido: el menú se ancla al icono (foco), nunca al mouse.
            // Sin rect (icono no localizable) el cursor es fallback documentado.
            if (Win32.Shell_NotifyIconGetRect(ref identifier, out var rect) == 0)
                return (rect.Right, rect.Top);

            Win32.GetCursorPos(out var pt);
            return (pt.X, pt.Y);
        };
    }

    public string Name { get; set; } = "";
    public LayoutLength? LayoutWidth { get; set; }
    public LayoutLength? LayoutHeight { get; set; }
    public LayoutOptions LayoutOptions { get; set; } = new();
    public Thickness Margin { get; set; }
    public Thickness Padding { get; set; }
    public DockPosition Dock { get; set; }
    public int GridRow { get; set; }
    public int GridColumn { get; set; }
    public int GridRowSpan { get; set; }
    public int GridColumnSpan { get; set; }
    public Color? BackgroundColor { get; set; }

    public event Action? Clicked;

    public void OnClick(Action handler) => Clicked += handler;

    public void Show()
    {
        if (_iconAnchor is not null)
        {
            var (x, y) = _iconAnchor();
            ShowAt(x, y, TPM.RIGHTALIGN | TPM.BOTTOMALIGN);
        }
        else
        {
            ShowAtCursor();
        }
    }

    public void ShowAtCursor()
    {
        Win32.GetCursorPos(out var pt);
        ShowAt(pt.X, pt.Y, TPM.LEFTALIGN | TPM.TOPALIGN);
    }

    public void ShowAtIcon()
    {
        if (_iconAnchor is not null)
        {
            var (x, y) = _iconAnchor();
            ShowAt(x, y, TPM.RIGHTALIGN | TPM.BOTTOMALIGN);
        }
        else
        {
            ShowAtCursor();
        }
    }

    public void Hide() { }

    public void Close() { }

    internal MenuModel BuildModel()
    {
        var model = new MenuModel();
        if (_content is null) return model;
        foreach (var child in _content.Children)
        {
            if (child is not ViewModel vm) continue;
            var surface = CreateSurface(vm);
            model.Items.Add(ToItemModel(surface, vm.SubContent));
        }
        return model;
    }

    internal MenuModel EnsureMaterialized()
    {
        _cachedModel ??= BuildModel();
        return _cachedModel;
    }

    private static IViewSurface CreateSurface(ViewModel vm)
    {
        if (vm.Control is IViewSurface existing) return existing;
        IViewSurface surface = vm.ControlType is not null
            && typeof(IMenuItemSeparator).IsAssignableFrom(vm.ControlType)
            ? new MenuSeparatorItem()
            : new MenuItem();
        if (!string.IsNullOrEmpty(vm.Name))
            surface.Name = vm.Name;
        vm.Configure?.Invoke(surface);
        return surface;
    }

    private MenuItemModel ToItemModel(IViewSurface surface, ContentModel? sub)
    {
        if (Registry is not null && !string.IsNullOrEmpty(surface.Name))
            Registry.RegisterSurface(surface.Name, surface);
        if (surface is IMenuItemSeparator)
        {
            return new MenuItemModel
            {
                Id = Win32MenuEngine.NextId(),
                IsSeparator = true
            };
        }
        var item = (IMenuItem)surface;
        var model = new MenuItemModel
        {
            Id = Win32MenuEngine.NextId(),
            Text = item.Text,
            IsEnabled = item.IsEnabled,
            IsChecked = item.IsChecked,
            OnClick = () =>
            {
                if (item is MenuItem concrete) concrete.RaiseClicked();
            }
        };
        if (sub is not null)
        {
            foreach (var child in sub.Children)
            {
                if (child is not ViewModel vm) continue;
                var subSurface = CreateSurface(vm);
                model.Children.Add(ToItemModel(subSurface, vm.SubContent));
            }
        }
        return model;
    }

    private void ShowAt(int x, int y, uint align)
    {
        var model = EnsureMaterialized();
        int id = Win32MenuEngine.Show(_ownerHwnd, model, x, y, align);
        if (id > 0) Clicked?.Invoke();
    }
}
