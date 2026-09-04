namespace Fluid.Avalonia.Demo.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Hot reload needs no code here: the package's source generator wires itself up when the
    // opt-in FluidAvaloniaHotReload build is used (see the csproj).
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
