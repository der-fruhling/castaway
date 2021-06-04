using System;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public struct Window : IOpenGLObject
    {
        internal GLFW.Window GlfwWindow;
        public IGraphicsObject.ObjectType Type => IGraphicsObject.ObjectType.Window;
        public bool Destroyed { get; set; }
        public uint Number { get; set; }

        public bool Equals(Window other)
        {
            return GlfwWindow.Equals(other.GlfwWindow) && Destroyed == other.Destroyed && Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Window other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GlfwWindow, Destroyed, Number);
        }

        public static bool operator ==(Window left, Window right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Window left, Window right)
        {
            return !left.Equals(right);
        }
    }
}