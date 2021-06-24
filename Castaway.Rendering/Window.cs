using System;
using System.Threading.Tasks;
using Castaway.Base;
using GLFW;
using Serilog;

namespace Castaway.Rendering
{
    public sealed class Window : IDisposable, IAsyncDisposable, IEquatable<Window>
    {
        public delegate void ResizeEventHandler(int newWidth, int newHeight);
        
        // ReSharper disable once InconsistentNaming
        public readonly Graphics GL;
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();

        public event ResizeEventHandler WindowResize = delegate {  };
        private SizeCallback _sizeCallback;

        static Window()
        {
            Glfw.Init();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => Glfw.Terminate();
        }
        
        public Window(int width, int height, string title, bool fullscreen, bool visible, Graphics? api = null)
        {
            Logger.Debug("Starting window creation");
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.CocoaRetinaFrameBuffer, true);
            Logger.Debug("Window Hint {Hint} = {Value}", Hint.CocoaRetinaFrameBuffer, true);
            Glfw.WindowHint(Hint.Visible, visible);
            Logger.Debug("Window Hint {Hint} = {Value}", Hint.Visible, visible);
            Logger.Debug("Window Size = {Width}x{Height}", width, height);
            Native = Glfw.CreateWindow(width, height, title, fullscreen ? Glfw.PrimaryMonitor : Monitor.None, GLFW.Window.None);
            Bind();
            Logger.Debug("Window created successfully");
            api ??= ImplFinder.FindOptimalImplementation(this).Result;
            if (api == null)
            {
                api = ImplFinder.Find("OpenGL-3.2").Result;
                Logger.Warning("API from FindOptimalImplementation was null; using {ApiType} instead", api!.GetType());
            }
            GL = api;
            Logger.Debug("Applied API {ApiType} to window", GL.GetType());
            GL.Window = this;
            GL.WindowInit(this);
            Glfw.SwapInterval(1);
            _title = title;
            _visible = visible;
            VSync = true;
            Glfw.SetWindowSizeCallback(Native, _sizeCallback = (_, w, h) =>
            {
                WindowResize(w, h);
                Logger.Verbose("Resized {Window} to {Width}x{Height}", Title, w, h);
            });
            _fullscreen = fullscreen;
            Logger.Debug("Finished setting up window");
            GetSize(out var w, out var h);
            Logger.Information("Created window {Window} with size {Width}x{Height}", Title, w, h);
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

        public void Dispose()
        {
            Logger.Information("Destroyed window {Window}", Title);
            Glfw.DestroyWindow(Native);
            GL.Dispose();
        }

        internal void IBind()
        {
            Glfw.MakeContextCurrent(Native);
        }

        public void Bind() => Graphics.BindWindow(this);
        public void GetSize(out int x, out int y) => Glfw.GetWindowSize(Native, out x, out y);
        public void SetSize(int x, int y) => Glfw.SetWindowSize(Native, x, y);
        public void GetFramebufferSize(out int x, out int y) => Glfw.GetFramebufferSize(Native, out x, out y);

        private string _title;
        private bool _visible, _vsync, _fullscreen;
        
        public string Title
        {
            get => _title;
            set => Glfw.SetWindowTitle(Native, _title = value);
        }

        public bool ShouldClose
        {
            get => Glfw.WindowShouldClose(Native);
            set => Glfw.SetWindowShouldClose(Native, value);
        }

        public bool Visible
        {
            get => _visible;
            set
            {
                // ReSharper disable once AssignmentInConditionalExpression
                if (_visible = value) Glfw.ShowWindow(Native);
                else Glfw.HideWindow(Native);
            }
        }

        public bool VSync
        {
            get => _vsync;
            set
            {
                _vsync = value;
                Glfw.SwapInterval(value ? 1 : 0);
                Logger.Debug("VSync = {Value}", value);
            }
        }

        public void SwapBuffers()
        {
            Glfw.SwapBuffers(Native);
        }

        public bool Equals(Window? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _title == other._title && _visible == other._visible && _vsync == other._vsync && Native.Equals(other.Native);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Window other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_title, _visible, _vsync, Native);
        }

        public static bool operator ==(Window? left, Window? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Window? left, Window? right)
        {
            return !Equals(left, right);
        }

        public bool Fullscreen
        {
            get => _fullscreen;
            set
            {
                var mon = Glfw.PrimaryMonitor;
                var vid = Glfw.GetVideoMode(mon);
                int x, y, w, h;
                if (value)
                {
                    w = vid.Width;
                    h = vid.Height;
                    x = 0;
                    y = 0;
                }
                else
                {
                    Glfw.GetWindowSize(Native, out w, out h);
                    x = vid.Width / 2 - w;
                    y = vid.Height / 2 - w;
                }

                Glfw.SetWindowMonitor(Native, value ? mon : Monitor.None, x, y, w, h, vid.RefreshRate);
                _fullscreen = value;
            }
        }
    }
}