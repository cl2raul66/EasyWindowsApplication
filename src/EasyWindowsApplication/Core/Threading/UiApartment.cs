using System.Threading;

namespace EasyWindowsApplication.Core.Threading;

// Fontanería interna de apartment COM. Los diálogos del shell
// (IFileOpenDialog / IFileSaveDialog / IFileDialog con FOS_PICKFOLDERS)
// requieren un hilo STA, y el hilo de entrada de .NET es MTA por defecto
// (Thread.ApartmentState: el main thread arranca en MTA). El apartment COM
// es inmutable, así que CoInitializeEx(STA) sobre MTA devolvería
// RPC_E_CHANGED_MODE; la única puerta es ejecutar el pipeline en un
// Thread STA propio ("EWA-UI").
//
// Esto es especialmente importante porque el framework no usa ni Task, ni
// Dispatcher, ni SynchronizationContext: no hay nada más que capture el
// hilo de UI, y todos los HWND se crean aquí mismo, así que diálogo y
// dueño viven siempre en el mismo hilo (cero cross-thread).
internal static class UiApartment
{
    // Si el llamante ya está en STA (p. ej. Program.Main con [STAThread]),
    // ejecuta el body en el mismo hilo y no se crea ningún hilo extra.
    // En caso contrario crea un Thread STA, ejecuta el body ahí y espera
    // con Join() hasta que el body (que contiene el message loop) termine.
    internal static void RunStaOrCurrent(Action body)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            body();
            return;
        }

        var ui = new Thread(new ThreadStart(body)) { Name = "EWA-UI", IsBackground = false };
        ui.SetApartmentState(ApartmentState.STA);
        ui.Start();
        ui.Join();
    }
}