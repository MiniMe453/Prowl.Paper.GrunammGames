using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace OpenTKSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings() {
                ClientSize = new Vector2i(1280, 720),
                Title = "Paper OpenTK Example",
                Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible
            };

            using (var app = new PaperTKWindow(GameWindowSettings.Default, nativeWindowSettings))
            {
                app.Run();
            }
        }
    }
}
