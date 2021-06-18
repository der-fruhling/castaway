using System;
using System.Threading.Tasks;
using GLFW;

namespace Castaway.Rendering
{
    public sealed class Window : IDisposable, IAsyncDisposable
    {
        // ReSharper disable once InconsistentNaming
        public readonly Graphics GL;

        static Window()
        {
            Glfw.Init();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => Glfw.Terminate();
        }
        
        public Window(int width, int height, string title, bool fullscreen, bool visible, Graphics? api = null)
        {
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 2);
            Glfw.WindowHint(Hint.CocoaRetinaFrameBuffer, false);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Visible, visible);
            if (fullscreen)
            {
                var mon = Glfw.PrimaryMonitor;
                var vid = Glfw.GetVideoMode(mon);
                Native = Glfw.CreateWindow(vid.Width, vid.Height, title, mon, GLFW.Window.None);
            }
            else
            {
                Native = Glfw.CreateWindow(width, height, title, Monitor.None, GLFW.Window.None);
            }
            Bind();
            GL = api ?? ImplFinder.FindOptimalImplementation().Result!;
            GL.Window = this;
            GL.WindowInit(this);
            Glfw.SwapInterval(1);
        }

        public Window(int width, int height, string title, bool visible = true)
            : this(width, height, title, false, visible)
        {
        }

        public Window(string title, bool visible = true)
            : this(0, 0, title, true, visible)
        {
        }

        public GLFW.Window Native { get; }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(Dispose);
        }

        private void ReleaseUnmanagedResources()
        {
            Glfw.DestroyWindow(Native);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Window()
        {
            ReleaseUnmanagedResources();
        }

        internal void IBind()
        {
            Glfw.MakeContextCurrent(Native);
        }

        public void Bind() => Graphics.BindWindow(this);
        public void GetSize(out int x, out int y) => Glfw.GetWindowSize(Native, out x, out y);
        public void SetSize(int x, int y) => Glfw.SetWindowSize(Native, x, y);
        public void GetFramebufferSize(out int x, out int y) => Glfw.GetFramebufferSize(Native, out x, out y);

        public string Title
        {
            set => Glfw.SetWindowTitle(Native, value);
        }

        public bool ShouldClose
        {
            get => Glfw.WindowShouldClose(Native);
            set => Glfw.SetWindowShouldClose(Native, value);
        }

        public bool Visible
        {
            set
            {
                if (value) Glfw.ShowWindow(Native);
                else Glfw.HideWindow(Native);
            }
        }

        public void SwapBuffers()
        {
            Glfw.SwapBuffers(Native);
        }
    }
}