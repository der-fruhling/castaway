namespace Castaway.OpenGL
{
    public interface IDrawable
    {
        public Buffer? VertexArray { get; }
        public Buffer? ElementArray { get; }
        public int VertexCount { get; }
    }

    public readonly struct BufferDrawable : IDrawable
    {
        public Buffer? VertexArray { get; }
        public Buffer? ElementArray => null;
        public int VertexCount { get; }

        public BufferDrawable(Buffer vertexArray, int vertexCount)
        {
            VertexArray = vertexArray;
            VertexCount = vertexCount;
        }
    }
    
    public struct ElementDrawable : IDrawable
    {
        public Buffer? VertexArray { get; }
        public Buffer? ElementArray { get; }
        public int VertexCount { get; }

        public ElementDrawable(Buffer vertexArray, Buffer elementArray, int vertexCount)
        {
            VertexArray = vertexArray;
            ElementArray = elementArray;
            VertexCount = vertexCount;
        }
    }
}