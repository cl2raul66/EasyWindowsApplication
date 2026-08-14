using EasyWindowsApplication.LayoutModule.Frontend;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IViewBuilder 
{
    IViewBuilder Name(string name);
    IViewBuilder Margin(float uniform);
    IViewBuilder Margin(float vertical, float horizontal);
    IViewBuilder Margin(float top, float right, float bottom, float left);
    IViewBuilder HorizontalAlignment(LayoutAlignment alignment);
    IViewBuilder VerticalAlignment(LayoutAlignment alignment);
    IViewBuilder Width(float length);
    IViewBuilder Height(float length);
    IViewBuilder Padding(float uniform);
    IViewBuilder Padding(float vertical, float horizontal);
    IViewBuilder Padding(float top, float right, float bottom, float left);
    IViewBuilder Background(Color color);
    IViewBuilder Content(Action<IContentBuilder> configure);
}
