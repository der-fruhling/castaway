using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
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
        private bool _setup = false;
        private uint _buf = uint.MaxValue;

        public override Vertex[] Vertices
        {
            get => _vertices.ToArray();
            set => _vertices = value.ToList();
        }

        /// <inheritdoc cref="VertexBuffer.Add(Castaway.Render.VertexBuffer.Vertex)"/>
        public override void Add(Vertex vertex)
        {
            _vertices.Add(vertex);
            _setup = false;
        }

        public void Setup()
        {
            if (ShaderManager.ActiveHandle.Equals(null!))
                throw new ApplicationException("Shader is not set up. VBOs require shaders.");
            var size = ShaderManager.ActiveHandle.Attributes.Aggregate(0, (c, a) => c + a.Value switch
                {
                    VertexAttribInfo.AttribValue.Position => 3,
                    VertexAttribInfo.AttribValue.Color => 4,
                    VertexAttribInfo.AttribValue.Normal => 3,
                    VertexAttribInfo.AttribValue.Texture => 2,
                    _ => throw new ArgumentOutOfRangeException()
                });
            var vboSize = size * _vertices.Count;
            var vbo = (float*)Marshal.AllocHGlobal(vboSize * sizeof(float)).ToPointer();
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
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            if(_buf == uint.MaxValue)
                fixed (uint* b = &_buf) GL.GenBuffers(1, b);
            GL.BindBuffer(GL.ARRAY_BUFFER, _buf);
            GL.BufferData(GL.ARRAY_BUFFER, (uint) (vboSize * sizeof(float)), vbo, GL.STATIC_DRAW);
            Marshal.FreeHGlobal(new IntPtr(vbo));
            ShaderManager.SetupAttributes(ShaderManager.ActiveHandle);
            _setup = true;
        }

        public void Reset()
        {
            _vertices.Clear();
            _setup = false;
        }

        /// <summary>
        /// Draws this VBO to the screen. Also locks it.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid
        /// <see cref="VertexAttribInfo.AttribValue"/> is encountered.
        /// </exception>
        public void Draw()
        {
            if (ShaderManager.ActiveHandle.Equals(null!))
                throw new ApplicationException("Shader is not set up. VBOs require shaders.");
            if(!_setup) Setup();
            GL.BindBuffer(GL.ARRAY_BUFFER, _buf);
            ShaderManager.SetupAttributes(ShaderManager.ActiveHandle);
            GL.DrawArrays(GL.TRIANGLES, 0, (uint) _vertices.Count);
        }
    }
}