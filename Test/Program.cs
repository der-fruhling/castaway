using System;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering;

namespace Test
{
    internal static class Program
    {
        private static void Main()
        {
            Graphics.SetImpl<GLGraphics>();
            var g = Graphics.GetImpl();
            var w = g.CreateWindow("Test", 800, 600);

            var v = g.CreateShader(ShaderStage.Vertex, "#version 400 core\nin vec2 inPos;\nvoid main() { gl_Position = vec4(inPos, 0, 1); }");
            var f = g.CreateShader(ShaderStage.Fragment, "#version 400 core\nout vec4 outCol;\nuniform vec3 color;\nvoid main() { outCol = vec4(color, 1); }");
            var p = g.CreateProgram(v, f);
            g.CreateInput(p, VertexInputType.PositionXY, "inPos");
            g.CreateOutput(p, 0, "outCol");
            g.CreateUniform(p, "color", UniformType.Custom);
            g.FinishProgram(p);

            g.SetClearColor(0, 0, 0);

            var frame = 0;
            
            g.Use(p);
                
            var b = g.CreateBuffer(BufferTarget.VertexArray);
            var va = new VertexArray()
                .Position(0, 0).Next()
                .Position(0, 1).Next()
                .Position(1, 0).Next()
                .Position(0, 0).Next()
                .Position(0, -1).Next()
                .Position(1, 0).Next()
                .Position(0, 0).Next()
                .Position(0, -1).Next()
                .Position(-1, 0).Next()
                .Position(0, 0).Next()
                .Position(0, 1).Next()
                .Position(-1, 0).Next()
                .Position(1, 1).Next()
                .Position(0, 1).Next()
                .Position(1, 0).Next()
                .Position(1, -1).Next()
                .Position(0, -1).Next()
                .Position(1, 0).Next()
                .Position(-1, 1).Next()
                .Position(0, 1).Next()
                .Position(-1, 0).Next()
                .Position(-1, -1).Next()
                .Position(0, -1).Next()
                .Position(-1, 0).Next();
            b.Upload(va);
            var db = g.CreateDrawBuffer(b, 24);

            while (!w.ShouldClose)
            {
                g.StartFrame(w);
                g.Clear();

                var red = (-MathF.Sin(frame / 60f * MathF.PI) + 1) / 2;
                var green = (-MathF.Sin(frame / 60f * MathF.PI + MathF.PI / 2) + 1) / 2;
                var blue = (-MathF.Sin(frame / 60f * MathF.PI + MathF.PI) + 1) / 2;
                g.SetUniform("color", new Vector3(red, green, blue));
                g.Draw(db);
                
                g.FinishFrame(w);
                frame++;
            }
            
            // g.Destroy(p);
            g.Destroy(b);
            w.Close();
            g.Dispose();
        }
    }
}