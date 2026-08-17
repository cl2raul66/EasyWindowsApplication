using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.LayoutModule.Backend;
using EasyWindowsApplication.Share.Infrastructure;

namespace EasyWindowsApplication.WindowingModule.Backend;

internal sealed class UserControl : ControlBase
{
    internal List<ILayoutable> Children { get; } = [];
    internal float Spacing { get; set; }
    internal ILayoutStrategy? Strategy { get; set; }

    internal UserControl()
    {
        Strategy = new VerticalStackLayoutStrategy();
    }

    protected override (float Width, float Height) MeasureContent(float availableWidth, float availableHeight)
    {
        Strategy!.Measure(Children, availableWidth, availableHeight);

        float sTotalH = 0;
        float sMaxW = 0;
        foreach (var child in Children)
        {
            sTotalH += child.MeasuredHeight + child.Margin.Top + child.Margin.Bottom;
            sMaxW = Math.Max(sMaxW, child.MeasuredWidth + child.Margin.Left + child.Margin.Right);
        }

        if (Children.Count == 0)
        {
            sTotalH = Padding.Top + Padding.Bottom;
            sMaxW = Padding.Left + Padding.Right;
        }

        return (sMaxW, sTotalH);
    }

    protected override void PostRender()
    {
        if (Hwnd == 0 || Children.Count == 0) return;

        Win32.GetClientRect(Hwnd, out RECT rect);
        float padW = Padding.Left + Padding.Right;
        float padH = Padding.Top + Padding.Bottom;
        float viewW = Math.Max(rect.Right - rect.Left - padW, 0);
        float viewH = Math.Max(rect.Bottom - rect.Top - padH, 0);

        Strategy!.Measure(Children, viewW, viewH);
        Strategy.Arrange(Children, viewW, viewH, Spacing, Padding);

        foreach (var child in Children)
            child.Render();
    }
}
