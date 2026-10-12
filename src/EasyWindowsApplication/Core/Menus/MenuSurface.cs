using System.Diagnostics;
using System.Runtime.InteropServices;
using EasyWindowsApplication.Core.Surfaces;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;

namespace EasyWindowsApplication.Core.Menus;

internal sealed class MenuSurface : IMenu
{
    private readonly ContentModel? _content;
    private nint _ownerHwnd;
    private Func<(int X, int Y)>? _iconAnchor;
    private MenuModel? _cachedModel;

    internal MenuSurface(ContentModel? content) => _content = content;

    internal HandleRegistry? Registry { get; set; }

    // Dueño de respaldo resuelto en Show(): diálogos usan el mismo patrón
    // (DialogSurfaceBase.OwnerProvider) para no depender del orden de
    // declaración en Layout. El tray lo sobrescribe vía SetOwner().
    internal Func<nint> OwnerProvider { get; set; } = static () => 0;

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

    public void Show()
    {
        // Inteligente (issue #53, sin cambiar IMenu): control disparador en
        // curso → icono del tray → cursor. Las explícitas ShowAtCursor() /
        // ShowAtIcon() no leen el contexto por contrato.
        if (InputTriggerContext.TryGetFresh(out var triggerHwnd)
            && TryAnchorToControl(triggerHwnd, out var pos, out var align, out var owner))
        {
            ShowAt(pos.X, pos.Y, align, owner);
            return;
        }
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
        Registry?.Attach(EntityKind.Surface, surface, hwnd: 0, surface.Name);
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
            OnSelected = () => ((IInputFeed)item).FeedTrigger(typeof(MainTap), 1)
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

    private void ShowAt(int x, int y, uint align) => ShowAt(x, y, align, ResolveOwner());

    private void ShowAt(int x, int y, uint align, nint owner)
    {
        var model = EnsureMaterialized();
        Win32MenuEngine.Show(owner, model, x, y, align);
    }

    /// <summary>
    /// Ancla al rect del control disparador con flip según el área de trabajo
    /// del monitor (igual que el ancla del icono del tray). El dueño es el
    /// propio control para ese Show() puntual. Falla si el HWND ya no es válido.
    /// </summary>
    private static bool TryAnchorToControl(nint hwnd, out (int X, int Y) pos, out uint align, out nint owner)
    {
        pos = default;
        align = TPM.LEFTALIGN | TPM.TOPALIGN;
        owner = hwnd;
        if (!Win32.GetWindowRect(hwnd, out RECT rc)) return false;
        nint mon = Win32.MonitorFromWindow(hwnd, MONITOR.DEFAULTTONEAREST);
        if (mon == 0) mon = Win32.MonitorFromWindow(hwnd, MONITOR.DEFAULTTOPRIMARY);
        if (mon == 0)
        {
            pos = (rc.Left, rc.Bottom);
            return true;
        }
        MONITORINFO mi = new() { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
        if (!Win32.GetMonitorInfoW(mon, ref mi)) return false;
        var work = mi.rcWork;
        bool below = work.Bottom - rc.Bottom >= rc.Top - work.Top;
        bool toRight = work.Right - rc.Left >= rc.Right - work.Left;
        pos = below ? (rc.Left, rc.Bottom) : (rc.Left, rc.Top);
        align = (toRight ? TPM.LEFTALIGN : TPM.RIGHTALIGN)
            | (below ? TPM.TOPALIGN : TPM.BOTTOMALIGN);
        return true;
    }

    private nint ResolveOwner() => _ownerHwnd != 0 ? _ownerHwnd : OwnerProvider();
}
