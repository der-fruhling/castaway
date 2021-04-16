#nullable enable
using System.Collections.Generic;
using Castaway.Math;
using Castaway.Mesh;

namespace Castaway.Render
{
    /// <summary>
    /// Converts mesh data into other structures.
    /// </summary>
    public static class MeshConverter
    {
        /// <summary>
        /// Converts between <see cref="CompleteVertex"/> and
        /// <see cref="VertexBuffer.Vertex"/>.
        /// </summary>
        public static VertexBuffer.Vertex[] Vertices(IMesh mesh, Vector4? color = null)
        {
            var vertices = new List<VertexBuffer.Vertex>();
            var c = mesh.Converter;
            
            while (c.More)
            {
                c.Next(out var p, out var t, out var n);
                vertices.Add(new VertexBuffer.Vertex(
                    p ?? Vector3.Zero, 
                    n ?? Vector3.Zero, 
                    t ?? Vector2.Zero,
                    color ?? new Vector4(1, 1, 1, 1)));
            }

            return vertices.ToArray();
        }
    }
}