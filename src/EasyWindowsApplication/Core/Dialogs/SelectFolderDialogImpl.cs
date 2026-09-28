using EasyWindowsApplication.Core.Dialogs.Com;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

internal sealed class SelectFolderDialogImpl : DialogSurfaceBase, ISelectFolderDialog
{
    internal string? DefaultDirectoryValue;
    internal bool PersistLastDirectoryValue = true; // default = true (comportamiento nativo)

    ISelectFolderDialog ISelectFolderDialog.Name(string name)
    {
        Name = name;
        return this;
    }

    ISelectFolderDialog ISelectFolderDialog.Title(string title)
    {
        DialogTitle = title;
        return this;
    }

    public ISelectFolderDialog Content(Action<ISelectFolderDialogContentBuilder> configure)
    {
        var builder = new SelectFolderDialogContentBuilder(this);
        configure(builder);
        return this;
    }

    public FolderSelectResult Show()
    {
        var (paths, canceled) = FileDialogCom.ShowFileDialog(
            OwnerProvider(), FileDialogKind.Folder, [],
            false, DefaultDirectoryValue, null, null, DialogTitle, PersistLastDirectoryValue);
        return new FolderSelectResult(canceled, canceled || paths.Count == 0 ? null : paths[0]);
    }
}

internal sealed class SelectFolderDialogContentBuilder(SelectFolderDialogImpl target) : ISelectFolderDialogContentBuilder
{
    public ISelectFolderDialogContentBuilder DefaultDirectory(string path)
    {
        target.DefaultDirectoryValue = path;
        return this;
    }

    public ISelectFolderDialogContentBuilder PersistLastDirectory(bool persist)
    {
        target.PersistLastDirectoryValue = persist;
        return this;
    }
}