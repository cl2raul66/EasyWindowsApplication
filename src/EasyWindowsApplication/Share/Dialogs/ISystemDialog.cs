namespace EasyWindowsApplication.Share;

/// <summary>Marker + fluent base de los diálogos del sistema.</summary>
public interface ISystemDialog : IViewSurface
{
    new ISystemDialog Name(string name);
    ISystemDialog Title(string title);
}
