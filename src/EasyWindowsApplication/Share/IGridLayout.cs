using EasyWindowsApplication.Core.LayoutEngine;

namespace EasyWindowsApplication.Share;

public interface IGridLayout : IStackLayout
{
    IReadOnlyList<RowDefinition> RowDefinitions { get; }
    IReadOnlyList<ColumnDefinition> ColumnDefinitions { get; }

    float RowSpacing { get; }
    float ColumnSpacing { get; }
}
