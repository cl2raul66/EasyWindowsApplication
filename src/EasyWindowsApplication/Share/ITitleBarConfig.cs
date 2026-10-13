namespace EasyWindowsApplication.Share;

/// <summary>Configuración de title bar.</summary>
public interface ITitleBarConfig
{
    TitleBarMode Mode { get; }

    /// <summary>Icono (HICON) en el caption. Relevante solo en <see cref="TitleBarMode.Simple"/>.</summary>
    ITitleBarConfig Icon(nint hIcon);

    /// <summary>Título mostrado en caption (si difiere de <c>Title</c> de la ventana).</summary>
    ITitleBarConfig Title(string title);

    /// <summary>En <see cref="TitleBarMode.Custom"/>: contenido a materializar en el área de caption.</summary>
    ITitleBarConfig Custom<TView>(Action<TView> configure) where TView : class, IViewSurface;
}
