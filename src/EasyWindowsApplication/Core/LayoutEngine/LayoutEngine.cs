using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.LayoutEngine;

internal sealed class LayoutEngine
{
    private readonly ILayoutStrategy _strategy;

    public LayoutEngine(ILayoutStrategy strategy)
    {
        _strategy = strategy;
    }

    public void Execute(
        IReadOnlyList<ILayoutable> children,
        float availableWidth,
        float availableHeight,
        float spacing,
        Thickness padding)
    {
        if (children.Count == 0) return;

        _strategy.Measure(children, availableWidth, availableHeight);
        _strategy.Arrange(children, availableWidth, availableHeight, spacing, padding);

        foreach (var child in children)
            child.Render();
    }
}
