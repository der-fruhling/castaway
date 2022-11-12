using System;
using System.Threading.Tasks;
using Castaway.Base;
using Castaway.Math;
using GLFW;
using Serilog;

namespace Castaway.Rendering;

public sealed class Window : IDisposable, IAsyncDisposable, IEquatable<Window>
{
    public delegate void ResizeEventHandler(int newWidth, int newHeight);

    private static readonly ILogger Logger = CastawayGlobal.GetLogger();

    // ReSharper disable once InconsistentNaming
    public readonly Graphics GL;
    private Vector2? _sizeBeforeFullscreen, _positionBeforeFullscreen;
    private SizeCallback _sizeCallback;

    private string _title;
    private bool _visible, _vsync;

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
        Glfw.WindowHint(Hint.ContextVersionMajor, 3);
        Glfw.WindowHint(Hint.ContextVersionMinor, 2);
        Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
        Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
        Logger.Debug("Window Hint {Hint} = {Value}", Hint.CocoaRetinaFrameBuffer, true);
        Glfw.WindowHint(Hint.Visible, visible);
        Logger.Debug("Window Hint {Hint} = {Value}", Hint.Visible, visible);
        Logger.Debug("Window Size = {Width}x{Height}", width, height);
        Native = Glfw.CreateWindow(width, height, title, fullscreen ? Glfw.PrimaryMonitor : Monitor.None,
            GLFW.Window.None);
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
        _title = title;
        _visible = visible;
        VSync = false;
        Glfw.SetWindowSizeCallback(Native, _sizeCallback = (_, w, h) =>
        {
            WindowResize(w, h);
            Logger.Verbose("Resized {Window} to {Width}x{Height}", Title, w, h);
        });
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

    public string Title
    {
        get => _title;
        set
        {
            Glfw.SetWindowTitle(Native, _title = value);
            Logger.Debug("Successfully changed monitor state ({Name}={Value})", nameof(Title), Title);
        }
    }

    public bool ShouldClose
    {
        get => Glfw.WindowShouldClose(Native);
        set
        {
            Glfw.SetWindowShouldClose(Native, value);
            Logger.Debug("Successfully changed monitor state ({Name}={Value})", nameof(ShouldClose), ShouldClose);
        }
    }

    public bool Visible
    {
        get => _visible;
        set
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (_visible = value) Glfw.ShowWindow(Native);
            else Glfw.HideWindow(Native);
            Logger.Debug("Successfully changed monitor state ({Name}={Value})", nameof(Visible), Visible);
        }
    }

    public bool VSync
    {
        get => _vsync;
        set
        {
            _vsync = value;
            Glfw.SwapInterval(value ? 1 : 0);
            Logger.Debug("Successfully changed monitor state ({Name}={Value})", nameof(VSync), VSync);
        }
    }

    public bool Fullscreen
    {
        get => Glfw.GetWindowMonitor(Native) != Monitor.None;
        set
        {
            Logger.Information("{EnterOrExit} fullscreen...", value ? "Entering" : "Exiting");
            var mon = Glfw.PrimaryMonitor;
            var vid = Glfw.GetVideoMode(mon);
            int x, y, w, h;
            GetSize(out var ow, out var oh);
            Glfw.GetWindowPosition(Native, out var ox, out var oy);
            if (value)
            {
                _sizeBeforeFullscreen = new Vector2(ow, oh);
                _positionBeforeFullscreen = new Vector2(ox, oy);
                w = vid.Width;
                h = vid.Height;
                x = 0;
                y = 0;
            }
            else
            {
                if (_sizeBeforeFullscreen.HasValue)
                {
                    w = (int) _sizeBeforeFullscreen.Value.X;
                    h = (int) _sizeBeforeFullscreen.Value.Y;
                }
                else
                {
                    w = ow;
                    h = oh;
                }

                if (_positionBeforeFullscreen.HasValue)
                {
                    x = (int) _positionBeforeFullscreen.Value.X;
                    y = (int) _positionBeforeFullscreen.Value.Y;
                }
                else
                {
                    x = ox;
                    y = oy;
                }

                _positionBeforeFullscreen = null;
                _sizeBeforeFullscreen = null;
            }

            if (value) Glfw.SetWindowMonitor(Native, mon, 0, 0, vid.Width, vid.Height, vid.RefreshRate);
            else Glfw.SetWindowMonitor(Native, Monitor.None, x, y, w, h, 0);

            Logger.Debug("Successfully changed monitor state ({Name}={Value})", nameof(Fullscreen), Fullscreen);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Run(Dispose);
    }

    public void Dispose()
    {
        Logger.Information("Destroyed window {Window}", Title);
        GL.Dispose();
        Glfw.DestroyWindow(Native);
    }

    public bool Equals(Window? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _title == other._title && _visible == other._visible && _vsync == other._vsync &&
               Native.Equals(other.Native);
    }

    public event ResizeEventHandler WindowResize = delegate { };

    internal void IBind()
    {
        Glfw.MakeContextCurrent(Native);
    }

    public void Bind()
    {
        Graphics.BindWindow(this);
    }

    public void GetSize(out int x, out int y)
    {
        Glfw.GetWindowSize(Native, out x, out y);
    }

    public void SetSize(int x, int y)
    {
        Glfw.SetWindowSize(Native, x, y);
    }

    public void GetFramebufferSize(out int x, out int y)
    {
        Glfw.GetFramebufferSize(Native, out x, out y);
    }

    public void SwapBuffers()
    {
        Glfw.SwapBuffers(Native);
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
}