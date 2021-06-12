using Castaway;
using Castaway.Level;
using Castaway.OpenGL;
using static Castaway.Assets.AssetLoader;

namespace Test
{
    internal static class Program
    {
        private static void Main()
        {
            // Load assets from config.json
            CastawayEngine.Init();

            // Graphics setup.
            using var g = OpenGL.Setup();
            
            // Window setup
            var window = g.CreateWindowWindowed("name", 800, 600, false);
            g.Bind(window);

            // Level setup
            var level = new Level(Loader!.GetAssetByName("/test_level.xml"));

            // Start level.
            level.Start();
            
            // Show window.
            g.ShowWindow(window);
            
            // Rendering loop!
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame();
                level.Render();
                g.FinishFrame(window);
                level.Update();
            }
            
            level.End();

            g.Destroy(window); // Absolutely ensure that the window is
                               // destroyed last. If it isn't all destroy
                               // operations after it will fail.
        }
    }
}