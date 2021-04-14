using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castaway.Native;

namespace Castaway.Render
{
    /// <summary>
    /// Old OpenGL Vertex Buffer. Prefer new <see cref="VBO"/>s.
    /// </summary>
    [Obsolete, SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GLVertexBuffer : VertexBuffer
    {
        private List<Vertex> _vertices = new List<Vertex>();
        
        public override Vertex[] Vertices
        {
            get => _vertices.ToArray();
            set => _vertices = value.ToList();
        }

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