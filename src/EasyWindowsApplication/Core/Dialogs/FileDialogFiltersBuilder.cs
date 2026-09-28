using EasyWindowsApplication.Core.Dialogs.Com;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

// Builder de filtros: acumula grupos Win32 (descripción + patrones).
internal sealed class FileDialogFiltersBuilder : IFileDialogFiltersBuilder
{
    internal readonly List<FileDialogFilter> Filters = [];

    public IFileDialogFiltersBuilder Children(Action<IFileFilterChildrenBuilder> configure)
    {
        var children = new FileFilterChildrenBuilder(Filters);
        configure(children);
        return this;
    }
}

internal sealed class FileFilterChildrenBuilder(List<FileDialogFilter> target) : IFileFilterChildrenBuilder
{
    public IFileFilterChildrenBuilder FileFilter(FilePattern preset)
    {
        target.Add(new FileDialogFilter(preset.Description, preset.Win32Patterns));
        return this;
    }

    public IFileFilterChildrenBuilder FileFilter(string description, params FilePattern[] patterns)
    {
        string spec = string.Join(';', patterns.SelectMany(p => p.Filters));
        target.Add(new FileDialogFilter(description, spec));
        return this;
    }
}
