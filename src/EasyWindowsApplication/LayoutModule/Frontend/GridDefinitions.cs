namespace EasyWindowsApplication.LayoutModule.Frontend;

public readonly struct RowDefinition
{
    public LayoutLength Height { get; }
    public RowDefinition(LayoutLength height) => Height = height;
}

public readonly struct ColumnDefinition
{
    public LayoutLength Width { get; }
    public ColumnDefinition(LayoutLength width) => Width = width;
}
