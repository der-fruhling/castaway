using System.Collections.Generic;
using Castaway.Native;

namespace Castaway.Render
{
    public unsafe class VBO : VertexBuffer
    {
        private readonly List<Vertex> _vertices = new List<Vertex>();

        public override Vertex[] Vertices => _vertices.ToArray();
        
        public override void Add(Vertex vertex)
        {
            _vertices.Add(vertex);
        }

        public void Draw()
        {
            var vbo = new float[Vertex.VBOFloatCount * _vertices.Count];
            for (var i = 0; i < _vertices.Count; i++)
            {
                var j = i * Vertex.VBOFloatCount;
                var v = _vertices[i];
                vbo[j + 0] = v.Position.X;
                vbo[j + 1] = v.Position.Y;
                vbo[j + 2] = v.Position.Z;
                vbo[j + 3] = v.Normal.X;
                vbo[j + 4] = v.Normal.Y;
                vbo[j + 5] = v.Normal.Z;
                vbo[j + 6] = v.Texture.X;
                vbo[j + 7] = v.Texture.Y;
                vbo[j + 8] = v.Texture.Z;
                vbo[j + 9] = v.Color.R;
                vbo[j + 10] = v.Color.G;
                vbo[j + 11] = v.Color.B;
                vbo[j + 12] = v.Color.A;
            }
            
            uint buf;
            GL.GenBuffers(1, &buf);
            GL.BindBuffer(GL.ARRAY_BUFFER, buf);
            fixed (float* p = vbo)
                GL.BufferData(GL.ARRAY_BUFFER, (uint) (vbo.Length * sizeof(float)), p, GL.STATIC_DRAW);
        }
    }
}