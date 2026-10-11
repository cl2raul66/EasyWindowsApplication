using EasyWindowsApplication.Core.Dialogs.Com;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

internal sealed class OpenFileDialogImpl : DialogSurfaceBase, IOpenFileDialog
{
    internal readonly List<FileDialogFilter> FilterList = [];
    internal string? DefaultDirectoryValue;
    internal bool MultiSelectValue;

    IOpenFileDialog IOpenFileDialog.Name(string name)
    {
        Name = name;
        return this;
    }

    IOpenFileDialog IOpenFileDialog.Title(string title)
    {
        DialogTitle = title;
        return this;
    }

    public IOpenFileDialog Filters(Action<IFileDialogFiltersBuilder> configure)
    {
        var builder = new FileDialogFiltersBuilder();
        builder.Filters.AddRange(FilterList);
        configure(builder);
        FilterList.Clear();
        FilterList.AddRange(builder.Filters);
        return this;
    }

    public IOpenFileDialog Content(Action<IOpenFileDialogContentBuilder> configure)
    {
        var builder = new OpenFileDialogContentBuilder(this);
        configure(builder);
        return this;
    }

    public FileOpenResult Show()
    {
        var (paths, canceled) = FileDialogCom.ShowFileDialog(
            OwnerProvider(), FileDialogKind.Open, FilterList,
            MultiSelectValue, DefaultDirectoryValue, null, null, DialogTitle);
        return new FileOpenResult(canceled, paths);
    }
}

internal sealed class OpenFileDialogContentBuilder(OpenFileDialogImpl target) : IOpenFileDialogContentBuilder
{
    public IOpenFileDialogContentBuilder DefaultDirectory(string path)
    {
        target.DefaultDirectoryValue = path;
        return this;
    }

    public IOpenFileDialogContentBuilder MultiSelect(bool value)
    {
        target.MultiSelectValue = value;
        return this;
    }
}