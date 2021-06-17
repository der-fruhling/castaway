using System;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public class VertexArrayDrawable : Drawable
    {
        internal uint VAO;
        internal bool SetUp;
        
        public VertexArrayDrawable(int vertexCount, BufferObject vertexArray) : base(vertexCount, vertexArray)
        {
            if (Graphics.Current is not OpenGL32 gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
            VAO = gl.CreateVAO();
            gl.BindVAO(VAO);
            VertexArray?.Bind();
            gl.UnbindVAO();
        }

        public VertexArrayDrawable(int vertexCount, BufferObject vertexArray, BufferObject elementArray) : base(vertexCount, vertexArray, elementArray)
        {
            if (Graphics.Current is not OpenGL32 gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
            VAO = gl.CreateVAO();
            gl.BindVAO(VAO);
            VertexArray?.Bind();
            ElementArray?.Bind();
            gl.UnbindVAO();
        }
    }
}