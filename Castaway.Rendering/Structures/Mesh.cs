using System;
using Castaway.Math;

namespace Castaway.Rendering.Structures
{
    public struct Mesh
    {
        public struct Vertex
        {
            public Vector3 Position;
            public Vector4 Color;
            public Vector3 Normal;
            public Vector3 Texture;
            
            public Vertex(Vector3 position, Vector4? color = null, Vector3? texture = null, Vector3? normal = null)
            {
                Position = position;
                Color = color ?? new Vector4(1, 1, 1, 1);
                Normal = normal ?? new Vector3(0, 0, 0);
                Texture = texture ?? new Vector3(0, 0, 0);
            }

            public bool Equals(Vertex other)
            {
                return Position.Equals(other.Position) && Color.Equals(other.Color) && Normal.Equals(other.Normal) && Texture.Equals(other.Texture);
            }

            public override bool Equals(object? obj)
            {
                return obj is Vertex other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Position, Color, Normal, Texture);
            }

            public static bool operator ==(Vertex left, Vertex right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Vertex left, Vertex right)
            {
                return !left.Equals(right);
            }
        }

        public Vertex[] Vertices;
        public uint[] Elements;

        public Mesh(Vertex[] vertices, uint[] elements)
        {
            Vertices = vertices;
            Elements = elements;
        }
    }
}