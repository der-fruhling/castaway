using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castaway.Native;

namespace Castaway.Render
{
    /// <summary>
    /// Allows using OpenGL 2 VBOs. A valid shader must be set up before this
    /// class can be used.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public unsafe class VBO : VertexBuffer
    {
        private List<Vertex> _vertices = new List<Vertex>();
        private bool _locked = false;
        private uint _buf;

        public override Vertex[] Vertices
        {
            get => _vertices.ToArray();
            set => _vertices = value.ToList();
        }

        /// <inheritdoc cref="VertexBuffer.Add(Castaway.Render.VertexBuffer.Vertex)"/>
        /// <exception cref="ApplicationException">Thrown if this VBO is
        /// locked by <see cref="Lock"/>.</exception>
        public override void Add(Vertex vertex)
        {
            if (_locked) throw new ApplicationException("Cannot modify locked VBO.");
            _vertices.Add(vertex);
        }

        public void Lock() => _locked = true;

        public void Setup()
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
            
            fixed(uint* b = &_buf) GL.GenBuffers(1, b);
            GL.BindBuffer(GL.ARRAY_BUFFER, _buf);
            fixed (float* p = vbo)
                GL.BufferData(GL.ARRAY_BUFFER, (uint) (vbo.Length * sizeof(float)), p, GL.STATIC_DRAW);
            ShaderManager.SetupAttributes(ShaderManager.ActiveHandle);
            Lock();
        }

        /// <summary>
        /// Draws this VBO to the screen. Also locks it.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid
        /// <see cref="VertexAttribInfo.AttribValue"/> is encountered.
        /// </exception>
        public void Draw()
        {
            if(!_locked) Setup();
            GL.BindBuffer(GL.ARRAY_BUFFER, _buf);
            GL.DrawArrays(GL.TRIANGLES, 0, (uint) _vertices.Count);
        }
    }
}
