namespace EasyWindowsApplication.Share;

public interface IOpenFileDialog : ISystemDialog
{
    new IOpenFileDialog Name(string name);
    new IOpenFileDialog Title(string title);
    IOpenFileDialog Filters(Action<IFileDialogFiltersBuilder> configure); // TOP-LEVEL
    IOpenFileDialog Content(Action<IOpenFileDialogContentBuilder> configure); // COMPLETO
    FileOpenResult Show();
}

public interface IOpenFileDialogContentBuilder
{
    IOpenFileDialogContentBuilder DefaultDirectory(string path);
    IOpenFileDialogContentBuilder MultiSelect(bool value);
}