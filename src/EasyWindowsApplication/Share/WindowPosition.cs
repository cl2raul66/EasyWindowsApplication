namespace EasyWindowsApplication.Share;

/// <summary>
/// Posición deseada de una ventana. Unifica el modelo: coordenada, preset de pantalla o snap a borde.
/// </summary>
public interface IWindowPosition { }

/// <summary>Posición exacta en coordenadas de pantalla.</summary>
public sealed record PositionAt(int X, int Y) : IWindowPosition;

/// <summary>Preset multi-monitor de la pantalla donde reside el handle.</summary>
public sealed record PositionPreset(WindowPositionPreset Preset) : IWindowPosition;

/// <summary>Snap a borde de pantalla (o trackbar) estilo Aero Snap.</summary>
public sealed record PositionSnap(SnapRegion Region) : IWindowPosition;
