namespace EasyWindowsApplication.WindowingModule.Frontend;

public interface IContentBuilder
{
    IContentBuilder Spacing(int pixels);
    IContentBuilder Children(Action<IChildrenBuilder> configure);
}
