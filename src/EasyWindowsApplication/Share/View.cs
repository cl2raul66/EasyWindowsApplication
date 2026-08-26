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

public readonly struct View<T> where T : class, IControl
{
    public T Instance { get; }

    public View(T instance) => Instance = instance;

    public View<T> Name(string name) { Instance.Name = name; return this; }

    public View<T> Margin(float uniform) { Instance.Margin = new Thickness(uniform); return this; }
    public View<T> Margin(float vertical, float horizontal) { Instance.Margin = new Thickness(vertical, horizontal); return this; }
    public View<T> Margin(float top, float right, float bottom, float left) { Instance.Margin = new Thickness(top, right, bottom, left); return this; }

    public View<T> Padding(float uniform) { Instance.Padding = new Thickness(uniform); return this; }
    public View<T> Padding(float vertical, float horizontal) { Instance.Padding = new Thickness(vertical, horizontal); return this; }
    public View<T> Padding(float top, float right, float bottom, float left) { Instance.Padding = new Thickness(top, right, bottom, left); return this; }

    public View<T> Width(float width) { Instance.LayoutWidth = LayoutLength.Absolute(width); return this; }
    public View<T> Height(float height) { Instance.LayoutHeight = LayoutLength.Absolute(height); return this; }

    public View<T> Background(Color color) { Instance.BackgroundColor = color; return this; }

    public View<T> Dock(DockPosition dock) { Instance.Dock = dock; return this; }
    public View<T> DockLeft() { Instance.Dock = DockPosition.Left; return this; }
    public View<T> DockTop() { Instance.Dock = DockPosition.Top; return this; }
    public View<T> DockRight() { Instance.Dock = DockPosition.Right; return this; }
    public View<T> DockBottom() { Instance.Dock = DockPosition.Bottom; return this; }

    public View<T> Row(int row) { Instance.GridRow = row; return this; }
    public View<T> Column(int column) { Instance.GridColumn = column; return this; }
    public View<T> RowSpan(int span) { Instance.GridRowSpan = span; return this; }
    public View<T> ColumnSpan(int span) { Instance.GridColumnSpan = span; return this; }

    public View<T> OnClick(Action handler) { Instance.OnClick(handler); return this; }

    public View<T> Text(string text)
    {
        if (Instance is IText t) t.Text = text;
        return this;
    }

    public View<T> OnMessage(uint msg, Win32MessageHandler handler)
    {
        if (Instance is ControlBase cb) cb.OnMessage(msg, handler);
        return this;
    }
}
