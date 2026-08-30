using EasyWindowsApplication.Core;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Win32ControlsModule.Backend;

public sealed class Button : ControlBase, IButton
{
    private string _text = "";
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? "";
            if (Hwnd != 0) ControlProcedures.SetWindowText(Hwnd, _text);
        }
    }
    public bool Enabled { get; set; } = true;
    public void Click() => OnClick(() => { });
    public void SetStyle(uint style, bool redraw = true) { }

    protected override (float Width, float Height) MeasureContent(float availableWidth, float availableHeight)
    {
        string text = GetWindowText();
        var (tw, th) = string.IsNullOrEmpty(text) ? (20, 24) : MeasureTextByGlyphs(text);
        var defaults = UiDefaultsProvider.Current.GetFor<IButton>();
        if (defaults == null)
            defaults = UiDefaultsProvider.Current.GetFor<Button>();
        float preferredH = defaults?.PreferredHeight ?? 24f;
        // DPI scaling: defaults are at 96 DPI, scale to current DPI if possible
        try
        {
            uint dpi = 0;
            if (Hwnd != 0)
                dpi = Win32Controls.GetDpiForWindow(Hwnd);
            if (dpi == 0) dpi = Win32Controls.GetDpiForSystem();
            if (dpi != 0 && dpi != 96)
                preferredH = preferredH * dpi / 96f;
        }
        catch { }
        // Height is fixed by defaults, width by text
        return (tw, preferredH);
    }
}

public sealed class Label : ControlBase, ILabel
{
    private string _text = "";
    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? "";
            if (Hwnd != 0) ControlProcedures.SetWindowText(Hwnd, _text);
        }
    }
}
