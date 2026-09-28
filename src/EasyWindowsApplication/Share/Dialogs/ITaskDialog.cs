namespace EasyWindowsApplication.Share;

public interface ITaskDialog : ISystemDialog
{
    // Identidad / chrome
    new ITaskDialog Name(string name);
    new ITaskDialog Title(string title);

    // --- MÍNIMO ---
    ITaskDialog Text(string text);                              // cuerpo único, defaults: icono=None, Choices=Ok

    // --- COMPLETO (builder) ---
    ITaskDialog Content(Action<ITaskDialogContentBuilder> configure);
    
    TaskDialogResult Show();
}

// Builder interno del diálogo (recibido en Content)
public interface ITaskDialogContentBuilder
{
    ITaskDialogContentBuilder NotificationIcon(TaskDialogIcon icon);
    ITaskDialogContentBuilder PrimaryText(string text);
    ITaskDialogContentBuilder SecondaryText(string text);
    ITaskDialogContentBuilder Choices(TaskDialogChoices choices);
    ITaskDialogContentBuilder DefaultChoice(TaskDialogChoice choice);
}