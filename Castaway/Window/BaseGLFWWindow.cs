using System;
using Castaway.Rendering;
using GLFW;

namespace Castaway.Window
{
    public abstract class BaseGLFWWindow : IWindow
    {
        protected GLFW.Window Window;
        
        public (int Width, int Height) Size
        {
            get
            {
                Glfw.GetWindowSize(Window, out var width, out var height);
                return (width, height);
            }
            
            set
            {
                var (width, height) = value;
                Glfw.SetWindowSize(Window, width, height);
            }
        }

        public string Title
        {
            set => Glfw.SetWindowTitle(Window, value);
        }

        public bool Visible
        {
            get => Glfw.GetWindowAttribute(Window, WindowAttribute.Visible);
            set
            {
                if(value) Glfw.ShowWindow(Window);
                else Glfw.HideWindow(Window);
            }
        }

        public bool ShouldClose
        {
            get => Glfw.WindowShouldClose(Window);
            set => Glfw.SetWindowShouldClose(Window, value);
        }

        public event EventHandler<(int Width, int Height)> OnResize;
        public event EventHandler<(int X, int Y, MouseButton Button)> OnMousePress;
        public event EventHandler<(int X, int Y, MouseButton Button)> OnMouseRelease;
        public event EventHandler<Keys> OnKeyPress;
        public event EventHandler<Keys> OnKeyRelease;
        private bool _mouseInWindow;
        private (int X, int Y) _mousePos = (0, 0);

        private readonly KeyCallback _keyCallback;
        private readonly MouseCallback _mouseCallback;
        private readonly MouseButtonCallback _mouseButtonCallback;
        private readonly SizeCallback _sizeCallback;

        protected BaseGLFWWindow()
        {
            _keyCallback = KeyCallback;
            _mouseCallback = CursorPosCallback;
            _mouseButtonCallback = MouseCallback;
            _sizeCallback = SizeCallback;
        }

        public void Open(string name, int width, int height, bool resizeable)
        {
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.Focused, true);
            Glfw.WindowHint(Hint.Visible, false);
            Glfw.WindowHint(Hint.Resizable, resizeable);
            SetupWindowHints();
            Window = Glfw.CreateWindow(width, height, name, Monitor.None, GLFW.Window.None);
            if (Glfw.GetError(out var desc) != ErrorCode.None)
                throw new GraphicsException($"GLFW Error: {desc}");
            Glfw.SetKeyCallback(Window, _keyCallback);
            Glfw.SetMouseButtonCallback(Window, _mouseButtonCallback);
            Glfw.SetCursorPositionCallback(Window, _mouseCallback);
            Glfw.SetWindowSizeCallback(Window, _sizeCallback);
            if (Glfw.GetError(out desc) != ErrorCode.None)
                throw new GraphicsException($"GLFW Error: {desc}");
            Use();
            Visible = true;
        }

        private void SizeCallback(IntPtr _, int w, int h)
        {
            OnResize?.Invoke(this, (w, h));
        }

        protected virtual void CursorPosCallback(IntPtr window, double x, double y)
        {
            var (w, h) = Size;
            _mouseInWindow = x < 0 || x > w || y < 0 || y > h;
            _mousePos = ((int) x, (int) y);
        }

        protected virtual void MouseCallback(IntPtr window, MouseButton button, InputState state, ModifierKeys modifiers)
        {
            var a = (_mousePos.X, _mousePos.Y, button);
            switch (state)
            {
                case InputState.Press:
                    OnMousePress?.Invoke(this, a);
                    break;
                case InputState.Release:
                    OnMouseRelease?.Invoke(this, a);
                    break;
            }
        }

        protected virtual void KeyCallback(IntPtr window, Keys key, int code, InputState state, ModifierKeys mods)
        {
            switch (state)
            {
                case InputState.Press:
                    OnKeyPress?.Invoke(this, key);
                    break;
                case InputState.Release:
                    OnKeyRelease?.Invoke(this, key);
                    break;
            }
        }

        public void Close()
        {
            Glfw.DestroyWindow(Window);
        }

        public void FinishFrame()
        {
            Glfw.SwapBuffers(Window);
            Glfw.PollEvents();
        }

        public void Use() => UseContext();

        protected abstract void SetupWindowHints();
        protected abstract void UseContext();
    }
}