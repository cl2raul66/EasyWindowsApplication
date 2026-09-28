namespace EasyWindowsApplication.Share;

public interface ISelectFolderDialog : ISystemDialog
{
    new ISelectFolderDialog Name(string name);
    new ISelectFolderDialog Title(string title);
    ISelectFolderDialog Content(Action<ISelectFolderDialogContentBuilder> configure); // COMPLETO
    FolderSelectResult Show();
}

public interface ISelectFolderDialogContentBuilder
{
    ISelectFolderDialogContentBuilder DefaultDirectory(string path);
    ISelectFolderDialogContentBuilder PersistLastDirectory(bool persist); // NUEVO
}