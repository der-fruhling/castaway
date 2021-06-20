using System;

namespace Castaway.Rendering
{
    public class Drawable : IDisposable
    {
        public int VertexCount;
        public BufferObject? VertexArray;
        public BufferObject? ElementArray;

        public Drawable(int vertexCount, BufferObject vertexArray)
        {
            VertexCount = vertexCount;
            VertexArray = vertexArray;
            ElementArray = null;
        }

        public Drawable(int vertexCount, BufferObject vertexArray, BufferObject elementArray)
        {
            VertexCount = vertexCount;
            VertexArray = vertexArray;
            ElementArray = elementArray;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            VertexArray?.Dispose();
            ElementArray?.Dispose();
        }
    }
}