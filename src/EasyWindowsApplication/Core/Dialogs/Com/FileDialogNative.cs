namespace EasyWindowsApplication.Core.Dialogs.Com;

// Constantes del motor de diálogos comunes (shobjidl_core.h).
// Se pinan contra la documentación oficial de Microsoft Learn.
internal static class FileDialogNative
{
    // CLSID / IID
    internal static readonly Guid CLSID_FileOpenDialog = new("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
    internal static readonly Guid CLSID_FileSaveDialog = new("C0B4E2F3-BA21-4773-8DBA-335EC946EB8B");
    internal static readonly Guid IID_IFileOpenDialog = new("D57C7288-D4AD-4768-BE02-9D969532D960");
    internal static readonly Guid IID_IFileSaveDialog = new("84BCCD23-5FDE-4CDB-AEA4-AF64B83D78AB");
    internal static readonly Guid IID_IShellItem = new("43826D1E-E718-42EE-BC55-A1E261C37BFE");

    internal const uint CLSCTX_INPROC_SERVER = 1;

    // FILEOPENDIALOGOPTIONS (shobjidl_core.h)
    internal const uint FOS_OVERWRITEPROMPT = 0x00000002;
    internal const uint FOS_PICKFOLDERS = 0x00000020;
    internal const uint FOS_FORCEFILESYSTEM = 0x00000040;
    internal const uint FOS_ALLOWMULTISELECT = 0x00000200;

    // SIGDN
    internal const uint SIGDN_FILESYSPATH = 0x80058000;

    // HRESULT de cancelación (HRESULT_FROM_WIN32(ERROR_CANCELLED))
    internal const int HRESULT_CANCELLED = unchecked((int)0x800704C7);
}
