using System.Runtime.InteropServices;

namespace EasyWindowsApplication.Core.Dialogs.Com;

internal enum FileDialogKind
{
    Open,
    Save,
    Folder
}

internal sealed record FileDialogFilter(string Name, string Spec);

// Motor COM unificado (IFileDialog + IFileOpenDialog + IFileSaveDialog).
// Interop manual vtable estilo TaskbarList.cs: AOT-safe, cero dependencias.
//
// Índices vtable absolutos (base 0 desde IUnknown), según shobjidl_core.h:
//   IUnknown: 0 QueryInterface, 1 AddRef, 2 Release
//   IModalWindow: 3 Show
//   IFileDialog: 4 SetFileTypes, 5 SetFileTypeIndex, 6 GetFileTypeIndex,
//     7 Advise, 8 Unadvise, 9 SetOptions, 10 GetOptions, 11 SetDefaultFolder,
//     12 SetFolder, 13 GetFolder, 14 GetCurrentSelection, 15 SetFileName,
//     16 GetFileName, 17 SetTitle, 18 SetOkButtonLabel, 19 SetFileNameLabel,
//     20 GetResult, 21 AddPlace, 22 SetDefaultExtension, 23 Close,
//     24 SetClientGuid, 25 ClearClientData, 26 SetFilter
//   IFileOpenDialog: 27 GetResults, 28 GetSelectedItems
//   IShellItem (IUnknown): 3 BindToHandler, 4 GetParent, 5 GetDisplayName,
//     6 GetAttributes, 7 Compare
//   IShellItemArray (IUnknown): 3 BindToHandler, 4 GetPropertyStore,
//     5 GetPropertyDescriptionList, 6 GetAttributes, 7 GetCount,
//     8 GetItemAt, 9 EnumItems
internal static partial class FileDialogCom
{
    [LibraryImport("ole32.dll")]
    private static partial int CoCreateInstance(
        ref Guid rclsid, nint pUnkOuter, uint dwClsContext, ref Guid riid, out nint ppv);

    [LibraryImport("ole32.dll")]
    private static partial int CoInitializeEx(nint pvReserved, uint dwCoInit);

    [LibraryImport("ole32.dll")]
    private static partial void CoUninitialize();

    private const uint COINIT_APARTMENTTHREADED = 0x2;
    private const int S_OK = 0;
    private const int S_FALSE_COM = 1;
    private const int RPC_E_CHANGED_MODE = unchecked((int)0x80010106);

    [LibraryImport("ole32.dll")]
    private static partial void CoTaskMemFree(nint pv);

    [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int SHCreateItemFromParsingName(
        string pszPath, nint pbc, ref Guid riid, out nint ppv);

    [StructLayout(LayoutKind.Sequential)]
    private struct COMDLG_FILTERSPEC
    {
        internal nint pszName;
        internal nint pszSpec;
    }

    internal static unsafe (List<string> Paths, bool Canceled) ShowFileDialog(
        nint owner,
        FileDialogKind kind,
        IReadOnlyList<FileDialogFilter> filters,
        bool multiSelect,
        string? defaultFolder,
        string? defaultFileName,
        string? defaultExtension,
        string? title,
        bool persistLastDirectory = true)
    {
        Guid clsid = kind == FileDialogKind.Save
            ? FileDialogNative.CLSID_FileSaveDialog
            : FileDialogNative.CLSID_FileOpenDialog;
        Guid iid = kind == FileDialogKind.Save
            ? FileDialogNative.IID_IFileSaveDialog
            : FileDialogNative.IID_IFileOpenDialog;

        // Los diálogos comunes COM son STA-only. En el diseño actual el pipeline
        // corre en el hilo EWA-UI (STA), así que esto es solo una red de seguridad:
        // el único modo de llegar aquí sin STA es invocar Show() desde otra hebra
        // (p. ej. Task.Run / ThreadPool), caso en el que CoInitializeEx(STA)
        // devuelve RPC_E_CHANGED_MODE porque el apartment COM es inmutable.
        // S_OK => inicializamos nosotros y debemos CoUninitialize al final;
        // S_FALSE => ya estaba inicializado en este mismo hilo (compatible).
        int coinit = CoInitializeEx(0, COINIT_APARTMENTTHREADED);
        if (coinit == RPC_E_CHANGED_MODE)
            throw new InvalidOperationException(
                "Un diálogo del sistema debe invocarse desde el hilo de UI (EWA-UI, STA). " +
                "El hilo actual está en MTA (CoInitializeEx devolvió RPC_E_CHANGED_MODE). " +
                "NO uses Task.Run / ThreadPool / hilos propios alrededor de Show(): " +
                "los resultados solo se pueden devolver desde el behavior que abre el diálogo.");
        bool ownCom = coinit == S_OK;
        try
        {
            int hr = CoCreateInstance(ref clsid, 0, FileDialogNative.CLSCTX_INPROC_SERVER, ref iid, out nint dialog);
            if (hr != 0 || dialog == 0)
                throw new InvalidOperationException($"CoCreateInstance del diálogo común falló (HRESULT 0x{hr:X8}).");
            try
            {
                nint* vtable = *(nint**)dialog;

                // Opciones: leer las actuales y OR con las necesarias (no pisar defaults del OS).
                uint options = GetOptions(dialog, vtable);
                uint wanted = 0;
                if (kind == FileDialogKind.Save)
                    wanted |= FileDialogNative.FOS_OVERWRITEPROMPT;
                if (kind == FileDialogKind.Folder)
                    wanted |= FileDialogNative.FOS_PICKFOLDERS | FileDialogNative.FOS_FORCEFILESYSTEM;
                if (kind == FileDialogKind.Open && multiSelect)
                    wanted |= FileDialogNative.FOS_ALLOWMULTISELECT;
                if (wanted != 0)
                    SetOptions(dialog, vtable, options | wanted);

                // Filtros (no aplican a Folder: SetFileTypes falla con FOS_PICKFOLDERS).
                if (kind != FileDialogKind.Folder && filters.Count > 0)
                    SetFileTypes(dialog, vtable, filters);

                if (!string.IsNullOrEmpty(title))
                    SetTitle(dialog, vtable, title);

                if (!string.IsNullOrEmpty(defaultFolder))
                    SetDefaultFolder(dialog, vtable, defaultFolder);

                if (kind == FileDialogKind.Save)
                {
                    if (!string.IsNullOrEmpty(defaultFileName))
                        SetFileName(dialog, vtable, defaultFileName);
                    if (!string.IsNullOrEmpty(defaultExtension))
                        SetDefaultExtension(dialog, vtable, defaultExtension);
                }

                // PersistLastDirectory(false): aislar la persistencia del shell
                // generando un ClientGuid único por llamada, de modo que
                // DefaultDirectory se respete siempre.
                if (!persistLastDirectory)
                    SetClientGuid(dialog, vtable, Guid.NewGuid());

                var show = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[3];
                int showHr = show(dialog, owner);
                if (showHr == FileDialogNative.HRESULT_CANCELLED)
                    return ([], true);
                if (showHr != 0)
                    return ([], true);

                if (kind == FileDialogKind.Open && multiSelect)
                    return (GetResults(dialog, vtable), false);

                string? single = GetResultPath(dialog, vtable);
                return (single is null ? [] : [single], false);
            }
            finally
            {
                var release = (delegate* unmanaged[Stdcall]<nint, uint>)((*(nint**)dialog)[2]);
                release(dialog);
            }
        }
        finally
        {
            if (ownCom)
                CoUninitialize();
        }
    }

    private static unsafe uint GetOptions(nint dialog, nint* vtable)
    {
        var getOptions = (delegate* unmanaged[Stdcall]<nint, uint*, int>)vtable[10];
        uint options = 0;
        getOptions(dialog, &options);
        return options;
    }

    private static unsafe void SetOptions(nint dialog, nint* vtable, uint options)
    {
        var setOptions = (delegate* unmanaged[Stdcall]<nint, uint, int>)vtable[9];
        setOptions(dialog, options);
    }

    private static unsafe void SetFileTypes(nint dialog, nint* vtable, IReadOnlyList<FileDialogFilter> filters)
    {
        int count = filters.Count;
        nint specsMem = Marshal.AllocHGlobal(sizeof(COMDLG_FILTERSPEC) * count);
        var allocated = new List<nint>(count * 2);
        try
        {
            var specs = (COMDLG_FILTERSPEC*)specsMem;
            for (int i = 0; i < count; i++)
            {
                nint name = Marshal.StringToCoTaskMemUni(filters[i].Name);
                nint spec = Marshal.StringToCoTaskMemUni(filters[i].Spec);
                allocated.Add(name);
                allocated.Add(spec);
                specs[i].pszName = name;
                specs[i].pszSpec = spec;
            }
            var setFileTypes = (delegate* unmanaged[Stdcall]<nint, uint, nint, int>)vtable[4];
            setFileTypes(dialog, (uint)count, specsMem);
            var setFileTypeIndex = (delegate* unmanaged[Stdcall]<nint, uint, int>)vtable[5];
            setFileTypeIndex(dialog, 1);
        }
        finally
        {
            foreach (nint p in allocated)
                Marshal.FreeCoTaskMem(p);
            Marshal.FreeHGlobal(specsMem);
        }
    }

    private static unsafe void SetTitle(nint dialog, nint* vtable, string title)
    {
        nint ptr = Marshal.StringToCoTaskMemUni(title);
        try
        {
            var setTitle = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[17];
            setTitle(dialog, ptr);
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    private static unsafe void SetFileName(nint dialog, nint* vtable, string fileName)
    {
        nint ptr = Marshal.StringToCoTaskMemUni(fileName);
        try
        {
            var setFileName = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[15];
            setFileName(dialog, ptr);
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    private static unsafe void SetDefaultExtension(nint dialog, nint* vtable, string extension)
    {
        string ext = extension.TrimStart('.', '*');
        nint ptr = Marshal.StringToCoTaskMemUni(ext);
        try
        {
            var setDefaultExtension = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[22];
            setDefaultExtension(dialog, ptr);
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    private static unsafe void SetDefaultFolder(nint dialog, nint* vtable, string path)
    {
        try
        {
            Guid iid = FileDialogNative.IID_IShellItem;
            int hr = SHCreateItemFromParsingName(path, 0, ref iid, out nint item);
            if (hr != 0 || item == 0)
                return;
            try
            {
                var setDefaultFolder = (delegate* unmanaged[Stdcall]<nint, nint, int>)vtable[11];
                setDefaultFolder(dialog, item);
            }
            finally
            {
                var release = (delegate* unmanaged[Stdcall]<nint, uint>)((*(nint**)item)[2]);
                release(item);
            }
        }
        catch
        {
            // DefaultDirectory inválido: el diálogo abre con la carpeta por defecto del OS.
        }
    }

    private static unsafe void SetClientGuid(nint dialog, nint* vtable, Guid guid)
    {
        var setClientGuid = (delegate* unmanaged[Stdcall]<nint, Guid*, int>)vtable[24];
        setClientGuid(dialog, &guid);
    }

    private static unsafe string? GetResultPath(nint dialog, nint* vtable)
    {
        var getResult = (delegate* unmanaged[Stdcall]<nint, nint*, int>)vtable[20];
        nint item = 0;
        if (getResult(dialog, &item) != 0 || item == 0)
            return null;
        try
        {
            return GetItemPath(item);
        }
        finally
        {
            var release = (delegate* unmanaged[Stdcall]<nint, uint>)((*(nint**)item)[2]);
            release(item);
        }
    }

    private static unsafe List<string> GetResults(nint dialog, nint* vtable)
    {
        var result = new List<string>();
        var getResults = (delegate* unmanaged[Stdcall]<nint, nint*, int>)vtable[27];
        nint array = 0;
        if (getResults(dialog, &array) != 0 || array == 0)
        {
            string? fallback = GetResultPath(dialog, vtable);
            if (fallback is not null)
                result.Add(fallback);
            return result;
        }
        try
        {
            nint* arrayVtable = *(nint**)array;
            var getCount = (delegate* unmanaged[Stdcall]<nint, uint*, int>)arrayVtable[7];
            var getItemAt = (delegate* unmanaged[Stdcall]<nint, uint, nint*, int>)arrayVtable[8];
            uint count = 0;
            if (getCount(array, &count) != 0)
                return result;
            for (uint i = 0; i < count; i++)
            {
                nint item = 0;
                if (getItemAt(array, i, &item) != 0 || item == 0)
                    continue;
                try
                {
                    string? path = GetItemPath(item);
                    if (path is not null)
                        result.Add(path);
                }
                finally
                {
                    var release = (delegate* unmanaged[Stdcall]<nint, uint>)((*(nint**)item)[2]);
                    release(item);
                }
            }
            return result;
        }
        finally
        {
            var release = (delegate* unmanaged[Stdcall]<nint, uint>)((*(nint**)array)[2]);
            release(array);
        }
    }

    private static unsafe string? GetItemPath(nint item)
    {
        nint* itemVtable = *(nint**)item;
        var getDisplayName = (delegate* unmanaged[Stdcall]<nint, uint, nint*, int>)itemVtable[5];
        nint psz = 0;
        if (getDisplayName(item, FileDialogNative.SIGDN_FILESYSPATH, &psz) != 0 || psz == 0)
            return null;
        try
        {
            return Marshal.PtrToStringUni(psz);
        }
        finally
        {
            CoTaskMemFree(psz);
        }
    }
}
