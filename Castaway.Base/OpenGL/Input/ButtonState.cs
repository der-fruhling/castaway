using System;

namespace Castaway.OpenGL.Input
{
    [Flags]
    public enum ButtonState
    {
        Down = 1 << 0,
        Up = 1 << 1,
        JustPressed = 1 << 2,
        JustReleased = 1 << 3,
        NeverPressed = 1 << 4
    }
}