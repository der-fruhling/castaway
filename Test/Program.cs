using Castaway;
using Castaway.Level;
using Castaway.OpenGL;
using Castaway.OpenGL.Input;
using GLFW;
using static Castaway.Assets.AssetLoader;

namespace Test
{
    internal static class Program
    {
        private static void Main()
        {
            // Perform global initialization
            CastawayEngine.Init();

            // Graphics setup (using OpenGL)
            using var g = OpenGL.Setup();
            
            // Window setup
            var window = g.CreateWindowWindowed("name", 800, 600, false);
            g.Bind(window);

            // Level setup
            var level = new Level(Loader!.GetAssetByName("/test_level.xml"));
            level.Start();
            
            // Rendering loop!
            g.ShowWindow(window);
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame();
                level.Render();
                g.FinishFrame(window);
                level.Update();
                if (InputSystem.Gamepad.Valid && InputSystem.Gamepad.Start || InputSystem.Keyboard.IsDown(Keys.Escape)) break;
            }
            g.HideWindow(window);
            
            // Destroy everything that needs destroying
            level.End();
            g.Destroy(window); // Absolutely ensure that the window is
                               // destroyed last. If it isn't all non-window
                               // destroy operations after it will fail.
        }
    }
}