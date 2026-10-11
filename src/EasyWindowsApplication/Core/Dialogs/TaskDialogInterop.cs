using System.Runtime.InteropServices;
using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

// Interop de TaskDialogIndirect (comctl32.dll, sin COM).
// Estructura TASKDIALOGCONFIG según commctrl.h (ver docs oficiales).
// IMPORTANTE: la ABI efectiva de esta struct usa packing de 4 (no natural
// de 8): verificado empiricamente contra el struct de CsWin32 de WinForms
// (offsets observados con Pack=4, sizeof=160). Sin Pack=4 los punteros quedan
// en offsets 8-alineados distintos y TaskDialogIndirect devuelve E_INVALIDARG
// (la API espera sizeof=160).
[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct TaskDialogConfig
{
    internal uint cbSize;
    internal nint hwndParent;
    internal nint hInstance;
    internal uint dwFlags;
    internal uint dwCommonButtons;
    internal nint pszWindowTitle;
    internal nint pszMainIcon;
    internal nint pszMainInstruction;
    internal nint pszContent;
    internal uint cButtons;
    internal nint pButtons;
    internal int nDefaultButton;
    internal uint cRadioButtons;
    internal nint pRadioButtons;
    internal int nDefaultRadioButton;
    internal nint pszVerificationText;
    internal nint pszExpandedInformation;
    internal nint pszExpandedControlText;
    internal nint pszCollapsedControlText;
    internal nint pszFooterIcon;
    internal nint pszFooter;
    internal nint pfCallback;
    internal nint lpCallbackData;
    internal uint cxWidth;
}

internal static partial class TaskDialogInterop
{
    [LibraryImport("comctl32.dll")]
    private static partial int TaskDialogIndirect(
        ref TaskDialogConfig pTaskConfig,
        out int pnButton,
        out int pnRadioButton,
        out int pfVerificationFlagChecked);

    // TDCBF_* (commctrl.h)
    internal const uint TDCBF_OK_BUTTON = 0x0001;
    internal const uint TDCBF_YES_BUTTON = 0x0002;
    internal const uint TDCBF_NO_BUTTON = 0x0004;
    internal const uint TDCBF_CANCEL_BUTTON = 0x0008;
    internal const uint TDCBF_RETRY_BUTTON = 0x0010;
    internal const uint TDCBF_CLOSE_BUTTON = 0x0020;

    // TDF_* usados en v1
    internal const uint TDF_ALLOW_DIALOG_CANCELLATION = 0x0008;
    internal const uint TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000;

    // MAKEINTRESOURCEW(TD_*_ICON) (commctrl.h)
    internal static readonly nint TD_WARNING_ICON = 65535;
    internal static readonly nint TD_ERROR_ICON = 65534;
    internal static readonly nint TD_INFORMATION_ICON = 65533;
    internal static readonly nint TD_SHIELD_ICON = 65532;

    internal static TaskDialogResult Show(
        nint owner,
        string? title,
        string? text,
        string? primaryText,
        string? secondaryText,
        TaskDialogIcon notificationIcon,
        TaskDialogChoices choices,
        TaskDialogChoice defaultChoice)
    {
        // Modo mínimo: TextValue != null → el texto único va en pszMainInstruction.
        // Modo builder: PrimaryText → pszMainInstruction, SecondaryText → pszContent.
        string? instruction = text ?? primaryText;
        string? content = text is not null ? null : secondaryText;
        nint titlePtr = AllocOrNull(title);
        nint instructionPtr = AllocOrNull(instruction);
        nint contentPtr = AllocOrNull(content);
        try
        {
            var config = new TaskDialogConfig
            {
                hwndParent = owner,
                hInstance = 0,
                dwFlags = TDF_POSITION_RELATIVE_TO_WINDOW | TDF_ALLOW_DIALOG_CANCELLATION,
                dwCommonButtons = (uint)choices,
                pszWindowTitle = titlePtr,
                pszMainIcon = MapIcon(notificationIcon),
                pszMainInstruction = instructionPtr,
                pszContent = contentPtr,
                nDefaultButton = (int)defaultChoice,
                cxWidth = 0
            };
            config.cbSize = (uint)Marshal.SizeOf<TaskDialogConfig>();

            int hr = TaskDialogIndirect(ref config, out int button, out _, out _);
            if (hr != 0)
                return new TaskDialogResult(true, TaskDialogChoice.Cancel);

            var mapped = MapButton(button);
            bool canceled = button is 2 or 0;
            return new TaskDialogResult(canceled, mapped);
        }
        finally
        {
            FreeOrNull(titlePtr);
            FreeOrNull(instructionPtr);
            FreeOrNull(contentPtr);
        }
    }

    private static nint MapIcon(TaskDialogIcon icon) => icon switch
    {
        TaskDialogIcon.Information => TD_INFORMATION_ICON,
        TaskDialogIcon.Warning => TD_WARNING_ICON,
        TaskDialogIcon.Error => TD_ERROR_ICON,
        TaskDialogIcon.Shield => TD_SHIELD_ICON,
        _ => 0
    };

    private static TaskDialogChoice MapButton(int id) => id switch
    {
        1 => TaskDialogChoice.Ok,
        6 => TaskDialogChoice.Yes,
        7 => TaskDialogChoice.No,
        2 => TaskDialogChoice.Cancel,
        4 => TaskDialogChoice.Retry,
        8 => TaskDialogChoice.Close,
        _ => TaskDialogChoice.None
    };

    private static nint AllocOrNull(string? s) =>
        string.IsNullOrEmpty(s) ? 0 : Marshal.StringToCoTaskMemUni(s);

    private static void FreeOrNull(nint p)
    {
        if (p != 0)
            Marshal.FreeCoTaskMem(p);
    }
}
