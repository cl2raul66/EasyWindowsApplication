using EasyWindowsApplication;

namespace Sample;

internal class Program
{
    static void Main(string[] args)
    {
        WindowsApplication.Resources().Layout().Behavior().Initialize();
        WindowsApplication.Resources().Layout().Initialize();
        WindowsApplication.Layout().Initialize();

        //Console.WriteLine("Hello, World!");
    }
}
