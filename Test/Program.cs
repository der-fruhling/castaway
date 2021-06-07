using Castaway;
using Castaway.Level;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering;
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

            var level = new Level(Loader!.GetAssetByName("/test_level.xml"));

            // Set up shader programs.
            g.Bind(BuiltinShaders.Default);
            g.SetUniform(BuiltinShaders.Default, UniformType.TransformPerspective, CameraMath.Persp(g, window, 100f, 0.01f, MathEx.ToRadians(90f)));

            // Construct a buffer that spans the entire area.
            var fulls = new Mesh(new Mesh.Vertex[]
            {
                new() {Position = new Vector3(-1, -1, 0), Texture = new Vector3(0, 0, 0), Color = new Vector4(1, 1, 1, 1)},
                new() {Position = new Vector3(-1, 1, 0), Texture = new Vector3(0, 1, 0), Color = new Vector4(1, 1, 1, 1)},
                new() {Position = new Vector3(1, -1, 0), Texture = new Vector3(1, 0, 0), Color = new Vector4(1, 1, 1, 1)},
                new() {Position = new Vector3(1, 1, 0), Texture = new Vector3(1, 1, 0), Color = new Vector4(1, 1, 1, 1)},
            }, new uint[] {0, 1, 2, 3, 1, 2});
            var fullsD = fulls.ConstructFor(g, BuiltinShaders.NoTransformTextured);

            // Create a new framebuffer.
            var framebuffer = g.CreateFramebuffer(window);

            // Start level.
            level.Start();
            
            // Show window.
            g.ShowWindow(window);
            
            // Rendering loop!
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame();

                // Render base data to framebuffer.
                g.Bind(framebuffer);
                g.Bind(BuiltinShaders.Default);
                level.Render();
                g.UnbindFramebuffer();

                // Use another shader to modify what actually rendered.
                g.Bind(framebuffer.Texture, 0);
                g.Bind(BuiltinShaders.NoTransformTextured);
                g.Draw(BuiltinShaders.NoTransformTextured, fullsD);

                g.FinishFrame(window);
                level.Update();
            }
            
            level.End();

            g.Destroy(framebuffer);
            g.Destroy(fullsD.ElementArray!, fullsD.VertexArray!); // TODO need better way
            g.Destroy(window); // Absolutely ensure that the window is
                               // destroyed last. If it isn't all destroy
                               // operations after it will fail.
        }
    }
}