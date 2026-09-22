namespace EasyWindowsApplication.Share;

public interface IMenuItemSeparator : IViewSurface
{
    // Sin miembros propios en v1 (ver proposal-imenuitemseparator.md).
    // Name (heredado) sirve para orden + lookup bh.*.
    // Inerte por naturaleza: el OS no permite seleccionar un separador
    // (MF_SEPARATOR, sin ID ni comando), por eso no expone IInputSurface.
}
