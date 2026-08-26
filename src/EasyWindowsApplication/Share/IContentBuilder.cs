
namespace EasyWindowsApplication.Share;

public interface IContentBuilder
{
    IContentBuilder Spacing(float pixels);
    IContentBuilder Padding(float uniform);
    IContentBuilder Padding(float vertical, float horizontal);
    IContentBuilder Padding(float top, float right, float bottom, float left);
    IContentBuilder Margin(float uniform);
    IContentBuilder Margin(float vertical, float horizontal);
    IContentBuilder Margin(float top, float right, float bottom, float left);
    IContentBuilder Children(Action<IChildrenBuilder> configure);

    IContentBuilder RowDefinition(GridUnitType unitType, float value = 1);
    IContentBuilder ColumnDefinition(GridUnitType unitType, float value = 1);
    IContentBuilder RowSpacing(float pixels);
    IContentBuilder ColumnSpacing(float pixels);
    IContentBuilder DefaultDock(DockPosition position);
    IContentBuilder ShouldExpandLastChild(bool expand);
}
