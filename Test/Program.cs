using System;
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
            g.BindUniform(program, "tex1");
            g.BindUniform(program, "tex2");
            g.BindUniform(program, "intensity");
            
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

            // Load the textures from the assets.
            var texture1 = g.CreateTexture(Loader.GetAssetByName("/cat1.jpg"));
            var texture2 = g.CreateTexture(Loader.GetAssetByName("/cat2.jpg"));

            // Create a new framebuffer.
            var framebuffer = g.CreateFramebuffer(window);

            // Show window.
            g.ShowWindow(window);
            
            // Rendering loop!
            var frames = 0;
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame();
                
                // Render base data to framebuffer.
                g.Bind(texture1, 0);
                g.Bind(texture2, 1);
                g.Bind(renderProgram, buffer, framebuffer);
                g.SetUniform(renderProgram, "tex1", 0);
                g.SetUniform(renderProgram, "tex2", 1);
                g.SetUniform(renderProgram, "intensity", (MathF.Sin(frames / 60f * MathF.PI) + 1f) / 2f);
                g.Draw(renderProgram, buffer, 6);
                g.UnbindFramebuffer();

                // Allow another shader to modify what actually rendered.
                g.Bind(framebuffer.Texture, 0);
                g.Bind(copyProgram, fulls);
                g.Draw(copyProgram, fulls, 6);

                g.FinishFrame(window);
                frames++;
            }

            g.Destroy(
                // Programs
                renderProgram, copyProgram,
                // Textures
                texture1, texture2,
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