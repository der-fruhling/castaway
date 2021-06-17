using System.Threading.Tasks;
using Castaway;
using Castaway.Assets;
using Castaway.Level;
using Castaway.Rendering;

namespace Test
{
    internal static class Program
    {
        private static async Task Main()
        {
            // Perform global initialization
            CastawayEngine.Init();

            await using var window = new Window(800, 600, "name");
            window.Bind();

            var g = window.GL;
            g.ExpectedFrameTime = 1f / 144f;

            var level = new Level(AssetLoader.Loader!.GetAssetByName("/test_level.xml"));
            
            level.Start();
            while (!window.ShouldClose)
            {
                g.StartFrame();
                level.Update();
                level.Render();
                g.FinishFrame(window);
            }
        }
    }
}