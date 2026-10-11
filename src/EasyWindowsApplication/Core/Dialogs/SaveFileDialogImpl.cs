using EasyWindowsApplication.Core.Dialogs.Com;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

internal sealed class SaveFileDialogImpl : DialogSurfaceBase, ISaveFileDialog
{
    internal readonly List<FileDialogFilter> FilterList = [];
    internal string? DefaultDirectoryValue;
    internal string? DefaultFileNameValue;
    internal string? DefaultExtensionValue;

    ISaveFileDialog ISaveFileDialog.Name(string name)
    {
        Name = name;
        return this;
    }

    ISaveFileDialog ISaveFileDialog.Title(string title)
    {
        DialogTitle = title;
        return this;
    }

    public ISaveFileDialog Filters(Action<IFileDialogFiltersBuilder> configure)
    {
        var builder = new FileDialogFiltersBuilder();
        builder.Filters.AddRange(FilterList);
        configure(builder);
        FilterList.Clear();
        FilterList.AddRange(builder.Filters);
        return this;
    }

    public ISaveFileDialog Content(Action<ISaveFileDialogContentBuilder> configure)
    {
        var builder = new SaveFileDialogContentBuilder(this);
        configure(builder);
        return this;
    }

    public FileSaveResult Show()
    {
        var (paths, canceled) = FileDialogCom.ShowFileDialog(
            OwnerProvider(), FileDialogKind.Save, FilterList,
            false, DefaultDirectoryValue, DefaultFileNameValue, DefaultExtensionValue, DialogTitle);
        return new FileSaveResult(canceled, canceled || paths.Count == 0 ? null : paths[0]);
    }
}

internal sealed class SaveFileDialogContentBuilder(SaveFileDialogImpl target) : ISaveFileDialogContentBuilder
{
    public ISaveFileDialogContentBuilder DefaultDirectory(string path)
    {
        target.DefaultDirectoryValue = path;
        return this;
    }

    public ISaveFileDialogContentBuilder DefaultFileName(string fileName)
    {
        target.DefaultFileNameValue = fileName;
        return this;
    }

    public ISaveFileDialogContentBuilder DefaultExtension(FilePattern pattern)
    {
        target.DefaultExtensionValue = pattern.DefaultExtension;
        return this;
    }
}