using EasyWindowsApplication.Core;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal sealed class WindowConfigBuilder : IWindowConfig
{
    private readonly WindowModel _model;

    internal WindowConfigBuilder(WindowModel model) => _model = model;

    public IWindowConfig Name(string name)
    {
        _model.Name = name;
        return this;
    }

    public IWindowConfig Title(string title)
    {
        _model.Title = title;
        return this;
    }

    public IWindowConfig Dimensions(int width, int height)
    {
        _model.Width = width;
        _model.Height = height;
        return this;
    }

    public IWindowConfig Position(WindowPositionOnScreen position)
    {
        _model.Position = position;
        return this;
    }

    public IWindowConfig Background(Color color)
    {
        _model.Background = color;
        return this;
    }

    public IWindowConfig Scroll(Action<IWindowsScrollConfig> configure)
    {
        var cfg = new WindowsScroll();
        configure(cfg);
        _model.ScrollConfig = cfg;
        return this;
    }

    public IWindowContentConfig Content<TLayout>(Action<IContentBuilder> configure) where TLayout : IStackLayout
        => RegisterContent(configure);

    public IWindowContentConfig Content(Action<IContentBuilder> configure)
        => RegisterContent(configure);

    private IWindowContentConfig RegisterContent(Action<IContentBuilder> configure)
    {
        var content = new ContentBuilderImpl();
        configure(content);
        _model.Content = content.Model;
        return new WindowContentConfig();
    }
}

internal sealed class WindowContentConfig : IWindowContentConfig { }
