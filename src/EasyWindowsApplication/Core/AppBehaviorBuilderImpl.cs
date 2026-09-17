using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core;

internal sealed class AppBehaviorBuilderImpl : IAppBehavior
{
    private readonly List<Action> _launched = new();
    private readonly List<Action<IAppBehavior>> _launchedWithSelf = new();
    private readonly List<Action<CancelEventArgs>> _terminating = new();
    private readonly List<Action> _terminated = new();

    internal nint MainHwnd { get; set; }

    public IAppBehavior OnLaunched(Action handler)
    {
        _launched.Add(handler);
        return this;
    }

    public IAppBehavior OnLaunched(Action<IAppBehavior> handler)
    {
        _launchedWithSelf.Add(handler);
        return this;
    }

    public IAppBehavior OnTerminating(Action<CancelEventArgs> handler)
    {
        _terminating.Add(handler);
        return this;
    }

    public IAppBehavior OnTerminated(Action handler)
    {
        _terminated.Add(handler);
        return this;
    }

    public IAppBehavior TaskbarButtonVisibility(bool visible)
    {
        if (MainHwnd != 0)
            TaskbarList.SetVisible(MainHwnd, visible);
        return this;
    }

    internal void RaiseLaunched()
    {
        foreach (var handler in _launched.ToArray()) handler();
        foreach (var handler in _launchedWithSelf.ToArray()) handler(this);
    }

    internal bool RaiseTerminating()
    {
        var args = new CancelEventArgs();
        foreach (var handler in _terminating.ToArray()) handler(args);
        return args.Cancel;
    }

    internal void RaiseTerminated()
    {
        foreach (var handler in _terminated.ToArray()) handler();
    }
}
