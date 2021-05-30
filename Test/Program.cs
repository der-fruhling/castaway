using System;
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
            var f = g.CreateShader(ShaderStage.Fragment, "#version 400 core\nout vec4 outCol;\nvoid main() { outCol = vec4(1, 1, 1, 1); }");
            var p = g.CreateProgram(v, f);
            g.CreateInput(p, VertexInputType.PositionXY, "inPos");
            g.CreateOutput(p, 0, "outCol");
            g.FinishProgram(p);

            g.SetClearColor(0, 0, 0);

            while (!w.ShouldClose)
            {
                g.StartFrame(w);
                g.Clear();
                g.Use(p);
                
                var b = g.CreateBuffer(BufferTarget.VertexArray);
                var va = new VertexArray()
                    .Position(0, 0).Next()
                    .Position(0, 1).Next()
                    .Position(1, 0).Next();
                b.Upload(va);
                var db = g.CreateDrawBuffer(b, 3);
                
                g.Draw(db);
                
                g.Destroy(b);
                g.FinishFrame(w);
            }
            
            w.Close();
            g.Dispose();
        }
    }
}