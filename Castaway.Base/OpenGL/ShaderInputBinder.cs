using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Native.GL;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public class ShaderInputBinder
    {
        private (int Location, int Index, int Size, VertexInputType Type)[] _bindings;
        private int _stride;
        private uint _number;

        internal ShaderInputBinder(Shader program)
        {
            var bindings = new List<(int, int, int, VertexInputType)>();

            var i = 0;
            foreach (var (name, from) in program.GetInputs().Select(n => (n, program.GetInput(n))))
            {
                var loc = GL.GetAttribLocation(program.Number, name);
                var size = from switch
                {
                    VertexInputType.PositionXY => 2,
                    VertexInputType.PositionXYZ => 3,
                    VertexInputType.ColorG => 1,
                    VertexInputType.ColorRGB => 3,
                    VertexInputType.ColorRGBA => 4,
                    #pragma warning disable 618
                    VertexInputType.ColorBGRA => 4,
                    #pragma warning restore 618
                    VertexInputType.NormalXY => 2,
                    VertexInputType.NormalXYZ => 3,
                    VertexInputType.TextureS => 1,
                    VertexInputType.TextureST => 2,
                    VertexInputType.TextureSTV => 3,
                    _ => throw new ArgumentOutOfRangeException(nameof(program), from, null)
                };
                bindings.Add((loc, i, size, from));
                i += size;
            }

            _bindings = bindings.ToArray();
            _stride = i;
            _number = program.Number;
        }

        internal void Apply(Buffer buffer)
        {
            foreach (var (loc, index, size, _) in _bindings)
                GL.VertexAttribPointer(
                    loc, size, GLC.GL_FLOAT, false, _stride * sizeof(float), index * sizeof(float));
            foreach (var (loc, _, _, _) in _bindings) GL.EnableVertexAttrib(loc);
            buffer.SetupProgram = _number;
        }
    }
}