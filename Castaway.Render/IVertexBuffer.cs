using Castaway.Math;

namespace Castaway.Render
{
    public abstract class VertexBuffer
    {
        public struct Vertex
        {
            public Vector3 Position, Normal, Texture;
            public Vector4 Color;
            
            public static uint VBOFloatCount => 3 * 3 + 4 * 1;
            public static uint VBOSize => sizeof(float) * VBOFloatCount;

            public Vertex(Vector3 position, Vector3 normal, Vector3 texture, Vector4 color) : this()
            {
                Position = position;
                Normal = normal;
                Texture = texture;
                Color = color;
            }
        }
        
        public abstract Vertex[] Vertices { get; }

        public abstract void Add(Vertex vertex);
        public void Add(Vector3 pos, Vector3 norm, Vector3 tex, Vector4 col)
            => Add(new Vertex(pos, norm, tex, col));

        public void Add(float x, float y, float z = 0, float nx = 0, float ny = 0, float nz = 0, float u = 0, float v = 0,
            float t = 0, float r = 1, float g = 1, float b = 1, float a = 1)
            => Add(new Vector3(x, y, z), new Vector3(nx, ny, nz), new Vector3(u, v, t),
                new Vector4(r, g, b, a));
    }
}