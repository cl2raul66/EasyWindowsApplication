using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.LayoutModule.Backend;

internal sealed class GridLayoutStrategy : ILayoutStrategy
{
    private readonly IGridLayout _grid;
    private float[] _rowHeights = [];
    private float[] _colWidths = [];

    public GridLayoutStrategy(IGridLayout grid)
    {
        _grid = grid;
    }

    public void Measure(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight)
    {
        var rows = _grid.RowDefinitions;
        var cols = _grid.ColumnDefinitions;

        int rowCount = Math.Max(rows.Count, 1);
        int colCount = Math.Max(cols.Count, 1);

        _rowHeights = new float[rowCount];
        _colWidths = new float[colCount];

        for (int r = 0; r < rowCount; r++)
        {
            bool isAuto = rows.Count > 0 && rows[r].Height.Type == GridUnitType.Auto;
            if (!isAuto) continue;

            float maxH = 0;
            
            _rowHeights[r] = maxH;
        }

        float totalFixedHeight = 0;
        float totalStarHeight = 0;

        for (int r = 0; r < rowCount; r++)
        {
            if (rows.Count > 0 && rows[r].Height.Type == GridUnitType.Absolute)
            {
                _rowHeights[r] = rows[r].Height.Value;
                totalFixedHeight += _rowHeights[r];
            }
            else if (rows.Count > 0 && rows[r].Height.Type == GridUnitType.Star)
            {
                totalStarHeight += rows[r].Height.Value;
            }
            else if (rows.Count == 0)
            {
                _rowHeights[r] = availableHeight;
            }
        }

        float remainingHeight = availableHeight - totalFixedHeight;
        if (totalStarHeight > 0)
        {
            for (int r = 0; r < rowCount; r++)
            {
                if (rows.Count > 0 && rows[r].Height.Type == GridUnitType.Star)
                {
                    _rowHeights[r] = (rows[r].Height.Value / totalStarHeight) * remainingHeight;
                }
            }
        }

        for (int r = 0; r < rowCount; r++)
        {
            if (rows.Count == 0 || rows[r].Height.Type != GridUnitType.Auto)
                continue;

            
        }

        float totalFixedWidth = 0;
        float totalStarWidth = 0;

        for (int c = 0; c < colCount; c++)
        {
            if (cols.Count > 0 && cols[c].Width.Type == GridUnitType.Absolute)
            {
                _colWidths[c] = cols[c].Width.Value;
                totalFixedWidth += _colWidths[c];
            }
            else if (cols.Count > 0 && cols[c].Width.Type == GridUnitType.Star)
            {
                totalStarWidth += cols[c].Width.Value;
            }
            else if (cols.Count == 0)
            {
                _colWidths[c] = availableWidth;
            }
        }

        float remainingWidth = availableWidth - totalFixedWidth;
        if (totalStarWidth > 0)
        {
            for (int c = 0; c < colCount; c++)
            {
                if (cols.Count > 0 && cols[c].Width.Type == GridUnitType.Star)
                {
                    _colWidths[c] = (cols[c].Width.Value / totalStarWidth) * remainingWidth;
                }
            }
        }

        for (int c = 0; c < colCount; c++)
        {
            if (cols.Count == 0 || cols[c].Width.Type != GridUnitType.Auto)
                continue;

            float maxW = 0;
            
            _colWidths[c] = maxW;
        }
    }

    public void Arrange(IReadOnlyList<ILayoutable> children, float availableWidth, float availableHeight, float spacing, Thickness padding)
    {
        var rows = _grid.RowDefinitions;
        var cols = _grid.ColumnDefinitions;

        int rowCount = Math.Max(rows.Count, 1);
        int colCount = Math.Max(cols.Count, 1);

        float rowSpacing = _grid.RowSpacing;
        float colSpacing = _grid.ColumnSpacing;

        float[] rowY = new float[rowCount];
        float y = padding.Top;
        for (int r = 0; r < rowCount; r++)
        {
            rowY[r] = y;
            y += _rowHeights[r] + (r < rowCount - 1 ? rowSpacing : 0);
        }

        float[] colX = new float[colCount];
        float x = padding.Left;
        for (int c = 0; c < colCount; c++)
        {
            colX[c] = x;
            x += _colWidths[c] + (c < colCount - 1 ? colSpacing : 0);
        }

        
    }
}
