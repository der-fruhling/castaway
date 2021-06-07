using System;
using Castaway;
using Castaway.Assets;
using Castaway.Math;
using Castaway.Level;
using Castaway.OpenGL;
using Castaway.Rendering;
using static Castaway.Assets.AssetLoader;

namespace Test
{
    internal static class Program
    {
        // Set to:
        // "copy" for just a copy
        // "blur/box" for a box blur effect
        private const string CopyShaderDir = "copy";

        private static ShaderProgram CreateRenderProgram(OpenGL g)
        {
            // Create shaders.
            var vertexShader = g.CreateShader(ShaderStage.Vertex, Loader!.GetAssetByName("/default/vertex.glsl"));
            var fragmentShader = g.CreateShader(ShaderStage.Fragment, Loader.GetAssetByName("/default/fragment.glsl"));
            
            // Link them into a program.
            var program = g.CreateProgram(vertexShader, fragmentShader);
            
            // Inputs
            g.CreateInput(program, VertexInputType.PositionXYZ, "inPos");
            g.CreateInput(program, VertexInputType.ColorRGBA, "inCol");
            
            // Outputs
            g.CreateOutput(program, 0, "outCol");
            
            // Uniforms
            g.BindUniform(program, "tPersp", UniformType.TransformPerspective);
            g.BindUniform(program, "tView", UniformType.TransformView);
            g.BindUniform(program, "tModel", UniformType.TransformModel);
            
            // Done!
            g.FinishProgram(ref program);
            return program;
        }

        private static ShaderProgram CreateCopyProgram(OpenGL g)
        {
            // Create shaders.
            var vertexShader =
                g.CreateShader(ShaderStage.Vertex, Loader!.GetAssetByName($"/{CopyShaderDir}/vertex.glsl"));
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
            // Load assets from config.json
            CastawayEngine.Init();

            // Graphics setup.
            using var g = OpenGL.Setup();
            
            // Window setup
            var window = g.CreateWindowWindowed("name", 800, 600, false);
            g.Bind(window);

            var level = new Level(Loader!.GetAssetByName("/test_level.xml"));

            // Create shader programs.
            var renderProgram = CreateRenderProgram(g);
            g.SetUniform(renderProgram, UniformType.TransformPerspective, CameraMath.Persp(g, window, 100f, 0.01f, MathEx.ToRadians(90f)));
            g.SetUniform(renderProgram, UniformType.TransformView, Matrix4.Ident);
            g.SetUniform(renderProgram, UniformType.TransformModel, Matrix4.Ident);
            
            var copyProgram = CreateCopyProgram(g);

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

            // Create a new framebuffer.
            var framebuffer = g.CreateFramebuffer(window);

            // Start level.
            g.Bind(renderProgram);
            level.Start();
            
            // Show window.
            g.ShowWindow(window);
            
            // Rendering loop!
            var frames = 0;
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame();

                // Render base data to framebuffer.
                g.Bind(renderProgram, framebuffer);
                level.Render();
                g.UnbindFramebuffer();

                // Allow another shader to modify what actually rendered.
                g.Bind(framebuffer.Texture, 0);
                g.Bind(copyProgram, fulls);
                g.Draw(copyProgram, new BufferDrawable(fulls, 6));

                g.FinishFrame(window);
                frames++;
                level.Update();
            }
            
            level.End();

            g.Destroy(
                // Programs
                renderProgram, copyProgram,
                // Framebuffers
                framebuffer,
                // Buffers
                fulls
            );
            g.Destroy(window); // Absolutely ensure that the window is
                               // destroyed last. If it isn't all destroy
                               // operations after it will fail.
        }
    }
}