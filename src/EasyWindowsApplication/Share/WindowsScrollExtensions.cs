namespace EasyWindowsApplication.Share;

public static class WindowsScrollExtensions
{
    public static IWindowsScrollConfig Vertical(this IWindowsScrollConfig config)
    {
        config.Orientation(ScrollOrientation.Vertical);
        return config;
    }

    public static IWindowsScrollConfig Horizontal(this IWindowsScrollConfig config)
    {
        config.Orientation(ScrollOrientation.Horizontal);
        return config;
    }

    public static IWindowsScrollConfig Both(this IWindowsScrollConfig config)
    {
        config.Orientation(ScrollOrientation.Both);
        return config;
    }

    public static IWindowsScrollConfig Neither(this IWindowsScrollConfig config)
    {
        config.Orientation(ScrollOrientation.Neither);
        return config;
    }

    public static IWindowsScrollConfig AutoHide(this IWindowsScrollConfig config)
    {
        config.VerticalScrollBarVisibility(ScrollBarVisibility.Default);
        config.HorizontalScrollBarVisibility(ScrollBarVisibility.Default);
        return config;
    }

    public static IWindowsScrollConfig Always(this IWindowsScrollConfig config)
    {
        config.VerticalScrollBarVisibility(ScrollBarVisibility.Always);
        config.HorizontalScrollBarVisibility(ScrollBarVisibility.Always);
        return config;
    }

    public static IWindowsScrollConfig Never(this IWindowsScrollConfig config)
    {
        config.VerticalScrollBarVisibility(ScrollBarVisibility.Never);
        config.HorizontalScrollBarVisibility(ScrollBarVisibility.Never);
        return config;
    }
}
