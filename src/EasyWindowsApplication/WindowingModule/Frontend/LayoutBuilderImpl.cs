using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Backend;
using Button = EasyWindowsApplication.Win32ControlsModule.Frontend.Button;
using Edit = EasyWindowsApplication.Win32ControlsModule.Frontend.Edit;
using Progress = EasyWindowsApplication.Win32ControlsModule.Frontend.Progress;
using ListView = EasyWindowsApplication.Win32ControlsModule.Frontend.ListView;

namespace EasyWindowsApplication.WindowingModule.Frontend;

internal sealed class LayoutBuilderImpl :
    ILayoutBuilder,
    IWindowConfig,
    IContentBuilder,
    IChildrenBuilder
{
    // ── State collected during fluent chain ──
    internal string? WindowName { get; private set; }
    internal string? WindowTitle { get; private set; }
    internal int WindowWidth { get; private set; } = 800;
    internal int WindowHeight { get; private set; } = 600;
    internal WindowPosition WindowPositionMode { get; private set; } = WindowPosition.Center;
    internal Action<IContentBuilder>? PendingContentAction { get; private set; }

    internal List<ControlConfig> Controls { get; } = new();

    // Set externally before content creation
    internal nint ParentHwnd { get; set; }
    internal MasterRouter? Router { get; set; }
    internal HandleRegistry? Registry { get; set; }

    // ── ILayoutBuilder ──
    public ILayoutBuilder Window(Action<IWindowConfig> configure)
    {
        configure(this);

        var hInstance = Win32.GetModuleHandleW(0);
        ControlProcedures.SetInstance(hInstance);

        nint hwnd = Procedures.CreateMainWindow(
            Router!, WindowTitle ?? "Easy Win App", WindowWidth, WindowHeight);

        ParentHwnd = hwnd;

        if (WindowPositionMode == WindowPosition.Center)
            CenterWindow(hwnd, WindowWidth, WindowHeight);

        PendingContentAction?.Invoke(this);

        return this;
    }

    // ── IWindowConfig ──
    public IWindowConfig Name(string name)
    {
        WindowName = name;
        return this;
    }

    public IWindowConfig Title(string title)
    {
        WindowTitle = title;
        return this;
    }

    public IWindowConfig Dimensions(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;
        return this;
    }

    public IWindowConfig Position(WindowPosition position)
    {
        WindowPositionMode = position;
        return this;
    }

    public IWindowConfig Content(Action<IContentBuilder> configure)
    {
        PendingContentAction = configure;
        return this;
    }

    public IWindowConfig Content<TLayout>(Action<IContentBuilder> configure) where TLayout : IStackLayout
    {
        PendingContentAction = configure;
        return this;
    }

    // ── IContentBuilder ──
    public IContentBuilder Spacing(int pixels) => this;

    public IContentBuilder Children(Action<IChildrenBuilder> configure)
    {
        configure(this);
        return this;
    }

    // ── IChildrenBuilder ──
    public IChildrenBuilder View<T>(Action<ControlBuilder<T>> configure) where T : ControlBase<T>, new()
    {
        var mapping = Win32ControlMap.Get<T>();
        nint hwnd = ControlProcedures.CreateControl(
            mapping.ClassName, ParentHwnd, mapping.DefaultStyle, 0, "", 0, 0, 0, 0, 0);

        var control = new T
        {
            Hwnd = hwnd,
            Router = Router!,
            Registry = Registry!
        };

        var builder = new ControlBuilder<T>(control);
        configure(builder);

        if (hwnd != 0 && !string.IsNullOrEmpty(control.Name))
            Registry!.Register(hwnd, control);

        Controls.Add(new ControlConfig(typeof(T), control.Name, hwnd));
        return this;
    }

    // ── Legacy shortcuts ──
    public Button Button(string name, string text, int x, int y, int w, int h)
    {
        var style = WS.CHILD | WS.VISIBLE | BS.PUSHBUTTON;
        return CreateControlLegacy<Button>(name, WC.BUTTON, text, style, 0, x, y, w, h, 0, null);
    }

    public Edit Edit(string name, string text, int x, int y, int w, int h)
    {
        var style = WS.CHILD | WS.VISIBLE | WS.TABSTOP | WS.BORDER | ES.AUTOHSCROLL;
        return CreateControlLegacy<Edit>(name, WC.EDIT, text, style, 0, x, y, w, h, 0, null);
    }

    public Progress Progress(string name, int x, int y, int w, int h)
    {
        var style = WS.CHILD | WS.VISIBLE;
        return CreateControlLegacy<Progress>(name, WC.PROGRESS, "", style, 0, x, y, w, h, 0, null);
    }

    public ListView ListView(string name, int x, int y, int w, int h)
    {
        var style = WS.CHILD | WS.VISIBLE | WS.BORDER | LVS.REPORT | LVS.SHOWSELALWAYS;
        var control = CreateControlLegacy<ListView>(name, WC.LISTVIEW, "", style, 0, x, y, w, h, 0, null);
        if (control.Hwnd != 0)
            control.EnableFullRowSelect();
        return control;
    }

    private T CreateControlLegacy<T>(string name, string windowClass, string text, uint style, uint exStyle,
        int x, int y, int w, int h, nint hMenu, Action<T>? configure) where T : ControlBase<T>, new()
    {
        nint hwnd = ControlProcedures.CreateControl(windowClass, ParentHwnd, style, exStyle, text, x, y, w, h, hMenu);
        var control = new T
        {
            Hwnd = hwnd,
            Name = name,
            Router = Router!,
            Registry = Registry!
        };

        if (hwnd != 0)
            Registry!.Register(hwnd, control);

        configure?.Invoke(control);

        Controls.Add(new ControlConfig(typeof(T), name, hwnd));
        return control;
    }

    private static void CenterWindow(nint hwnd, int width, int height)
    {
        int screenW = Win32.GetSystemMetrics(0);
        int screenH = Win32.GetSystemMetrics(1);
        int x = (screenW - width) / 2;
        int y = (screenH - height) / 2;
        Win32.MoveWindow(hwnd, x, y, width, height, true);
    }
}

internal record ControlConfig(Type Type, string Name, nint Hwnd);
