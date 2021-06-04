using System;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public struct Framebuffer : IOpenGLObject
    {
        public IGraphicsObject.ObjectType Type => IGraphicsObject.ObjectType.Framebuffer;
        public bool Destroyed { get; set; }
        public uint Number { get; set; }

        public Texture Texture;

        public bool Equals(Framebuffer other)
        {
            return Texture == other.Texture && Destroyed == other.Destroyed && Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Framebuffer other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Texture, Destroyed, Number);
        }

        public static bool operator ==(Framebuffer left, Framebuffer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Framebuffer left, Framebuffer right)
        {
            return !left.Equals(right);
        }
    }
}