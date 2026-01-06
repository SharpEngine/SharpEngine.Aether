using SharpEngine.AetherPhysics;
using SharpEngine.Core;
using SharpEngine.Core.Utils;

namespace Testing;

internal static class Program
{
    private static void Main()
    {
        SEAetherPhysics.AddVersions();

        var window = new Window(
            1280,
            920,
            "SE Aether Physics",
            Color.CornflowerBlue,
            null,
            true,
            true,
            true
        );

        window.AddScene(new MyScene());

        window.Run();
    }
}
