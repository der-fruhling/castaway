using Castaway.Assets;
using Castaway.OpenGL;
using Castaway.Rendering;
using Graphics = Castaway.Rendering.Graphics;

namespace Test
{
    internal static class Program
    {
        // Asset loader to load assets.
        private static readonly AssetLoader Loader = new();

        // Set to:
        // "copy" for just a copy
        // "blur/box" for a box blur effect
        private const string CopyShaderDir = "blur/box";

        private static ShaderProgram CreateRenderProgram(OpenGL g)
        {
            // Create shaders.
            var vertexShader = g.CreateShader(ShaderStage.Vertex, Loader.GetAssetByName("/default/vertex.glsl"));
            var fragmentShader = g.CreateShader(ShaderStage.Fragment, Loader.GetAssetByName("/default/fragment.glsl"));
            
            // Link them into a program.
            var program = g.CreateProgram(vertexShader, fragmentShader);
            
            // Inputs
            g.CreateInput(program, VertexInputType.PositionXYZ, "inPos");
            g.CreateInput(program, VertexInputType.ColorRGBA, "inCol");
            g.CreateInput(program, VertexInputType.TextureUV, "inTex");
            
            // Outputs
            g.CreateOutput(program, 0, "outCol");
            
            // Uniforms
            g.BindUniform(program, "tex");
            
            // Done!
            g.FinishProgram(ref program);
            return program;
        }

        private static ShaderProgram CreateCopyProgram(OpenGL g)
        {
            // Create shaders.
            var vertexShader =
                g.CreateShader(ShaderStage.Vertex, Loader.GetAssetByName($"/{CopyShaderDir}/vertex.glsl"));
            var fragmentShader = g.CreateShader(ShaderStage.Fragment,
                Loader.GetAssetByName($"/{CopyShaderDir}/fragment.glsl"));
            
            // Link them into a program.
            var program = g.CreateProgram(vertexShader, fragmentShader);
            
            // Inputs
            g.CreateInput(program, VertexInputType.PositionXY, "inPos");
            g.CreateInput(program, VertexInputType.TextureUV, "inTex");
            
            // Outputs
            g.CreateOutput(program, 0, "outCol");
            
            // Uniforms
            g.BindUniform(program, "tex");
            
            // Done!
            g.FinishProgram(ref program);
            return program;
        }

        private static void Main()
        {
            // Asset loader discovery.
            Loader.Discover("Assets");

            // Graphics setup.
            using var g = Graphics.Setup<OpenGL>();
            
            // Window setup
            var window = g.CreateWindowWindowed("name", 800, 600, false);
            g.Bind(window);

            // Create shader programs.
            var renderProgram = CreateRenderProgram(g);
            var copyProgram = CreateCopyProgram(g);

            // Construct a buffer that just spans the middle of the area.
            var buffer = g.CreateBuffer(BufferTarget.VertexArray);
            g.Upload(buffer, new float[]
            {
                -.75f, -.75f, 0, 1, 1, 1, 1, 0, 0,
                -.75f, .75f, 0, 1, 1, 1, 1, 0, 1,
                .75f, -.75f, 0, 1, 1, 1, 1, 1, 0,
                .75f, .75f, 0, 1, 1, 1, 1, 1, 1,
                -.75f, .75f, 0, 1, 1, 1, 1, 0, 1,
                .75f, -.75f, 0, 1, 1, 1, 1, 1, 0
            });

            // Construct a buffer that spans the entire area.
            var fulls = g.CreateBuffer(BufferTarget.VertexArray);
            g.Upload(fulls, new float[]
            {
                -1, -1, 0, 0,
                -1, 1, 0, 1,
                1, -1, 1, 0,
                1, 1, 1, 1,
                -1, 1, 0, 1,
                1, -1, 1, 0
            });

            // Load the texture from the assets.
            var texture = g.CreateTexture(Loader.GetAssetByName("/test.jpg"));

            // Create a new framebuffer.
            var framebuffer = g.CreateFramebuffer(window);

            // Show window.
            g.ShowWindow(window);
            
            // Rendering loop!
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame(window);
                
                // Render base data to framebuffer.
                g.Bind(renderProgram, texture, buffer, framebuffer);
                g.Draw(renderProgram, buffer, 6);
                g.UnbindFramebuffer();

                // Allow another shader to modify what actually rendered.
                g.Bind(copyProgram, framebuffer.Texture, fulls);
                g.Draw(copyProgram, fulls, 6);

                g.FinishFrame(window);
            }

            g.Destroy(
                // Programs
                renderProgram, copyProgram,
                // Textures
                texture,
                // Framebuffers
                framebuffer,
                // Buffers
                buffer, fulls
            );
            g.Destroy(window); // Absolutely ensure that the window is
                               // destroyed last. If it isn't all destroy
                               // operations after it will fail.
        }
    }
}