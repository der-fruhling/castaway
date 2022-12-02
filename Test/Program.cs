using System.Diagnostics;
using Castaway.Assets;
using Castaway.Base;
using Castaway.Input;
using Castaway.Level;
using Castaway.Level.Controllers;
using Castaway.OpenGL;
using Castaway.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = Castaway.Rendering.Window;

namespace Test;

[Imports(typeof(OpenGLImpl), typeof(ShaderController))]
internal class Program : IApplication
{
	private readonly Stopwatch _stopwatch = new();
	private Level? _level;

	private Window? _window;
#pragma warning disable 649
	// ReSharper disable once InconsistentNaming
	private Graphics g = null!;
#pragma warning restore 649

	public bool ShouldStop => _window?.ShouldClose ?? true;

	public void Init()
	{
		// Perform global initialization
		AssetLoader.Init();

		_window = new Window(800, 600, "name", false);
		_window.Bind();

		g = _window.GL;

		_level = new Level(AssetLoader.Loader!.GetAssetByName("/test_level.xml"));

		_level.Start();
		_window.Visible = true;

		InputSystem.Mouse.RawInput = true;

		_stopwatch.Start();
	}

	public void StartFrame()
	{
		g.StartFrame();
	}

	public void Render()
	{
		_level?.Render();
	}

	public void Update()
	{
		_level?.Update();
	}

	public void EndFrame()
	{
		if (_window is null) return;
		if (InputSystem.Keyboard.WasJustPressed(Keys.J))
			CastawayGlobal.GetLogger().Debug(
				"(Last Frame) {FrameTime}, {Change} @ {Rate}fps",
				CastawayGlobal.FrameTime,
				g.FrameChange,
				CastawayGlobal.Framerate);
		g.FinishFrame(_window);
		if ((InputSystem.Gamepad.Valid && InputSystem.Gamepad.Start) || InputSystem.Keyboard.IsDown(Keys.Escape))
			_window.ShouldClose = true;
	}

	public void Recover(RecoverableException e)
	{
		g.Clear();
		if (_window is not null) g.FinishFrame(_window);
	}

	public void Dispose()
	{
		if (_window is null) return;
		_window.Visible = false;
		_level?.End();
		_level?.Dispose();
		_window.Dispose();
	}

	private static int Main()
	{
		return CastawayGlobal.Run<Program>();
	}
}