using EasyWindowsApplication;
using EasyWinApp;

WindowsApplication
    .Resources(ResourcesConfig.ConfigureResources)
    .Layout(LayoutConfig.ConfigureLayout)
    .Behavior(BehaviorConfig.ConfigureBehavior)
    .Initialize();
