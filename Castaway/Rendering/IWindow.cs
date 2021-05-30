using System;
using GLFW;

namespace Castaway.Rendering
{
    public interface IWindow
    {
        (int Width, int Height) Size { get; set; }
        string Title { set; }
        bool Visible { get; set; }
        bool ShouldClose { get; set; }

        event EventHandler<(int Width, int Height)> OnResize;
        event EventHandler<(int X, int Y, MouseButton Button)> OnMousePress;
        event EventHandler<(int X, int Y, MouseButton Button)> OnMouseRelease;
        event EventHandler<Keys> OnKeyPress;
        event EventHandler<Keys> OnKeyRelease;

        void Close();
        void FinishFrame();
    }
}