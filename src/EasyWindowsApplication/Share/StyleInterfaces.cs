namespace EasyWindowsApplication.Share;

public interface IControlStyle
{
    static abstract uint Value { get; }
}

public interface IButtonStyle : IControlStyle { }
public interface IEditStyle : IControlStyle { }
public interface IStaticStyle : IControlStyle { }
public interface IListBoxStyle : IControlStyle { }
public interface IComboBoxStyle : IControlStyle { }
public interface IScrollBarStyle : IControlStyle { }
public interface IProgressBarStyle : IControlStyle { }
public interface IDateTimePickerStyle : IControlStyle { }
public interface IMonthCalendarStyle : IControlStyle { }
public interface IHotKeyStyle : IControlStyle { }
public interface ITrackBarStyle : IControlStyle { }
public interface IUpDownStyle : IControlStyle { }
public interface IStatusBarStyle : IControlStyle { }
public interface IToolbarStyle : IControlStyle { }
public interface IToolTipStyle : IControlStyle { }
public interface IHeaderStyle : IControlStyle { }
public interface ITreeViewStyle : IControlStyle { }
public interface ITabControlStyle : IControlStyle { }
public interface IIpAddressStyle : IControlStyle { }
public interface IComboBoxExStyle : IControlStyle { }
public interface IReBarStyle : IControlStyle { }
public interface ILinkStyle : IControlStyle { }
public interface IListViewStyle : IControlStyle { }
