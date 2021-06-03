using Castaway.Assets;
using Castaway.OpenGL;
using Castaway.Rendering;
using Graphics = Castaway.Rendering.Graphics;

namespace Test
{
    internal class Program
    {
        public static AssetLoader Loader = new();

        // Set to "blur/box" for a box blur effect
        private const string CopyShaderDir = "copy";

        private static ShaderProgram CreateRenderProgram(OpenGL g)
        {
            var vertexShader = g.CreateShader(ShaderStage.Vertex, Loader.GetAssetByName("/default/vertex.glsl"));
            var fragmentShader = g.CreateShader(ShaderStage.Fragment, Loader.GetAssetByName("/default/fragment.glsl"));
            var program = g.CreateProgram(vertexShader, fragmentShader);
            g.CreateInput(program, VertexInputType.PositionXYZ, "inPos");
            g.CreateInput(program, VertexInputType.ColorRGBA, "inCol");
            g.CreateInput(program, VertexInputType.TextureUV, "inTex");
            g.CreateOutput(program, 0, "outCol");
            g.FinishProgram(ref program);

            return program;
        }

        private static ShaderProgram CreateCopyProgram(OpenGL g)
        {
            var vertexShader =
                g.CreateShader(ShaderStage.Vertex, Loader.GetAssetByName($"/{CopyShaderDir}/vertex.glsl"));
            var fragmentShader = g.CreateShader(ShaderStage.Fragment,
                Loader.GetAssetByName($"/{CopyShaderDir}/fragment.glsl"));
            var program = g.CreateProgram(vertexShader, fragmentShader);
            g.CreateInput(program, VertexInputType.PositionXY, "inPos");
            g.CreateInput(program, VertexInputType.TextureUV, "inTex");
            g.CreateOutput(program, 0, "outCol");
            g.BindUniform(program, "tex");
            g.FinishProgram(ref program);

            return program;
        }

        private static void Main()
        {
            Loader.Discover("Assets");

            using var g = Graphics.Setup<OpenGL>();
            var window = g.CreateWindowWindowed("name", 800, 600);
            g.Bind(window);

            var renderProgram = CreateRenderProgram(g);
            var copyProgram = CreateCopyProgram(g);

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

            var texture = g.CreateTexture(Loader.GetAssetByName("/test.jpg"));

            var framebuffer = g.CreateFramebuffer(window);
            g.Bind(copyProgram);

            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame(window);

                g.Bind(renderProgram);
                g.Bind(framebuffer);
                g.Bind(texture);

                g.Bind(buffer);
                g.Draw(renderProgram, buffer, 6);

                g.UnbindFramebuffer();

                g.Bind(copyProgram);
                g.Bind(framebuffer.Texture);
                g.Bind(fulls);
                g.Draw(copyProgram, fulls, 6);

                g.FinishFrame(window);
            }

            g.Destroy(renderProgram, copyProgram);
            g.Destroy(texture);
            g.Destroy(framebuffer);
            g.Destroy(window);
        }
    }
}