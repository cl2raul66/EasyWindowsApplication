namespace EasyWindowsApplication.Share;

public interface IMenuItemSeparator : IViewSurface
{
    // Sin miembros propios en v1 (ver proposal-imenuitemseparator.md).
    // Name (heredado) sirve para orden + lookup bh.*.
    // OnClick/Clicked (heredados) son inertes: el OS no permite
    // seleccionar un separador (MF_SEPARATOR, sin ID ni comando).
}
