using EasyWindowsApplication.LayoutModule.Backend;

namespace EasyWindowsApplication.LayoutModule.Frontend;

public interface IGridLayout : IStackLayout
{
    IReadOnlyList<RowDefinition> RowDefinitions { get; }
    IReadOnlyList<ColumnDefinition> ColumnDefinitions { get; }

    float RowSpacing { get; }
    float ColumnSpacing { get; }
}
