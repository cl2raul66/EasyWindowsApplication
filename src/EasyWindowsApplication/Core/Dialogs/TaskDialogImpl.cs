using EasyWindowsApplication.Share;

namespace EasyWindowsApplication.Core.Dialogs;

internal sealed class TaskDialogImpl : DialogSurfaceBase, ITaskDialog
{
    internal string? TextValue;                    // modo mínimo
    internal string? PrimaryTextValue;             // builder
    internal string? SecondaryTextValue;           // builder
    internal TaskDialogIcon NotificationIconValue = TaskDialogIcon.None;
    internal TaskDialogChoices ChoicesValue = TaskDialogChoices.Ok;
    internal TaskDialogChoice DefaultChoiceValue;

    ITaskDialog ITaskDialog.Name(string name)
    {
        Name = name;
        return this;
    }

    ITaskDialog ITaskDialog.Title(string title)
    {
        DialogTitle = title;
        return this;
    }

    public ITaskDialog Text(string text)
    {
        TextValue = text;
        return this;
    }

    public ITaskDialog Content(Action<ITaskDialogContentBuilder> configure)
    {
        var builder = new TaskDialogContentBuilder(this);
        configure(builder);
        return this;
    }

    public TaskDialogResult Show() => TaskDialogInterop.Show(
        OwnerProvider(),
        DialogTitle,
        TextValue,                 // si TextValue != null → modo mínimo
        PrimaryTextValue,
        SecondaryTextValue,
        NotificationIconValue,
        ChoicesValue,
        DefaultChoiceValue);
}

// Builder interno para TaskDialog
internal sealed class TaskDialogContentBuilder(TaskDialogImpl target) : ITaskDialogContentBuilder
{
    public ITaskDialogContentBuilder NotificationIcon(TaskDialogIcon icon)
    {
        target.NotificationIconValue = icon;
        return this;
    }

    public ITaskDialogContentBuilder PrimaryText(string text)
    {
        target.PrimaryTextValue = text;
        return this;
    }

    public ITaskDialogContentBuilder SecondaryText(string text)
    {
        target.SecondaryTextValue = text;
        return this;
    }

    public ITaskDialogContentBuilder Choices(TaskDialogChoices choices)
    {
        target.ChoicesValue = choices;
        return this;
    }

    public ITaskDialogContentBuilder DefaultChoice(TaskDialogChoice choice)
    {
        target.DefaultChoiceValue = choice;
        return this;
    }
}