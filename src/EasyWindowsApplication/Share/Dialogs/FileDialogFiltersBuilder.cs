namespace EasyWindowsApplication.Share;

/// <summary>Builder de filtros (espejo de IContentBuilder.Children).</summary>
public interface IFileDialogFiltersBuilder
{
    IFileDialogFiltersBuilder Children(Action<IFileFilterChildrenBuilder> configure);
}

public interface IFileFilterChildrenBuilder
{
    IFileFilterChildrenBuilder FileFilter(FilePattern preset);
    IFileFilterChildrenBuilder FileFilter(string description, params FilePattern[] patterns);
}
