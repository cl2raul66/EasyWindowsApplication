namespace EasyWindowsApplication.Share;

public sealed record FileOpenResult(bool IsCanceled, IReadOnlyList<string> FilePaths);
public sealed record FileSaveResult(bool IsCanceled, string? FilePath);
public sealed record FolderSelectResult(bool IsCanceled, string? FolderPath);
public sealed record TaskDialogResult(bool IsCanceled, TaskDialogChoice Choice);