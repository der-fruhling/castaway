using System.Threading.Tasks;
using Castaway;
using Castaway.Assets;
using Castaway.Level;
using Castaway.OpenGL.Input;
using GLFW;
using Window = Castaway.Rendering.Window;

namespace Test
{
    internal static class Program
    {
        private static async Task Main()
        {
            // Perform global initialization
            CastawayEngine.Init();

            await using var window = new Window("name", false);
            window.Bind();

            var g = window.GL;
            g.ExpectedFrameTime = 1f / 144f;

            var level = new Level(AssetLoader.Loader!.GetAssetByName("/test_level.xml"));

            level.Start();
            window.Visible = true;
            
            while (!window.ShouldClose)
            {
                g.StartFrame();
                level.Update();
                level.Render();
                g.FinishFrame(window);
                if (InputSystem.Gamepad.Start || InputSystem.Keyboard.IsDown(Keys.Escape))
                    window.ShouldClose = true;
            }

            window.Visible = false;
            level.End();
        }
    }
}