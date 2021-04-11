using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castaway.Native;

namespace Castaway.Render
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
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
            var size = ShaderManager.ActiveHandle.Attributes.Aggregate(0, (c, a) => c + a.Value switch
                {
                    VertexAttribInfo.AttribValue.Position => 3,
                    VertexAttribInfo.AttribValue.Color => 4,
                    VertexAttribInfo.AttribValue.Normal => 3,
                    VertexAttribInfo.AttribValue.Texture => 3,
                    _ => throw new ArgumentOutOfRangeException()
                });
            var vbo = new float[size * _vertices.Count];
            var j = 0;
            foreach (var v in _vertices)
            {
                foreach (var a in ShaderManager.ActiveHandle.Attributes)
                {
                    switch (a.Value)
                    {
                        case VertexAttribInfo.AttribValue.Position:
                            vbo[j++] = v.Position.X;
                            vbo[j++] = v.Position.Y;
                            vbo[j++] = v.Position.Z;
                            break;
                        case VertexAttribInfo.AttribValue.Color:
                            vbo[j++] = v.Color.R;
                            vbo[j++] = v.Color.G;
                            vbo[j++] = v.Color.B;
                            vbo[j++] = v.Color.A;
                            break;
                        case VertexAttribInfo.AttribValue.Normal:
                            vbo[j++] = v.Normal.X;
                            vbo[j++] = v.Normal.Y;
                            vbo[j++] = v.Normal.Z;
                            break;
                        case VertexAttribInfo.AttribValue.Texture:
                            vbo[j++] = v.Texture.X;
                            vbo[j++] = v.Texture.Y;
                            vbo[j++] = v.Texture.Z;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            uint buf;
            GL.GenBuffers(1, &buf);
            GL.BindBuffer(GL.ARRAY_BUFFER, buf);
            fixed (float* p = vbo)
                GL.BufferData(GL.ARRAY_BUFFER, (uint) (vbo.Length * sizeof(float)), p, GL.STATIC_DRAW);
            ShaderManager.SetupAttributes(ShaderManager.ActiveHandle);
            GL.DrawArrays(GL.TRIANGLES, 0, (uint) _vertices.Count);
        }
    }
}