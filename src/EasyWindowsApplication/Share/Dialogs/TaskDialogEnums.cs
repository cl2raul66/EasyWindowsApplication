namespace EasyWindowsApplication.Share;

public enum TaskDialogIcon
{
    None,
    Information,
    Warning,
    Error,
    Shield
}

[Flags]
public enum TaskDialogChoices
{
    None = 0,
    Ok = 0x1,
    Yes = 0x2,
    No = 0x4,
    Cancel = 0x8,
    Retry = 0x10,
    Close = 0x20
}

public enum TaskDialogChoice
{
    None = 0,
    Ok = 1,
    Yes = 6,
    No = 7,
    Cancel = 2,
    Retry = 4,
    Close = 8
}