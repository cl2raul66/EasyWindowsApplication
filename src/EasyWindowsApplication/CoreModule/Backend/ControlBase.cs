using System.Runtime.InteropServices;
using EasyWindowsApplication.LayoutModule.Backend;
using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;
using static EasyWindowsApplication.CoreModule.Backend.Win32;

namespace EasyWindowsApplication.CoreModule.Backend;

public abstract class ControlBase : IControl, IClickEventSource, ILayoutable, IDockable
{
    public nint Hwnd { get; internal set; }
    public string Name { get; set; } = "";

    private float _x, _y, _w, _h;

    internal event Action? InternalClick;

    void IClickEventSource.RaiseClickInternal() => InternalClick?.Invoke();
    void IClickEventSource.AddClickHandler(Action handler) => InternalClick += handler;

    public event Action? Clicked
    {
        add => InternalClick += value;
        remove => InternalClick -= value;
    }

    public void OnClick(Action handler)
    {
        InternalClick += handler;
    }

    public nint OnMessage(uint msg, Win32MessageHandler handler)
    {
        Router.RegisterHandler(Hwnd, msg, handler);
        return Hwnd;
    }

    public float X
    {
        set { _x = value; ApplyBounds(); }
    }
    public float Y
    {
        set { _y = value; ApplyBounds(); }
    }
    public float W
    {
        set { _w = value; ApplyBounds(); }
    }
    public float H
    {
        set { _h = value; ApplyBounds(); }
    }

    public void SetBounds(float x, float y, float w, float h)
    {
        _x = x; _y = y; _w = w; _h = h;
        ApplyBounds();
    }

    internal void SetPositionDirect(float x, float y)
    {
        _x = x; _y = y;
    }

    internal void SetDimensionsDirect(float w, float h)
    {
        _w = w; _h = h;
    }

    internal void ApplyBounds()
    {
        if (Hwnd != 0)
            MoveWindow(Hwnd, (int)_x, (int)_y, (int)_w, (int)_h, true);
    }

    public void Show()
    {
        if (Hwnd != 0)
            ShowWindow(Hwnd, 5);
    }

    public void Hide()
    {
        if (Hwnd != 0)
            ShowWindow(Hwnd, 0);
    }

    public void Close()
    {
        DestroyBackgroundBrush();
        if (Hwnd != 0)
            DestroyWindow(Hwnd);
    }

    internal MasterRouter Router { get; set; } = default!;
    internal HandleRegistry Registry { get; set; } = default!;

    public LayoutLength? LayoutWidth { get; set; }
    public LayoutLength? LayoutHeight { get; set; }
    public LayoutOptions LayoutOptions { get; set; } = new();
    public Thickness Margin { get; set; }
    public Thickness Padding { get; set; }
    public DockPosition Dock { get; set; } = DockPosition.Left;

    public int GridRow { get; set; }
    public int GridColumn { get; set; }
    public int GridRowSpan { get; set; } = 1;
    public int GridColumnSpan { get; set; } = 1;

    public Color? BackgroundColor { get; set; }

    private nint? _backgroundBrush;

    internal nint GetOrCreateBackgroundBrush()
    {
        if (!BackgroundColor.HasValue)
            return 0;
        if (BackgroundColor.Value.IsTransparent)
            return 0;
        if (_backgroundBrush.HasValue)
            return _backgroundBrush.Value;
        _backgroundBrush = CreateSolidBrush(BackgroundColor.Value.ToCOLORREF());
        return _backgroundBrush.Value;
    }

    internal void DestroyBackgroundBrush()
    {
        if (_backgroundBrush.HasValue)
        {
            DeleteObject(_backgroundBrush.Value);
            _backgroundBrush = null;
        }
    }

    float ILayoutable.MeasuredWidth => _measuredW;
    float ILayoutable.MeasuredHeight => _measuredH;

    private float _measuredW, _measuredH;
    private float _arrangedX, _arrangedY, _arrangedW, _arrangedH;

    void ILayoutable.Measure(float availableWidth, float availableHeight)
    {
        var (contentW, contentH) = MeasureContent(availableWidth, availableHeight);

        if (LayoutWidth.HasValue && LayoutWidth.Value.Type == GridUnitType.Absolute)
            _measuredW = Math.Max((int)LayoutWidth.Value.Value, 0);
        else
            _measuredW = Math.Max(contentW + Padding.Left + Padding.Right, 0);

        if (LayoutHeight.HasValue && LayoutHeight.Value.Type == GridUnitType.Absolute)
            _measuredH = Math.Max((int)LayoutHeight.Value.Value, 0);
        else
            _measuredH = Math.Max(contentH + Padding.Top + Padding.Bottom, 0);
    }

    void ILayoutable.Arrange(float x, float y, float width, float height)
    {
        _arrangedX = x;
        _arrangedY = y;
        _arrangedW = Math.Max(width, 0);
        _arrangedH = Math.Max(height, 0);
    }

    void ILayoutable.Render()
    {
        if (Hwnd != 0)
        {
            PreRender();
            MoveWindow(Hwnd, (int)_arrangedX, (int)_arrangedY, (int)_arrangedW, (int)_arrangedH, true);
            PostRender();
        }
    }

    protected virtual void PreRender() { }
    protected virtual void PostRender() { }

    protected virtual (float Width, float Height) MeasureContent(float availableWidth, float availableHeight)
    {
        string text = GetWindowText();
        if (string.IsNullOrEmpty(text)) return (20, 24);
        return MeasureTextByGlyphs(text);
    }

    protected string GetWindowText()
    {
        int len = GetWindowTextLengthW(Hwnd);
        if (len == 0) return "";
        nint buffer = Marshal.AllocHGlobal((len + 1) * 2);
        try
        {
            GetWindowTextW(Hwnd, buffer, len + 1);
            return Marshal.PtrToStringUni(buffer) ?? "";
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    protected (int Width, int Height) MeasureText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return (20, 24);

        nint hdc = GetDC(Hwnd);
        nint hfont = SendMessageW(Hwnd, WM.GETFONT, 0, 0);
        nint oldFont = SelectObject(hdc, hfont != 0 ? hfont : GetStockObject(17));
        RECT rect = default;
        DrawTextW(hdc, text, -1, ref rect, DT.CALCRECT | DT.SINGLELINE);
        SelectObject(hdc, oldFont);
        _ = ReleaseDC(Hwnd, hdc);
        return (rect.Right - rect.Left, rect.Bottom - rect.Top);
    }

    protected (int Width, int Height) MeasureTextV2(string text)
    {
        if (string.IsNullOrEmpty(text))
            return (20, 24);

        nint hdc = GetDC(Hwnd);
        nint hfont = SendMessageW(Hwnd, WM.GETFONT, 0, 0);
        nint oldFont = SelectObject(hdc, hfont != 0 ? hfont : GetStockObject(17));

        GetTextExtentPoint32W(hdc, text, text.Length, out SIZE size);

        SelectObject(hdc, oldFont);
        _ = ReleaseDC(Hwnd, hdc);
        return (size.cx, size.cy);
    }

    protected (int Width, int Height) MeasureTextByGlyphs(string text)
    {
        if (string.IsNullOrEmpty(text))
            return (20, 24);

        nint hdc = GetDC(Hwnd);
        nint hfont = SendMessageW(Hwnd, WM.GETFONT, 0, 0);
        nint oldFont = SelectObject(hdc, hfont != 0 ? hfont : GetStockObject(17));

        int totalWidth = 0;
        int maxHeight = 0;
        int i = 0;

        while (i < text.Length)
        {
            int cp = char.ConvertToUtf32(text, i);
            int charLen = cp > 0xFFFF ? 2 : 1;

            if (cp > 0xFFFF)
            {
                totalWidth += MeasureSurrogatePairFallback(hdc, text, i);
            }
            else if (GetCharABCWidthsW(hdc, (uint)cp, (uint)cp, out ABC abc))
            {
                totalWidth += abc.abcA + (int)abc.abcB + abc.abcC;
            }
            else if (GetCharWidth32W(hdc, (uint)cp, (uint)cp, out int singleWidth))
            {
                totalWidth += singleWidth;
            }
            else
            {
                totalWidth += 8;
            }

            if (maxHeight == 0)
            {
                GetTextExtentPoint32W(hdc, text[i].ToString(), 1, out SIZE chSize);
                maxHeight = chSize.cy;
            }

            i += charLen;
        }

        if (maxHeight == 0)
            maxHeight = 24;

        SelectObject(hdc, oldFont);
        _ = ReleaseDC(Hwnd, hdc);
        return (totalWidth, maxHeight);
    }

    private static int MeasureSurrogatePairFallback(nint hdc, string text, int index)
    {
        if (index + 1 < text.Length)
        {
            string pair = text.Substring(index, 2);
            GetTextExtentPoint32W(hdc, pair, 2, out SIZE size);
            return size.cx;
        }
        return 8;
    }
}
