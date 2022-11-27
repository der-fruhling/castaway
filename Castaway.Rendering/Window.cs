using System;
using System.Threading.Tasks;
using Castaway.Base;
using Castaway.Math;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Serilog;
using GLFWWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace Castaway.Rendering;

public sealed class Window : IDisposable, IAsyncDisposable, IEquatable<Window>
{
	public delegate void ResizeEventHandler(int newWidth, int newHeight);

	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	// ReSharper disable once InconsistentNaming
	public readonly Graphics GL;
	private Vector2? _sizeBeforeFullscreen, _positionBeforeFullscreen;

	private string _title;
	private bool _visible, _vsync;

	static Window()
	{
		GLFW.Init();
		AppDomain.CurrentDomain.ProcessExit += (_, _) => GLFW.Terminate();
	}

	public Window(int width, int height, string title, bool fullscreen, bool visible, Graphics? api = null)
	{
		Logger.Debug("Starting window creation");
		GLFW.DefaultWindowHints();
		GLFW.WindowHint((WindowHintBool)0x00023001, true);
		GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 2);
		GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
		GLFW.WindowHint(WindowHintBool.Visible, visible);
		Logger.Debug("Window Hint {Hint} = {Value}", WindowHintBool.Visible, visible);
		Logger.Debug("Window Size = {Width}x{Height}", width, height);
		unsafe
		{
			Native = GLFW.CreateWindow(width, height, title, fullscreen ? GLFW.GetPrimaryMonitor() : null, null);
		}

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
		unsafe
		{
			GLFW.SetWindowSizeCallback(Native!, (_, newWidth, newHeight) =>
			{
				WindowResize(newWidth, newHeight);
				Logger.Verbose("Resized {Window} to {Width}x{Height}", Title, newWidth, newHeight);
			});
		}

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

	public unsafe GLFWWindow* Native { get; }

	public string Title
	{
		get => _title;
		set
		{
			unsafe
			{
				GLFW.SetWindowTitle(Native!, _title = value);
				Logger.Debug("Successfully changed window state ({Name}={Value})", nameof(Title), Title);
			}
		}
	}

	public bool ShouldClose
	{
		get
		{
			unsafe
			{
				return GLFW.WindowShouldClose(Native!);
			}
		}
		set
		{
			unsafe
			{
				GLFW.SetWindowShouldClose(Native!, value);
				Logger.Debug("Successfully changed window state ({Name}={Value})", nameof(ShouldClose), ShouldClose);
			}
		}
	}

	public bool Visible
	{
		get => _visible;
		set
		{
			unsafe
			{
				// ReSharper disable once AssignmentInConditionalExpression
				if (_visible = value) GLFW.ShowWindow(Native!);
				else GLFW.HideWindow(Native!);
				Logger.Debug("Successfully changed window state ({Name}={Value})", nameof(Visible), Visible);
			}
		}
	}

	public bool VSync
	{
		get => _vsync;
		set
		{
			_vsync = value;
			GLFW.SwapInterval(value ? 1 : 0);
			Logger.Debug("Successfully changed window state ({Name}={Value})", nameof(VSync), VSync);
		}
	}

	public bool Fullscreen
	{
		get
		{
			unsafe
			{
				return GLFW.GetWindowMonitor(Native!) != null;
			}
		}
		set
		{
			unsafe
			{
				Logger.Information("{EnterOrExit} fullscreen...", value ? "Entering" : "Exiting");
				var mon = GLFW.GetPrimaryMonitor();
				if (mon == null) throw new InvalidOperationException("No monitors found");
				var vid = GLFW.GetVideoMode(mon);
				int x, y, w, h;
				GetSize(out var ow, out var oh);
				GLFW.GetWindowPos(Native!, out var ox, out var oy);
				if (value)
				{
					_sizeBeforeFullscreen = new Vector2(ow, oh);
					_positionBeforeFullscreen = new Vector2(ox, oy);
					w = vid->Width;
					h = vid->Height;
					x = 0;
					y = 0;
				}
				else
				{
					if (_sizeBeforeFullscreen.HasValue)
					{
						w = (int)_sizeBeforeFullscreen.Value.X;
						h = (int)_sizeBeforeFullscreen.Value.Y;
					}
					else
					{
						w = ow;
						h = oh;
					}

					if (_positionBeforeFullscreen.HasValue)
					{
						x = (int)_positionBeforeFullscreen.Value.X;
						y = (int)_positionBeforeFullscreen.Value.Y;
					}
					else
					{
						x = ox;
						y = oy;
					}

					_positionBeforeFullscreen = null;
					_sizeBeforeFullscreen = null;
				}

				if (value) GLFW.SetWindowMonitor(Native!, mon, 0, 0, vid->Width, vid->Height, GLFW.DontCare);
				else GLFW.SetWindowMonitor(Native!, null, x, y, w, h, GLFW.DontCare);

				Logger.Debug("Successfully changed monitor state ({Name}={Value})", nameof(Fullscreen), Fullscreen);
			}
		}
	}

	public async ValueTask DisposeAsync()
	{
		await Task.Run(Dispose);
	}

	public void Dispose()
	{
		unsafe
		{
			Logger.Information("Destroyed window {Window}", Title);
			GL.Dispose();
			GLFW.DestroyWindow(Native!);
		}
	}

	public bool Equals(Window? other)
	{
		unsafe
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return _title == other._title && _visible == other._visible && _vsync == other._vsync &&
			       Native == other.Native;
		}
	}

	public event ResizeEventHandler WindowResize = delegate { };

	internal void BindInternal()
	{
		unsafe
		{
			GLFW.MakeContextCurrent(Native!);
		}
	}

	public void Bind()
	{
		Graphics.BindWindow(this);
	}

	public void GetSize(out int x, out int y)
	{
		unsafe
		{
			GLFW.GetWindowSize(Native!, out x, out y);
		}
	}

	public void SetSize(int x, int y)
	{
		unsafe
		{
			GLFW.SetWindowSize(Native!, x, y);
		}
	}

	public void GetFramebufferSize(out int x, out int y)
	{
		unsafe
		{
			GLFW.GetFramebufferSize(Native!, out x, out y);
		}
	}

	public void SwapBuffers()
	{
		unsafe
		{
			GLFW.SwapBuffers(Native!);
		}
	}

	public override bool Equals(object? obj)
	{
		return ReferenceEquals(this, obj) || (obj is Window other && Equals(other));
	}

	public override int GetHashCode()
	{
		unsafe
		{
			return ((nuint)Native).GetHashCode();
		}
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