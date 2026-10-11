namespace EasyWindowsApplication.Share;

public interface ISaveFileDialog : ISystemDialog
{
    new ISaveFileDialog Name(string name);
    new ISaveFileDialog Title(string title);
    ISaveFileDialog Filters(Action<IFileDialogFiltersBuilder> configure); // TOP-LEVEL
    ISaveFileDialog Content(Action<ISaveFileDialogContentBuilder> configure); // COMPLETO
    FileSaveResult Show();
}

public interface ISaveFileDialogContentBuilder
{
    ISaveFileDialogContentBuilder DefaultDirectory(string path);
    ISaveFileDialogContentBuilder DefaultFileName(string fileName);
    ISaveFileDialogContentBuilder DefaultExtension(FilePattern pattern);
}