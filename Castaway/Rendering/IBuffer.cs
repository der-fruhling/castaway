using System;

namespace Castaway.Rendering
{
    public enum BufferTarget
    {
        VertexArray,
        ElementArray
    }
    
    public interface IBuffer
    {
        void Upload(Span<byte> data);
        void Upload(Span<float> data);
        
        bool IsValid { get; }
    }
}