using EasyWindowsApplication.CoreModule.Backend;
using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.WindowingModule.Frontend;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class LayoutBuilderImpl : ILayoutBuilder, ILayoutBuilderAfterWindow
{
    private readonly Application _app;

    internal LayoutBuilderImpl(Application app) => _app = app;

    public ILayoutBuilderAfterWindow Window()
    {
        _app.AddWindow(new WindowModel());
        return this;
    }

    public ILayoutBuilderAfterWindow Window(Action<IWindowConfig> configure)
    {
        var model = new WindowModel();
        var config = new WindowConfigBuilder(model);
        configure(config);
        _app.AddWindow(model);
        return this;
    }

    public ILayoutBuilderAfterWindow AlternativeWindow()
    {
        _app.AddWindow(new WindowModel { IsAlternative = true });
        return this;
    }

    public ILayoutBuilderAfterWindow AlternativeWindow(Action<IWindowConfig> configure)
    {
        var model = new WindowModel { IsAlternative = true };
        var config = new WindowConfigBuilder(model);
        configure(config);
        _app.AddWindow(model);
        return this;
    }
}