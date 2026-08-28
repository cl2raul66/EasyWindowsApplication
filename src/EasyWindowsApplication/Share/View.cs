using System.ComponentModel;
using EasyWindowsApplication.Core;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

namespace EasyWindowsApplication.Share;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ViewBase<TSelf> : ControlBase where TSelf : ViewBase<TSelf>
{
    internal ViewBase() { }

    public new TSelf OnMessage(uint msg, Win32MessageHandler handler)
    {
        base.OnMessage(msg, handler);
        return (TSelf)this;
    }

    public new TSelf OnClick(Action handler)
    {
        base.OnClick(handler);
        return (TSelf)this;
    }
}

public sealed class View<T> where T : class, IControl
{
    private readonly List<Action<T>> _configure = new();
    internal string? PendingName { get; private set; }

    public View() { }

    public View<T> Name(string name)
    {
        PendingName = name;
        _configure.Add(c => c.Name = name);
        return this;
    }

    public View<T> Margin(float uniform)
    {
        var thickness = new Thickness(uniform);
        _configure.Add(c => c.Margin = thickness);
        return this;
    }

    public View<T> Margin(float vertical, float horizontal)
    {
        var thickness = new Thickness(vertical, horizontal);
        _configure.Add(c => c.Margin = thickness);
        return this;
    }

    public View<T> Margin(float top, float right, float bottom, float left)
    {
        var thickness = new Thickness(top, right, bottom, left);
        _configure.Add(c => c.Margin = thickness);
        return this;
    }

    public View<T> Padding(float uniform)
    {
        var thickness = new Thickness(uniform);
        _configure.Add(c => c.Padding = thickness);
        return this;
    }

    public View<T> Padding(float vertical, float horizontal)
    {
        var thickness = new Thickness(vertical, horizontal);
        _configure.Add(c => c.Padding = thickness);
        return this;
    }

    public View<T> Padding(float top, float right, float bottom, float left)
    {
        var thickness = new Thickness(top, right, bottom, left);
        _configure.Add(c => c.Padding = thickness);
        return this;
    }

    public View<T> Width(float width)
    {
        var layout = LayoutLength.Absolute(width);
        _configure.Add(c => c.LayoutWidth = layout);
        return this;
    }

    public View<T> Height(float height)
    {
        var layout = LayoutLength.Absolute(height);
        _configure.Add(c => c.LayoutHeight = layout);
        return this;
    }

    public View<T> Background(Color color)
    {
        _configure.Add(c => c.BackgroundColor = color);
        return this;
    }

    public View<T> Dock(DockPosition dock)
    {
        _configure.Add(c => c.Dock = dock);
        return this;
    }

    public View<T> DockLeft()
    {
        _configure.Add(c => c.Dock = DockPosition.Left);
        return this;
    }

    public View<T> DockTop()
    {
        _configure.Add(c => c.Dock = DockPosition.Top);
        return this;
    }

    public View<T> DockRight()
    {
        _configure.Add(c => c.Dock = DockPosition.Right);
        return this;
    }

    public View<T> DockBottom()
    {
        _configure.Add(c => c.Dock = DockPosition.Bottom);
        return this;
    }

    public View<T> Row(int row)
    {
        _configure.Add(c => c.GridRow = row);
        return this;
    }

    public View<T> Column(int column)
    {
        _configure.Add(c => c.GridColumn = column);
        return this;
    }

    public View<T> RowSpan(int span)
    {
        _configure.Add(c => c.GridRowSpan = span);
        return this;
    }

    public View<T> ColumnSpan(int span)
    {
        _configure.Add(c => c.GridColumnSpan = span);
        return this;
    }

    public View<T> OnClick(Action handler)
    {
        _configure.Add(c => c.OnClick(handler));
        return this;
    }

    public View<T> Text(string text)
    {
        _configure.Add(c => { if (c is IText t) t.Text = text; });
        return this;
    }

    public View<T> OnMessage(uint msg, Win32MessageHandler handler)
    {
        _configure.Add(c => { if (c is ControlBase cb) cb.OnMessage(msg, handler); });
        return this;
    }

    internal void Apply(T instance)
    {
        foreach (var a in _configure) a(instance);
    }

    internal Action<IControl> BuildConfigure()
    {
        var snapshot = _configure.ToArray();
        return control =>
        {
            var typed = (T)control;
            foreach (var a in snapshot) a(typed);
        };
    }
}
