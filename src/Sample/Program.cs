using EasyWindowsApplication;
using EasyWindowsApplication.Share;
using EasyWindowsApplication.Share.Input;
using EasyWindowsApplication.Win32ControlsModule.Frontend;

//WindowsApplication.Layout(ly => ly.Window()).Initialize();

WindowsApplication
    .Resources(rd => rd.Setting(st =>
    {
        st.UseWinApi();
    }))
    .Layout(ly =>
    {
        ly.Window(iw => iw
            .SystemTray(st =>
            {
                st.Tooltip("Click here for show main window");
            })
            .Name("MainWindow")
            .Title("Easy Win App")
            .Dimensions(420, 280)
            .Position(WindowPositionOnScreen.Center)
            .Content(c => c
                .Children(ch =>
                {
                    ch.View<ILabel>(lb => lb.Name("LbResult").Text("No hay resultados."));
                    ch.View<IButton>(btn => btn
                        .Name("BtnIncrement")
                        .Text("Click me")
                    );
                })
            )
        );
        ly.AlternativeWindow<IOpenFileDialog>(ofd => ofd
            .Name("MyOpenDialog")
            .Title("Abrir captura")
            .Filters(f => f.Children(ch =>
            {
                ch.FileFilter("Imágenes", PngFile, JpgFile);
                ch.FileFilter("Todas las imágenes", AllImageFile);
            }))
            .Content(c =>
            {
                c.DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                c.MultiSelect(true);
            }));
        ly.AlternativeWindow<ISaveFileDialog>(sfd => sfd
            .Name("MySaveDialog")
            .Title("Guardar captura")
            .Filters(f => f.Children(ch =>
            {
                ch.FileFilter(PngFile);
                ch.FileFilter("Todas los ficheros", AllFile);
            }))
            .Content(c =>
            {
                c.DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                c.DefaultFileName("captura");
                c.DefaultExtension(PngFile);
            }));
        ly.AlternativeWindow<ISelectFolderDialog>(sfd => sfd
            .Name("MyFolderDialog")
            .Title("Seleccionar carpeta")
            .Content(c =>
            {
                c.DefaultDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                c.PersistLastDirectory(false);
            }));
        ly.AlternativeWindow<ITaskDialog>(td => td
            .Name("ConfirmDelete")
            .Title("Confirmación")
            .Content(c =>
            {
                c.NotificationIcon(TaskDialogIcon.Warning);
                c.PrimaryText("¿Eliminar el elemento?");
                c.SecondaryText("Esta acción no se puede deshacer.");
                c.Choices(TaskDialogChoices.Yes | TaskDialogChoices.No);
                c.DefaultChoice(TaskDialogChoice.No);
            }));
        // Variante mínima solo Name/Title (path de defaults).
        // Nota: el parámetro lleva tipo explícito porque un lambda solo con
        // Name/Title sería convertible tanto a Action<IWindowConfig> como a
        // Action<TDialog> (CS0121, llamada ambigua).
        ly.AlternativeWindow<IOpenFileDialog>((IOpenFileDialog ofd) => ofd
            .Name("MinimalOpenDialog")
            .Title("Abrir")
            .Content(c => c.MultiSelect(false)));
    })
    .Behavior(bh =>
    {        
        bh.BtnIncrement.OnInputWithSpatialPosition<MainTap, OneTap>(() =>
        {
            var confirm = bh.ConfirmDelete.Show();
            if (!confirm.IsCanceled) { bh.LbResult.Text = $"TaskDialog: {confirm.Choice}"; }

            var folder = bh.MyFolderDialog.Show();
            if (!folder.IsCanceled) { bh.LbResult.Text = $"Carpeta: {folder.FolderPath}"; }

            var save = bh.MySaveDialog.Show();
            if (!save.IsCanceled) { bh.LbResult.Text = $"Guardar: {save.FilePath}"; }

            var result = bh.MyOpenDialog.Show();
            if (!result.IsCanceled) { bh.LbResult.Text = $"Aberturas: {result.FilePaths.Count}"; }

            var minimal = bh.MinimalOpenDialog.Show();
            if (!minimal.IsCanceled) { bh.LbResult.Text = $"Minimal: {minimal.FilePaths.Count}"; }
        });
    })
    .Initialize();