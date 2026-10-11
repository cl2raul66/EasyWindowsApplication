namespace EasyWindowsApplication.Share;

/// <summary>
/// Patrón tipado composite: descripción + array interno de filtros.
/// El preset compuesto ES un único <see cref="FilePattern"/>.
/// </summary>
public readonly record struct FilePattern(string Description, string[] Filters)
{
    internal string Win32Patterns => string.Join(';', Filters);

    /// <summary>Primera extensión del primer filtro (sin asterisco ni punto), p.ej. "png".</summary>
    public string DefaultExtension =>
        Filters.Length > 0 && Filters[0].Length > 0
            ? Filters[0].TrimStart('*', '.').Split(';')[0]
            : string.Empty;
}
