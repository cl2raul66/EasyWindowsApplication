using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

WindowsApplication
    .Resources(rd => rd.Setting(st =>
    {
        st.UseWinApi();
    }))
    .Layout(static ly =>
    {
        ly.Window(iw => iw
            .Name("MainWindow")
            .Title("EWA - AlternativeWindow Galery")
            .Dimensions(420, 406)
            .Content(c => c
                .Spacing(8)
                .Padding(8)
                .Children(ch =>
                {
                    ch.View<ILabel>(lb => lb.Name("LbResult").Text("Press a button to test an AlternativeWindow."));
                    ch.View<IButton>(b => b.Name("BtnAltWindow").Text("Secondary window(HWND)"));
                    ch.View<IButton>(b => b.Name("BtnMenu").Text("Context menu (IMenu)"));
                    ch.View<IButton>(b => b.Name("BtnTaskDialog").Text("TaskDialog — .Content() mode"));
                    ch.View<IButton>(b => b.Name("BtnTaskDialogMin").Text("TaskDialog — .Text() mode"));
                    ch.View<IButton>(b => b.Name("BtnOpenFile").Text("OpenFileDialog"));
                    ch.View<IButton>(b => b.Name("BtnSaveFile").Text("SaveFileDialog"));
                    ch.View<IButton>(b => b.Name("BtnSelectFolder").Text("SelectFolderDialog"));
                })
            )
        );
        ly.AlternativeWindow(aw => aw
            .Name("OtherWindow")
            .Title("Secondary window (HWND)")
            .Dimensions(340, 190)
            .Position(WindowPositionOnScreen.Center)
            .Background(Color.LightSkyBlue)
            .Content(c => c.Children(ch =>
            {
                ch.View<ILabel>(lb => lb.Text("Close it and show it again: it recreates itself."));
                ch.View<IButton>(b => b.Name("BtnOtherHide").Text("Hide"));
                ch.View<IButton>(b => b.Name("BtnOtherClose").Text("Close"));
            }))
        );
        ly.AlternativeWindow<IMenu>(m => m
            .Name("GalleryMenu")
            .Content(c => c.Children(ch =>
            {
                ch.View<IMenuItem>(mi => mi.Name("MiHello").Text("To greet"));
                ch.View<IMenuItemSeparator>();
                ch.View<IMenuItem>(mi => mi.Name("MiExit").Text("Exit"));
            }))
        );
        ly.AlternativeWindow<ITaskDialog>(td => td
            .Name("TaskDialog")
            .Title("Complete TaskDialog")
            .Content(c =>
            {
                c.NotificationIcon(TaskDialogIcon.Information);
                c.PrimaryText("Builder mode .Content()");
                c.SecondaryText("Choices Yes/No, Default choice No.");
                c.Choices(TaskDialogChoices.Yes | TaskDialogChoices.No);
                c.DefaultChoice(TaskDialogChoice.No);
            })
        );
        ly.AlternativeWindow<ITaskDialog>(td => td
            .Name("TaskDialogMin")
            .Title("Minimum TaskDialog")
            .Text("Minimal mode .Text() — defaults: Ok, no icon.")
        );
        ly.AlternativeWindow<IOpenFileDialog>(ofd => ofd
            .Name("OpenFileDialog")
            .Title("Open image")
            .Filters(f => f.Children(fc => fc
                .FileFilter(PngFile)
                .FileFilter(JpgFile)
                .FileFilter(AllFile)))
            .Content(c => c
                .DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
                .MultiSelect(true))
        );
        ly.AlternativeWindow<ISaveFileDialog>(sfd => sfd
            .Name("SaveFileDialog")
            .Title("Save text")
            .Filters(f => f.Children(fc => fc.FileFilter(TxtFile)))
            .Content(c => c
                .DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                .DefaultFileName("exit")
                .DefaultExtension(TxtFile))
        );
        ly.AlternativeWindow<ISelectFolderDialog>(sfd => sfd
            .Name("SelectFolderDialog")
            .Title("Select folder")
            .Content(c => c
                .DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                .PersistLastDirectory(false))
        );
    })
.Behavior(bh =>
{
    bh.BtnAltWindow.OnInputWithSpatialPosition(() => bh.OtherWindow.Show());
    bh.BtnMenu.OnInputWithSpatialPosition(() => bh.GalleryMenu.ShowAtCursor());

    bh.BtnTaskDialog.OnInputWithSpatialPosition(() =>
    {
        var r = bh.TaskDialog.Show();
        bh.LbResult.Text = r.IsCanceled ? "TaskDialog: cancelado." : $"TaskDialog: {r.Choice}.";
    });
    bh.BtnTaskDialogMin.OnInputWithSpatialPosition(() =>
    {
        var r = bh.TaskDialogMin.Show();
        bh.LbResult.Text = r.IsCanceled ? "TaskDialog mín: cancelado." : $"TaskDialog mín: {r.Choice}.";
    });
    bh.BtnOpenFile.OnInputWithSpatialPosition(() =>
    {
        var r = bh.OpenFileDialog.Show();
        bh.LbResult.Text = r.IsCanceled ? "OpenFile: cancelado."
            : $"OpenFile: {r.FilePaths.Count} archivo(s) — {string.Join(", ", r.FilePaths)}";
    });
    bh.BtnSaveFile.OnInputWithSpatialPosition(() =>
    {
        var r = bh.SaveFileDialog.Show();
        bh.LbResult.Text = r.IsCanceled ? "SaveFile: cancelado." : $"SaveFile: {r.FilePath}";
    });
    bh.BtnSelectFolder.OnInputWithSpatialPosition(() =>
    {
        var r = bh.SelectFolderDialog.Show();
        bh.LbResult.Text = r.IsCanceled ? "SelectFolder: cancelado." : $"SelectFolder: {r.FolderPath}";
    });
    bh.OtherWindow.OnLoaded(() => bh.LbResult.Text = "OtherWindow: Loaded (ciclo fresco).");
    bh.OtherWindow.OnClosing(e =>
    {
        bh.LbResult.Text = "OtherWindow: Closing...";
        System.Threading.Thread.Sleep(2000);
    }); // e.Cancel available
    bh.OtherWindow.OnClosed(() => bh.LbResult.Text = "OtherWindow: Closed — Show() la re-crea.");
    bh.BtnOtherHide.OnInputWithSpatialPosition(() => bh.OtherWindow.Visibility(false));
    bh.BtnOtherClose.OnInputWithSpatialPosition(() => bh.OtherWindow.Close());
    bh.MiHello.OnInputWithSpatialPosition(() => bh.LbResult.Text = "Menú: ¡Hola!");
    bh.MiExit.OnInputWithSpatialPosition(() => bh.WindowsApplication.Terminate());
    bh.WindowsApplication.OnTerminated(() => System.Diagnostics.Trace.WriteLine("[EWA] Terminated"));
})
.Initialize();
