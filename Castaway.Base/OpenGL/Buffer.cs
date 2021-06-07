using System;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public struct Buffer : IOpenGLObject
    {
        public IGraphicsObject.ObjectType Type => IGraphicsObject.ObjectType.Buffer;
        public bool Destroyed { get; set; }
        public uint Number { get; set; }

        public uint SetupProgram;
        public BufferTarget Target;

        public bool Equals(Buffer other)
        {
            return SetupProgram == other.SetupProgram && Target == other.Target && Destroyed == other.Destroyed &&
                   Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Buffer other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SetupProgram, (int) Target, Destroyed, Number);
        }

        public static bool operator ==(Buffer left, Buffer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Buffer left, Buffer right)
        {
            return !left.Equals(right);
        }
    }
}