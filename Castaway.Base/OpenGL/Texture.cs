using System;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public struct Texture : IOpenGLObject
    {
        public IGraphicsObject.ObjectType Type => IGraphicsObject.ObjectType.Texture;
        public bool Destroyed { get; set; }
        public uint Number { get; set; }
        
        public uint BindingPoint { get; internal set; }

        public static implicit operator uint(Texture t)
        {
            return t.Number;
        }

        public bool Equals(Texture other)
        {
            return Destroyed == other.Destroyed && Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Texture other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Destroyed, Number);
        }

        public static bool operator ==(Texture left, Texture right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Texture left, Texture right)
        {
            return !left.Equals(right);
        }
    }
}