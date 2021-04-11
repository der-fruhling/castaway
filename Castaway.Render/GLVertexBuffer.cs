using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Castaway.Native;

namespace Castaway.Render
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GLVertexBuffer : VertexBuffer
    {
        private readonly List<Vertex> _vertices = new List<Vertex>();
        
        public override Vertex[] Vertices => _vertices.ToArray();

        public void Draw()
        {
            GL.Begin(GL.TRIANGLES);
            foreach (var vertex in _vertices)
            {
                GL.Color(vertex.Color);
                GL.TexCoord(vertex.Texture);
                GL.Vertex(vertex.Position);
            }
            GL.End();
        }

        public override void Add(Vertex vertex) => _vertices.Add(vertex);
    }
}