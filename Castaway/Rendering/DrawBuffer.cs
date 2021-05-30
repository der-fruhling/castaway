namespace Castaway.Rendering
{
    public readonly struct DrawBuffer
    {
        public readonly IBuffer Buffer;
        public readonly (VertexInputType Type, int Index, int Size, int Location)[] Bindings;
        public readonly int VertexCount;

        public DrawBuffer(IBuffer buffer, (VertexInputType Type, int Index, int Size, int Location)[] bindings, int vertexCount)
        {
            Buffer = buffer;
            Bindings = bindings;
            VertexCount = vertexCount;
        }
    }
}